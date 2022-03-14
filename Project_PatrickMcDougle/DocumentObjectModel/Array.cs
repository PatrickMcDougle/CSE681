// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using System;
using System.Collections.Generic;

namespace CSE681.JSON.DOMs
{
    /// <summary>This JSON DOM Array class will store the array values for JSON types.</summary>
    public sealed class Array : Value<ICollection<object>, Array>
    {
        public Array() : base()
        {
            TheValue = new List<object>();
        }

        /// <summary>Method to add an element to the array list.</summary>
        /// <param name="obj">Is a CSE681.JSON.DOMs.* class and will be added to the array list.</param>
        public void Add(object obj)
        {
            if (obj is Array
                || obj is Boolean
                || obj is Members
                || obj is Number
                || obj is Object
                || obj is String
                || obj == null)
            {
                TheValue.Add(obj);
            }
        }
    }
}