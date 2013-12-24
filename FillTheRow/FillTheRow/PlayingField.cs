using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameUtils;
using GameUtils.Audio;
using GameUtils.Graphics;
using GameUtils.Input;
using GameUtils.Input.DefaultDevices;
using GameUtils.Math;
using HorizontalAlignment = GameUtils.Graphics.HorizontalAlignment;
using Point = System.Drawing.Point;

namespace FillTheRow
{
    public class PlayingField : IRenderable, IUpdateable, IInputListener<KeyboardState>
    {
        readonly ListenerHandle<KeyboardState> keyboardHandle;
        readonly TetrominoManager manager;
        readonly Block[,] field;
        readonly SortedSet<int> generatedIds;
        readonly Queue<Tetromino> nextTetrominos;
        readonly AudioHandle moveSound;
        readonly AudioHandle rotateSound;
        readonly AudioHandle lockSound;
        long score;
        int level;
        int comboCount;
        Tetromino currentTetromino;
        Tetromino holdTetromino;
        readonly SolidColorBrush gridBrush;
        readonly SolidColorBrush whiteBrush;
        readonly Font smallFont;
        readonly Font scoreFont;
        readonly Font levelFont;
        readonly TextFormat centerFormat;
        readonly Geometry frame;
        int millisecondsPerTick = 1000;
        int elapsedMilliseconds;
        const int millisecondsPerMove = 20;
        int keyMilliseconds;
        bool leftDown;
        int leftDownMilliseconds;
        bool rightDown;
        int rightDownMilliseconds;
        bool downDown;
        bool rotateLeftDown;
        bool rotateRightDown;
        bool dropDown;
        bool locking;
        int lockMilliseconds;
        bool lost;
        bool paused;
        bool canChange;
        int filledRows;

        public event EventHandler OnLost;

        bool IGameComponent.IsSynchronized
        {
            get { return true; }
        }

        public bool Lost
        {
            get { return lost; }
        }

        public bool Paused
        {
            get { return paused; }
        }

        public PlayingField()
        {
            this.manager = GameEngine.QueryComponent<TetrominoManager>();
            keyboardHandle = GameEngine.QueryComponent<IInputProvider<KeyboardState>>().RegisterListener(this);
            field = new Block[10, 20];
            generatedIds = new SortedSet<int>();
            nextTetrominos = new Queue<Tetromino>(5);
            for (int i = 0; i < 5; i++)
                nextTetrominos.Enqueue(this.NextTetromino());
            holdTetromino = this.NextTetromino();

            AudioEngine audioEngine = GameEngine.QueryComponent<AudioEngine>();
            moveSound = audioEngine.CreateHandle(Path.Combine(Environment.CurrentDirectory, "Sounds/move.wav"));
            moveSound.Volume = 0.5f;
            rotateSound = audioEngine.CreateHandle(Path.Combine(Environment.CurrentDirectory, "Sounds/rotate.wav"));
            rotateSound.Volume = 0.5f;
            lockSound = audioEngine.CreateHandle(Path.Combine(Environment.CurrentDirectory, "Sounds/lock.wav"));
            lockSound.Volume = 0.5f;

            level = 1;
            canChange = true;

            gridBrush = new SolidColorBrush(Color4.White);
            gridBrush.Opacity = 0.7f;
            whiteBrush = new SolidColorBrush(Color4.White);
            smallFont = new Font("Consolas", 8, FontStyle.Normal, FontWeight.Bold);
            scoreFont = new Font("Comic Sans MS", 14, FontStyle.Italic, FontWeight.Bold);
            levelFont = new Font("Consolas", 20, FontStyle.Normal, FontWeight.Bold);
            centerFormat = new TextFormat();
            centerFormat.HorizontalAlignment = HorizontalAlignment.Center;
            centerFormat.VerticalAlignment = VerticalAlignment.Center;
            frame = new Geometry();
            frame.BeginFigure(new Vector2(-5, -5), FigureBegin.Hollow);
            frame.AddLines(new[] { new Vector2(-5, 205), new Vector2(105, 205), new Vector2(105, -5) });
            frame.EndFigure(FigureEnd.Open);
            frame.AddRectangle(new Rectangle(-35, 15, 30, 30));
            frame.AddRectangle(new Rectangle(-35, 80, 30, 30));
            frame.AddRectangle(new Rectangle(105, 15, 30, 30));
            frame.Close();
        }

        public bool Fit(bool[,] map, Point location)
        {
            if (map.GetUpperBound(0) != 3 || map.GetUpperBound(1) != 3)
                throw new ArgumentOutOfRangeException("map");

            for (int x = location.X, i = 0; i < 4; x++, i++)
            {
                for (int y = location.Y, j = 0; j < 4; y++, j++)
                {
                    if (map[i, j] && (x < 0 || x > 9 || y > 19 || (y >= 0 && this.field[x, y] != null)))
                        return false;
                }
            }
            return true;
        }

        private int NextId()
        {
            if (generatedIds.Count < manager.Identifiers.Length - 1)
            {
                int id;
                do
                {
                    id = GameEngine.QueryComponent<XorshiftEngine>().Next(manager.Identifiers.Length);
                } while (generatedIds.Contains(id));
                generatedIds.Add(id);
                return id;
            }
            else
            {
                for (int i = 0; i < manager.Identifiers.Length; i++)
                {
                    if (!generatedIds.Contains(i))
                    {
                        generatedIds.Clear();
                        return i;
                    }
                }
            }

            throw new InvalidOperationException();
        }

        private Tetromino NextTetromino()
        {
            return new Tetromino(this, manager.Identifiers[this.NextId()]);
        }

        private void DrawTextureCentered(Renderer renderer, Texture texture, Rectangle rect)
        {
            float factor = Math.Min(rect.Width / texture.Width, rect.Height / texture.Height);
            float width = texture.Width * factor;
            float height = texture.Height * factor;
            renderer.DrawTexture(texture, new Rectangle(rect.X + (rect.Width - width) / 2, rect.Y + (rect.Height - height) / 2, width, height));
        }

        void IRenderable.Render(Renderer renderer)
        {
            for (int x = 0; x <= 10; x++)
                renderer.DrawLine(new Vector2(x * 10, -0.25f), new Vector2(x * 10, 200.25f), gridBrush, 0.5f);

            for (int y = 0; y <= 20; y++)
                renderer.DrawLine(new Vector2(-0.25f, y * 10), new Vector2(100.25f, y * 10), gridBrush, 0.5f);

            renderer.DrawGeometry(frame, whiteBrush, 2);
            renderer.DrawText("HOLD", smallFont, whiteBrush, new Rectangle(-35, 5, 30, 10), centerFormat);
            renderer.DrawText("LEVEL", smallFont, whiteBrush, new Rectangle(-35, 70, 30, 10), centerFormat);
            renderer.DrawText("NEXT", smallFont, whiteBrush, new Rectangle(105, 5, 30, 10), centerFormat);
            renderer.DrawText(score.ToString(), scoreFont, whiteBrush, new Rectangle(0, -40, 100, 14), centerFormat);

            this.DrawTextureCentered(renderer, holdTetromino.Image, new Rectangle(-32, 18, 24, 24));
            renderer.DrawText(level.ToString(), levelFont, whiteBrush, new Rectangle(-32, 83, 24, 24), centerFormat);
            this.DrawTextureCentered(renderer, nextTetrominos.First().Image, new Rectangle(108, 18, 24, 24));
            int i = 0;
            foreach (var tetromino in nextTetrominos.Skip(1))
            {
                this.DrawTextureCentered(renderer, tetromino.Image, new Rectangle(111 + i * 1.4f, 130 - 80 * (float)Math.Pow(0.75, i), 18 - i * 2.8f, 18 - i * 2.8f));
                i++;
            }
        }

        private void CheckKeys(TimeSpan elapsed)
        {
            keyMilliseconds += elapsed.Milliseconds;
            int moves = Math.DivRem(keyMilliseconds, millisecondsPerMove, out keyMilliseconds);
            for (int i = 0; i < moves; i++)
            {
                if (leftDown)
                {
                    if (leftDownMilliseconds > 80)
                    {
                        if (currentTetromino.TryMoveLeft())
                        {
                            moveSound.Play();
                            locking = false;
                            lockMilliseconds = 0;
                        }
                    }
                    else
                        leftDownMilliseconds += elapsed.Milliseconds;
                }
                if (rightDown)
                {
                    if (rightDownMilliseconds > 80)
                    {
                        if (currentTetromino.TryMoveRight())
                        {
                            moveSound.Play();
                            locking = false;
                            lockMilliseconds = 0;
                        }
                    }
                    else
                        rightDownMilliseconds += elapsed.Milliseconds;
                }
                if (downDown)
                {
                    if (currentTetromino.TryMoveDown())
                    {
                        moveSound.Play();
                        lockMilliseconds = 0;
                        score += 10;
                    }
                }
            }
        }

        private void DoMovement(TimeSpan elapsed)
        {
            elapsedMilliseconds += elapsed.Milliseconds;
            int steps = Math.DivRem(elapsedMilliseconds, millisecondsPerTick, out elapsedMilliseconds);
            for (int i = 0; i < steps; i++)
            {
                if (!currentTetromino.TryMoveDown())
                    locking = true;
                else
                {
                    locking = false;
                    lockMilliseconds = 0;
                }
            }

            if (locking)
            {
                lockMilliseconds += elapsed.Milliseconds;
                if (lockMilliseconds > 500)
                {
                    this.PlaceCurrent();
                    this.ClearRows();
                    lockMilliseconds = 0;
                    locking = false;
                }
            }
        }

        void IUpdateable.Update(TimeSpan elapsed)
        {
            if (lost || paused)
                return;

            if (currentTetromino == null)
            {
                currentTetromino = nextTetrominos.Dequeue();
                GameEngine.QueryComponent<GameLoop>().Components.GetSingle<GameLayer>().Components.Add(currentTetromino);
                nextTetrominos.Enqueue(this.NextTetromino());
                canChange = true;
            }

            this.CheckKeys(elapsed);

            this.DoMovement(elapsed);
        }

        void IInputListener<KeyboardState>.OnInputReceived(KeyboardState state)
        {
            if (currentTetromino == null)
                return;

            if (state.IsPressed(Key.Left) || state.IsPressed(Key.A))
            {
                if (!leftDown)
                {
                    leftDown = true;
                    if (!(lost || paused) && currentTetromino.TryMoveLeft())
                    {
                        moveSound.Play();
                        locking = false;
                        lockMilliseconds = 0;
                    }
                }
            }
            else
            {
                if (leftDown)
                {
                    leftDown = false;
                    leftDownMilliseconds = 0;
                }
            }

            if (state.IsPressed(Key.Right) || state.IsPressed(Key.D))
            {
                if (!rightDown)
                {
                    rightDown = true;
                    if (!(lost || paused) && currentTetromino.TryMoveRight())
                    {
                        moveSound.Play();
                        locking = false;
                        lockMilliseconds = 0;
                    }
                }
            }
            else
            {
                if (rightDown)
                {
                    rightDown = false;
                    rightDownMilliseconds = 0;
                }
            }

            if (state.IsPressed(Key.Down) || state.IsPressed(Key.S))
            {
                if (!downDown)
                {
                    downDown = true;
                    if (!(lost || paused) && currentTetromino.TryMoveDown())
                    {
                        moveSound.Play();
                        locking = false;
                        score += 10;
                    }
                }
            }
            else
            {
                if (downDown) { downDown = false; }
            }

            if (state.IsPressed(Key.Q))
            {
                if (!rotateLeftDown)
                {
                    rotateLeftDown = true;
                    if (!(lost || paused) && currentTetromino.TryRotateLeft())
                    {
                        rotateSound.Play();
                        locking = false;
                        lockMilliseconds = 0;
                    }
                }
            }
            else
            {
                if (rotateLeftDown) rotateLeftDown = false;
            }

            if (state.IsPressed(Key.E))
            {
                if (!rotateRightDown)
                {
                    rotateRightDown = true;
                    if (!(lost || paused) && currentTetromino.TryRotateRight())
                    {
                        rotateSound.Play();
                        locking = false;
                        lockMilliseconds = 0;
                    }
                }
            }
            else
            {
                if (rotateRightDown) rotateRightDown = false;
            }

            if (state.IsPressed(Key.Space))
            {
                if (!dropDown)
                {
                    dropDown = true;
                    if (!(lost || paused))
                    {
                        int count = 0;
                        while (currentTetromino.TryMoveDown()) { count++; }
                        this.PlaceCurrent();
                        this.ClearRows();
                        locking = false;
                        lockMilliseconds = 0;
                        score += 20 * count;
                    }
                }
            }
            else
            {
                if (dropDown) dropDown = false;
            }

            if (state.IsPressed(Key.Tab) && canChange)
            {
                canChange = false;
                Tetromino temp = holdTetromino;
                GameLayer layer = GameEngine.QueryComponent<GameLoop>().Components.GetSingle<GameLayer>();
                layer.Components.Remove(currentTetromino);
                holdTetromino = currentTetromino;
                currentTetromino = temp;
                currentTetromino.GoBackToStart();
                layer.Components.Add(currentTetromino);
            }
        }

        public void Pause()
        {
            if (!lost)
                paused = true;
        }

        public void Unpause()
        {
            if (!lost)
                paused = false;
        }

        private void OnLose()
        {
            lost = true;
            if (OnLost != null)
                this.OnLost(this, EventArgs.Empty);

            var file = new FileInfo(Path.Combine(Environment.CurrentDirectory, "highscores.dat"));
            using (FileStream fs = file.Exists ? file.Open(FileMode.Append, FileAccess.Write) : file.Create())
            {
                fs.Position = fs.Length;
                fs.Write(BitConverter.GetBytes(score), 0, 8);
                fs.Flush();
            }
        }

        private void PlaceCurrent()
        {
            GameLayer layer = GameEngine.QueryComponent<GameLoop>().Components.GetSingle<GameLayer>();
            Block[] blocks = currentTetromino.ToBlocks();
            for (int i = 0; i < blocks.Length; i++)
            {
                Block block = blocks[i];
                if (block.Location.Y < 0)
                {
                    this.OnLose();
                    return;
                }
                field[block.Location.X, block.Location.Y] = block;
                layer.Components.Add(block);
            }
            layer.Components.Remove(currentTetromino);
            currentTetromino = null;
            lockSound.Play();
        }

        private void ClearRows()
        {
            int count = 0;
            for (int y = 19; y >= 0; y--)
            {
                bool rowFilled = true;
                for (int x = 0; x < 10; x++)
                {
                    if (field[x, y] == null)
                    {
                        rowFilled = false;
                        break;
                    }
                }

                if (rowFilled)
                {
                    for (int x = 0; x < 10; x++)
                    {
                        GameEngine.QueryComponent<GameLoop>().Components.GetSingle<GameLayer>().Components.Remove(field[x, y]);
                        field[x, y] = null;
                    }
                    count++;
                }
                else if (count > 0)
                {
                    for (int x = 0; x < 10; x++)
                    {
                        Block block = field[x, y];
                        if (block != null)
                        {
                            field[x, y + count] = block;
                            block.Location = new Point(x, y + count);
                        }
                        field[x, y] = null;
                    }
                }
            }

            int[] points = { 0, 100, 300, 500, 800 };
            score += points[count] * level;
            if (count > 0)
                comboCount++;
            else
                comboCount = 0;
            if (comboCount > 1) score += 50 * (comboCount - 1) * level;

            filledRows += count;
            level = filledRows / 10 + 1;
            millisecondsPerTick = 1000 / (int)((level - 1) * 0.25f + 1);
        }

        private bool disposed;

        public void Dispose()
        {
            if (!disposed)
            {
                Dispose(true);
                GC.SuppressFinalize(this);

                disposed = true;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            GameLayer layer = GameEngine.QueryComponent<GameLoop>().Components.GetSingle<GameLayer>();
            if (currentTetromino != null) layer.Components.Remove(currentTetromino);
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 20; y++)
                {
                    if (field[x, y] != null)
                        layer.Components.Remove(field[x, y]);
                }
            }

            keyboardHandle.Dispose();
            moveSound.Dispose();
            rotateSound.Dispose();
            lockSound.Dispose();
            gridBrush.Dispose();
            whiteBrush.Dispose();
            smallFont.Dispose();
            scoreFont.Dispose();
            levelFont.Dispose();
            centerFormat.Dispose();
            frame.Dispose();
        }

        ~PlayingField()
        {
            Dispose(false);
        }
    }
}
