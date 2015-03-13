using System;
using System.Collections.Generic;
using System.Text;

namespace PmdDotnet
{
    public class Violation
    {
        public string Rule { get; set; }

        public string RuleSet { get; set; }

        public string Package { get; set; }

        public string Class { get; set; }

        public string Method { get; set; }

        public string ExternalInfoUrl { get; set; }

        public string Priority { get; set; }

        public string Text { get; set; }

        public string Comment { get; set; }

        public int Beginline { get; set; }

        public int Endline { get; set; }

        public int Begincolumn { get; set; }

        public int Endcolumn { get; set; }
    }
}
