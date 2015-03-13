using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace PmdDotnet
{
    class PmdWriter
    {
        public static void WriteToFile(Dictionary<String, List<Violation>> files, string fileName)
        {
            // Create a new file in C:\\ dir
            XmlTextWriter textWriter = new XmlTextWriter(fileName, null);
            // Opens the document
            textWriter.WriteStartDocument();

            // Write first element
            textWriter.WriteStartElement("pmd");
            textWriter.WriteAttributeString("version", "5.2.1");
            textWriter.WriteAttributeString("timestamp", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff"));

            // Write files
            foreach (KeyValuePair<String, List<Violation>> file in files)
            {
                textWriter.WriteStartElement("file", "");// textWriter.WriteString("Colony");
                textWriter.WriteAttributeString("name", file.Key);

                foreach (Violation v in file.Value) {
                    writeViolation(textWriter, v);
                }

                textWriter.WriteEndElement();
            }

            // Ends the document.
            textWriter.WriteEndDocument();
            // close writer
            textWriter.Close();
        }

        private static void writeViolation(XmlTextWriter textWriter, Violation v)
        {
            textWriter.WriteStartElement("violation", "");
            textWriter.WriteAttributeString("beginline", v.Beginline.ToString());
            textWriter.WriteAttributeString("endline", v.Endline.ToString());
            textWriter.WriteAttributeString("begincolumn", v.Begincolumn.ToString());
            textWriter.WriteAttributeString("endcolumn", v.Endcolumn.ToString());
            textWriter.WriteAttributeString("rule", v.Rule);
            textWriter.WriteAttributeString("ruleset", v.RuleSet);
            textWriter.WriteAttributeString("package", v.Package);
            textWriter.WriteAttributeString("class", v.Class);
            textWriter.WriteAttributeString("method", v.Method);
            textWriter.WriteAttributeString("externalInfoUrl", v.ExternalInfoUrl);
            textWriter.WriteAttributeString("priority", v.Priority);

            textWriter.WriteString(v.Text);

            textWriter.WriteEndElement();

            // Write comments
            if (v.Comment!=null)
                textWriter.WriteComment(v.Comment);
        }
    }
}
