// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using System;
using System.Collections;
using System.Collections.Generic;

namespace CSE681.Project4.DataStructures
{
    internal class BlockingLinkedListEnumerator<T> : IEnumerator<T>
    {
        private readonly T[] _theList;
        private int _position = -1;

        public BlockingLinkedListEnumerator(T[] theList)
        {
            _theList = theList;
        }

        public T Current
        {
            get
            {
                try
                {
                    return _theList[_position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            _position = -1;
        }

        public bool MoveNext()
        {
            _position++;
            return (_position < _theList.Length);
        }

        public void Reset()
        {
            _position = -1;
        }
    }
}