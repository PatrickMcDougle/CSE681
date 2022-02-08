// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
namespace CSE681.JSON.DOMs
{
    /// <summary>
    /// This JSON DOM Value is the base class that most of the other DOM elements extend from.
    /// </summary>
    /// <typeparam name="T">The Type that this Value class should use.</typeparam>
    public abstract class Value<T>
    {
        /// <summary>Is there an error in the DOM value tree.</summary>
        public bool IsError { get; set; } = false;

        /// <summary>Is the DOM Value object valid?</summary>
        public bool IsValid { get; set; } = false;

        /// <summary>The value that this DOM Value is storing.</summary>
        public T TheValue { get; set; }
    }
}