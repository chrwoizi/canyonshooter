using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.Item
{
    public class ItemDescriptionReader : ContentTypeReader<ItemDescription>
    {
        protected override ItemDescription Read(ContentReader input, ItemDescription existingInstance)
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(ItemDescription));
                existingInstance = (ItemDescription)ser.Deserialize(input.BaseStream);
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
            return existingInstance;
        }
    }
}
