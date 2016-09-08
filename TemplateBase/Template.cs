using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TemplateBase
{
    public abstract class Template
    {
        public class AttrStartTemplate : System.Attribute { }
        public class AttrFill : System.Attribute { }
        public class AttrTemplate : System.Attribute { }

                                                                      
        public static string PLACEHOLDER_PREFIX = "@@@";                //@@@SINGLE||varName@@@
        public static string PLACEHOLDER_SUFIX = "@@@";                 //@@@TEMPLATE||varName||VAB.Templates.Some.txt@@@
        public static string INTERNAL_SEPARATOR = "||";                 //@@@STARTTEMPLATE@@@
        public static string STARTTEMPLATE_KEYWORD = "STARTTEMPLATE";   //@@@TEXTI||varName@@@
        public static string SINGLEINSERTION_KEYWORD = "SINGLE";
        public static string MULTYINSERTION_KEYWORD = "MULTY";
        public static string TEXTINSERTION_KEYWORD = "TEXTI";
        public static string TEMPLATE_KEYWORD = "TEMPLATE";
    }
}

//CSharp engine V1  -   .cs files are associated with this engine
//