using System;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using Microsoft.Xna.Framework.Graphics;

namespace DescriptionLibs.Material
{
    [Serializable]
    public class MaterialDescription
    {
        #region Data Member

        /// <summary>
        /// Use this as a description of the desired shader or specify Effect+Technique.
        /// </summary>
        public ShaderDescription ShaderDescription;

        /// <summary>
        /// Use this and Technique to specify the desired Shader or let this be empty to use ShaderDescription instead.
        /// Set this to basic to use a basic xna shader
        /// </summary>
        public string Effect;

        /// <summary>
        /// Use this and Effect to specify the desired Shader or let Effect be empty to use ShaderDescription instead.
        /// </summary>
        public string Technique;

        public string Texture0;
        public string Texture1;
        public string Texture2;
        public string Texture3;
        public string Texture4;
        public string Texture5;
        public string Texture6;
        public string Texture7;
        public Color EmissiveColor;
        public Color DiffuseColor;
        public Color SpecularColor;
        public float Shininess;

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
            MaterialDescription tmp = new MaterialDescription();
            tmp.OutputXml();
        }
    }
}
