using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;

namespace DescriptionLibs.Menu
{
    [Serializable]
    public class OptionpartDescription
    {
        #region Data Member

        public string Name;
        public int Number;
        public string BackgroundImage;
        public string Mouse;
        public Rectangle Position;
        public Vector2 StartPosition;
        public Vector2 Spacer;
        public List<OptionButtonDescription> Buttons;
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
            OptionpartDescription tmp = new OptionpartDescription();
            tmp.OutputXml();
        }
    }
}
