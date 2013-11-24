using System;
using Artentus.GameUtils;
using Artentus.GameUtils.Graphics;
using Artentus.GameUtils.UI;

namespace FillTheRow.UI
{
    public class MenuButton : UIElement
    {
        SolidColorBrush solidBrush;
        Font font;
        TextFormat format;
        Factory factory;
        bool resized;

        public string Text { get; set; }

        public MenuButton()
        {
            Text = string.Empty;
            CanGetFocus = false;
        }

        private void DestroyResources()
        {
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
            solidBrush = factory.CreateSolidColorBrush(new Color4(1, 1, 1));
            font = factory.CreateFont("Segoe UI", AbsoluteBounds.Height * 0.6f);
            format = factory.CreateTextFormat();
            format.HorizontalAlignment = HorizontalAlignment.Center;
            format.VerticalAlignment = VerticalAlignment.Center;
        }

        protected override void OnFactoryChanged(FactoryChangedEventArgs e)
        {
            factory = e.Factory;

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
                this.DestroyResources();
                this.CreateResources(factory);
                resized = false;
            }

            e.Renderer.DrawRectangle(e.Bounds, solidBrush, e.Bounds.Height / 20.0f);
            e.Renderer.DrawText(Text, font, solidBrush, e.Bounds, format);

            base.OnPaint(e);
        }

        protected override void Dispose(bool disposing)
        {
            this.DestroyResources();

            base.Dispose(disposing);
        }
    }
}
