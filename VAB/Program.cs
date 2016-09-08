using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VAB.Builder;
using VAB.Ui;
 
namespace VAB
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            TemplateUnitBase[] mainTemplates = Statics.LoadTemplatesFromEmbeddedFolderStart("Templates");
            foreach (TemplateUnitBase tu in mainTemplates)
            {
                new ToxicFormMain(tu, "HiddenTear");
            }
        }
    }
}
