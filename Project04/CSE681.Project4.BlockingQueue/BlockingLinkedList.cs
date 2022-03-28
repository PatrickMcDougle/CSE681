// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSE681.Project4.DataStructures
{
    public class BlockingLinkedList<T> : IEnumerable<T>
    {
        private readonly LinkedList<T> _blockingLinkedList;
        private readonly object _lock = new object();

        public BlockingLinkedList()
        {
            _blockingLinkedList = new LinkedList<T>();
        }

        public void AddFirst(T item)
        {
            lock (_lock)
            {
                _blockingLinkedList.AddFirst(item);
                Monitor.Pulse(_lock);
            }
        }

        public void AddLast(T item)
        {
            lock (_lock)
            {
                _blockingLinkedList.AddLast(item);
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _blockingLinkedList.Clear();
            }
        }

        public bool Contains(T item)
        {
            bool contains;
            lock (_lock)
            {
                contains = _blockingLinkedList.Contains(item);
                return contains;
            }
        }

        public T Find(T userInfo)
        {
            lock (_lock)
            {
                return _blockingLinkedList.Find(userInfo).Value;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (_lock)
            {
                return new BlockingLinkedListEnumerator<T>(_blockingLinkedList.ToArray());
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Remove(T item)
        {
            lock (_lock)
            {
                _blockingLinkedList.Remove(item);
            }
        }

        public int Size()
        {
            int size;
            lock (_lock)
            {
                size = _blockingLinkedList.Count;
            }
            return size;
        }

        public T[] ToArray()
        {
            lock (_lock)
            {
                return _blockingLinkedList.ToArray();
            }
        }
    }
}