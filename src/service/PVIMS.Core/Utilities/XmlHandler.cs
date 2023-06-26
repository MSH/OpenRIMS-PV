using System.IO;
using System.Text;
using System.Xml;

namespace PVIMS.Core.Utilities
{
    public static class XmlHandler
    {
        static public string FormatXML(XmlDocument doc)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";
            settings.NewLineChars = "\r\n";
            settings.NewLineHandling = NewLineHandling.Replace;
            using (XmlWriter writer = XmlWriter.Create(sb, settings))
            {
                doc.Save(writer);
            }
            return sb.ToString();
        }

        static public void WriteXML(string xmlFileName, string xmlText)
        {
            // Write the string to a file.
            StreamWriter file = new System.IO.StreamWriter(xmlFileName);

            file.Write(xmlText);

            file.Close();
            file = null;
        }
    }
}
