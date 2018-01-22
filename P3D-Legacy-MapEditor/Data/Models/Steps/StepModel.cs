﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.Legacy.MapEditor.Data.Models
{
    public class StepModel : BaseModel<StepModel>
    {
        public override int ID => 7;

        public StepModel(EntityInfo entity, GraphicsDevice graphicsDevice) : base(entity, graphicsDevice)
        {
            if (ModelVertices.Count > 0)
            {
                Setup();
                return;
            }

            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(0.0f, 1.0f))); //h
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), new Vector3(0, 0, 1), new Vector2(0.0f, 0.0f))); //e
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(1.0f, 1.0f))); //c

            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(0, 0, 1), new Vector2(1.0f, 1.0f))); //c
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), new Vector3(0, 0, 1), new Vector2(0.0f, 0.0f))); //e
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.25f, 0.5f), new Vector3(0, 0, 1), new Vector2(1.0f, 0.0f))); //d

            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, 0.5f), new Vector3(1, 0, 0), new Vector2(0.0f, 1.0f))); //c
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.25f, 0.5f), new Vector3(1, 0, 0), new Vector2(0.0f, 0.0f))); //d
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(1, 0, 0), new Vector2(1.0f, 1.0f))); //b

            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1.0f, 0.0f))); //e
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0.5f), new Vector3(-1, 0, 0), new Vector2(1.0f, 1.0f))); //h
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-1, 0, 0), new Vector2(0.0f, 1.0f))); //a

            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), new Vector3(0, 0, -1), new Vector2(1, 0))); //e
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(1, 1))); //a
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(0, 1))); //b

            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector3(0, 0, -1), new Vector2(0, 1))); //b
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(0.5f, -0.25f, 0.5f), new Vector3(0, 0, -1), new Vector2(0, 0))); //d
            ModelVertices.Add(new VertexPositionNormalTexture(new Vector3(-0.5f, -0.25f, 0.5f), new Vector3(0, 0, -1), new Vector2(1, 0))); //e

            Setup();
        }

    }
}