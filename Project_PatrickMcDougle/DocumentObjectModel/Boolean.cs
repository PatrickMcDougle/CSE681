// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using System;

namespace CSE681.JSON.DOMs
{
    /// <summary>This JSON DOM Boolean will store the true or false value for JSON document elements.</summary>
    public sealed class Boolean : Value<bool, Boolean>
    {
        public Boolean() : base()
        {
            IsValid = false;
        }

        public Boolean(bool value)
        {
            TheValue = value;
            IsValid = true;
        }
    }
}