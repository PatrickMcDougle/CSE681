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
    /// This JSON DOM Value is the base class that most of the other DOM elements extend from.
    /// </summary>
    /// <typeparam name="T">The Type that this Value class should use.</typeparam>
    public abstract class Value<T, U> : IEquatable<U>, IEqualityComparer<T>, IDomTree
        //where T : Value<T, U>
        where U : IDomTree
    {
        protected Value()
        {
            UUID = Guid.NewGuid();
        }

        /// <summary>Is there an error in the DOM value tree.</summary>
        public bool IsError { get; set; } = false;

        /// <summary>Is the DOM Value object valid?</summary>
        public bool IsValid { get; set; } = false;

        /// <summary>The value that this DOM Value is storing.</summary>
        public T TheValue { get; set; }

        public Guid UUID { get; set; }

        public bool Equals(U other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            return UUID.Equals(other.UUID);
        }

        public bool Equals(T x, T y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(T obj)
        {
            return TheValue.GetHashCode() & UUID.GetHashCode();
        }
    }
}