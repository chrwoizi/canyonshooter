using System;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using Microsoft.Xna.Framework.Graphics;

namespace DescriptionLibs.ParticleEffect
{
    [Serializable]
    public class ParticleEffectDescription
    {
        #region Data Member

        // The specified EffectType
        public string EffectType;

        public ParticleSettings Settings;
        
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
            ParticleEffectDescription tmp = new ParticleEffectDescription();
            tmp.OutputXml();
        }
    }
}
