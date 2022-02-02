using System;

namespace CSE681.JSON.DOMs
{
    public sealed class Boolean : Value, IEquatable<Boolean>
    {
        public Boolean()
        { }

        public Boolean(bool value)
        {
            Value = value;
            IsValid = true;
        }

        public bool Value { get; set; }

        public bool Equals(Boolean other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }
    }
}