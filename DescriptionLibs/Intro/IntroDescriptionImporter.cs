using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.Intro
{
    [ContentImporter(".model", DefaultProcessor = "IntroDescriptionProcessor")]
    public class IntroDescriptionImporter : ContentImporter<IntroDescription>
    {
        public override IntroDescription Import(string filename, ContentImporterContext context)
        {
            try
            {
                IntroDescription desc = new IntroDescription();

                XmlSerializer ser = new XmlSerializer(typeof(IntroDescription));
                FileStream str = new FileStream(filename, FileMode.Open);

                desc = (IntroDescription)ser.Deserialize(str);
                return desc;
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
        }
    }
}
