using System;
using System.Collections.Generic;
using Artentus.GameUtils;
using Artentus.GameUtils.Graphics;
using Point = System.Drawing.Point;

namespace FillTheRow
{
    public class Tetromino : IRenderable
    {
        readonly TetrominoManager manager;
        readonly PlayingField field;
        readonly char identifier;
        bool[,] blockMap;
        Point center;
        readonly bool rotatesCenter;
        SolidColorBrush ghostBrush;

        bool IGameComponent.IsSynchronized
        {
            get { return false; }
        }

        public Texture Image
        {
            get { return manager.Image(identifier); }
        }

        public Point Location { get; private set; }

        public Tetromino(PlayingField field, TetrominoManager manager, char identifier)
        {
            this.field = field;
            this.manager = manager;
            this.identifier = identifier;

            byte[] data = manager.Data(identifier);
            blockMap = new bool[4, 4];
            for (int i = 0; i < 4; i++)
            {
                blockMap[i, 0] = (data[0] & (1 << i + 4)) != 0;
                blockMap[i, 1] = (data[0] & (1 << i)) != 0;
                blockMap[i, 2] = (data[1] & (1 << i + 4)) != 0;
                blockMap[i, 3] = (data[1] & (1 << i)) != 0;
            }
            center = new Point((data[2] & 0x0F) % 4, (data[2] & 0x0F) / 4);
            rotatesCenter = (data[2] & 0xF0) == 0;

            Location = new Point(4, -2);
        }

        public void GoBackToStart()
        {
            Location = new Point(4, -2);
        }

        public Block[] ToBlocks()
        {
            var result = new List<Block>(4);
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    if (blockMap[x, y])
                        result.Add(new Block(manager, identifier, new Point(Location.X - center.X + x, Location.Y - center.Y + y)));
                }
            }
            return result.ToArray();
        }

        private bool CanRotate(bool[,] map, Point newCenter, out int moveX, out int moveY)
        {
            moveX = default(int);
            moveY = default(int);

            Point[] checkPositions = {
                                         new Point(0, 0),
                                         new Point(1, 0), new Point(-1, 0), new Point(0, 1), new Point(0, -1),
                                         new Point(1, 1), new Point(-1, 1), new Point(1, -1), new Point(-1, -1),
                                         new Point(2, 0), new Point(-2, 0), new Point(0, 2), new Point(0, -2)    
                                     };

            for (int i = 0; i < checkPositions.Length; i++)
            {
                if (field.Fit(map, new Point(Location.X - newCenter.X + checkPositions[i].X, Location.Y - newCenter.Y + checkPositions[i].Y)))
                {
                    moveX = checkPositions[i].X;
                    moveY = checkPositions[i].Y;
                    return true;
                }
            }

            return false;
        }

        public bool TryRotateLeft()
        {
            var newMap = new bool[4, 4];
            for (int x = 0; x < 4; x++)
                for (int y = 0; y < 4; y++)
                    newMap[y, 3 - x] = blockMap[x, y];
            Point newCenter = rotatesCenter ? new Point(center.Y, 3 - center.X) : center;
            int moveX, moveY;
            if (this.CanRotate(newMap, newCenter, out moveX, out moveY))
            {
                blockMap = newMap;
                center = newCenter;
                Location = new Point(Location.X + moveX, Location.Y + moveY);
                return true;
            }
            return false;
        }

        public bool TryRotateRight()
        {
            var newMap = new bool[4, 4];
            for (int x = 0; x < 4; x++)
                for (int y = 0; y < 4; y++)
                    newMap[3 - y, x] = blockMap[x, y];
            Point newCenter = rotatesCenter ? new Point(3 - center.Y, center.X) : center;
            int moveX, moveY;
            if (this.CanRotate(newMap, newCenter, out moveX, out moveY))
            {
                blockMap = newMap;
                center = newCenter;
                Location = new Point(Location.X + moveX, Location.Y + moveY);
                return true;
            }
            return false;
        }

        public bool TryMoveLeft()
        {
            if (field.Fit(blockMap, new Point(Location.X - center.X - 1, Location.Y - center.Y)))
            {
                Location = new Point(Location.X - 1, Location.Y);
                return true;
            }
            return false;
        }

        public bool TryMoveRight()
        {
            if (field.Fit(blockMap, new Point(Location.X - center.X + 1, Location.Y - center.Y)))
            {
                Location = new Point(Location.X + 1, Location.Y);
                return true;
            }
            return false;
        }

        public bool TryMoveDown()
        {
            if (field.Fit(blockMap, new Point(Location.X - center.X, Location.Y - center.Y + 1)))
            {
                Location = new Point(Location.X, Location.Y + 1);
                return true;
            }
            return false;
        }

        void IRenderable.FactoryChanged(Factory factory)
        {
            if (ghostBrush != null)
            {
                ghostBrush.Dispose();
                ghostBrush = null;
            }

            if (factory != null)
            {
                ghostBrush = factory.CreateSolidColorBrush(new Color4(1, 1, 1));
                ghostBrush.Opacity = 0.4f;
            }
        }

        void IRenderable.Render(Renderer renderer)
        {
            int counter = Location.Y;
            while (field.Fit(blockMap, new Point(Location.X - center.X, counter - center.Y)))
                counter++;
            counter--;
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    if (blockMap[x, y])
                        renderer.FillRectangle(new Rectangle((Location.X - center.X + x) * 10, (counter - center.Y + y) * 10, 10, 10), ghostBrush);
                }
            }

            Texture blockImage = manager.BlockImage(identifier);
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    if (blockMap[x, y])
                        renderer.DrawTexture(blockImage, new Rectangle((Location.X - center.X + x) * 10, (Location.Y - center.Y + y) * 10, 10, 10));
                }
            }
        }

        void IDisposable.Dispose()
        {
            if (ghostBrush != null) ghostBrush.Dispose();
        }
    }
}
