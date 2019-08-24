using System;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DescriptionLibs.Profil
{
    [Serializable]
    public class ProfilDescription
    {
        #region Data Member

        public string BackgroundImage;
        public string Marker;
        public string Textback;
        public Rectangle TextbackRec;
        public string buttonBackground;
        public Rectangle ButtonBackRec;
        public string barCreate;
        public Rectangle barRec;
        public string barDelete;
        public string barOk;
        public string TextFieldImage;
        public Rectangle TextFieldRectangle;
        public string Mouse;
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
            ProfilDescription tmp = new ProfilDescription();
            tmp.OutputXml();
        }
    }
}
