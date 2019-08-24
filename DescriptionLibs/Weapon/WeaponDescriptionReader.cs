using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.Weapon
{
    public class WeaponDescriptionReader : ContentTypeReader<WeaponDescription>
    {
        protected override WeaponDescription Read(ContentReader input, WeaponDescription existingInstance)
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(WeaponDescription));
                existingInstance = (WeaponDescription)ser.Deserialize(input.BaseStream);
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
            return existingInstance;
        }
    }
}
