using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.Model
{
    public class ModelDescriptionReader : ContentTypeReader<ModelDescription>
    {
        protected override ModelDescription Read(ContentReader input, ModelDescription existingInstance)
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(ModelDescription));
                existingInstance = (ModelDescription)ser.Deserialize(input.BaseStream);
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
            return existingInstance;
        }
    }
}
