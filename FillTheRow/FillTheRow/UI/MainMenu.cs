﻿using System.Windows.Forms;
using Artentus.GameUtils.Graphics;

namespace FillTheRow.UI
{
    public class MainMenu : Menu
    {
        public MainMenu()
            : base(null)
        {
            Visible = true;

            var newGameButton = new Button();
            newGameButton.Location = new Vector2(0.2f, 0.3f);
            newGameButton.Size = new Vector2(0.6f, 0.055f);
            newGameButton.Text = "Neues Spiel";
            newGameButton.MouseDown += (sernder, e) => this.GoDown(new IngameMenu(this));
            this.Children.Add(newGameButton);

            var highscoresButton = new Button();
            highscoresButton.Location = new Vector2(0.2f, 0.4f);
            highscoresButton.Size = new Vector2(0.6f, 0.055f);
            highscoresButton.Text = "Highscores";
            highscoresButton.MouseDown += (sender, e) => this.GoDown(new HighscoreMenu(this));
            this.Children.Add(highscoresButton);

            var quitButton = new Button();
            quitButton.Location = new Vector2(0.2f, 0.8f);
            quitButton.Size = new Vector2(0.6f, 0.055f);
            quitButton.Text = "Beenden";
            quitButton.MouseDown += (sender, e) => Application.Exit();
            this.Children.Add(quitButton);

            Game.UIRoot.Children.Add(this);
        }
    }
}