using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using P3D.Legacy.MapEditor.Data;
using P3D.Legacy.MapEditor.Data.Models;

namespace P3D.Legacy.MapEditor.World
{
    public class Level
    {
        public bool IsDark { get; set; }

        private LevelInfo LevelInfo { get; }

        public List<BaseModel> Models { get; } = new List<BaseModel>();


        public Level(LevelInfo levelInfo, GraphicsDevice graphicsDevice)
        {
            LevelInfo = levelInfo;

            var entities = new List<EntityInfo>();
            foreach (var structure in LevelInfo.Structures)
            {
                var levelPath = LevelInfo.Path;
                var directory = "C:\\GitHub\\P3D-Legacy\\2.5DHero\\2.5DHero\\maps";//levelPath.Replace(Path.GetFileName(levelPath), "");
                if (!Path.HasExtension(structure.Map))
                    structure.Map += ".dat";
                var structurePath = Path.Combine(directory, structure.Map);
                var file = File.ReadAllText(structurePath);

                // Little hack
                var structureData = LevelLoader.Load(file, structurePath);
                foreach (var entityInfo in structureData.Entities)
                {
                    entityInfo.Position += structure.Offset;

                    var rot = Entity.GetRotationFromVector(entityInfo.Rotation) + structure.Rotation;
                    while (rot > 3)
                        rot -= 4;
                    entityInfo.Rotation = Entity.GetRotationFromInteger(rot);
                }

                entities.AddRange(structureData.Entities);
            }

            var combined = LevelInfo.Entities.Concat(entities).ToList();
            foreach (var entity in combined)
                Models.Add(BaseModel.GetModelByEntityInfo(entity, graphicsDevice));

            BaseModel.SetupStatic(graphicsDevice);
        }

        public void Draw(Effect effect)
        {
            //FloorModel.DrawStatic(this, effect, null, null);

            BaseModel.DrawStatic(this, effect);
            foreach (var model in Models)
                model.Draw(this, effect);
        }
    }
}
