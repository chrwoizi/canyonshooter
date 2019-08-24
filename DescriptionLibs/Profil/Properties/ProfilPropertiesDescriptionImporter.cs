using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.Profil.Properties
{
    [ContentImporter(".model", DefaultProcessor = "ProfilPropertiesDescriptionProcessor")]
    public class ProfilPropertiesDescriptionImporter : ContentImporter<ProfilPropertiesDescription>
    {
        public override ProfilPropertiesDescription Import(string filename, ContentImporterContext context)
        {
            try
            {
                ProfilPropertiesDescription desc = new ProfilPropertiesDescription();

                XmlSerializer ser = new XmlSerializer(typeof(ProfilPropertiesDescription));
                FileStream str = new FileStream(filename, FileMode.Open);

                desc = (ProfilPropertiesDescription)ser.Deserialize(str);
                return desc;
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
        }
    }
}
