using System;
using GameUtils.Graphics;
using GameUtils.UI;

namespace FillTheRow.UI
{
    public class MenuButton : UIElement
    {
        readonly SolidColorBrush solidBrush;
        readonly Font font;
        readonly TextFormat format;

        public string Text { get; set; }

        public MenuButton()
        {
            Text = string.Empty;
            CanGetFocus = false;

            solidBrush = new SolidColorBrush(Color4.White);
            font = new Font("Segoe UI", 1);
            format = new TextFormat();
            format.HorizontalAlignment = HorizontalAlignment.Center;
            format.VerticalAlignment = VerticalAlignment.Center;
        }

        protected override void OnAbsoluteBoundsChanged(EventArgs e)
        {
            if (Root == null)
                return;

            font.Size = AbsoluteBounds.Height * 0.6f;

            base.OnAbsoluteBoundsChanged(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Renderer.DrawRectangle(e.Bounds, solidBrush, e.Bounds.Height / 20.0f);
            e.Renderer.DrawText(Text, font, solidBrush, e.Bounds, format);

            base.OnPaint(e);
        }

        protected override void Dispose(bool disposing)
        {
            solidBrush.Dispose();
            font.Dispose();
            format.Dispose();

            base.Dispose(disposing);
        }
    }
}
