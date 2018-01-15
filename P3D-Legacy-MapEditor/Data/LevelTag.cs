namespace P3D_Legacy.MapEditor.Data
{
    public class LevelTag
    {
        public LevelTagType TagType { get; }
        public LevelTags Tags { get; }

        public LevelTag(LevelTagType tagType, LevelTags tags)
        {
            TagType = tagType;
            Tags = tags;
        }

        public object this[string tag] => Tags.ContainsKey(tag.ToLowerInvariant()) ? Tags[tag.ToLowerInvariant()] : null;

        public override string ToString() => $"{TagType} [{Tags.Count}]";
    }
}