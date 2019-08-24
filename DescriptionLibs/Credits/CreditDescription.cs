using System;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DescriptionLibs.Credits
{
    [Serializable]
    public class CreditDescription
    {
        #region Data Member

        public List<TextDescription> Zeile;
        public int CurrentLine;

        #endregion

        public void OutputXml()
        {
            MemoryStream ms = new MemoryStream();
            XmlTextWriter w = new XmlTextWriter(ms, Encoding.UTF8);
            w.Formatting = Formatting.Indented;
            IntermediateSerializer.Serialize(w, this, "simple");
            w.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            TextReader r = new StreamReader(ms);
            Console.Out.Write(r.ReadToEnd());
            Console.Out.WriteLine();
        }

        public static void OutputDefaultXml()
        {
            CreditDescription tmp = new CreditDescription();
            tmp.OutputXml();
        }
    }
}
