using System;
using Artentus.GameUtils;
using Artentus.GameUtils.Graphics;

namespace FillTheRow
{
    public class GameLayer : Layer
    {
        protected override void Render(Renderer renderer)
        {
            const float defaultWidth = 180;
            const float defaultHeight = 280;
            float factor = Math.Min(Game.Window.ClientSize.Width / defaultWidth, Game.Window.ClientSize.Height / defaultHeight);
            Matrix2x3 transform = Matrix2x3.Translation((Game.Window.ClientSize.Width - defaultWidth * factor) / 2, (Game.Window.ClientSize.Height - defaultHeight * factor) / 2) * Matrix2x3.Scaling(factor) * Matrix2x3.Translation(40, 60);
            renderer.SetTransform(transform);
            base.Render(renderer);
            renderer.ResetTransform();
        }
    }
}
