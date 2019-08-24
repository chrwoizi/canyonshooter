using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.Profil
{
    public class ProfilDescriptionReader : ContentTypeReader<ProfilDescription>
    {
        protected override ProfilDescription Read(ContentReader input, ProfilDescription existingInstance)
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(ProfilDescription));
                existingInstance = (ProfilDescription)ser.Deserialize(input.BaseStream);
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
            return existingInstance;
        }
    }
}
