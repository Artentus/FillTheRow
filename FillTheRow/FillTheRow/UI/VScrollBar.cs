﻿using System;
using GameUtils.Graphics;
using GameUtils.Math;
using GameUtils.UI;
using MouseEventArgs = GameUtils.UI.MouseEventArgs;

namespace FillTheRow.UI
{
    public class VScrollBar : UIElement
    {
        readonly LinearGradientBrush brush;
        float maximum;
        float value;
        float sliderLocation;
        float relativeMousePos;
        bool mouseDown;

        public float Maximum
        {
            get { return maximum; }
            set
            {
                maximum = value;
                if (maximum == 0)
                    sliderLocation = 0;
                else
                    sliderLocation = this.value / maximum * MaxSliderLocation;
            }
        }

        public float Value
        {
            get { return value; }
            set
            {
                this.value = value;
                if (maximum == 0)
                    sliderLocation = 0;
                else
                    sliderLocation = this.value / maximum * MaxSliderLocation;
            }
        }

        private float MaxSliderLocation
        {
            get { return AbsoluteSize.Y * 0.7f; }
        }

        public VScrollBar()
        {
            var stops = new GradientStop[4];
            stops[0] = new GradientStop(new Color4(0.1f, 1, 1, 1), 0);
            stops[1] = new GradientStop(new Color4(0.8f, 1, 1, 1), 0.45f);
            stops[2] = new GradientStop(new Color4(0.8f, 1, 1, 1), 0.55f);
            stops[3] = new GradientStop(new Color4(0.1f, 1, 1, 1), 1);
            Rectangle rect = SurfaceBounds;
            brush = new LinearGradientBrush(stops, new Vector2(0, rect.Y + sliderLocation), new Vector2(0, rect.Y + rect.Height * 0.3f + sliderLocation));
        }

        protected override void OnAbsoluteBoundsChanged(EventArgs e)
        {
            if (Root == null)
                return;

            Rectangle rect = SurfaceBounds;
            brush.StartPoint = new Vector2(0, rect.Y + sliderLocation);
            brush.EndPoint = new Vector2(0, rect.Y + rect.Height * 0.3f + sliderLocation);

            base.OnAbsoluteBoundsChanged(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            mouseDown = true;
            relativeMousePos = e.Y * AbsoluteSize.Y - sliderLocation;
            
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            mouseDown = false;
            
            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (mouseDown)
            {
                sliderLocation = MathHelper.Clamp(e.Y * AbsoluteSize.Y - relativeMousePos, 0, MaxSliderLocation);
                value = sliderLocation / MaxSliderLocation * maximum;

                Rectangle rect = SurfaceBounds;
                brush.StartPoint = new Vector2(0, rect.Y + sliderLocation);
                brush.EndPoint = new Vector2(0, rect.Y + rect.Height * 0.3f + sliderLocation);
            }

            base.OnMouseMove(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Renderer.FillRectangle(new Rectangle(e.Bounds.X, e.Bounds.Y + sliderLocation, e.Bounds.Width, e.Bounds.Height * 0.3f), brush);

            base.OnPaint(e);
        }

        protected override void Dispose(bool disposing)
        {
            brush.Dispose();

            base.Dispose(disposing);
        }
    }
}
