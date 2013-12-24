using System;
using GameUtils;
using GameUtils.Graphics;
using GameUtils.Math;

namespace FillTheRow
{
    public class GameLayer : Layer
    {
        protected override void Render(Renderer renderer)
        {
            const float defaultWidth = 180;
            const float defaultHeight = 280;
            float factor = Math.Min(renderer.SurfaceBounds.Width / defaultWidth, renderer.SurfaceBounds.Height / defaultHeight);
            Matrix2x3 transform = Matrix2x3.Translation((renderer.SurfaceBounds.Width - defaultWidth * factor) / 2, (renderer.SurfaceBounds.Height - defaultHeight * factor) / 2) * Matrix2x3.Scaling(factor) * Matrix2x3.Translation(40, 60);
            renderer.SetTransform(transform);
            base.Render(renderer);
            renderer.ResetTransform();
        }
    }
}
