using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

using P3D.Legacy.MapEditor.Data.Vertices;
using P3D.Legacy.MapEditor.World;

namespace P3D.Legacy.MapEditor.Data.Models
{
    public abstract class BaseVertexRenderer
    {
        public static int DrawCalls = 0;

        public List<VertexPositionNormalColorTexture> Vertices { get; }
        public List<int> Indices { get; }

        public Texture2D Atlas { get; }

        public VertexBuffer StaticVertexBuffer;
        public IndexBuffer StaticIndexBuffer;

        protected BaseVertexRenderer(List<VertexPositionNormalColorTexture> vertices, List<int> indices, Texture2D atlas)
        {
            Vertices = vertices;
            Indices = indices;
            Atlas = atlas;
        }

        public void Setup(GraphicsDevice graphicsDevice)
        {
            StaticVertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionNormalColorTexture), Vertices.Count, BufferUsage.WriteOnly);
            StaticVertexBuffer.SetData(Vertices.ToArray());

            StaticIndexBuffer = new IndexBuffer(graphicsDevice, typeof(int), Vertices.Count, BufferUsage.WriteOnly);
            StaticIndexBuffer.SetData(Indices.ToArray());
        }
    }

    public class OpaqueVertexRenderer : BaseVertexRenderer
    {
        public OpaqueVertexRenderer(List<VertexPositionNormalColorTexture> vertices, List<int> indices, Texture2D atlas) : base(vertices, indices, atlas)
        {
        }

        public void Draw(Level level, BasicEffect basicEffect, CullMode cullMode = CullMode.CullClockwiseFace)
        {
            var graphicsDevice = basicEffect.GraphicsDevice;

            var rasterizerState = graphicsDevice.RasterizerState;
            var blendState = graphicsDevice.BlendState;
            var depthStencilState = graphicsDevice.DepthStencilState;

            graphicsDevice.SetVertexBuffer(StaticVertexBuffer);
            //graphicsDevice.Indices = StaticIndexBuffer;
            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;

            switch (cullMode)
            {
                case CullMode.None:
                    graphicsDevice.RasterizerState = RasterizerState.CullNone;
                    break;

                case CullMode.CullClockwiseFace:
                    graphicsDevice.RasterizerState = RasterizerState.CullClockwise;
                    break;

                case CullMode.CullCounterClockwiseFace:
                    graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                    break;
            }

            basicEffect.Texture = Atlas;
            basicEffect.TextureEnabled = true;

            foreach (var effectPass in basicEffect.CurrentTechnique.Passes)
            {
                effectPass.Apply();

                graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, StaticVertexBuffer.VertexCount / 3);
                //graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, Vertices.Count, 0, Indices.Count / 3);
                DrawCalls++;
            }

            graphicsDevice.RasterizerState = rasterizerState;
            graphicsDevice.BlendState = blendState;
            graphicsDevice.DepthStencilState = depthStencilState;
        }
    }
    public class TransparentVertexRenderer : BaseVertexRenderer
    {
        private DepthStencilState StencilWriteOnly { get; } = new DepthStencilState()
        {
            DepthBufferEnable = true,
            DepthBufferWriteEnable = true,
            //CounterClockwiseStencilFunction = CompareFunction.Always,
            StencilFunction = CompareFunction.Always,
            StencilPass = StencilOperation.Replace,
            //StencilFail = StencilOperation.IncrementSaturation,
            //StencilPass = StencilOperation.IncrementSaturation,
            //CounterClockwiseStencilFail = StencilOperation.IncrementSaturation,
            //CounterClockwiseStencilPass = StencilOperation.IncrementSaturation,
            ReferenceStencil = 0,
            StencilEnable = true,
            //StencilMask = 0,
        };
        private DepthStencilState StencilReadOnly { get; } = new DepthStencilState()
        {
            DepthBufferEnable = true,
            DepthBufferWriteEnable = false,
            //CounterClockwiseStencilFunction = CompareFunction.Less,
            StencilFunction = CompareFunction.Equal,
            StencilPass = StencilOperation.Keep,
            //StencilFunction = CompareFunction.Less,
            //StencilFail = StencilOperation.Keep,
            //StencilPass = StencilOperation.Keep,
            //CounterClockwiseStencilFail = StencilOperation.Keep,
            //CounterClockwiseStencilPass = StencilOperation.Keep,
            ReferenceStencil = 0,
            StencilEnable = true
        };

        public TransparentVertexRenderer(List<VertexPositionNormalColorTexture> vertices, List<int> indices, Texture2D atlas) : base(vertices, indices, atlas)
        {
        }

        public void Draw(Level level, BasicEffect basicEffect, AlphaTestEffect alphaEffect, CullMode cullMode = CullMode.CullClockwiseFace)
        {
            // You could (at higher GPU cost) do a two pass render with stencil: 
            // first AlphaTestEffect with color writes disabled to draw a mask 
            // into the stencil buffer, then a second pass with BasicEffect to 
            // render colors only where that stencil mask allows
            // semi-transparent pixels won't work

            var graphicsDevice = basicEffect.GraphicsDevice;

            var rasterizerState = graphicsDevice.RasterizerState;
            var blendState = graphicsDevice.BlendState;
            var depthStencilState = graphicsDevice.DepthStencilState;

            graphicsDevice.SetVertexBuffer(StaticVertexBuffer);

            basicEffect.Texture = Atlas;
            alphaEffect.Texture = Atlas;

            switch (cullMode)
            {
                case CullMode.None:
                    graphicsDevice.RasterizerState = RasterizerState.CullNone;
                    break;

                case CullMode.CullClockwiseFace:
                    graphicsDevice.RasterizerState = RasterizerState.CullClockwise;
                    break;

                case CullMode.CullCounterClockwiseFace:
                    graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                    break;
            }

            graphicsDevice.DepthStencilState = StencilWriteOnly;
            graphicsDevice.BlendState = new BlendState()
            {
                ColorWriteChannels = ColorWriteChannels.Alpha,
                ColorWriteChannels1 = ColorWriteChannels.Alpha,
                ColorWriteChannels2 = ColorWriteChannels.Alpha,
                ColorWriteChannels3 = ColorWriteChannels.Alpha,
            };
            foreach (var effectPass in alphaEffect.CurrentTechnique.Passes)
            {
                effectPass.Apply();
                graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, StaticVertexBuffer.VertexCount / 3);
                DrawCalls++;
            }

            graphicsDevice.DepthStencilState = StencilReadOnly;
            graphicsDevice.BlendState = BlendState.AlphaBlend;
            foreach (var effectPass in basicEffect.CurrentTechnique.Passes)
            {
                effectPass.Apply();
                graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, StaticVertexBuffer.VertexCount / 3);
                DrawCalls++;
            }

            graphicsDevice.RasterizerState = rasterizerState;
            graphicsDevice.BlendState = blendState;
            graphicsDevice.DepthStencilState = depthStencilState;
        }
    }
}