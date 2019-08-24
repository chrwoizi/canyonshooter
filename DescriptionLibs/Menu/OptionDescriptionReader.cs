using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.Menu
{
    public class OptionDescriptionReader : ContentTypeReader<OptionDescription>
    {
        protected override OptionDescription Read(ContentReader input, OptionDescription existingInstance)
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(OptionDescription));
                existingInstance = (OptionDescription)ser.Deserialize(input.BaseStream);
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
            return existingInstance;
        }
    }
}
