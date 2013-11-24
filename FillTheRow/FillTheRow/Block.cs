using System;
using System.Drawing;
using Artentus.GameUtils;
using Artentus.GameUtils.Graphics;
using Rectangle = Artentus.GameUtils.Graphics.Rectangle;

namespace FillTheRow
{
    public class Block : IRenderable
    {
        readonly TetrominoManager manager;
        readonly char identifier;

        bool IGameComponent.IsSynchronized
        {
            get { return false; }
        }

        public Point Location { get; set; }

        public Block(TetrominoManager manager, char identifier, Point location)
        {
            this.manager = manager;
            this.identifier = identifier;
            Location = location;
        }

        void IRenderable.FactoryChanged(Factory factory)
        { }

        void IRenderable.Render(Renderer renderer)
        {
            Texture blockImage = manager.BlockImage(identifier);
            renderer.DrawTexture(blockImage, new Rectangle(Location.X * 10, Location.Y * 10, 10, 10));
        }

        void IDisposable.Dispose()
        { }
    }
}
