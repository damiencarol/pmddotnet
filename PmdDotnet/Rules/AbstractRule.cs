using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Text;

namespace PmdDotnet.Rules
{
    public abstract class AbstractRule
    {
        protected string category;

        protected string name;

        protected string text;

        protected AbstractRule(string cat, string nam, string tex)
        {
            this.category = cat;
            this.name = nam;
            this.text = tex;
        }

        public abstract void CheckTypes(AssemblyDefinition module, string fileName, Dictionary<String, List<Violation>> files);

        protected SequencePoint GetLastSequencePoint(Mono.Collections.Generic.Collection<Mono.Cecil.Cil.Instruction> collection)
        {
            for (int i = (collection.Count - 1); i > 0; --i)
            {
                if (collection[i].SequencePoint != null)
                    return collection[i].SequencePoint;
            }
            return null;
        }

        protected SequencePoint GetFirstSequencePoint(Mono.Collections.Generic.Collection<Mono.Cecil.Cil.Instruction> collection)
        {
            foreach (Instruction i in collection)
            {
                if (i.SequencePoint != null)
                    return i.SequencePoint;
            }
            return null;
        }

        protected void AddViolation(Dictionary<String, List<Violation>> files, string fileName, Violation v)
        {
            if (!files.ContainsKey(fileName))
            {
                files[fileName] = new List<Violation>();
            }
            files[fileName].Add(v);
        }
    }
}
