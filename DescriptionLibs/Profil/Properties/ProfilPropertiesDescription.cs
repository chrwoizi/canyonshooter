using System;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using Microsoft.Xna.Framework;

namespace DescriptionLibs.Profil.Properties
{
    [Serializable]
    public class ProfilPropertiesDescription
    {
        #region Data Member

        public int ProfilNummer;
        public string ProfilName;
        public Vector2 Position;
        public Rectangle actionrec;
        public string Schwierigkeitsgrad;
        public int Breite;
        public int Höhe;
        public float Musik;
        public float Effekt;
        public string Spielernamen;

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
            ProfilPropertiesDescription tmp = new ProfilPropertiesDescription();
            tmp.OutputXml();
        }
    }
}
