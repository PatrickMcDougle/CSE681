// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using System;

namespace CSE681.JSON.DOMs
{
    /// <summary>This JSON DOM String will store a string value for JSON document elements.</summary>
    public sealed class String : Value<string>, IEquatable<String>
    {
        public String()
        {
            IsValid = false;
        }

        public String(string value)
        {
            TheValue = value;
            IsValid = true;
        }

        public bool Equals(String other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            return TheValue == other.TheValue;
        }
    }
}