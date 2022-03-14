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
    public sealed class Number : Value<double, Number>
    {
        public Number() : base()
        {
            IsValid = false;
        }

        public Number(int value) : base()
        {
            IsWholeNumber = true;
            TheValue = value;
            IsValid = true;
        }

        public Number(double value) : base()
        {
            IsWholeNumber = false;
            TheValue = value;
            IsValid = true;
        }

        public bool IsWholeNumber { get; set; } = false;
    }
}