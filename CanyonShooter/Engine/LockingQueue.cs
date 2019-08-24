using System;
using System.Collections.Generic;
using System.Text;

namespace CanyonShooter.Engine
{
    class LockingQueue<T> : IQueue<T>
    {
        private Queue<T> queue;
        private int maxSize = int.MaxValue;

        public LockingQueue()
        {
            queue = new Queue<T>();
        }

        public LockingQueue(int maxSize)
        {
            queue = new Queue<T>(maxSize+1);
            this.maxSize = maxSize;
        }

        public void Clear()
        {
            Helper.Helper.BeginTimeMeasurementDebugOutput("queue_13");
            lock (queue)
            {
                Helper.Helper.EndTimeMeasurementDebugOutput("queue_13");
                queue.Clear();
            }
        }

        public int Count
        {
            get
            {
                Helper.Helper.BeginTimeMeasurementDebugOutput("queue_25");
                lock (queue)
                {
                    Helper.Helper.EndTimeMeasurementDebugOutput("queue_25");
                    return queue.Count;
                }
            }
        }

        public void Enqueue(T obj)
        {
            Helper.Helper.BeginTimeMeasurementDebugOutput("queue_36");
            lock (queue)
            {
                Helper.Helper.EndTimeMeasurementDebugOutput("queue_36");

                if(queue.Count >= maxSize)
                    return;
                queue.Enqueue(obj);
            }
        }

        public T Dequeue()
        {
            Helper.Helper.BeginTimeMeasurementDebugOutput("queue_46");
            lock (queue)
            {
                Helper.Helper.EndTimeMeasurementDebugOutput("queue_46");
                if (queue.Count > 0)
                {
                    return queue.Dequeue();
                }
                else return default(T);
            }
        }
    }
}
