using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Text;
using PmdDotnet.Rules;

namespace PmdDotnet.Rules.Codesize
{
    public class ExcessiveMethodLengthRule : AbstractRule
    {
        private int ExcessiveMethodLength_Minimum=200;

        public ExcessiveMethodLengthRule(int ExcessiveMethodLength_Minimum_val) : base("codesize", "ExcessiveMethodLength", 
            "Violations of this rule usually indicate that the method is doing too much. Try to reduce the method size by creating helper methods and removing any copy/pasted code.")
        {
            this.ExcessiveMethodLength_Minimum = ExcessiveMethodLength_Minimum_val;
        }

        public override void CheckTypes(AssemblyDefinition module, string fileName, Dictionary<String, List<Violation>> files)
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

                        if (!method.HasBody)
                            continue;

                        SequencePoint seqStart = GetFirstSequencePoint(method.Body.Instructions);
                        SequencePoint seqEnd = GetLastSequencePoint(method.Body.Instructions);
                        if (seqStart == null || seqEnd == null)
                            continue;

                        if ((seqEnd.StartLine - seqStart.StartLine) > ExcessiveMethodLength_Minimum)
                        {
                            AddViolation(files, seqStart.Document.Url,
                                BuildExcessiveMethodLength(type, method, "Taille="
                                + (seqEnd.StartLine - seqStart.StartLine)
                                + " > " + ExcessiveMethodLength_Minimum));
                        }
                    }
                }
            }
        }

        private Violation BuildExcessiveMethodLength(TypeDefinition type, MethodDefinition method, string comment)
        {
            Violation v = new Violation();
            v.Beginline = method.Body.Instructions[0].SequencePoint.StartLine;
            v.Endline = GetLastSequencePoint(method.Body.Instructions).StartLine;
            v.Begincolumn = method.Body.Instructions[0].SequencePoint.StartColumn;
            v.Endcolumn = GetLastSequencePoint(method.Body.Instructions).EndColumn;
            v.Rule = this.name;
            v.RuleSet = this.category;
            v.Package = type.ToString().Substring(0, type.ToString().LastIndexOf("."));
            v.Class = type.ToString().Substring(type.ToString().LastIndexOf(".") + 1);
            v.Method = method.Name.Substring(method.Name.LastIndexOf(".") + 1);
            v.ExternalInfoUrl = "http://pmd.sourceforge.net/pmd-5.2.1/pmd-java/rules/java/codesize.html#ExcessiveMethodLength";
            v.Priority = "3";
            v.Text = this.text;
            v.Comment = comment;

            return v;
        }
    }
}
