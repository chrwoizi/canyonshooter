using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.ParticleEffect
{
    [ContentImporter(".model", DefaultProcessor = "ParticleEffectDescriptionProcessor")]
    public class ParticleEffectDescriptionImporter : ContentImporter<ParticleEffectDescription>
    {
        public override ParticleEffectDescription Import(string filename, ContentImporterContext context)
        {
            try
            {
                ParticleEffectDescription desc = new ParticleEffectDescription();

                XmlSerializer ser = new XmlSerializer(typeof(ParticleEffectDescription));
                FileStream str = new FileStream(filename, FileMode.Open);

                desc = (ParticleEffectDescription)ser.Deserialize(str);
                return desc;
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
        }
    }
}
