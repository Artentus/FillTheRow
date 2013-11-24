using System;
using Artentus.GameUtils;
using Artentus.GameUtils.Graphics;
using Artentus.GameUtils.UI;

namespace FillTheRow.UI
{
    public class Button : UIElement
    {
        LinearGradientBrush gradientBrush;
        SolidColorBrush solidBrush;
        Font font;
        TextFormat format;
        bool mouseOver;
        bool resized;

        public string Text { get; set; }

        public Button()
        {
            Text = string.Empty;
            CanGetFocus = false;
        }

        private void DestroyResources()
        {
            if (gradientBrush != null)
            {
                gradientBrush.Dispose();
                gradientBrush = null;
            }
            if (solidBrush != null)
            {
                solidBrush.Dispose();
                solidBrush = null;
            }
            if (font != null)
            {
                font.Dispose();
                font = null;
            }
            if (format != null)
            {
                format.Dispose();
                format = null;
            }
        }

        private void CreateResources(Factory factory)
        {
            var stops = new GradientStop[3];
            stops[0] = new GradientStop(new Color4(0, 0, 0, 0), 0);
            stops[1] = new GradientStop(new Color4(1, 1, 1), 0.5f);
            stops[2] = new GradientStop(new Color4(0, 0, 0, 0), 1);
            Rectangle rect = SurfaceBounds;
            gradientBrush = factory.CreateLinearGradientBrush(stops, new Vector2(rect.Left, 0), new Vector2(rect.Right, 0));
            solidBrush = factory.CreateSolidColorBrush(new Color4(1, 1, 1));
            font = factory.CreateFont("Segoe UI", rect.Height * 0.6f);
            format = factory.CreateTextFormat();
            format.HorizontalAlignment = HorizontalAlignment.Center;
            format.VerticalAlignment = VerticalAlignment.Center;
        }

        protected override void OnFactoryChanged(FactoryChangedEventArgs e)
        {
            this.DestroyResources();
            if (e.Factory != null)
                this.CreateResources(e.Factory);

            base.OnFactoryChanged(e);
        }

        protected override void OnAbsoluteBoundsChanged(EventArgs e)
        {
            resized = true;

            base.OnAbsoluteBoundsChanged(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (resized)
            {
                Rectangle rect = SurfaceBounds;
                gradientBrush.StartPoint = new Vector2(rect.Left, 0);
                gradientBrush.EndPoint = new Vector2(rect.Right, 0);
                font.Size = rect.Height * 0.6f;
                resized = false;
            }

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
            this.DestroyResources();

            base.Dispose(disposing);
        }

        protected override bool HitTest(Vector2 point)
        {
            return new Rectangle(Location.X + 0.25f * Size.X, Location.Y, 0.5f * Size.X, Size.Y).Contains(point);
        }
    }
}
