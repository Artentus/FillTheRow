using System;
using System.Collections.Generic;
using System.IO;
using Artentus.GameUtils;
using Artentus.GameUtils.Graphics;

namespace FillTheRow
{
    public class TetrominoManager : IRenderable
    {
        readonly Tetrominos.TetrominoTemplatesDataTable templates;
        readonly char[] identifiers;
        readonly Dictionary<char, Texture> images;
        readonly Dictionary<char, Texture> blockImages;
        readonly Dictionary<char, byte[]> data;

        bool IGameComponent.IsSynchronized
        {
            get { return false; }
        }

        public char[] Identifiers
        {
            get { return identifiers; }
        }

        public Texture Image(char identifier)
        {
            return images.ContainsKey(identifier) ? images[identifier] : null;
        }

        public Texture BlockImage(char identifier)
        {
            return blockImages.ContainsKey(identifier) ? blockImages[identifier] : null;
        }

        public byte[] Data(char identifier)
        {
            return data[identifier];
        }

        public TetrominoManager()
        {
            var tetriminos = new Tetrominos();
            tetriminos.ReadXml(Path.Combine(Environment.CurrentDirectory, "TetrominoTemplates.dat"));
            templates = tetriminos.TetrominoTemplates;

            identifiers = new char[templates.Count];
            for (int i = 0; i < Identifiers.Length; i++)
                identifiers[i] = templates[i].Identifier;

            images = new Dictionary<char, Texture>(templates.Count);
            blockImages = new Dictionary<char, Texture>(templates.Count);

            data = new Dictionary<char, byte[]>(templates.Count);
            using (var fs = new FileStream(Path.Combine(Environment.CurrentDirectory, "Tetrominos.dat"), FileMode.Open))
            {
                while (fs.Position < fs.Length)
                {
                    char identifier = (char)fs.ReadByte();
                    var bytes = new byte[3];
                    fs.Read(bytes, 0, 3);
                    data.Add(identifier, bytes);
                }
            }
        }

        void IRenderable.FactoryChanged(Factory factory)
        {
            for (int i = 0; i < templates.Count; i++)
            {
                char identifier = templates[i].Identifier;
                if (images.ContainsKey(identifier)) images[identifier].Dispose();
                if (blockImages.ContainsKey(identifier)) blockImages[identifier].Dispose();
                if (factory != null)
                {
                    images[identifier] = factory.CreateTexture(templates[i].ImagePath);
                    blockImages[identifier] = factory.CreateTexture(templates[i].BlockImagePath);
                }
            }
        }

        void IRenderable.Render(Renderer renderer)
        { }

        private bool disposed;

        public void Dispose()
        {
            if (!disposed)
            {
                Dispose(true);
                GC.SuppressFinalize(this);

                disposed = true;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            foreach (var pair in images)
                pair.Value.Dispose();

            foreach (var pair in blockImages)
                pair.Value.Dispose();
        }

        ~TetrominoManager()
        {
            Dispose(false);
        }
    }
}
