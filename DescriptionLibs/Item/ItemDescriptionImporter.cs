using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.Item
{
    [ContentImporter(".model", DefaultProcessor = "ItemDescriptionProcessor")]
    public class ItemDescriptionImporter : ContentImporter<ItemDescription>
    {
        public override ItemDescription Import(string filename, ContentImporterContext context)
        {
            try
            {
                ItemDescription desc = new ItemDescription();

                XmlSerializer ser = new XmlSerializer(typeof(ItemDescription));
                FileStream str = new FileStream(filename, FileMode.Open);

                desc = (ItemDescription)ser.Deserialize(str);
                return desc;
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
        }
    }
}
