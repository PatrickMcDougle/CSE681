using System;

namespace CSE681.JSON.DOMs
{
    public sealed class String : Value, IEquatable<String>
    {
        public String()
        { }

        public String(string value)
        {
            Value = value;
            IsValid = true;
        }

        public string Value { get; set; }

        public bool Equals(String other)
        {
            if (other == null) return false;
            return Value == other.Value;
        }
    }
}