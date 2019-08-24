using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace DescriptionLibs.Profil.Properties
{
    [ContentTypeWriter]
    public class ProfilPropertiesDescriptionWriter : ContentTypeWriter<ProfilPropertiesDescription>
    {
        protected override void Write(ContentWriter output, ProfilPropertiesDescription value)
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(ProfilPropertiesDescription));
                ser.Serialize(output.BaseStream, value);
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(ProfilPropertiesDescriptionReader).AssemblyQualifiedName;
        }
    }
}
