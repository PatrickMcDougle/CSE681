// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using System.Collections.Generic;
using System.Threading;

namespace CSE681.Project4.DataStructures
{
    public class BlockingStack<T>
    {
        private readonly Stack<T> _blockingStack;
        private readonly object _lock = new object();

        public BlockingStack()
        {
            _blockingStack = new Stack<T>();
        }

        public void Clear()
        {
            lock (_lock)
            {
                _blockingStack.Clear();
            }
        }

        public T Peek()
        {
            lock (_lock)
            {
                while (Size() == 0)
                {
                    Monitor.Wait(_lock);
                }
                T message = _blockingStack.Peek();
                return message;
            }
        }

        public T Pop()
        {
            lock (_lock)
            {
                while (Size() == 0)
                {
                    Monitor.Wait(_lock);
                }
                T message = _blockingStack.Pop();
                return message;
            }
        }

        public void Push(T message)
        {
            lock (_lock)
            {
                _blockingStack.Push(message);
                Monitor.Pulse(_lock);
            }
        }

        public int Size()
        {
            int size;
            lock (_lock)
            {
                size = _blockingStack.Count;
            }
            return size;
        }
    }
}