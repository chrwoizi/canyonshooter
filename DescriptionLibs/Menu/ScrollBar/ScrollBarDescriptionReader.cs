using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.Menu
{
    public class ScrollBarDescriptionReader : ContentTypeReader<ScrollBarDescription>
    {
        protected override ScrollBarDescription Read(ContentReader input, ScrollBarDescription existingInstance)
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(ScrollBarDescription));
                existingInstance = (ScrollBarDescription)ser.Deserialize(input.BaseStream);
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
            return existingInstance;
        }
    }
}
