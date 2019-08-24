using System;
using System.Collections.Generic;
using System.Text;

namespace CanyonShooter.Engine
{
    class NonLockingQueue<T> : IQueue<T>
    {
        private Queue<T> queue;
        private int maxSize = int.MaxValue;

        public NonLockingQueue()
        {
            queue = new Queue<T>();
        }

        public NonLockingQueue(int maxSize)
        {
            queue = new Queue<T>(maxSize+1);
            this.maxSize = maxSize;
        }

        public void Clear()
        {
            queue.Clear();
        }

        public int Count
        {
            get
            {
                return queue.Count;
            }
        }

        public void Enqueue(T obj)
        {
            if(queue.Count >= maxSize) return;

            queue.Enqueue(obj);
        }

        public T Dequeue()
        {
            if (queue.Count > 0)
            {
                return queue.Dequeue();
            }
            else throw new Exception();
        }
    }
}
