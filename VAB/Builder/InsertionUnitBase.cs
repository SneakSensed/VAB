using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAB.Builder
{
    public abstract class InsertionUnitBase
    {
        public string Name;
        public Dictionary<string, string> Metas = new Dictionary<string,string>();
    }
}
