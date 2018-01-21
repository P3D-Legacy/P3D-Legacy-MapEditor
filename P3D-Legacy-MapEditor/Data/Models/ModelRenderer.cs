using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

using P3D.Legacy.MapEditor.Data.Vertices;
using P3D.Legacy.MapEditor.World;

namespace P3D.Legacy.MapEditor.Data.Models
{
    public class ModelRenderer
    {
        public static int DrawCalls = 0;

        public List<VertexPositionNormalColorTexture> Vertices { get; } = new List<VertexPositionNormalColorTexture>();
        public List<int> Indices { get; } = new List<int>();

        public Texture2D Atlas;

        public VertexBuffer StaticVertexBuffer;
        public IndexBuffer StaticIndexBuffer;

        public void Setup(GraphicsDevice graphicsDevice)
        {
            StaticVertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionNormalColorTexture), Vertices.Count, BufferUsage.WriteOnly);
            StaticVertexBuffer.SetData(Vertices.ToArray());

            StaticIndexBuffer = new IndexBuffer(graphicsDevice, typeof(int), Vertices.Count, BufferUsage.WriteOnly);
            StaticIndexBuffer.SetData(Indices.ToArray());
        }

        public void Draw(Level level, BasicEffect effect)
        {
            var rasterizerState = effect.GraphicsDevice.RasterizerState;
            var blendState = effect.GraphicsDevice.BlendState;
            var depthStencilState = effect.GraphicsDevice.DepthStencilState;

            effect.GraphicsDevice.SetVertexBuffer(StaticVertexBuffer);
            effect.GraphicsDevice.Indices = StaticIndexBuffer;
            effect.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            effect.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            effect.Texture = Atlas;
            effect.TextureEnabled = true;

            foreach (var effectPass in effect.CurrentTechnique.Passes)
            {
                effectPass.Apply();
                effect.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                effect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, StaticVertexBuffer.VertexCount / 3);
                //effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, Vertices.Count, 0, Indices.Count / 3);
                DrawCalls++;
            }

            foreach (var effectPass in effect.CurrentTechnique.Passes)
            {
                effectPass.Apply();
                effect.GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;
                effect.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, StaticVertexBuffer.VertexCount / 3);
                //effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, 0, Vertices.Count, 0, Indices.Count / 3);
                DrawCalls++;
            }

            effect.GraphicsDevice.RasterizerState = rasterizerState;
            effect.GraphicsDevice.BlendState = blendState;
            effect.GraphicsDevice.DepthStencilState = depthStencilState;
        }
    }
}