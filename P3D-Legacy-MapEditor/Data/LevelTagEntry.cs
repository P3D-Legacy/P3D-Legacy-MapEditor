using System;
using System.Collections.Generic;

namespace P3D.Legacy.MapEditor.Data
{
    public struct LevelTagEntry : IEquatable<LevelTagEntry>
    {
        public string Name { get; }
        public object Value { get; }

        public LevelTagEntry(string name, object value) : this()
        {
            Name = name;
            Value = value;
        }

        public override string ToString() => $"{Name}; {Value}";

        public override int GetHashCode() => Name.GetHashCode();

        public override bool Equals(object obj)
        {
            return obj is LevelTagEntry entry && Equals(entry);
        }

        public bool Equals(LevelTagEntry other)
        {
            return Name == other.Name && EqualityComparer<object>.Default.Equals(Value, other.Value);
        }
    }
}