#region File Description
//-----------------------------------------------------------------------------
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using P3D.Legacy.MapEditor.Data.Models;
using P3D.Legacy.MapEditor.Data.Vertices;

namespace P3D.Legacy.MapEditor.Primitives
{
    public abstract class GeometricPrimitive
    {
        public BaseModel Model { get; set; }

        public void Recalc()
        {
            _positionedVertices = new List<VertexPositionNormalColorTexture>();
            foreach (var vertex in _vertices)
            {
                var scaleMatrix = string.Equals(Model.Entity.EntityID, "Floor", StringComparison.OrdinalIgnoreCase)
                    ? Matrix.CreateScale(1f, 0.2f, 1f) * Matrix.CreateTranslation(new Vector3(0f, -0.5f, 0f))
                    : Matrix.Identity;
                _positionedVertices.Add(new VertexPositionNormalColorTexture(
                    Vector3.Transform(vertex.Position, scaleMatrix * Model.WorldMatrix),
                    Vector3.TransformNormal(vertex.Normal, scaleMatrix * Model.WorldMatrix),
                    Color.White,
                    Vector2.Zero));
            }
        }

        private readonly List<VertexPositionNormal> _vertices = new List<VertexPositionNormal>();
        private readonly List<int> _indices = new List<int>();
        private List<VertexPositionNormalColorTexture> _positionedVertices = new List<VertexPositionNormalColorTexture>();

        protected void AddVertex(VertexPositionNormalTexture vertex) => _vertices.Add(new VertexPositionNormal(vertex.Position, vertex.Normal));
        protected void AddVertex(Vector3 position, Vector3 normal) => _vertices.Add(new VertexPositionNormal(position, normal));
        protected void AddIndex(int index) => _indices.Add(index);

        protected int CurrentVertex => _vertices.Count;

        public void Draw(BasicEffect basicEffect, Color color)
        {
            var graphicsDevice = basicEffect.GraphicsDevice;

            var rasterizerState = graphicsDevice.RasterizerState;
            var blendState = graphicsDevice.BlendState;
            var depthStencilState = graphicsDevice.DepthStencilState;
            var diffuseColor = basicEffect.DiffuseColor;
            var alpha = basicEffect.Alpha;


            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.BlendState = BlendState.AlphaBlend;
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            basicEffect.TextureEnabled = false;
            basicEffect.DiffuseColor = color.ToVector3();
            basicEffect.Alpha = color.A / 255f;

            foreach (var effectPass in basicEffect.CurrentTechnique.Passes)
            {
                effectPass.Apply();
                graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, _positionedVertices.ToArray(), 0, _positionedVertices.Count / 3);
                //graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _positionedVertices.ToArray(), 0,
                //    _positionedVertices.Count, _indices.ToArray(), 0, _indices.Count / 3, VertexPositionNormalColorTexture.VertexDeclaration);
            }


            graphicsDevice.RasterizerState = rasterizerState;
            graphicsDevice.BlendState = blendState;
            graphicsDevice.DepthStencilState = depthStencilState;
            basicEffect.DiffuseColor = diffuseColor;
            basicEffect.Alpha = alpha;
        }
    }
}
