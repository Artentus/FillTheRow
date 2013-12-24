using System;
using System.Collections.ObjectModel;
using GameUtils.Graphics;
using GameUtils.Math;
using GameUtils.UI;

namespace FillTheRow.UI
{
    public class ListBox : ContainerElement
    {
        readonly VScrollBar scrollBar;
        readonly LinearGradientBrush gradientBrush;
        readonly SolidColorBrush textBrush;
        readonly Font font;
        readonly TextFormat format;
        float itemHeight;

        public ObservableCollection<string> Items { get; private set; }

        public float ItemHeight
        {
            get { return itemHeight; }
            set
            {
                itemHeight = value;
                if (Root != null)
                {
                    Rectangle rect = SurfaceBounds;
                    gradientBrush.StartPoint = new Vector2(0, rect.Top);
                    gradientBrush.EndPoint = new Vector2(0, rect.Bottom);
                    font.Size = ItemHeight * rect.Height * 0.7f;
                }
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

            var stops = new GradientStop[3];
            stops[0] = new GradientStop(new Color4(0, 0, 0, 0), 0);
            stops[1] = new GradientStop(Color4.White, 0.5f);
            stops[2] = new GradientStop(new Color4(0, 0, 0, 0), 1);
            Rectangle rect = SurfaceBounds;
            gradientBrush = new LinearGradientBrush(stops, new Vector2(0, rect.Top), new Vector2(0, rect.Bottom));
            textBrush = new SolidColorBrush(Color4.White);
            font = new Font("Segoe UI", 1);
            format = new TextFormat();
            format.HorizontalAlignment = HorizontalAlignment.Leading;
            format.VerticalAlignment = VerticalAlignment.Center;
        }

        protected override void OnAbsoluteBoundsChanged(EventArgs e)
        {
            if (Root == null)
                return;

            Rectangle rect = SurfaceBounds;
            gradientBrush.StartPoint = new Vector2(0, rect.Top);
            gradientBrush.EndPoint = new Vector2(0, rect.Bottom);
            font.Size = ItemHeight * rect.Height * 0.7f;

            base.OnAbsoluteBoundsChanged(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
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
            gradientBrush.Dispose();
            textBrush.Dispose();
            font.Dispose();
            format.Dispose();
            
            base.Dispose(disposing);
        }
    }
}
