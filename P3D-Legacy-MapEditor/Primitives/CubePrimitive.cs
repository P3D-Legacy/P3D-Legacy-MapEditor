﻿#region File Description
//-----------------------------------------------------------------------------
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using P3D.Legacy.MapEditor.Data.Vertices;

namespace P3D.Legacy.MapEditor.Primitives
{
    /// <summary>
    /// Geometric primitive class for drawing cubes.
    /// </summary>
    public class CubePrimitive : GeometricPrimitive
    {
        /// <summary>
        /// Constructs a new cube primitive, using default settings.
        /// </summary>
        public CubePrimitive()
            : this(1.1f)
        {
        }

        /// <summary>
        /// Constructs a new cube primitive, with the specified size.
        /// </summary>
        public CubePrimitive(float size)
        {
            AddVertex(new Vector3(-0.5f, -0.5f, 0.5f) * size, new Vector3(0, 0, 1)); //h
            AddVertex(new Vector3(-0.5f, 0.5f, 0.5f) * size, new Vector3(0, 0, 1)); //e
            AddVertex(new Vector3(0.5f, -0.5f, 0.5f) * size, new Vector3(0, 0, 1)); //c

            AddVertex(new Vector3(0.5f, -0.5f, 0.5f) * size, new Vector3(0, 0, 1)); //c
            AddVertex(new Vector3(-0.5f, 0.5f, 0.5f) * size, new Vector3(0, 0, 1)); //e
            AddVertex(new Vector3(0.5f, 0.5f, 0.5f) * size, new Vector3(0, 0, 1)); //d

            AddVertex(new Vector3(0.5f, -0.5f, 0.5f) * size, new Vector3(1, 0, 0)); //c
            AddVertex(new Vector3(0.5f, 0.5f, 0.5f) * size, new Vector3(1, 0, 0)); //d
            AddVertex(new Vector3(0.5f, -0.5f, -0.5f) * size, new Vector3(1, 0, 0)); //b

            AddVertex(new Vector3(0.5f, -0.5f, -0.5f) * size, new Vector3(1, 0, 0)); //b
            AddVertex(new Vector3(0.5f, 0.5f, 0.5f) * size, new Vector3(1, 0, 0)); //d
            AddVertex(new Vector3(0.5f, 0.5f, -0.5f) * size, new Vector3(1, 0, 0)); //g

            AddVertex(new Vector3(-0.5f, 0.5f, 0.5f) * size, new Vector3(-1, 0, 0)); //e
            AddVertex(new Vector3(-0.5f, -0.5f, 0.5f) * size, new Vector3(-1, 0, 0)); //h
            AddVertex(new Vector3(-0.5f, -0.5f, -0.5f) * size, new Vector3(-1, 0, 0)); //a

            AddVertex(new Vector3(-0.5f, -0.5f, -0.5f) * size, new Vector3(-1, 0, 0)); //a
            AddVertex(new Vector3(-0.5f, 0.5f, -0.5f) * size, new Vector3(-1, 0, 0)); //f
            AddVertex(new Vector3(-0.5f, 0.5f, 0.5f) * size, new Vector3(-1, 0, 0)); //e

            AddVertex(new Vector3(-0.5f, 0.5f, -0.5f) * size, new Vector3(0, 0, -1)); //f
            AddVertex(new Vector3(-0.5f, -0.5f, -0.5f) * size, new Vector3(0, 0, -1)); //a
            AddVertex(new Vector3(0.5f, -0.5f, -0.5f) * size, new Vector3(0, 0, -1)); //b

            AddVertex(new Vector3(0.5f, -0.5f, -0.5f) * size, new Vector3(0, 0, -1)); //b
            AddVertex(new Vector3(0.5f, 0.5f, -0.5f) * size, new Vector3(0, 0, -1)); //g
            AddVertex(new Vector3(-0.5f, 0.5f, -0.5f) * size, new Vector3(0, 0, -1)); //f

            AddVertex(new Vector3(-0.5f, 0.5f, 0.5f) * size, Vector3.Up);
            AddVertex(new Vector3(-0.5f, 0.5f, -0.5f) * size, Vector3.Up);
            AddVertex(new Vector3(0.5f, 0.5f, -0.5f) * size, Vector3.Up);

            AddVertex(new Vector3(0.5f, 0.5f, -0.5f) * size, Vector3.Up);
            AddVertex(new Vector3(0.5f, 0.5f, 0.5f) * size, Vector3.Up);
            AddVertex(new Vector3(-0.5f, 0.5f, 0.5f) * size, Vector3.Up);

            AddVertex(new Vector3(-0.5f, -0.5f, 0.5f) * size, Vector3.Down);
            AddVertex(new Vector3(-0.5f, -0.5f, -0.5f) * size, Vector3.Down);
            AddVertex(new Vector3(0.5f, -0.5f, -0.5f) * size, Vector3.Down);

            AddVertex(new Vector3(0.5f, -0.5f, -0.5f) * size, Vector3.Down);
            AddVertex(new Vector3(0.5f, -0.5f, 0.5f) * size, Vector3.Down);
            AddVertex(new Vector3(-0.5f, -0.5f, 0.5f) * size, Vector3.Down);

            /*
            // A cube has six faces, each one pointing in a different direction.
            Vector3[] normals =
            {
                new Vector3(0, 0, 1),
                new Vector3(0, 0, -1),
                new Vector3(1, 0, 0),
                new Vector3(-1, 0, 0),
                new Vector3(0, 1, 0),
                new Vector3(0, -1, 0),
            };

            // Create each face in turn.
            foreach (var normal in normals)
            {
                // Get two vectors perpendicular to the face normal and to each other.
                var side1 = new Vector3(normal.Y, normal.Z, normal.X);
                var side2 = Vector3.Cross(normal, side1);

                // Six indices (two triangles) per face.
                AddIndex(CurrentVertex + 0);
                AddIndex(CurrentVertex + 1);
                AddIndex(CurrentVertex + 2);

                AddIndex(CurrentVertex + 0);
                AddIndex(CurrentVertex + 2);
                AddIndex(CurrentVertex + 3);

                // Four vertices per face.
                AddVertex((normal - side1 - side2) * size / 2, normal);
                AddVertex((normal - side1 + side2) * size / 2, normal);
                AddVertex((normal + side1 + side2) * size / 2, normal);
                AddVertex((normal + side1 - side2) * size / 2, normal);
            }
            */
        }
    }
}
