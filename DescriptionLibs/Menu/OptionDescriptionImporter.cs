using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.Menu
{
    [ContentImporter(".model", DefaultProcessor = "OptionDescriptionProcessor")]
    public class OptionDescriptionImporter : ContentImporter<OptionDescription>
    {
        public override OptionDescription Import(string filename, ContentImporterContext context)
        {
            try
            {
                OptionDescription desc = new OptionDescription();

                XmlSerializer ser = new XmlSerializer(typeof(OptionDescription));
                FileStream str = new FileStream(filename, FileMode.Open);

                desc = (OptionDescription)ser.Deserialize(str);
                return desc;
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
        }
    }
}
