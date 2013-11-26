using System.Drawing;
using System.Windows.Forms;
using Artentus.GameUtils;

namespace FillTheRow
{
    public class GameWindow : GameWindowBase
    {
        public GameWindow(bool fullscreen)
        {
            if (fullscreen)
            {
                FormBorderStyle = FormBorderStyle.None;
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                ClientSize = new Size(1280, 720);
                MinimumSize = new Size(Width - 1280 + 480, Height - 720 + 360);
            }
            Text = "Fill the Row";
        }
    }
}
