using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.Menu
{
    [ContentImporter(".model", DefaultProcessor = "ScrollBarDescriptionProcessor")]
    public class ScrollBarDescriptionImporter : ContentImporter<ScrollBarDescription>
    {
        public override ScrollBarDescription Import(string filename, ContentImporterContext context)
        {
            try
            {
                ScrollBarDescription desc = new ScrollBarDescription();

                XmlSerializer ser = new XmlSerializer(typeof(ScrollBarDescription));
                FileStream str = new FileStream(filename, FileMode.Open);

                desc = (ScrollBarDescription)ser.Deserialize(str);
                return desc;
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
        }
    }
}
