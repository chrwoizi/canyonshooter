using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.Static
{
    [ContentImporter(".model", DefaultProcessor = "StaticDescriptionProcessor")]
    public class StaticDescriptionImporter : ContentImporter<StaticDescription>
    {
        public override StaticDescription Import(string filename, ContentImporterContext context)
        {
            try
            {
                StaticDescription desc = new StaticDescription();

                XmlSerializer ser = new XmlSerializer(typeof(StaticDescription));
                FileStream str = new FileStream(filename, FileMode.Open);

                desc = (StaticDescription)ser.Deserialize(str);
                return desc;
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
        }
    }
}
