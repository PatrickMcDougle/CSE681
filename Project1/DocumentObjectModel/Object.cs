// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using System;
using System.Collections.Generic;

namespace CSE681.JSON.DOMs
{
    /// <summary>
    /// This JSON DOM Object will store an Object value for JSON document elements. All elements in
    /// an Object is a Members
    /// </summary>
    public sealed class Object : Value<ICollection<Members>>, IEquatable<ICollection<Members>>
    {
        public Object()
        {
            TheValue = new List<Members>();
        }

        /// <summary>Add a new Members to this object. Members have a key/id and a Value set.</summary>
        /// <param name="value">The Members value that will be added to this objects list.</param>
        public void Add(Members value)
        {
            TheValue.Add(value);
        }

        public bool Equals(ICollection<Members> collection)
        {
            if (collection == null) return false;
            if (ReferenceEquals(this, collection)) return true;
            return TheValue.Equals(collection);
        }
    }
}