using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VAB.Ui
{
    abstract class InsertionPanelBase : Panel
    {
        public abstract void Propagate();                                   //populate back the insertion object.
    }
}
