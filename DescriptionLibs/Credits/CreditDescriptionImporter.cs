using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.Credits
{
    [ContentImporter(".model", DefaultProcessor = "CreditDescriptionProcessor")]
    public class CreditDescriptionImporter : ContentImporter<CreditDescription>
    {
        public override CreditDescription Import(string filename, ContentImporterContext context)
        {
            try
            {
                CreditDescription desc = new CreditDescription();

                XmlSerializer ser = new XmlSerializer(typeof(CreditDescription));
                FileStream str = new FileStream(filename, FileMode.Open);

                desc = (CreditDescription)ser.Deserialize(str);
                return desc;
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
        }
    }
}
