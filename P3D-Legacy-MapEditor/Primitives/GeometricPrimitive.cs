#region File Description
//-----------------------------------------------------------------------------
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using P3D.Legacy.MapEditor.Data;

namespace P3D.Legacy.MapEditor.Primitives
{
    /// <summary>
    /// Base class for simple geometric primitive models. This provides a vertex
    /// buffer, an index buffer, plus methods for drawing the model. Classes for
    /// specific types of primitive (CubePrimitive, SpherePrimitive, etc.) are
    /// derived from this common base, and use the AddVertex and AddIndex methods
    /// to specify their geometry.
    /// </summary>
    public abstract class GeometricPrimitive : IDisposable
    {
        #region Fields

        // During the process of constructing a primitive model, vertex
        // and index data is stored on the CPU in these managed lists.
        private readonly List<VertexPositionNormal> _vertices = new List<VertexPositionNormal>();
        private readonly List<ushort> _indices = new List<ushort>();

        // Once all the geometry has been specified, the InitializePrimitive
        // method copies the vertex and index data into these buffers, which
        // store it on the GPU ready for efficient rendering.
        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;
        //private Effect _effect;

        #endregion

        #region Initialization

        /// <summary>
        /// Adds a new vertex to the primitive model. This should only be called
        /// during the initialization process, before InitializePrimitive.
        /// </summary>
        protected void AddVertex(Vector3 position, Vector3 normal)
        {
            _vertices.Add(new VertexPositionNormal(position, normal));
            //_vertices.Add(new VertexWorldNormalTexture(
            //    Matrix.Transpose(Matrix.CreateTranslation(position)), 
            //    normal, 
            //    Vector2.Zero));
        }

        /// <summary>
        /// Adds a new index to the primitive model. This should only be called
        /// during the initialization process, before InitializePrimitive.
        /// </summary>
        protected void AddIndex(int index)
        {
            if (index > ushort.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(index));

            _indices.Add((ushort)index);
        }

        /// <summary>
        /// Queries the index of the current vertex. This starts at
        /// zero, and increments every time AddVertex is called.
        /// </summary>
        protected int CurrentVertex => _vertices.Count;


        /// <summary>
        /// Once all the geometry has been specified by calling AddVertex and AddIndex,
        /// this method copies the vertex and index data into GPU format buffers, ready
        /// for efficient rendering.
        /// </summary>
        public void Initialize(GraphicsDevice graphicsDevice)
        {
            // Create a vertex declaration, describing the format of our vertex data.

            // Create a vertex buffer, and copy our vertex data into it.
            _vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionNormal), _vertices.Count, BufferUsage.None);

            _vertexBuffer.SetData(_vertices.ToArray());

            // Create an index buffer, and copy our index data into it.
            _indexBuffer = new IndexBuffer(graphicsDevice, typeof(ushort), _indices.Count, BufferUsage.None);

            _indexBuffer.SetData(_indices.ToArray());

            //var reader = new BinaryReader(File.Open("C:\\GitHub\\P3D-Legacy-MapEditor — копия\\P3D-Legacy-MapEditor\\Content\\Shader.mgfx", FileMode.Open));
            //_effect = new Effect(graphicsDevice, reader.ReadBytes((int) reader.BaseStream.Length));
            //reader.Dispose();

            // Create a BasicEffect, which will be used to render the primitive.
            //_effect = new BasicEffect(graphicsDevice);

            //_effect.EnableDefaultLighting();            
        }

        /// <summary>
        /// Finalizer.
        /// </summary>
        ~GeometricPrimitive()
        {
            Dispose(false);
        }

        /// <summary>
        /// Frees resources used by this object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Frees resources used by this object.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _vertexBuffer?.Dispose();

                _indexBuffer?.Dispose();

                //_effect?.Dispose();
            }
        }

        #endregion

        #region Draw

        /// <summary>
        /// Draws the primitive model, using the specified effect. Unlike the other
        /// Draw overload where you just specify the world/view/projection matrices
        /// and color, this method does not set any renderstates, so you must make
        /// sure all states are set to sensible values before you call it.
        /// </summary>
        public void Draw(Effect effect)
        {
            var graphicsDevice = effect.GraphicsDevice;

            // Set our vertex declaration, vertex buffer, and index buffer.
            graphicsDevice.SetVertexBuffer(_vertexBuffer);

            graphicsDevice.Indices = _indexBuffer;            

            foreach (var effectPass in effect.CurrentTechnique.Passes)
            {
                effectPass.Apply();

                var primitiveCount = _indices.Count / 3;

                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _vertices.Count, 0, primitiveCount);
            }
        }

        /// <summary>
        /// Draws the primitive model, using a BasicEffect shader with default
        /// lighting. Unlike the other Draw overload where you specify a custom
        /// effect, this method sets important renderstates to sensible values
        /// for 3D model rendering, so you do not need to set these states before
        /// you call it.
        /// </summary>
        public void Draw(Effect effect, Color color)
        {
            // Set BasicEffect parameters.
            /*
            _effect.World = world;
            _effect.View = view;
            _effect.Projection = projection;
            _effect.DiffuseColor = color.ToVector3();
            _effect.Alpha = color.A / 255.0f
            */

            //_effect.Parameters["ViewProjection"].SetValue(view * projection);
            effect.Parameters["DiffuseColor"].SetValue(color.ToVector4());
            //effect.Parameters["Alpha"].SetValue(color.A / 255.0f);

            var device = effect.GraphicsDevice;

            device.DepthStencilState = DepthStencilState.Default;

            device.BlendState = color.A < 255 ? BlendState.AlphaBlend : BlendState.Opaque;

            // Draw the model, using BasicEffect.
            Draw(effect);
        }

        #endregion
    }
}
