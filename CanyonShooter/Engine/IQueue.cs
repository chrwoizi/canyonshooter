using System;
using System.Collections.Generic;
using System.Text;

namespace CanyonShooter.Engine
{
    interface IQueue<T>
    {
        void Clear();

        int Count { get; }

        void Enqueue(T obj);

        T Dequeue();
    }
}
