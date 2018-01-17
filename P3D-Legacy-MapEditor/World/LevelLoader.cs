using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Microsoft.Xna.Framework;

using P3D_Legacy.MapEditor.Data;

namespace P3D_Legacy.MapEditor.World
{
    public class LevelLoader
    {
        public static LevelInfo Load(string text)
        {
            var data = text.Split(new [] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToArray();

            var levelTags = new LevelTags();
            var actionTags = new LevelTags();
            var entities = new List<EntityInfo>();
            var structures = new List<StructureInfo>();
            var offsetMaps = new List<OffsetMapInfo>();
            var shader = new ShaderInfo(); 
            var backdrop = new BackdropInfo();

            var list = new List<LevelTag>(); // for debug purpose
            for (var i = 0; i < data.Length; i++)
            {
                if (data[i].Contains("{") && data[i].Contains("}"))
                {
                    data[i] = data[i].Remove(0, data[i].IndexOf("{", StringComparison.Ordinal) + 2);

                    var tagType = LevelTagType.None;
                    var currLine = data[i];
                    data[i] = data[i].Remove(0, data[i].IndexOf("{", StringComparison.Ordinal) + 2);

                    data[i] = data[i].Remove(0, data[i].IndexOf("[", StringComparison.Ordinal) + 1);
                    data[i] = data[i].Remove(data[i].Length - 3, 3);

                    var tags = GetTags(data[i]);

                    if (currLine.ToLowerInvariant().StartsWith(@"structure"""))//
                    {
                        tagType = LevelTagType.Structure;

                        var map = tags.GetTag<string>("map");
                        var offset = tags.GetTag<float[]>("offset");
                        structures.Add(new StructureInfo()
                        {
                            Map = map,
                            Offset = new Vector3(offset[0], offset[1], offset[2])
                        });
                    }
                    else if (currLine.ToLowerInvariant().StartsWith(@"entity"""))
                    {
                        tagType = LevelTagType.Entity;
                        entities.Add(TagsLoader.LoadEntity(tags, new System.Drawing.Size(1, 1), 1, true, new Vector3(1, 1, 1)));
                    }
                    else if (currLine.ToLowerInvariant().StartsWith(@"floor"""))
                    {
                        tagType = LevelTagType.Floor;
                        entities.Add(TagsLoader.LoadFloor(tags));
                    }
                    else if (currLine.ToLowerInvariant().StartsWith(@"entityfield"""))
                    {
                        tagType = LevelTagType.EntityField;
                        entities.Add(TagsLoader.LoadEntityField(tags));
                    }
                    else if (currLine.ToLowerInvariant().StartsWith(@"level"""))
                    {
                        tagType = LevelTagType.Level;
                        levelTags = new LevelTags(tags);
                    }
                    else if (currLine.ToLowerInvariant().StartsWith(@"actions"""))
                    {
                        tagType = LevelTagType.LevelActions;
                        actionTags = new LevelTags(tags);
                    }
                    else if (currLine.ToLowerInvariant().StartsWith(@"npc"""))
                    {
                        tagType = LevelTagType.NPC;
                        entities.Add(TagsLoader.LoadNpc(tags));
                    }
                    else if (currLine.ToLowerInvariant().StartsWith(@"shader"""))
                    {
                        tagType = LevelTagType.Shader;
                        shader = TagsLoader.LoadShader(tags);
                    }
                    else if (currLine.ToLowerInvariant().StartsWith(@"offsetmap"""))
                    {
                        tagType = LevelTagType.OffsetMap;

                        var map = tags.GetTag<string>("map");
                        var offset = tags.GetTag<int[]>("offset");
                        offsetMaps.Add(new OffsetMapInfo()
                        {
                            Map = map,
                            Offset = offset.Length == 2 ? new Vector3(offset[0], offset[1], 0) : new Vector3(offset[0], offset[1], offset[2])
                        });
                    }
                    else if (currLine.ToLowerInvariant().StartsWith(@"backdrop"""))
                    {
                        tagType = LevelTagType.Backdrop;
                        backdrop = TagsLoader.LoadBackdrop(tags);
                    }

                    list.Add(new LevelTag(tagType, tags));
                }
            }

            return new LevelInfo(levelTags, actionTags, entities, structures, offsetMaps, shader, backdrop);
        }

        private static LevelTags GetTags(string line)
        {
            var tags = new LevelTags();

            var tagList = line.Split(new[] {"}{"}, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < tagList.Length; i++)
            {
                if (!tagList[i].EndsWith("}}"))
                    tagList[i] += "}";

                if (!tagList[i].StartsWith("{"))
                    tagList[i] = "{" + tagList[i];

                ProcessTag(ref tags, tagList[i]);
            }

            return tags;
        }
        public static void ProcessTag(ref LevelTags tags, string tag)
        {
            var tagName = "";
            var tagContent = "";

            tag = tag.Remove(0, 1);
            tag = tag.Remove(tag.Length - 1, 1);

            tagName = tag.Remove(tag.IndexOf("{", StringComparison.Ordinal) - 1).Remove(0, 1).ToLowerInvariant();
            tagContent = tag.Remove(0, tag.IndexOf("{", StringComparison.Ordinal));

            var contentRows = tagContent.Split('}');
            for (var i = 0; i < contentRows.Length; i++)
            {
                if (contentRows[i].Length > 0)
                {
                    contentRows[i] = contentRows[i].Remove(0, 1);

                    var subTagType = contentRows[i].Remove(contentRows[i].IndexOf("[", StringComparison.Ordinal));
                    var subTagValue = contentRows[i].Remove(0, contentRows[i].IndexOf("[", StringComparison.Ordinal) + 1);
                    subTagValue = subTagValue.Remove(subTagValue.Length - 1, 1);

                    switch (subTagType.ToLowerInvariant())
                    {
                        case "int":
                            tags.Add(tagName, int.Parse(subTagValue, CultureInfo.InvariantCulture));
                            break;

                        case "str":
                            tags.Add(tagName, subTagValue);
                            break;

                        case "sng":
                            tags.Add(tagName, float.Parse(subTagValue, CultureInfo.InvariantCulture));
                            break;

                        case "bool":
                            tags.Add(tagName, int.Parse(subTagValue) == 1);
                            break;

                        case "intarr":
                            tags.Add(tagName, subTagValue.Split(',').Select(s => int.Parse(s, CultureInfo.InvariantCulture)).ToArray());
                            break;

                        case "rec":
                            var content = subTagValue.Split(',').Select(s => int.Parse(s, CultureInfo.InvariantCulture)).ToArray();
                            tags.Add(tagName, new Rectangle(content[0], content[1], content[2], content[3]));
                            break;

                        case "recarr":
                            tags.Add(tagName,
                                subTagValue.Split(']').Where(s => s.Length > 0).Select(s => s.Remove(0, 1))
                                    .Select(s => s.Split(',')).Select(array => array.Select(s => int.Parse(s, CultureInfo.InvariantCulture))).Select(e =>
                                    {
                                        var array = e.ToArray();
                                        return new Rectangle(array[0], array[1], array[2], array[3]);
                                    }).ToArray());
                            break;

                        case "sngarr":
                            tags.Add(tagName, subTagValue.Split(',').Select(s => float.Parse(s, CultureInfo.InvariantCulture)).ToArray());
                            break;

                        default:
                            Logger.Log(LogType.Info, $"Unknown tag type! {subTagType.ToLowerInvariant()}");
                            break;
                    }
                }
            }
        }
    }
}
