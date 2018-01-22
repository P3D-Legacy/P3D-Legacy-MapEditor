using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using P3D.Legacy.MapEditor.Data;
using P3D.Legacy.MapEditor.Data.Models;
using P3D.Legacy.MapEditor.Data.World;
using P3D.Legacy.MapEditor.Utils;

namespace P3D.Legacy.MapEditor.World
{
    public class Level
    {
        public bool IsDark { get; set; }

        private LevelInfo LevelInfo { get; }

        public List<BaseModel> Models { get; } = new List<BaseModel>();

        public Texture2D DayCycleTexture;


        public Level(LevelInfo levelInfo, GraphicsDevice graphicsDevice)
        {
            LevelInfo = levelInfo;

            DayCycleTexture = TextureHandler.LoadTexture(graphicsDevice, "C:\\GitHub\\Maps\\YourRoom\\textures\\daycycle.png");
            SetLastColor();

            var entities = new List<EntityInfo>();
            foreach (var structure in LevelInfo.Structures)
            {
                var directory = LevelInfo.DirectoryLocation;
                if (!Path.HasExtension(structure.Map))
                    structure.Map += ".dat";
                var structurePath = Path.Combine(directory, structure.Map);
                var file = File.ReadAllText(structurePath);

                // Little hack
                var structureData = LevelLoader.Load(file, structurePath);
                foreach (var entityInfo in structureData.Entities)
                {
                    entityInfo.Parent = levelInfo;

                    entityInfo.Position += structure.Offset;

                    var rot = Entity.GetRotationFromVector(entityInfo.Rotation) + (structure.Rotation == -1 ? 0 : structure.Rotation);
                    while (rot > 3)
                        rot -= 4;
                    entityInfo.Rotation = Entity.GetRotationFromInteger(rot);
                }

                entities.AddRange(structureData.Entities);
            }

            var combined = LevelInfo.Entities.Concat(entities).ToList();
            foreach (var entity in combined)
            {
                entity.Shader = GetDaytimeColor(true);
                Models.Add(BaseModel.GetModelByEntityInfo(entity, graphicsDevice));
            }


            BaseModel.SetupStatic(graphicsDevice);
        }

        public void UpdateLighting(BasicEffect effect)
        {
            if (true)
            {
                effect.LightingEnabled = true;
                effect.PreferPerPixelLighting = true;
                effect.SpecularPower = 2000f;

                switch (GetLightingType())
                {
                    case DayTime.Night:
                        effect.AmbientLightColor = new Vector3(0.8F);

                        effect.DirectionalLight0.DiffuseColor = new Vector3(0.4F, 0.4F, 0.6F);
                        effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(-1F, 0F, 1F));
                        effect.DirectionalLight0.SpecularColor = new Vector3(0F);
                        effect.DirectionalLight0.Enabled = true;
                        break;

                    case DayTime.Morning:
                        effect.AmbientLightColor = new Vector3(0.7F);

                        effect.DirectionalLight0.DiffuseColor = Color.Orange.ToVector3();
                        effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(1F, -1F, 1F));
                        effect.DirectionalLight0.SpecularColor = new Vector3(0F);
                        effect.DirectionalLight0.Enabled = true;
                        break;

                    case DayTime.Day:
                        effect.AmbientLightColor = new Vector3(1F);

                        effect.DirectionalLight0.DiffuseColor = new Vector3(-0.3F);
                        effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(1F, 1F, 1F));
                        effect.DirectionalLight0.SpecularColor = new Vector3(0F);
                        effect.DirectionalLight0.Enabled = true;
                        break;

                    case DayTime.Evening:
                        effect.AmbientLightColor = new Vector3(1F);

                        effect.DirectionalLight0.DiffuseColor = new Vector3(-0.45F);
                        effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(1F, 0F, 1F));
                        effect.DirectionalLight0.SpecularColor = new Vector3(0F);
                        effect.DirectionalLight0.Enabled = true;
                        break;

                    default:
                        effect.LightingEnabled = false;
                        break;
                }
            }
            else
            {
                effect.LightingEnabled = false;
            }

        }
        public DayTime GetLightingType()
        {
            var lightType = DayTime.Day;//(int) World.GetTime();

            // Level's lighttype values:
            // 0 = Get lighting from the current time of day.
            // 1 = Disable lighting
            // 2 = Always Night
            // 3 = Always Morning
            // 4 = Always Day
            // 5 = Always Evening

            if (LevelInfo.LightingType == 1) // If the level lighting type is 1, disable lighting (set index to 99).
                lightType = (DayTime) 99;

            if (LevelInfo.LightingType > 1 & LevelInfo.LightingType < 6) // If the level's lighting type is 2, 3, 4 or 5, set to the respective LightType (set time of day).
                lightType = (DayTime) (LevelInfo.LightingType - 2);

            return lightType;
        }

        public void SetWeather(BasicEffect effect, Weather weather)
        {
            switch (weather)
            {
                case Weather.Clear:
                    effect.DiffuseColor = new Vector3(1);
                    break;

                case Weather.Rain:
                case Weather.Thunderstorm:
                    effect.DiffuseColor = new Vector3(0.4f, 0.4f, 0.7f);
                    break;

                case Weather.Snow:
                    effect.DiffuseColor = new Vector3(0.8f);
                    break;

                case Weather.Underwater:
                    effect.DiffuseColor = new Vector3(0.1f, 0.3f, 0.9f);
                    break;

                case Weather.Sunny:
                    effect.DiffuseColor = new Vector3(1.6f, 1.3f, 1.3f);
                    break;

                case Weather.Fog:
                    effect.DiffuseColor = new Vector3(0.5f, 0.5f, 0.6f);
                    break;

                case Weather.Sandstorm:
                    effect.DiffuseColor = new Vector3(0.8f, 0.5f, 0.2f);
                    break;

                case Weather.Ash:
                    effect.DiffuseColor = new Vector3(0.5f, 0.5f, 0.5f);
                    break;

                case Weather.Blizzard:
                    effect.DiffuseColor = new Vector3(0.6f, 0.6f, 0.6f);
                    break;
            }
            effect.DiffuseColor = GetWeatherColorMultiplier(effect.DiffuseColor);
        }
        public Vector3 GetWeatherColorMultiplier(Vector3 color)
        {
            int progress = GetTimeValue();
            float p = 0F;
            if (progress < 720)
            {
                p = (float)(720 - progress) / 720;
            }
            else
            {
                p = (float)(progress - 720) / 720;
            }

            return new Vector3(color.X + ((1 - color.X) * p), color.Y + ((1 - color.Y) * p), color.Z + ((1 - color.Z) * p));
        }
        private int GetTimeValue()
        {
            //var time = DateTime.Now;
            var time = new DateTime(1, 1, 1, 12, 30, 0);
            return time.Hour * 60 + time.Minute;

            /*
            if (FASTTIMECYCLE == true)
            {
                return Hour * 60 + Minute;
            }
            else
            {
                return World.MinutesOfDay;
            }
            */
        }

        public Color GetDaytimeColor(bool shader) => shader ? LastEntityColor : LastSkyColor;

        private Color[] DaycycleTextureData;
        private Color LastSkyColor;
        private Color LastEntityColor;
        private void SetLastColor()
        {
            if (DaycycleTextureData == null)
            {
                DaycycleTextureData = new Color[DayCycleTexture.Width * DayCycleTexture.Height - 1 + 1];
                DayCycleTexture.GetData(DaycycleTextureData);
            }

            int pixel = GetTimeValue();
            Color pixelColor = DaycycleTextureData[pixel];
            if (pixelColor != LastSkyColor)
            {
                LastSkyColor = pixelColor;
                LastEntityColor = DaycycleTextureData[(pixel + DayCycleTexture.Width).Clamp(0, DayCycleTexture.Width * DayCycleTexture.Height - 1)];
            }
        }

        public void Draw(BasicEffect basicEffect, AlphaTestEffect alphaTestEffect)
        {
            ModelRenderer.DrawCalls = 0;

            BaseModel.DrawStatic(this, basicEffect, alphaTestEffect);

            foreach (var model in Models)
                model.Draw(this, basicEffect);
        }
    }
}
