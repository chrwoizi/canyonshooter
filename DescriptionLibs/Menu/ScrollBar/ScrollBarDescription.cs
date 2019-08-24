using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;

namespace DescriptionLibs.Menu
{
    [Serializable]
    public class ScrollBarDescription
    {
        #region Data Member

        public int UnitWidth;
        public int UnitHeight;
        public List<ButtonDescription> Buttons;  
        public List<TextureDescription> Textures;
        


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
            ScrollBarDescription tmp = new ScrollBarDescription();
            tmp.OutputXml();
        }
    }
}
