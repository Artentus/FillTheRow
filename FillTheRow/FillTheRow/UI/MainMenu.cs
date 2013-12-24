using System.Windows.Forms;
using GameUtils.Math;

namespace FillTheRow.UI
{
    public class MainMenu : Menu
    {
        public MainMenu()
            : base(null)
        {
            Visible = true;

            var newGameButton = new Button();
            this.Children.Add(newGameButton);
            newGameButton.Location = new Vector2(0.2f, 0.3f);
            newGameButton.Size = new Vector2(0.6f, 0.055f);
            newGameButton.Text = "Neues Spiel";
            newGameButton.MouseDown += (sernder, e) =>
            {
                Root.AspectRatio = 0;
                var menu = new IngameMenu(this);
                Root.Children.Add(menu);
                this.GoDown(menu);
            };

            var highscoresButton = new Button();
            this.Children.Add(highscoresButton);
            highscoresButton.Location = new Vector2(0.2f, 0.4f);
            highscoresButton.Size = new Vector2(0.6f, 0.055f);
            highscoresButton.Text = "Highscores";
            highscoresButton.MouseDown += (sender, e) =>
            {
                var menu = new HighscoreMenu(this);
                Root.Children.Add(menu);
                this.GoDown(menu);
            };

            var quitButton = new Button();
            this.Children.Add(quitButton);
            quitButton.Location = new Vector2(0.2f, 0.8f);
            quitButton.Size = new Vector2(0.6f, 0.055f);
            quitButton.Text = "Beenden";
            quitButton.MouseDown += (sender, e) => Application.Exit();
        }
    }
}
