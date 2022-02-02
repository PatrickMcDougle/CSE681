// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using System;

namespace CSE681.JSON.DOMs
{
    public sealed class Number : Value, IEquatable<Number>
    {
        public Number()
        { }

        public Number(int value)
        {
            IsWholeNumber = true;
            Value = value;
            IsValid = true;
        }

        public Number(double value)
        {
            IsWholeNumber = false;
            Value = value;
            IsValid = true;
        }

        public bool IsWholeNumber { get; set; } = false;

        public double Value { get; set; }

        public bool Equals(Number other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsWholeNumber == other.IsWholeNumber
                && Value == other.Value;
        }
    }
}