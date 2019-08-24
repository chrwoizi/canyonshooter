using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.ParticleEffect
{
    public class ParticleEffectDescriptionReader : ContentTypeReader<ParticleEffectDescription>
    {
        protected override ParticleEffectDescription Read(ContentReader input, ParticleEffectDescription existingInstance)
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(ParticleEffectDescription));
                existingInstance = (ParticleEffectDescription)ser.Deserialize(input.BaseStream);
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
            return existingInstance;
        }
    }
}
