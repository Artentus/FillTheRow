using System;
using System.IO;
using GameUtils;
using GameUtils.Graphics;

namespace FillTheRow
{
    public class TetrominoManager : IEngineComponent
    {
        readonly char[] identifiers;

        bool IEngineComponent.IsCompatibleTo(IEngineComponent component)
        {
            return !(component is TetrominoManager);
        }

        object IEngineComponent.Tag
        {
            get { return null; }
        }

        public char[] Identifiers
        {
            get { return identifiers; }
        }

        public TetrominoManager()
        {
            var tetriminos = new Tetrominos();
            tetriminos.ReadXml(Path.Combine(Environment.CurrentDirectory, "TetrominoTemplates.dat"));
            var templates = tetriminos.TetrominoTemplates;

            identifiers = new char[templates.Count];
            for (int i = 0; i < Identifiers.Length; i++)
                identifiers[i] = templates[i].Identifier;

            using (var fs = new FileStream(Path.Combine(Environment.CurrentDirectory, "Tetrominos.dat"), FileMode.Open))
            {
                while (fs.Position < fs.Length)
                {
                    char identifier = (char)fs.ReadByte();
                    var bytes = new byte[3];
                    fs.Read(bytes, 0, 3);
                    var data = new TetrominoData();
                    data.Data = bytes;
                    data.Tag = identifier + "_data";
                    GameEngine.RegisterResource(data);
                }
            }

            for (int i = 0; i < templates.Count; i++)
            {
                char identifier = templates[i].Identifier;
                var image = new Texture(templates[i].ImagePath);
                image.Tag = identifier + "_image";
                var block = new Texture(templates[i].BlockImagePath);
                block.Tag = identifier + "_block";
            }
        }
    }
}
