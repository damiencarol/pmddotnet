using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Text;

namespace PmdDotnet.Rules
{
    public class MethodNamingConventionsRule : AbstractRule
    {
        public MethodNamingConventionsRule()
            : base("naming", 
                "MethodNamingConventions",
                "Method names should always begin with a upper case character, and should not contain underscores.")
        {
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

                        if (method.Name.Contains("_") || method.Name.Substring(0, 1) != method.Name.Substring(0, 1).ToUpperInvariant())
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

        private SequencePoint GetLastSequencePoint(Mono.Collections.Generic.Collection<Mono.Cecil.Cil.Instruction> collection)
        {
            for (int i = (collection.Count - 1); i > 0; --i)
            {
                if (collection[i].SequencePoint != null)
                    return collection[i].SequencePoint;
            }
            return null;
        }

        private void AddViolation(Dictionary<String, List<Violation>> files, string fileName, Violation v)
        {
            if (!files.ContainsKey(fileName))
            {
                files[fileName] = new List<Violation>();
            }
            files[fileName].Add(v);
        }
    }
}
