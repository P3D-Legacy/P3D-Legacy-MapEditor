using System.Collections.Generic;

namespace P3D.Legacy.MapEditor.Data
{
    public class LevelTags : Dictionary<string, object>
    {
        public LevelTags() { }
        public LevelTags(LevelTags levelTags) : base(levelTags) { }

        //public object GetTag(string tagName) => ContainsKey(tagName.ToLowerInvariant()) ? this[tagName.ToLowerInvariant()] : null;
        public TResultType GetTag<TResultType>(string tagName)
        {
            return ContainsKey(tagName.ToLowerInvariant())
                ? (this[tagName.ToLowerInvariant()] is TResultType
                    ? (TResultType) this[tagName.ToLowerInvariant()]
                    : default(TResultType))
                : default(TResultType);
        }

        public bool TagExists(string tagName) => ContainsKey(tagName.ToLowerInvariant());
    }
}