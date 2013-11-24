using System.Drawing;
using System.Windows.Forms;

namespace FillTheRow
{
    public class GameWindow : Form
    {
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams param = base.CreateParams;
                param.ClassStyle = param.ClassStyle | 0x200;
                return param;
            }
        }

        public GameWindow(bool fullscreen)
        {
            this.SetStyle(ControlStyles.UserPaint, true);
            this.UpdateStyles();
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

        protected override void OnPaint(PaintEventArgs e)
        { }

        protected override void OnPaintBackground(PaintEventArgs e)
        { }
    }
}
