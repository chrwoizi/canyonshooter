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
    public class MenupartDescription
    {
        #region Data Member

        public string Name;
        public string DisplayName;
        public string BackgroundImage;
        public string Mouse;
        public string MenuBackground;
        public Rectangle MenuPosition;
        public Vector2 StartPosition;
        public Vector2 Spacer;
        public Vector2 ShiftOffset;
        public int ButtonCount;
        public List<ButtonDescription> Buttons;

        

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
            MenupartDescription tmp = new MenupartDescription();
            tmp.OutputXml();
        }
    }
}
