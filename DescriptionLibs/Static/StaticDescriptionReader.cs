using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.Static
{
    public class StaticDescriptionReader : ContentTypeReader<StaticDescription>
    {
        protected override StaticDescription Read(ContentReader input, StaticDescription existingInstance)
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(StaticDescription));
                existingInstance = (StaticDescription)ser.Deserialize(input.BaseStream);
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
            return existingInstance;
        }
    }
}
