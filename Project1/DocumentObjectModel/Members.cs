// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using System;

namespace CSE681.JSON.DOMs
{
    /// <summary>This JSON DOM Members will store a key value set for JSON document objects.</summary>
    public sealed class Members : IEquatable<Members>
    {
        /// <summary>Is there an error in the DOM value tree.</summary>
        public bool IsError { get; set; } = false;

        /// <summary>Is the DOM Value object valid?</summary>

        public bool IsValid { get; set; } = false;

        /// <summary>The key is the ID of the members value. This is a string.</summary>
        public string Key { get; set; }

        /// <summary> This Member should be of Type Value<T> </summary>
        public object Member { get; set; }

        public bool Equals(Members other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Key == other.Key
                && Member.Equals(other.Member);
        }
    }
}