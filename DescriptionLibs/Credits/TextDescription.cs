using System;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DescriptionLibs.Credits
{
    [Serializable]
    public class TextDescription
    {
        #region Data Member

        //Text
        public string Text;
        public float Y;
        public float Scale;

        //Graphik
        public string Picture;
        public int Pwidth;
        public int Pheight;
        public bool Load;
        public bool Start;

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
            TextDescription tmp = new TextDescription();
            tmp.OutputXml();
        }
    }
}
