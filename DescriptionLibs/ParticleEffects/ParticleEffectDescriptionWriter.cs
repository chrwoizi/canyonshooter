using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace DescriptionLibs.ParticleEffect
{
    [ContentTypeWriter]
    public class ParticleEffectDescriptionWriter : ContentTypeWriter<ParticleEffectDescription>
    {
        protected override void Write(ContentWriter output, ParticleEffectDescription value)
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(ParticleEffectDescription));
                ser.Serialize(output.BaseStream, value);
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(ParticleEffectDescriptionReader).AssemblyQualifiedName;
        }
    }
}
