using System;
using System.Collections.ObjectModel;
using Artentus.GameUtils;
using Artentus.GameUtils.Graphics;
using Artentus.GameUtils.UI;

namespace FillTheRow.UI
{
    public class ListBox : ContainerElement
    {
        readonly VScrollBar scrollBar;
        LinearGradientBrush gradientBrush;
        SolidColorBrush textBrush;
        Font font;
        TextFormat format;
        bool resized;
        float itemHeight;

        public ObservableCollection<string> Items { get; private set; }

        public float ItemHeight
        {
            get { return itemHeight; }
            set
            {
                itemHeight = value;
                resized = true;
                scrollBar.Maximum = Math.Max(Items.Count - 1 / itemHeight * 0.7f, 0);
            }
        }

        public ListBox()
        {
            Items = new ObservableCollection<string>();
            Items.CollectionChanged += (sender, e) => scrollBar.Maximum = Math.Max(Items.Count - 1 / itemHeight * 0.7f, 0);

            scrollBar = new VScrollBar();
            scrollBar.Location = new Vector2(0.98f, 0.15f);
            scrollBar.Size = new Vector2(0.02f, 0.7f);
            Children.Add(scrollBar);
        }

        private void DestroyResources()
        {
            if (gradientBrush != null)
            {
                gradientBrush.Dispose();
                gradientBrush = null;
            }
            if (textBrush != null)
            {
                textBrush.Dispose();
                textBrush = null;
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
            gradientBrush = factory.CreateLinearGradientBrush(stops, new Vector2(0, rect.Top), new Vector2(0, rect.Bottom));
            textBrush = factory.CreateSolidColorBrush(new Color4(1, 1, 1));
            font = factory.CreateFont("Segoe UI", ItemHeight * rect.Height * 0.7f);
            format = factory.CreateTextFormat();
            format.HorizontalAlignment = HorizontalAlignment.Leading;
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
                gradientBrush.StartPoint = new Vector2(0, rect.Top);
                gradientBrush.EndPoint = new Vector2(0, rect.Bottom);
                font.Size = ItemHeight * rect.Height * 0.7f;
                resized = false;
            }

            e.Renderer.DrawLine(new Vector2(e.Bounds.Left, e.Bounds.Top), new Vector2(e.Bounds.Left, e.Bounds.Bottom), gradientBrush, e.Bounds.Width / 170.0f);
            e.Renderer.DrawLine(new Vector2(e.Bounds.Left + e.Bounds.Width * 0.96f, e.Bounds.Top), new Vector2(e.Bounds.Left + e.Bounds.Width * 0.96f, e.Bounds.Bottom), gradientBrush, e.Bounds.Width / 170.0f);

            e.Renderer.PushClip(new Rectangle(e.Bounds.X, e.Bounds.Y + e.Bounds.Height * 0.15f, e.Bounds.Width, e.Bounds.Height * 0.7f));
            float absoluteItemHeight = ItemHeight * e.Bounds.Height;
            for (int i = 0; i < Items.Count; i++)
            {
                float y = i * absoluteItemHeight - scrollBar.Value * absoluteItemHeight + e.Bounds.Y + e.Bounds.Height * 0.15f;
                e.Renderer.DrawText(Items[i], font, textBrush, new Rectangle(e.Bounds.X + e.Bounds.Width * 0.02f, y, e.Bounds.Width * 0.92f, absoluteItemHeight), format);
            }
            e.Renderer.PopClip();
            
            base.OnPaint(e);
        }

        protected override void Dispose(bool disposing)
        {
            this.DestroyResources();
            
            base.Dispose(disposing);
        }
    }
}
