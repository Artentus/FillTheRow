using System;
using System.Drawing;
using GameUtils;
using GameUtils.Graphics;
using Rectangle = GameUtils.Math.Rectangle;

namespace FillTheRow
{
    public class Block : IRenderable
    {
        //readonly TetrominoManager manager;
        readonly char identifier;

        bool IGameComponent.IsSynchronized
        {
            get { return false; }
        }

        public Point Location { get; set; }

        public Block(char identifier, Point location)
        {
            //this.manager = manager;
            this.identifier = identifier;
            Location = location;
        }

        void IRenderable.Render(Renderer renderer)
        {
            Texture blockImage = GameEngine.QueryResource<Texture>(identifier + "_block"); //manager.BlockImage(identifier);
            renderer.DrawTexture(blockImage, new Rectangle(Location.X * 10, Location.Y * 10, 10, 10));
        }

        void IDisposable.Dispose()
        { }
    }
}
