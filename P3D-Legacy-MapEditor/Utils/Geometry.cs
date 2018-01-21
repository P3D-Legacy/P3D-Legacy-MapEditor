using System;
using System.Collections.Generic;
using System.Linq;

namespace P3D.Legacy.MapEditor.Utils
{
    //https://github.com/nilllzz/Game-Dev-Common/blob/5d3e220b7fe20b51d6046ebb7fc95e0a07ab4b9d/src/GameDevCommon/Rendering/Geometry.cs
    public sealed class Geometry<TVertexType> : IDisposable where TVertexType : struct
    {
        private List<TVertexType> _vertices = new List<TVertexType>();
        private List<int> _indices = new List<int>();

        public TVertexType[] Vertices => _vertices.ToArray();
        public int[] Indices => _indices.ToArray();
        public bool IsDisposed { get; private set; }

        public void AddVertices(TVertexType[] vertices)
        {
            foreach (var vertex in vertices)
            {
                var index = _vertices.IndexOf(vertex);
                if (index == -1)
                {
                    _indices.Add(_vertices.Count);
                    _vertices.Add(vertex);
                }
                else
                {
                    _indices.Add(index);
                }
            }
        }

        public void AddIndexedVertices(IEnumerable<TVertexType> vertices)
        {
            foreach (var vertex in vertices)
            {
                if (!_vertices.Contains(vertex))
                    _vertices.Add(vertex);
            }
        }

        public void AddIndices(IEnumerable<int> indices)
        {
            _indices.AddRange(indices);
        }

        public void CopyTo(Geometry<TVertexType> target)
        {
            // using ToList() to create shallow copies
            target._vertices = _vertices.ToList();
            target._indices = _indices.ToList();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Geometry()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                _vertices = null;
                _indices = null;

                IsDisposed = true;
            }
        }
    }
}
