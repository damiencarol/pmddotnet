using System;
using System.Collections.Generic;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using PmdDotnet.Rules;
using PmdDotnet.Rules.Codesize;

namespace PmdDotnet
{
    class Program
    {
        static int Main(string[] args)
        {
            // Parse arguments
            Arguments arguments = new Arguments(args);


            // Build Rules
            List<AbstractRule> rules = new List<AbstractRule>();
            // codesize:ExcessiveMethodLength
            int ExcessiveMethodLength_Minimum = 200;
            if (arguments["ExcessiveMethodLength_Minimum"] != null)
                ExcessiveMethodLength_Minimum = Int32.Parse(arguments["ExcessiveMethodLength_Minimum"]);
            rules.Add(new ExcessiveMethodLengthRule(ExcessiveMethodLength_Minimum));
            // codesize:ExcessiveParameterListRule
            int ExcessiveParameterListRule_Minimum = 10;
            if (arguments["ExcessiveParameterListRule_Minimum"] != null)
                ExcessiveParameterListRule_Minimum = Int32.Parse(arguments["ExcessiveParameterListRule_Minimum"]);
            rules.Add(new ExcessiveParameterListRule(ExcessiveParameterListRule_Minimum));
            // codesize:TooManyMethods
            rules.Add(new TooManyMethods(10));
            // naming:MethodNamingConventions
            rules.Add(new MethodNamingConventionsRule());
            



            string fileName = args[0];
            ReaderParameters parms = new ReaderParameters();
            parms.ReadSymbols = true;
            
            Dictionary<String, List<Violation>> files = new Dictionary<string, List<Violation>>();
            foreach (AbstractRule rule in rules)
            {
                rule.CheckTypes(AssemblyDefinition.ReadAssembly(fileName, parms), fileName, files);
            }
            // Generate report
            PmdWriter.WriteToFile(files, "pmd.xml");
            return 0;
        }
    }
}
