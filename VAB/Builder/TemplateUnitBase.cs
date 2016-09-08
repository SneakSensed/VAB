using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAB.Builder
{
    public abstract class TemplateUnitBase
    {
        protected string source;
        public string TemplateFileName { get; set; }
        public string SavePath { get; set; }
        public bool IsBuilt { get; set; }

        public Dictionary<string, string> Metas;
        public List<InsertionUnitBase> Insertions;
        public List<TemplateUnitBase> Templates;


        public TemplateUnitBase(string scr, string filename)
        {
            source = scr;
            Metas = new Dictionary<string, string>();
            Insertions = GetInsertionUnits().ToList();
            Templates = GetTemplateUnits().ToList();
            TemplateFileName = filename;
        }


        public abstract void Build();
        protected abstract InsertionUnitBase[] GetInsertionUnits();
        protected abstract TemplateUnitBase[] GetTemplateUnits();
    }
}