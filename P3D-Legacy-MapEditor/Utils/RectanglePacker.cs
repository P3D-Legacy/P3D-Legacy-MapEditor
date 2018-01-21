using System;
using System.Collections.Generic;

namespace P3D.Legacy.MapEditor.Utils
{
    // https://github.com/ChevyRay/RectanglePacker/blob/1a3f0fd49b79a8a1bc9d5d15624ed645ef212b06/RectanglePacker/RectanglePacker.cs
    // Some shitty simple rectangle packer within rectangle
    public class RectanglePacker
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        List<Node> nodes = new List<Node>();

        public RectanglePacker(int maxWidth, int maxHeight)
        {
            nodes.Add(new Node(0, 0, maxWidth, maxHeight));
        }

        public bool Pack(int w, int h, out int x, out int y)
        {
            for (int i = 0; i < nodes.Count; ++i)
            {
                if (w <= nodes[i].W && h <= nodes[i].H)
                {
                    var node = nodes[i];
                    nodes.RemoveAt(i);
                    x = node.X;
                    y = node.Y;
                    int r = x + w;
                    int b = y + h;
                    nodes.Add(new Node(r, y, node.Right - r, h));
                    nodes.Add(new Node(x, b, w, node.Bottom - b));
                    nodes.Add(new Node(r, b, node.Right - r, node.Bottom - b));
                    Width = Math.Max(Width, r);
                    Height = Math.Max(Height, b);
                    return true;
                }
            }

            x = 0;
            y = 0;
            return false;
        }

        public struct Node
        {
            public int X;
            public int Y;
            public int W;
            public int H;

            public Node(int x, int y, int w, int h)
            {
                X = x;
                Y = y;
                W = w;
                H = h;
            }

            public int Right => X + W;

            public int Bottom => Y + H;
        }
    }
}