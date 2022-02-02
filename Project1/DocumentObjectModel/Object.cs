using System;
using System.Collections.Generic;

namespace CSE681.JSON.DOMs
{
    public sealed class Object : Value, IEquatable<Object>
    {
        public IList<Members> Properties { get; } = new List<Members>();

        public void Add(Members value)
        {
            Properties.Add(value);
        }

        public bool Equals(Object other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Properties.Equals(other.Properties);
        }
    }
}