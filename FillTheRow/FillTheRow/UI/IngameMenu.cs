using Artentus.GameUtils;
using Artentus.GameUtils.Graphics;
using Artentus.GameUtils.Input.DefaultDevices;
using Artentus.GameUtils.UI;
using PaintEventArgs = Artentus.GameUtils.UI.PaintEventArgs;

namespace FillTheRow.UI
{
    public class IngameMenu : Menu
    {
        readonly PlayingField field;
        SolidColorBrush dimBrush;
        readonly object locker;
        readonly Button continueButton;
        readonly Button exitButton;
        readonly MenuButton menuButton;

        public IngameMenu(Menu parent)
            : base(parent)
        {
            field = new PlayingField(Game.Manager);
            Game.MainGameLayer.Components.Add(field);
            CanGetFocus = true;
            Game.UIRoot.Children.Add(this);
            locker = new object();

            continueButton = new Button();
            continueButton.Location = new Vector2(0.2f, 0.4f);
            continueButton.Size = new Vector2(0.6f, 0.055f);
            continueButton.Text = "Fortsetzen";
            continueButton.Visible = false;
            continueButton.MouseDown += (sernder, e) => this.Unpause();
            this.Children.Add(continueButton);

            exitButton = new Button();
            exitButton.Location = new Vector2(0.2f, 0.5f);
            exitButton.Size = new Vector2(0.6f, 0.055f);
            exitButton.Text = "Verlassen";
            exitButton.Visible = false;
            exitButton.MouseDown += (sender, e) =>
            {
                Game.MainGameLayer.Components.Remove(field);
                field.Dispose();
                this.GoUp();
                Game.UIRoot.Children.Remove(this);
            };
            this.Children.Add(exitButton);

            menuButton = new MenuButton();
            menuButton.Location = new Vector2(0.4f, 0.02f);
            menuButton.Size = new Vector2(0.2f, 0.04f);
            menuButton.Text = "Menü";
            menuButton.Click += (sender, e) => this.Pause();
            this.Children.Add(menuButton);

            field.OnLost += (sender, e) =>
            {
                exitButton.Location = new Vector2(0.2f, 0.45f);
                exitButton.Visible = true;
            };
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

        private void DestroyResources()
        {
            lock (locker)
            {
                if (dimBrush != null)
                {
                    dimBrush.Dispose();
                    dimBrush = null;
                }
            }
        }

        private void CreateResources(Factory factory)
        {
            lock (locker)
            {
                dimBrush = factory.CreateSolidColorBrush(new Color4(0, 0, 0));
                dimBrush.Opacity = 0.8f;
            }
        }

        protected override void OnFactoryChanged(FactoryChangedEventArgs e)
        {
            this.Focus();

            this.DestroyResources();
            if (e.Factory != null)
                this.CreateResources(e.Factory);

            base.OnFactoryChanged(e);
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
            this.DestroyResources();

            base.Dispose(disposing);
        }
    }
}
