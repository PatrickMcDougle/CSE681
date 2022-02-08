// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using System;

namespace CSE681.JSON.DOMs
{
    /// <summary>This JSON DOM Boolean will store the true or false value for JSON document elements.</summary>
    public sealed class Boolean : Value<bool>, IEquatable<Boolean>
    {
        public Boolean()
        {
            IsValid = false;
        }

        public Boolean(bool value)
        {
            TheValue = value;
            IsValid = true;
        }

        /// <summary>Checks to make sure the boolan values are the same.</summary>
        /// <param name="other">The other Boolean class with a bool value.</param>
        /// <returns>
        /// True if the Boolean classes are the same one or if the bool values are the same.
        /// </returns>
        public bool Equals(Boolean other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            return TheValue == other.TheValue;
        }
    }
}