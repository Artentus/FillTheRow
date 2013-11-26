using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Artentus.GameUtils.Graphics;

namespace FillTheRow.UI
{
    public class HighscoreMenu : Menu
    {
        public HighscoreMenu(Menu parent)
            : base(parent)
        {
            var listBox = new ListBox();
            listBox.Location = new Vector2(0.2f, 0.15f);
            listBox.Size = new Vector2(0.6f, 0.6f);
            listBox.ItemHeight = 0.06f;
            var scores = new List<long>();
            var file = new FileInfo(Path.Combine(Environment.CurrentDirectory, "highscores.dat"));
            if (file.Exists)
            {
                using (FileStream fs = file.Open(FileMode.Open, FileAccess.Read))
                {
                    while (fs.Position < fs.Length)
                    {
                        var bytes = new byte[8];
                        if (fs.Read(bytes, 0, 8) == 8)
                            scores.Add(BitConverter.ToInt64(bytes, 0));
                    }
                }
            }
            foreach (long score in scores.OrderByDescending(item => item))
                listBox.Items.Add(score.ToString());
            Children.Add(listBox);

            var backButton = new Button();
            backButton.Location = new Vector2(0.2f, 0.8f);
            backButton.Size = new Vector2(0.6f, 0.055f);
            backButton.Text = "Zurück";
            backButton.MouseDown += (sender, e) =>
            {
                this.GoUp();
                Parent = null;
            };
            Children.Add(backButton);
        }
    }
}
