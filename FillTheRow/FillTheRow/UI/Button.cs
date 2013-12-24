using System;
using GameUtils.Graphics;
using GameUtils.Math;
using GameUtils.UI;

namespace FillTheRow.UI
{
    public class Button : UIElement
    {
        readonly LinearGradientBrush gradientBrush;
        readonly SolidColorBrush solidBrush;
        readonly Font font;
        readonly TextFormat format;
        bool mouseOver;

        public string Text { get; set; }

        public Button()
        {
            Text = string.Empty;
            CanGetFocus = false;

            var stops = new GradientStop[3];
            stops[0] = new GradientStop(new Color4(0, 0, 0, 0), 0);
            stops[1] = new GradientStop(Color4.White, 0.5f);
            stops[2] = new GradientStop(new Color4(0, 0, 0, 0), 1);
            gradientBrush = new LinearGradientBrush(stops, new Vector2(0, 0), new Vector2(1, 0));
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

            Rectangle rect = SurfaceBounds;
            gradientBrush.StartPoint = new Vector2(rect.Left, 0);
            gradientBrush.EndPoint = new Vector2(rect.Right, 0);
            font.Size = rect.Height * 0.6f;

            base.OnAbsoluteBoundsChanged(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (mouseOver)
            {
                e.Renderer.DrawLine(new Vector2(e.Bounds.Left, e.Bounds.Top), new Vector2(e.Bounds.Right, e.Bounds.Top), gradientBrush, e.Bounds.Height / 15.0f);
                e.Renderer.DrawLine(new Vector2(e.Bounds.Left, e.Bounds.Bottom), new Vector2(e.Bounds.Right, e.Bounds.Bottom), gradientBrush, e.Bounds.Height / 15.0f);
            }
            e.Renderer.DrawText(Text, font, solidBrush, e.Bounds, format);

            base.OnPaint(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            mouseOver = true;

            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            mouseOver = false;
            
            base.OnMouseLeave(e);
        }

        protected override void Dispose(bool disposing)
        {
            gradientBrush.Dispose();
            solidBrush.Dispose();
            font.Dispose();
            format.Dispose();

            base.Dispose(disposing);
        }

        protected override bool HitTest(Vector2 point)
        {
            return new Rectangle(Location.X + 0.25f * Size.X, Location.Y, 0.5f * Size.X, Size.Y).Contains(point);
        }
    }
}
