// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using System;
using System.Collections.Generic;

namespace CSE681.JSON.DOMs
{
    public sealed class Array : Value, IEquatable<Array>

    {
        public IList<Value> Items { get; } = new List<Value>();

        public void Add(Value value)
        {
            // check that value types are the same.
            if (Items.Count > 0 && Items[0].GetType() != value.GetType())
            {
                Console.WriteLine($"Types do not match Expected [{Items[0].GetType()}] got [{value.GetType()}]");
            }

            Items.Add(value);
        }

        public bool Equals(Array other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Items.Equals(other.Items);
        }
    }
}