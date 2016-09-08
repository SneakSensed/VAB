using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using VAB.Builder;
 
namespace VAB.Ui
{
    public abstract class ToxicFormBase : Form
    {
        TemplateUnitBase TU;
        List<InsertionPanelBase> insertionPanels;
        List<InsertionPanelBase> templatePanels;

        public ToxicFormBase(TemplateUnitBase tu) : base()
        {
            TU = tu;
            insertionPanels = new List<InsertionPanelBase>();
            templatePanels = new List<InsertionPanelBase>();
        }

        public abstract void Populate();								//TemplateUnit to insertion panels
        public virtual void Propagate()									//Fill TemplateUnit with insertion panels values
        {
            foreach (InsertionPanelBase ipanel in insertionPanels)
            {
                ipanel.Propagate();
            }
            foreach (InsertionPanelBase tpanel in templatePanels)
            {
                tpanel.Propagate();
            }
            TU.Build();
        }
    }
}
