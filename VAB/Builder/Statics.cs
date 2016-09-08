using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;
using Microsoft.CSharp;
using System.CodeDom.Compiler;

using TemplateBase;

namespace VAB.Builder
{
    static class Statics
    {
        internal static TemplateUnitBase[] LoadTemplatesFromEmbeddedFolderStart(string EmbeddedFolderName)
        {
            string[] resources = GetEmbeddedResourcesNames(EmbeddedFolderName);

            List<TemplateUnitBase> outp = new List<TemplateUnitBase>();
            foreach (string path in resources)
            {
                switch (Path.GetExtension(path))
                {
                    case ".cs":
                        {
                            string source = GetEmbeddedResourceS(path);
                            Type t = BuildWithAttrCS(source);
                            if (Attribute.GetCustomAttribute(t, typeof(Template.AttrStartTemplate)) != null)
                            {
                                outp.Add(new TemplateUnitCs(source, path));
                            }
                            break;
                        }
                    case ".txt":
                        {
                            string source = GetEmbeddedResourceS(path);
                            if (source.StartsWith(Template.PLACEHOLDER_PREFIX + Template.STARTTEMPLATE_KEYWORD + Template.PLACEHOLDER_SUFIX))
                            {
                                outp.Add(new TemplateUnitTxt(source, path));
                            }
                            break;
                        }
                }
            }
            return outp.ToArray();
        }
        internal static TemplateUnitBase[] LoadTemplatesFromFolderStart(string FolderName)
        {
            string[] resources = Directory.GetFiles(FolderName, "*.*", SearchOption.TopDirectoryOnly);

            List<TemplateUnitBase> outp = new List<TemplateUnitBase>();
            foreach (string path in resources)
            {
                switch (Path.GetExtension(path))
                {
                    case ".cs":
                        {
                            string source = File.ReadAllText(path);
                            Type t = BuildWithAttrCS(source);
                            if (Attribute.GetCustomAttribute(t, typeof(Template.AttrStartTemplate)) != null)
                            {
                                outp.Add(new TemplateUnitCs(source, path));
                            }
                            break;
                        }
                    case ".txt":
                        {
                            string source = File.ReadAllText(path);
                            if (source.StartsWith(Template.PLACEHOLDER_PREFIX + Template.STARTTEMPLATE_KEYWORD + Template.PLACEHOLDER_SUFIX))
                            {
                                outp.Add(new TemplateUnitTxt(source, path));
                            }
                            break;
                        }
                }
            }
            return outp.ToArray();
        }
        internal static TemplateUnitBase LoadTemplateFromEmbeddedStart(string EmbeddedFileName)
        {
            List<TemplateUnitBase> outp = new List<TemplateUnitBase>();
            switch (Path.GetExtension(EmbeddedFileName))
            {
                case ".cs":
                    {
                        string source = GetEmbeddedResourceS(EmbeddedFileName);
                        Type t = BuildWithAttrCS(source);
                        if (Attribute.GetCustomAttribute(t, typeof(Template.AttrStartTemplate)) != null)
                        {
                            return new TemplateUnitCs(source, EmbeddedFileName);
                        }
                        return null;
                    }
                case ".txt":
                    {
                        string source = GetEmbeddedResourceS(EmbeddedFileName);
                        if (source.StartsWith(Template.PLACEHOLDER_PREFIX + Template.STARTTEMPLATE_KEYWORD + Template.PLACEHOLDER_SUFIX))
                        {
                            return new TemplateUnitTxt(source, EmbeddedFileName);
                        }
                        return null;
                    }
            }
            return null;
        }
        internal static TemplateUnitBase LoadTemplateFromFileStart(string FileName)
        {
            switch (Path.GetExtension(FileName))
            {
                case ".cs":
                    {
                        string source = File.ReadAllText(FileName);
                        Type t = BuildWithAttrCS(source);
                        if (Attribute.GetCustomAttribute(t, typeof(Template.AttrStartTemplate)) != null)
                        {
                            return new TemplateUnitCs(source, FileName);
                        }
                        return null;
                    }
                case ".txt":
                    {
                        string source = File.ReadAllText(FileName);
                        if (source.StartsWith(Template.PLACEHOLDER_PREFIX + Template.STARTTEMPLATE_KEYWORD + Template.PLACEHOLDER_SUFIX))
                        {
                            return new TemplateUnitTxt(source, FileName);
                        }
                        return null;
                    }
            }
            return null;
        }

        internal static TemplateUnitBase[] LoadTemplatesFromEmbeddedFolder(string EmbeddedFolderName)
        {
            string[] resources = GetEmbeddedResourcesNames(EmbeddedFolderName);

            List<TemplateUnitBase> outp = new List<TemplateUnitBase>();
            foreach (string path in resources)
            {
                switch (Path.GetExtension(path))
                {
                    case ".cs":
                        {
                            string source = GetEmbeddedResourceS(path);
                            outp.Add(new TemplateUnitCs(source, path));
                            break;
                        }
                    case ".txt":
                        {
                            string source = GetEmbeddedResourceS(path);
                            outp.Add(new TemplateUnitTxt(source, path));
                            break;
                        }
                }
            }
            return outp.ToArray();
        }
        internal static TemplateUnitBase[] LoadTemplatesFromFolder(string FolderName)
        {
            string[] resources = Directory.GetFiles(FolderName, "*.*", SearchOption.TopDirectoryOnly);

            List<TemplateUnitBase> outp = new List<TemplateUnitBase>();
            foreach (string path in resources)
            {
                switch (Path.GetExtension(path))
                {
                    case ".cs":
                        {
                            string source = File.ReadAllText(path);
                            outp.Add(new TemplateUnitCs(source, path)); 
                            break;
                        }
                    case ".txt":
                        {
                            string source = File.ReadAllText(path);
                            outp.Add(new TemplateUnitTxt(source, path));
                            break;
                        }
                }
            }
            return outp.ToArray();
        }
        internal static TemplateUnitBase LoadTemplateFromEmbedded(string EmbeddedFileName)
        {
            List<TemplateUnitBase> outp = new List<TemplateUnitBase>();
            switch (Path.GetExtension(EmbeddedFileName))
            {
                case ".cs":
                    {
                        string source = GetEmbeddedResourceS(EmbeddedFileName);
                        return new TemplateUnitCs(source, EmbeddedFileName);
                    }
                case ".txt":
                    {
                        string source = GetEmbeddedResourceS(EmbeddedFileName);
                        return new TemplateUnitTxt(source, EmbeddedFileName);
                    }
            }
            return null;
        }
        internal static TemplateUnitBase LoadTemplateFromFile(string FileName)
        {
            switch (Path.GetExtension(FileName))
            {
                case ".cs":
                    {
                        string source = File.ReadAllText(FileName);
                        return new TemplateUnitCs(source, FileName);
                    }
                case ".txt":
                    {
                        string source = File.ReadAllText(FileName);
                        return new TemplateUnitTxt(source, FileName);
                    }
            }
            return null;
        }

        internal static Type BuildWithAttrCS(string source, string[] additionalReferencedAssemblies = null)
        {
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();

            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateExecutable = true;
            parameters.GenerateInMemory = true;
            parameters.ReferencedAssemblies.Add(typeof(Template.AttrFill).Assembly.Location);
            parameters.ReferencedAssemblies.Add("System.Core.dll");
            parameters.ReferencedAssemblies.Add("System.dll");
            if (additionalReferencedAssemblies != null)
            {
                foreach (string dll in additionalReferencedAssemblies)
                {
                    parameters.ReferencedAssemblies.Add(dll);
                }
            }
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
            }

            var type = results.CompiledAssembly.GetTypes()[0];
            codeProvider.Dispose();

            return type;
        }
        internal static string RemoveBaseCS(string scr)
        {
            return scr.Replace(" : Template", "")
                .Replace("[AttrStartTemplate]", "")
                .Replace("[AttrTemplate]", "")
                .Replace("[AttrFill]", "")
                .Replace("using TemplateBase;", "");
        }
        internal static string RemoveBaseTXT(string scr)
        {
            string otp = scr;

            Regex regex = new Regex(Template.PLACEHOLDER_PREFIX + "(.*?)" + Template.PLACEHOLDER_SUFIX, RegexOptions.Singleline);
            var maches = regex.Matches(scr);
            foreach (Match m in maches)
            {
                otp = otp.Replace(Template.PLACEHOLDER_PREFIX + m.Value + Template.PLACEHOLDER_SUFIX, "");
            }

            return otp;
        }

        internal static string ERname(string er)
        {
            string[] sep = er.Split('.');
            return sep[sep.Count() - 2];
        }
        internal static string[] GetEmbeddedResourcesNames(string folderName)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            folderName = string.Format("{0}.{1}", executingAssembly.GetName().Name, folderName);
            string[] resources = executingAssembly.GetManifestResourceNames()
                            .Where(r => r.StartsWith(folderName)).ToArray();

            return resources;
        }
        internal static byte[] GetEmbeddedResourceB(string fullName)
        {
            byte[] decryptorBuffer = default(byte[]);
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fullName);
            using (var memstream = new MemoryStream())
            {
                stream.CopyTo(memstream);
                decryptorBuffer = memstream.ToArray();
            }
            return decryptorBuffer;
        }
        internal static string GetEmbeddedResourceS(string fullName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            string result = "";
            using (Stream stream = assembly.GetManifestResourceStream(fullName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }

            return result;
        }
    }
}
