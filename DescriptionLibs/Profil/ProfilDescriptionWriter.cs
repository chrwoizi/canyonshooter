using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace DescriptionLibs.Profil
{
    [ContentTypeWriter]
    public class ProfilDescriptionWriter : ContentTypeWriter<ProfilDescription>
    {
        protected override void Write(ContentWriter output, ProfilDescription value)
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(ProfilDescription));
                ser.Serialize(output.BaseStream, value);
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(ProfilDescriptionReader).AssemblyQualifiedName;
        }
    }
}
