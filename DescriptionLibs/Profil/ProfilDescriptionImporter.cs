using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.Profil
{
    [ContentImporter(".model", DefaultProcessor = "ProfilDescriptionProcessor")]
    public class ProfilDescriptionImporter : ContentImporter<ProfilDescription>
    {
        public override ProfilDescription Import(string filename, ContentImporterContext context)
        {
            try
            {
                ProfilDescription desc = new ProfilDescription();

                XmlSerializer ser = new XmlSerializer(typeof(ProfilDescription));
                FileStream str = new FileStream(filename, FileMode.Open);

                desc = (ProfilDescription)ser.Deserialize(str);
                return desc;
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
        }
    }
}
