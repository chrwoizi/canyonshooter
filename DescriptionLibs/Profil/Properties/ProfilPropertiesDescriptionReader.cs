using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.Profil.Properties
{
    public class ProfilPropertiesDescriptionReader : ContentTypeReader<ProfilPropertiesDescription>
    {
        protected override ProfilPropertiesDescription Read(ContentReader input, ProfilPropertiesDescription existingInstance)
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(ProfilPropertiesDescription));
                existingInstance = (ProfilPropertiesDescription)ser.Deserialize(input.BaseStream);
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
            return existingInstance;
        }
    }
}
