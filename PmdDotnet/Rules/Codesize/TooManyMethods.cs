using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PmdDotnet.Rules.Codesize
{
    public class TooManyMethods : AbstractRule
    {
        public TooManyMethods(int minimum_val)
            : base("codesize", "TooManyMethods",
                "A class with too many methods is probably a good target for refactoring, in order to reduce its complexity and find a way to have more fine grained objects.")
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
                    int count = 0;
                    foreach (MethodDefinition method in type.Methods)
                    {
                        // ignore cstor
                        if (method.IsConstructor || method.IsGetter || method.IsSetter)
                            continue;

                        count++;
                    }

                    if (count > this.minimum)
                    {
                        Console.WriteLine(this.category + ":" + this.name + ":" + type.FullName);
                        //Console.WriteLine("Document=" + method.Body.Instructions[0].SequencePoint.Document.ToString());
                        string sourcecodefile = "???";
                        if (type.Methods[0].HasBody)
                        {
                            sourcecodefile = type.Methods[0].Body.Instructions[0].SequencePoint.Document.Url;
                        }
                        AddViolation(files, sourcecodefile, BuildViolation(type, null));
                    }
                }
            }
        }

        private Violation BuildViolation(TypeDefinition type, string comment)
        {
            Violation v = new Violation();
            v.Beginline = 0;
            v.Endline = 0;
            v.Begincolumn = 0;
            v.Endcolumn = 999;
            v.Rule = this.name;
            v.RuleSet = this.category;
            v.Package = type.ToString().Substring(0, type.ToString().LastIndexOf("."));
            v.Class = type.ToString().Substring(type.ToString().LastIndexOf(".") + 1);
            v.Method = "";
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

        public int minimum { get; set; }
    }
}
