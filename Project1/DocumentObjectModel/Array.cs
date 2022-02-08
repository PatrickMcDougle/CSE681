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
    public sealed class Array : Value<ICollection<object>>, IEquatable<Array>
    {
        public Array()
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

        /// <summary>Checks to make sure the array passed in is the same as this array</summary>
        /// <param name="array">The array to see if it matches this array.</param>
        /// <returns>True if they are arrays are the same array or the values are the same.</returns>
        public bool Equals(Array array)
        {
            if (array == null) return false;
            if (ReferenceEquals(this, array)) return true;
            return TheValue.Equals(array.TheValue);
        }
    }
}