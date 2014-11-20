using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tetris
{
    public class Pair<T> : ICloneable
    {
        /// <summary>
        /// Stores pairs of values of the same type.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>

        public T First { get; set; }
        public T Second { get; set; }

        public Pair(T t1, T t2)
        {
            First = t1;
            Second = t2;
        }

        public object Clone()
        {
            return new Pair<T>(First, Second);
        }

    }
}
