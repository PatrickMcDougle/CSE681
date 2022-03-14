// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using System.Collections;
using System.Threading;

namespace CSE681.Project4.DataStructures
{
    public class BlockingQueue<T>
    {
        private readonly Queue _blockingQueue;
        private readonly object _lock = new object();

        public BlockingQueue()
        {
            _blockingQueue = new Queue();
        }

        public void Clear()
        {
            lock (_lock)
            {
                _blockingQueue.Clear();
            }
        }

        public T Dequeue()
        {
            lock (_lock)
            {
                while (Size() == 0)
                {
                    Monitor.Wait(_lock);
                }
                T message = (T)_blockingQueue.Dequeue();
                return message;
            }
        }

        public void Enqueue(T message)
        {
            lock (_lock)
            {
                _blockingQueue.Enqueue(message);
                Monitor.Pulse(_lock);
            }
        }

        public int Size()
        {
            int size;
            lock (_lock)
            {
                size = _blockingQueue.Count;
            }
            return size;
        }
    }
}