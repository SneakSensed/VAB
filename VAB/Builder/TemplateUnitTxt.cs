using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;

using TemplateBase;

namespace VAB.Builder
{
    class TemplateUnitTxt : TemplateUnitBase
    {
        public TemplateUnitTxt(string scr, string filename) : base(scr, filename)
        {
        }

        public override void Build()
        {
            source = Statics.RemoveBaseTXT(source);
            if (string.IsNullOrEmpty(SavePath))
            {
                string name = Statics.ERname(TemplateFileName) + ".exe";
                SavePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase) + name;
            }
            parse();
            File.WriteAllText(SavePath, source);
            IsBuilt = true;
        }
        void parse()
        {
            foreach (InsertionUnitBase unit in Insertions)
            {
                if (unit is InsertionUnit<string>)
                {
                    if (unit.Metas.ContainsKey(Template.TEXTINSERTION_KEYWORD))
                    {
                        string repl = Template.PLACEHOLDER_PREFIX +
                                            Template.SINGLEINSERTION_KEYWORD +
                                            Template.INTERNAL_SEPARATOR +
                                            (unit as InsertionUnit<string>).Name +
                                            Template.PLACEHOLDER_SUFIX;
                        string contents = (unit as InsertionUnit<string>).Fill;
                        source = source.Replace(repl, contents);
                    }
                    else
                    {
                        string path = (unit as InsertionUnit<string>).Fill;
                        if (File.Exists(path))
                        {
                            string repl = Template.PLACEHOLDER_PREFIX +
                                            Template.SINGLEINSERTION_KEYWORD +
                                            Template.INTERNAL_SEPARATOR +
                                            (unit as InsertionUnit<string>).Name +
                                            Template.PLACEHOLDER_SUFIX;
                            string contents = File.ReadAllText(path);
                            source = source.Replace(repl, contents);
                        }
                    }
                }
                else if (unit is InsertionUnit<List<string>>)
                {
                    string path = (unit as InsertionUnit<string>).Fill;
                    if (File.Exists(path))
                    {
                        string repl = Template.PLACEHOLDER_PREFIX +
                                        Template.SINGLEINSERTION_KEYWORD +
                                        Template.INTERNAL_SEPARATOR +
                                        (unit as InsertionUnit<string>).Name +
                                        Template.PLACEHOLDER_SUFIX;

                        string contents = "";
                        foreach (string pth in (unit as InsertionUnit<List<string>>).Fill)
                        {
                            contents += File.ReadAllText(pth);
                        }

                        source = source.Replace(repl, contents);
                    }
                }
            }
            foreach (TemplateUnitBase unit in Templates)
            {
                if (unit.IsBuilt)
                {
                    string path = unit.SavePath;
                    if (File.Exists(path))
                    {
                        string varName = unit.Metas["varPathStored"];
                        string templatePath = unit.TemplateFileName;

                        string repl = Template.PLACEHOLDER_PREFIX +
                                       Template.TEMPLATE_KEYWORD +
                                       Template.INTERNAL_SEPARATOR +
                                       varName + Template.INTERNAL_SEPARATOR + templatePath +
                                       Template.PLACEHOLDER_SUFIX;
                        string contents = File.ReadAllText(path);
                        source = source.Replace(repl, contents);
                    }
                }
            }
        }
        protected override InsertionUnitBase[] GetInsertionUnits()
        {
            List<InsertionUnitBase> insertions = new List<InsertionUnitBase>();

            Regex regex = new Regex(Template.PLACEHOLDER_PREFIX + Template.SINGLEINSERTION_KEYWORD + Template.INTERNAL_SEPARATOR + "(.*?)" + Template.PLACEHOLDER_SUFIX, RegexOptions.Singleline);
            var maches = regex.Matches(source);
            foreach (Match m in maches)
            {
                insertions.Add(new InsertionUnit<string>(m.Value));
            }
            Regex regex1 = new Regex(Template.PLACEHOLDER_PREFIX + Template.MULTYINSERTION_KEYWORD + Template.INTERNAL_SEPARATOR + "(.*?)" + Template.PLACEHOLDER_SUFIX, RegexOptions.Singleline);
            var maches1 = regex1.Matches(source);
            foreach (Match m in maches1)
            {
                insertions.Add(new InsertionUnit<List<string>>(m.Value));
            }
            Regex regex2 = new Regex(Template.PLACEHOLDER_PREFIX + Template.TEXTINSERTION_KEYWORD + Template.INTERNAL_SEPARATOR + "(.*?)" + Template.PLACEHOLDER_SUFIX, RegexOptions.Singleline);
            var maches2 = regex1.Matches(source);
            foreach (Match m in maches2)
            {
                var u = new InsertionUnit<List<string>>(m.Value);
                u.Metas.Add(Template.TEXTINSERTION_KEYWORD, "");
                insertions.Add(u);
            }
            return insertions.ToArray();
        }
        protected override TemplateUnitBase[] GetTemplateUnits()
        {
            List<TemplateUnitBase> templates = new List<TemplateUnitBase>();

            Regex regex = new Regex(Template.PLACEHOLDER_PREFIX + Template.TEMPLATE_KEYWORD + Template.INTERNAL_SEPARATOR + "(.*?)" + Template.PLACEHOLDER_SUFIX, RegexOptions.Singleline);
            var maches = regex.Matches(source);
            foreach (Match m in maches)
            {
                string[] sep = m.Value.Split(new string[] {Template.INTERNAL_SEPARATOR}, StringSplitOptions.None);
                string path = sep[1];

                TemplateUnitBase tpl;
                if (File.Exists(path)) tpl = Statics.LoadTemplateFromFile(path);
                else tpl = Statics.LoadTemplateFromEmbedded(path);

                tpl.Metas.Add("varPathStored", sep[0]);
                templates.Add(tpl);
            }

            return templates.ToArray();
        }
    }
}
