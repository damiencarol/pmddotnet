using Mono.Cecil;
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
    }
}
