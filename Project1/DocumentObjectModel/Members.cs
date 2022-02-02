// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using System;

namespace CSE681.JSON.DOMs
{
    public sealed class Members : IEquatable<Members>
    {
        public bool IsValid { get; set; } = false;

        public string Key { get; set; }
        public Value Member { get; set; }

        public bool Equals(Members other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Key == other.Key
                && Member == other.Member;
        }
    }
}