using System.Collections.Generic;

namespace P3D.Legacy.MapEditor.Data
{
    public class LevelInfo
    {
        public string Name { get; }
        public string MusicLoop { get; }
        public bool WildPokemonFloor { get; }
        public bool ShowOverworldPokemon { get; }
        public string CurrentRegion { get; }
        public int HiddenAbilityChance { get; }
        public bool CanTeleport { get; set; }
        public bool CanDig { get; set; }
        public bool CanFly { get; set; }
        public int RideType { get; set; }
        public int EnvironmentType { get; set; }
        public int WeatherType { get; set; }
        public int LightingType { get; set; }
        public bool IsDark { get; set; }
        // Terrain shit
        public bool IsSafariZone { get; set; }
        public bool IsBugCatchingContest { get; set; }
        public string BugCatchingContestData { get; set; }
        public string MapScript { get; set; }
        // Radio shit
        public string BattleMapData { get; set; }
        public string SurfingBattleMapData { get; set; }

        public List<EntityInfo> Entities { get; }
        public List<StructureInfo> Structures { get; }
        public List<OffsetMapInfo> OffsetMaps { get; }
        public ShaderInfo Shader { get; }
        public BackdropInfo Backdrop { get; }

        public string Path { get; }
        public string DirectoryLocation => Path.Replace(System.IO.Path.GetFileName(Path), "");
        public string TexturesLocation => System.IO.Path.Combine(DirectoryLocation, "Textures");
        public string StructuresLocation => System.IO.Path.Combine(DirectoryLocation, "Structures");

        public LevelInfo(LevelTags levelTags, string path, LevelTags actionTags, List<EntityInfo> entities, List<StructureInfo> structures, List<OffsetMapInfo> offsetMaps, ShaderInfo shader, BackdropInfo backdrop)
        {
            Name = levelTags.GetTag<string>("Name");
            MusicLoop = levelTags.GetTag<string>("MusicLoop");
            WildPokemonFloor = levelTags.TagExists("WildPokemon") && levelTags.GetTag<bool>("WildPokemon");
            ShowOverworldPokemon = !levelTags.TagExists("OverworldPokemon") || levelTags.GetTag<bool>("OverworldPokemon");
            CurrentRegion = levelTags.TagExists("CurrentRegion") ? levelTags.GetTag<string>("CurrentRegion") : "Johto";
            HiddenAbilityChance = levelTags.TagExists("HiddenAbility") ? levelTags.GetTag<int>("HiddenAbility") : 0;


            CanTeleport = actionTags.TagExists("CanTeleport") && actionTags.GetTag<bool>("CanTeleport");
            CanDig = actionTags.TagExists("CanDig") && actionTags.GetTag<bool>("CanDig");
            CanFly = actionTags.TagExists("CanFly") && actionTags.GetTag<bool>("CanFly");
            RideType = actionTags.TagExists("RideType") ? actionTags.GetTag<int>("RideType") : 0;
            EnvironmentType = actionTags.TagExists("EnviromentType") ? actionTags.GetTag<int>("EnviromentType") : 0;
            WeatherType = actionTags.TagExists("Weather") ? actionTags.GetTag<int>("Weather") : 0;

            //It's not my fault I swear. The keyboard was slippy, I was partly sick and there was fog on the road and I couldnt see.
            var lightningExists = actionTags.TagExists("Lightning");
            var lightingExists = actionTags.TagExists("Lighting");
            if (lightningExists && lightingExists)
                LightingType = actionTags.GetTag<int>("Lighting");
            else if (lightingExists)
                LightingType = actionTags.GetTag<int>("Lighting");
            else if (lightningExists)
                LightingType = actionTags.GetTag<int>("Lightning");
            else
                LightingType = 1;

            IsDark = actionTags.TagExists("IsDark") && actionTags.GetTag<bool>("IsDark");

            //if (actionTags.TagExists("Terrain"))
            //    Terrain.TerrainType = Terrain.FromString(actionTags.GetTag<string>("Terrain"));
            //else
            //    Terrain.TerrainType = TerrainTypeEnums.Plain;

            IsSafariZone = actionTags.TagExists("IsSafariZone") && actionTags.GetTag<bool>("IsSafariZone");

            if (actionTags.TagExists("BugCatchingContest"))
            {
                IsBugCatchingContest = true;
                BugCatchingContestData = actionTags.GetTag<string>("BugCatchingContest");
            }
            else
            {
                IsBugCatchingContest = false;
                BugCatchingContestData = "";
            }

            MapScript = actionTags.TagExists("MapScript") ? actionTags.GetTag<string>("MapScript") : "";

            // TODO
            //if (actionTags.TagExists("RadioChannels"))
            //    foreach (var c in actionTags.GetTag<string>("RadioChannels").Split(Convert.ToChar(",")))
            //        AllowedRadioChannels.Add(Convert.ToDecimal(c.Replace(".", GameController.DecSeparator)));
            //else
            //    AllowedRadioChannels.Clear();

            BattleMapData = actionTags.TagExists("BattleMap") ? actionTags.GetTag<string>("BattleMap") : "";
            SurfingBattleMapData = actionTags.TagExists("SurfingBattleMap") ? actionTags.GetTag<string>("SurfingBattleMap") : "";

            Entities = entities;
            Structures = structures;
            OffsetMaps = offsetMaps;
            Shader = shader;
            Backdrop = backdrop;

            Path = path;
        }

        public override string ToString() => Name;
    }
}