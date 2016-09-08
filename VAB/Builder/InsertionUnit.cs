using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAB.Builder
{
    class InsertionUnit<T> : InsertionUnitBase
    {
        public T Fill;

        public InsertionUnit(string name)
        {
            Name = name;
            Fill = (T)Activator.CreateInstance(typeof(T));
        }
        public InsertionUnit(string name, T obj)
        {
            Name = name;
            Fill = obj;
        }
    }
}
