using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Networking
{
    public class CircularBuffer<T> 
    {
        T[] buffer;
        int bufferSize;

        public CircularBuffer(int bufferSize)
        {
            this.bufferSize = bufferSize;
            buffer = new T[bufferSize];
        }

        public void Add(T item, uint index) => buffer[index % bufferSize] = item;
        public T Get(uint index) => buffer[index % bufferSize];
        public void Clear() => buffer = new T[bufferSize];
    }
}
