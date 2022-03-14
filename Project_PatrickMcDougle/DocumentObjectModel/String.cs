// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using System;

namespace CSE681.JSON.DOMs
{
    /// <summary>This JSON DOM String will store a string value for JSON document elements.</summary>
    public sealed class String : Value<string, String>
    {
        public String() : base()
        {
            IsValid = false;
        }

        public String(string value)
        {
            TheValue = value;
            IsValid = true;
        }
    }
}