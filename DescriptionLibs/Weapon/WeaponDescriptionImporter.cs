using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.Weapon
{
    [ContentImporter(".model", DefaultProcessor = "WeaponDescriptionProcessor")]
    public class WeaponDescriptionImporter : ContentImporter<WeaponDescription>
    {
        public override WeaponDescription Import(string filename, ContentImporterContext context)
        {
            try
            {
                WeaponDescription desc = new WeaponDescription();

                XmlSerializer ser = new XmlSerializer(typeof(WeaponDescription));
                FileStream str = new FileStream(filename, FileMode.Open);

                desc = (WeaponDescription)ser.Deserialize(str);
                return desc;
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
        }
    }
}
