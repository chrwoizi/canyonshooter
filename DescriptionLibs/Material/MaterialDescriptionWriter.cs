using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace DescriptionLibs.Material
{
    [ContentTypeWriter]
    public class MaterialDescriptionWriter : ContentTypeWriter<MaterialDescription>
    {
        protected override void Write(ContentWriter output, MaterialDescription value)
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(MaterialDescription));
                ser.Serialize(output.BaseStream, value);
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(MaterialDescriptionReader).AssemblyQualifiedName;
        }
    }
}
