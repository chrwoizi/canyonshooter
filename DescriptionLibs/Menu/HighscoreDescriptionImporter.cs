using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.Menu
{
    [ContentImporter(".model", DefaultProcessor = "HighscoreDescriptionProcessor")]
    public class HighscoreDescriptionImporter : ContentImporter<HighscoreDescription>
    {
        public override HighscoreDescription Import(string filename, ContentImporterContext context)
        {
            try
            {
               HighscoreDescription desc = new HighscoreDescription();

                XmlSerializer ser = new XmlSerializer(typeof(HighscoreDescription));
                FileStream str = new FileStream(filename, FileMode.Open);

                desc = (HighscoreDescription)ser.Deserialize(str);
                return desc;
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
        }
    }
}
