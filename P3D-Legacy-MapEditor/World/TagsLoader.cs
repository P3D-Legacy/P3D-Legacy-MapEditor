using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework;
using P3D.Legacy.MapEditor.Data;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace P3D.Legacy.MapEditor.World
{
    public static class TagsLoader
    {
        public static IList<EntityFloorInfo> LoadFloor(LevelTags tags)
        {
            var floorInfo = new EntityFloorInfo();
            var list = new List<EntityFloorInfo>();

            var sizeList = tags.GetTag<int[]>("Size");
            floorInfo.Size = new Size(sizeList[0], sizeList[1]);
            var posList = tags.GetTag<int[]>("Position");
            floorInfo.Position = new Vector3(posList[0], posList[1], posList[2]);
            floorInfo.TexturePath = tags.GetTag<string>("TexturePath");
            floorInfo.TextureRectangles = new [] { tags.GetTag<Rectangle>("Texture") };

            if (tags.TagExists("Visible"))
                floorInfo.Visible = tags.GetTag<bool>("Visible");

            if (tags.TagExists("Shader"))
            {
                var shaderList = tags.GetTag<float[]>("Shader");
                floorInfo.Shader = new Color(shaderList[0], shaderList[1], shaderList[2]);
            }

            if (tags.TagExists("Remove"))
                floorInfo.RemoveFloor = tags.GetTag<bool>("Remove");

            if (tags.TagExists("hasSnow"))
                floorInfo.HasSnow = tags.GetTag<bool>("hasSnow");

            if (tags.TagExists("hasSand"))
                floorInfo.HasSand = tags.GetTag<bool>("hasSand");

            if (tags.TagExists("isIce"))
                floorInfo.HasIce = tags.GetTag<bool>("isIce");

            if (tags.TagExists("Rotation"))
            {
                var rotation = tags.GetTag<int>("Rotation");
                // TODO
                /*
                switch (rotation)
                {
                    case 0:
                        floorInfo.Rotation.Y = 0;
                        break;

                    case 1:
                        floorInfo.Rotation.Y = MathHelper.PiOver2;
                        break;

                    case 2:
                        floorInfo.Rotation.Y = MathHelper.Pi;
                        break;

                    case 3:
                        floorInfo.Rotation.Y = MathHelper.Pi * 1.5F;
                        break;
                }
                */
            }

            if (tags.TagExists("SeasonTexture"))
                floorInfo.SeasonTexture = tags.GetTag<string>("SeasonTexture");

            if(floorInfo.Size.Width > 0 || floorInfo.Size.Height > 0)
            {
                for (int x = 0; x < floorInfo.Size.Width; x++)
                for (int z = 0; z < floorInfo.Size.Height; z++)
                {
                    var ent1 = floorInfo.ShallowCopy();
                    ent1.Position += new Vector3(x, 0, z);
                    list.Add((EntityFloorInfo) ent1);
                }
            }
            else
                list.Add(floorInfo);

            return list;
        }

        public static EntityNPCInfo LoadNpc(LevelTags tags)
        {
            var npcInfo = new EntityNPCInfo();

            var posList = tags.GetTag<float[]>("Position");
            npcInfo.Position = new Vector3(posList[0], posList[1], posList[2]);

            if (tags.TagExists("Scale"))
            {
                var scaleList = tags.GetTag<float[]>("Scale");
                npcInfo.Scale = new Vector3(scaleList[0], scaleList[1], scaleList[2]);
            }

            npcInfo.TextureID = tags.GetTag<string>("TextureID");

            npcInfo.FaceRotation = tags.GetTag<int>("Rotation");

            npcInfo.ActionValue = tags.GetTag<int>("Action");

            npcInfo.AdditionalValue = tags.GetTag<string>("AdditionalValue");

            npcInfo.Name = tags.GetTag<string>("Name");

            npcInfo.ID = tags.GetTag<int>("ID");

            npcInfo.Movement = tags.GetTag<string>("Movement");

            npcInfo.MoveRectangles = tags.GetTag<Rectangle[]>("MoveRectangles");

            if (tags.TagExists("Shader"))
            {
                var shaderList = tags.GetTag<float[]>("Shader");
                npcInfo.Shader = new Color(shaderList[0], shaderList[1], shaderList[2]);
            }

            if (tags.TagExists("AnimateIdle"))
                npcInfo.AnimateIdle = tags.GetTag<bool>("AnimateIdle");

            return npcInfo;
        }

        public static ShaderInfo LoadShader(LevelTags tags)
        {
            var shaderInfo = new ShaderInfo();

            var sizeList = tags.GetTag<int[]>("Size");
            shaderInfo.Size = new Vector3(sizeList[0], 1, sizeList[1]);
            if (sizeList.Length == 3)
                shaderInfo.Size = new Vector3(sizeList[0], sizeList[1], sizeList[2]);
            var shaderList = tags.GetTag<float[]>("Shader");
            shaderInfo.Shader = new Vector3(shaderList[0], shaderList[1], shaderList[2]);
            shaderInfo.StopOnContact = tags.GetTag<bool>("StopOnContact");
            var posList = tags.GetTag<int[]>("Position");
            shaderInfo.Position = new Vector3(posList[0], posList[1], posList[2]);
            var objectSizeList = tags.GetTag<int[]>("Size");
            shaderInfo.ObjectSize = new Size(objectSizeList[0], objectSizeList[1]);
            var dayTime = default(int[]);
            if (tags.TagExists("DayTime"))
                shaderInfo.DayTime = tags.GetTag<int[]>("DayTime");

            return shaderInfo;
        }

        public static BackdropInfo LoadBackdrop(LevelTags tags)
        {
            var backdropInfo = new BackdropInfo();

            var sizeList = tags.GetTag<int[]>("Size");
            backdropInfo.Width = sizeList[0];
            backdropInfo.Height = sizeList[1];

            var posList = tags.GetTag<float[]>("Position");
            backdropInfo.Position = new Vector3(posList[0], posList[1], posList[2]);

            if (tags.TagExists("Rotation"))
            {
                var rotationList = tags.GetTag<float[]>("Rotation");
                backdropInfo.Rotation = new Vector3(rotationList[0], rotationList[1], rotationList[2]);
            }

            backdropInfo.BackdropType = tags.GetTag<string>("Type");

            backdropInfo.TexturePath = tags.GetTag<string>("TexturePath");
            backdropInfo.TextureRectangle = tags.GetTag<Rectangle>("Texture");

            if (tags.TagExists("Trigger"))
                backdropInfo.Trigger = tags.GetTag<string>("Trigger");

            return backdropInfo;
        }

        public static OffsetMapInfo LoadOffsetMap(LevelTags tags)
        {
            var offsetMapInfo = new OffsetMapInfo();

            var offsetList = tags.GetTag<int[]>("Offset");
            offsetMapInfo.Offset = new Vector3(offsetList[0], 0, offsetList[1]);
            if (offsetList.Length >= 3)
                offsetMapInfo.Offset = new Vector3(offsetList[0], offsetList[1], offsetList[2]);

            offsetMapInfo.Map = tags.GetTag<string>("Map");

            return offsetMapInfo;
        }

        public static IList<EntityInfo> LoadEntityField(LevelTags tags)
        {
            var sizeList = tags.GetTag<int[]>("Size");

            var fill = true;
            if (tags.TagExists("Fill"))
                fill = tags.GetTag<bool>("Fill");

            var steps = new Vector3(1, 1, 1);
            if (tags.TagExists("Steps"))
            {
                var stepList = tags.GetTag<float[]>("Steps");
                steps = stepList.Length == 3 ? new Vector3(stepList[0], stepList[1], stepList[2]) : new Vector3(stepList[0], 1, stepList[1]);
            }

            if (sizeList.Length == 3)
                return LoadEntity(tags, new Vector3(sizeList[0], sizeList[1], sizeList[2]), fill, steps);
            else
                return LoadEntity(tags, new Vector3(sizeList[0], 1, sizeList[1]), fill, steps);
        }

        public static IList<EntityInfo> LoadEntity(LevelTags tags) => LoadEntity(tags, Vector3.Zero, false, Vector3.Zero);
        public static IList<EntityInfo> LoadEntity(LevelTags tags, Vector3 size, bool fill, Vector3 steps)
        {
            var entityInfo = new EntityInfo();
            var list = new List<EntityInfo>();

            entityInfo.Fill = fill;
            entityInfo.Size = size;
            entityInfo.Steps = steps;

            entityInfo.EntityID = tags.GetTag<string>("EntityID");

            if (tags.TagExists("ID"))
                entityInfo.ID = tags.GetTag<int>("ID");

            var posList = tags.GetTag<float[]>("Position");
            entityInfo.Position = new Vector3(posList[0], posList[1], posList[2]);

            entityInfo.TexturePath = tags.GetTag<string>("TexturePath");
            entityInfo.TextureRectangles = tags.GetTag<Rectangle[]>("Textures");
            entityInfo.TextureIndexList = tags.GetTag<int[]>("TextureIndex");


            if (tags.TagExists("Scale"))
            {
                var scaleList = tags.GetTag<float[]>("Scale");
                entityInfo.Scale = new Vector3(scaleList[0], scaleList[1], scaleList[2]);
            }

            entityInfo.Collision = tags.GetTag<bool>("Collision");

            entityInfo.ModelID = tags.GetTag<int>("ModelID");

            entityInfo.ActionValue = tags.GetTag<int>("Action");

            if (tags.TagExists("AdditionalValue"))
                entityInfo.AdditionalValue = tags.GetTag<string>("AdditionalValue");

            entityInfo.Rotation = Entity.GetRotationFromInteger(tags.GetTag<int>("Rotation"));

            if (tags.TagExists("Visible"))
                entityInfo.Visible = tags.GetTag<bool>("Visible");

            if (tags.TagExists("Shader"))
            {
                var shaderList = tags.GetTag<float[]>("Shader");
                entityInfo.Shader = new Color(shaderList[0], shaderList[1], shaderList[2]);
            }

            if (tags.TagExists("RotationXYZ"))
            {
                var rotationList = tags.GetTag<float[]>("RotationXYZ");
                entityInfo.Rotation = new Vector3(rotationList[0], rotationList[1], rotationList[2]);
            }

            if (tags.TagExists("SeasonTexture"))
                entityInfo.SeasonTexture = tags.GetTag<string>("SeasonTexture");

            if (tags.TagExists("SeasonToggle"))
                entityInfo.SeasonToggle = tags.GetTag<string>("SeasonToggle");

            if (tags.TagExists("Opacity"))
                entityInfo.Opacity = tags.GetTag<float>("Opacity");

            if (fill || entityInfo.Size.X > 1 || entityInfo.Size.Y > 1 || entityInfo.Size.Z > 1)
            {
                //continue;
                for (int x = 0; x < entityInfo.Size.X; x++)
                for (int y = 0; y < entityInfo.Size.Y; y++)
                for (int z = 0; z < entityInfo.Size.Z; z++)
                {
                    var ent1 = entityInfo.ShallowCopy();
                    ent1.Position += new Vector3(x, y, z);
                    list.Add(ent1);
                }
            }
            else
                list.Add(entityInfo);

            return list;
        }
    }
}