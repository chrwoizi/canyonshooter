using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.Intro
{
    public class IntroDescriptionReader : ContentTypeReader<IntroDescription>
    {
        protected override IntroDescription Read(ContentReader input, IntroDescription existingInstance)
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(IntroDescription));
                existingInstance = (IntroDescription)ser.Deserialize(input.BaseStream);
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
            return existingInstance;
        }
    }
}
