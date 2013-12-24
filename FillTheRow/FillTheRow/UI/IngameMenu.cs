using GameUtils;
using GameUtils.Graphics;
using GameUtils.Input.DefaultDevices;
using GameUtils.Math;
using GameUtils.UI;
using PaintEventArgs = GameUtils.UI.PaintEventArgs;

namespace FillTheRow.UI
{
    public class IngameMenu : Menu
    {
        readonly PlayingField field;
        readonly SolidColorBrush dimBrush;
        readonly object locker;
        readonly Button continueButton;
        readonly Button exitButton;
        readonly MenuButton menuButton;

        public IngameMenu(Menu parent)
            : base(parent)
        {
            field = new PlayingField();
            GameEngine.QueryComponent<GameLoop>().Components.GetSingle<GameLayer>().Components.Add(field);
            CanGetFocus = true;
            locker = new object();

            continueButton = new Button();
            this.Children.Add(continueButton);
            continueButton.Location = new Vector2(0.2f, 0.4f);
            continueButton.Size = new Vector2(0.6f, 0.055f);
            continueButton.Text = "Fortsetzen";
            continueButton.Visible = false;
            continueButton.MouseDown += (sernder, e) => this.Unpause();

            exitButton = new Button();
            this.Children.Add(exitButton);
            exitButton.Location = new Vector2(0.2f, 0.5f);
            exitButton.Size = new Vector2(0.6f, 0.055f);
            exitButton.Text = "Verlassen";
            exitButton.Visible = false;
            exitButton.MouseDown += (sender, e) =>
            {
                GameEngine.QueryComponent<GameLoop>().Components.GetSingle<GameLayer>().Components.Remove(field);
                field.Dispose();
                Root.AspectRatio = 16.0f / 9.0f;
                this.GoUp();
                Parent = null;
            };

            menuButton = new MenuButton();
            this.Children.Add(menuButton);
            menuButton.Location = new Vector2(0.4f, 0.02f);
            menuButton.Size = new Vector2(0.2f, 0.04f);
            menuButton.Text = "Menü";
            menuButton.Click += (sender, e) => this.Pause();

            field.OnLost += (sender, e) =>
            {
                exitButton.Location = new Vector2(0.2f, 0.45f);
                exitButton.Visible = true;
            };

            dimBrush = new SolidColorBrush(Color4.Black);
            dimBrush.Opacity = 0.8f;
        }

        private void Pause()
        {
            field.Pause();
            continueButton.Visible = true;
            exitButton.Visible = true;
            menuButton.Visible = false;
        }

        private void Unpause()
        {
            field.Unpause();
            continueButton.Visible = false;
            exitButton.Visible = false;
            menuButton.Visible = true;
        }

        protected override void OnKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (field.Paused)
                    this.Unpause();
                else
                    this.Pause();
            }

            base.OnKeyDown(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            lock (locker)
            {
                if ((field.Lost || field.Paused) && dimBrush != null)
                    e.Renderer.FillRectangle(e.Bounds, dimBrush);
            }

            base.OnPaint(e);
        }

        protected override void Dispose(bool disposing)
        {
            dimBrush.Dispose();

            base.Dispose(disposing);
        }
    }
}
