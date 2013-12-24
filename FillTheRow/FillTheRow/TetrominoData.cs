using System;
using GameUtils;

namespace FillTheRow
{
    public struct TetrominoData : IResource
    {
        void IResource.ApplyChanges()
        { }

        public object Tag { get; set; }

        public byte[] Data { get; set; }

        void IDisposable.Dispose()
        { }
    }
}
