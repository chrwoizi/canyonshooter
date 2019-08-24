using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.Menu
{
    public class MenuDescriptionReader : ContentTypeReader<MenuDescription>
    {
        protected override MenuDescription Read(ContentReader input, MenuDescription existingInstance)
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(MenuDescription));
                existingInstance = (MenuDescription)ser.Deserialize(input.BaseStream);
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
            return existingInstance;
        }
    }
}
