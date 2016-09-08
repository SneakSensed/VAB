using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Windows.Forms;

using TemplateBase;

namespace VAB.Builder
{
    class TemplateUnitCs : TemplateUnitBase
    {
        public string IconPath;

        public TemplateUnitCs(string scr, string filename) : base(scr, filename)
        {
        }

  
        public override void Build()
        {
            source = Statics.RemoveBaseCS(source);
            if (string.IsNullOrEmpty(SavePath))
            {
                string name = Statics.ERname(TemplateFileName) + ".exe";
                SavePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace(@"file:\", "") + @"\" + name;
            }
            using (CSharpCodeProvider codeProvider = new CSharpCodeProvider())
            {
                CompilerParameters parameters = new CompilerParameters();
                parse(parameters);

                string fff = source;
                parameters.CompilerOptions = "/target:winexe";
                if (!String.IsNullOrEmpty(IconPath) && File.Exists(IconPath) == true)
                {
                    parameters.CompilerOptions += " /win32icon:\"" + IconPath + "\"";
                }

                parameters.GenerateExecutable = true;
                parameters.IncludeDebugInformation = false;
                parameters.OutputAssembly = SavePath;
                parameters.GenerateInMemory = false;
                parameters.ReferencedAssemblies.Add("System.Core.dll");
                parameters.ReferencedAssemblies.Add("System.dll");
                parameters.TreatWarningsAsErrors = false;
                CompilerResults results = codeProvider.CompileAssemblyFromSource(parameters, source);

                if (results.Errors.Count > 0)
                {
                    string errs = "";
                    foreach (CompilerError CompErr in results.Errors)
                    {
                        errs += "Line number " + CompErr.Line +
                            ", Error Number: " + CompErr.ErrorNumber +
                            ", '" + CompErr.ErrorText + ";" + Environment.NewLine + Environment.NewLine;
                    }
                    MessageBox.Show(errs);
                }
                else IsBuilt = true;
            }
        }
        void parse(CompilerParameters parameters)
        {
            foreach (InsertionUnitBase unit in Insertions)
            {
                if (unit is InsertionUnit<string>)
                {
                    string path = (unit as InsertionUnit<string>).Fill;
                    if (File.Exists(path))
                    {
                        parameters.EmbeddedResources.Add(path);
                        string fname = Path.GetFileName(path);
                        string nspace = this.GetType().Namespace;
                        string resp = nspace + "." + fname;

                        if (source.Contains(unit.Name + " = \"\";"))
                        {
                            source = source.Replace(unit.Name + " = \"\";",
                                                    unit.Name + " = \"" + resp + "\";");
                        }
                        if (source.Contains(unit.Name + "= \"\";"))
                        {
                            source = source.Replace(unit.Name + "= \"\";",
                                                    unit.Name + " = \"" + resp + "\";");
                        }
                        if (source.Contains(unit.Name + " =\"\";"))
                        {
                            source = source.Replace(unit.Name + " =\"\";",
                                                    unit.Name + " = \"" + resp + "\";");
                        }
                        if (source.Contains(unit.Name + "=\"\";"))
                        {
                            source = source.Replace(unit.Name + "=\"\";",
                                                    unit.Name + " = \"" + resp + "\";");
                        }
                        if (source.Contains(unit.Name + ";"))
                        {
                            source = source.Replace(unit.Name + ";",
                                                    unit.Name + " = \"" + resp + "\";");
                        }
                        if (source.Contains(unit.Name + " ;"))
                        {
                            source = source.Replace(unit.Name + " ;",
                                                    unit.Name + " = \"" + resp + "\";");
                        }
                    }
                }
                else if (unit is InsertionUnit<List<string>>)
                {
                    string resp = "";
                    string nspace = this.GetType().Namespace;
                    foreach (string path in (unit as InsertionUnit<List<string>>).Fill)
                    {
                        if (File.Exists(path))
                        {
                            parameters.EmbeddedResources.Add(path);
                            string fname = Path.GetFileName(path);
                            if (resp != "") resp += ", ";
                            resp += "\"" + nspace + "." + fname + "\"";
                        }
                    }
                    if (source.Contains(unit.Name + " = new List<string>();"))
                    {
                        source = source.Replace(unit.Name + " = new List<string>();",
                                                unit.Name + " = new List<string>() " + "{ " + resp + " };");
                    }
                    if (source.Contains(unit.Name + "= new List<string>();"))
                    {
                        source = source.Replace(unit.Name + "= new List<string>();",
                                                unit.Name + " = new List<string>() " + "{ " + resp + " };");
                    }
                    if (source.Contains(unit.Name + " = new List<string>();"))
                    {
                        source = source.Replace(unit.Name + " =new List<string>();",
                                                unit.Name + " =new List<string>() " + "{ " + resp + " };");
                    }
                    if (source.Contains(unit.Name + "=new List<string>();"))
                    {
                        source = source.Replace(unit.Name + "=new List<string>();",
                                                unit.Name + " = new List<string>() " + "{ " + resp + " };");
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
                        parameters.EmbeddedResources.Add(path);
                        string fname = Path.GetFileName(path);
                        string nspace = this.GetType().Namespace;
                        string resp = nspace + "." + fname;

                        string varName = unit.Metas["varPathStored"];

                        if (source.Contains(varName + " = \"\";"))
                        {
                            source = source.Replace(varName + " = \"\";",
                                                    varName + " = \"" + resp + "\";");
                        }
                        if (source.Contains(varName + "= \"\";"))
                        {
                            source = source.Replace(varName + "= \"\";",
                                                    varName + " = \"" + resp + "\";");
                        }
                        if (source.Contains(varName + " =\"\";"))
                        {
                            source = source.Replace(varName + " =\"\";",
                                                    varName + " = \"" + resp + "\";");
                        }
                        if (source.Contains(varName + "=\"\";"))
                        {
                            source = source.Replace(varName + "=\"\";",
                                                    varName + " = \"" + resp + "\";");
                        }
                        if (source.Contains(varName + ";"))
                        {
                            source = source.Replace(varName + ";",
                                                    varName + " = \"" + resp + "\";");
                        }
                        if (source.Contains(varName + " ;"))
                        {
                            source = source.Replace(varName + " ;",
                                                    varName + " = \"" + resp + "\";");
                        }
                    }
                }
            }
        }
        protected override InsertionUnitBase[] GetInsertionUnits()
        {
            Type type = Statics.BuildWithAttrCS(source);
            FieldInfo[] infos = type.GetFields(BindingFlags.Public |
                                               BindingFlags.NonPublic |
                                               BindingFlags.Instance |
                                               BindingFlags.Static);
            List<InsertionUnitBase> insertions = new List<InsertionUnitBase>();
            foreach (FieldInfo f in infos)
            {
                if (Attribute.IsDefined(f, typeof(Template.AttrFill)))
                {
                    if (f.FieldType.GetInterfaces().Contains(typeof(IList<string>)))
                    {
                        insertions.Add(new InsertionUnit<List<string>>(f.Name));
                    }
                    else
                    {
                        insertions.Add(new InsertionUnit<string>(f.Name));
                    }
                }
            }
            return insertions.ToArray();
        }
        protected override TemplateUnitBase[] GetTemplateUnits()
        {
            Type type = Statics.BuildWithAttrCS(source);
            FieldInfo[] infos = type.GetFields(BindingFlags.Public | 
                                               BindingFlags.NonPublic |
                                               BindingFlags.Instance |
                                               BindingFlags.Static);
            List<TemplateUnitBase> templates = new List<TemplateUnitBase>();
            foreach (FieldInfo f in infos)
            {
                if (Attribute.IsDefined(f, typeof(Template.AttrTemplate)))
                {
                    string path = f.GetValue(type).ToString();

                    TemplateUnitBase tpl;
                    if(File.Exists(path)) tpl = Statics.LoadTemplateFromFile(path);
                    else tpl = Statics.LoadTemplateFromEmbedded(path);

                    tpl.Metas.Add("varPathStored", f.Name);
                    templates.Add(tpl);
                }
            }
            return templates.ToArray();
        }
    }
}