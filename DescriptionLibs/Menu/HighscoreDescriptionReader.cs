using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.Menu
{
    public class HighscoreDescriptionReader : ContentTypeReader<HighscoreDescription>
    {
        protected override HighscoreDescription Read(ContentReader input, HighscoreDescription existingInstance)
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(HighscoreDescription));
                existingInstance = (HighscoreDescription)ser.Deserialize(input.BaseStream);
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
            return existingInstance;
        }
    }
}
