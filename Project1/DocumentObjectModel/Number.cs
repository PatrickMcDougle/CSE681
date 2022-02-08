// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using System;

namespace CSE681.JSON.DOMs
{
    /// <summary>
    /// This JSON DOM Number will store a number value for JSON document elements. In JavaScript a
    /// number is stored as a floating point value even though it might be an integer. This class
    /// tries to mimic that ability.
    /// </summary>
    public sealed class Number : Value<double>, IEquatable<Number>
    {
        public Number()
        {
            IsValid = false;
        }

        public Number(int value)
        {
            IsWholeNumber = true;
            TheValue = value;
            IsValid = true;
        }

        public Number(double value)
        {
            IsWholeNumber = false;
            TheValue = value;
            IsValid = true;
        }

        public bool IsWholeNumber { get; set; } = false;

        public bool Equals(Number other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsWholeNumber == other.IsWholeNumber
                && TheValue == other.TheValue;
        }
    }
}