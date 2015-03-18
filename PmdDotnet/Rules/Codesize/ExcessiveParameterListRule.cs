using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Text;

namespace PmdDotnet.Rules.Codesize
{
    public class ExcessiveParameterListRule : AbstractRule
    {
        public int minimum { get; set; }

        public ExcessiveParameterListRule(int minimum_val)
            : base("codesize", "ExcessiveParameterList",
                "Long parameter lists can indicate that a new object should be created to wrap the numerous parameters. Basically, try to group the parameters together.")
        {
            this.minimum = minimum_val;
        }

        public override void CheckTypes(Mono.Cecil.AssemblyDefinition module, string fileName, Dictionary<string, List<Violation>> files)
        {
            foreach (TypeDefinition type in module.MainModule.Types)
            {
                if (!type.IsPublic)
                    continue;

                if (type.IsClass)
                {
                    foreach (MethodDefinition method in type.Methods)
                    {
                        // ignore cstor
                        if (method.IsConstructor || method.IsGetter || method.IsSetter)
                            continue;

                        if (method.Parameters.Count > this.minimum)
                        {
                            Console.WriteLine(type.FullName + ":" + method.FullName);
                            //Console.WriteLine("Document=" + method.Body.Instructions[0].SequencePoint.Document.ToString());
                            string sourcecodefile = "???";
                            if (method.HasBody)
                            {
                                sourcecodefile = method.Body.Instructions[0].SequencePoint.Document.Url;
                            }
                            AddViolation(files, sourcecodefile, BuildViolation(type, method, null));
                        }
                    }
                }
            }
        }

        private Violation BuildViolation(TypeDefinition type, MethodDefinition method, string comment)
        {
            Violation v = new Violation();
            v.Beginline = method.Body.Instructions[0].SequencePoint.StartLine-1;
            v.Endline = method.Body.Instructions[0].SequencePoint.StartLine-1;
            v.Begincolumn = 0;
            v.Endcolumn = 999;
            v.Rule = this.name;
            v.RuleSet = this.category;
            v.Package = type.ToString().Substring(0, type.ToString().LastIndexOf("."));
            v.Class = type.ToString().Substring(type.ToString().LastIndexOf(".") + 1);
            v.Method = method.Name.Substring(method.Name.LastIndexOf(".") + 1);
            v.ExternalInfoUrl = "http://pmd.sourceforge.net/pmd-5.2.1/pmd-java/rules/java/" + this.category + ".html#" + this.name;
            v.Priority = "3";
            v.Text = this.text;
            v.Comment = comment;

            return v;
        }
    }
}
