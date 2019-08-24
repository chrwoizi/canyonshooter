using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.Material
{
    public class MaterialDescriptionReader : ContentTypeReader<MaterialDescription>
    {
        protected override MaterialDescription Read(ContentReader input, MaterialDescription existingInstance)
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(MaterialDescription));
                existingInstance = (MaterialDescription)ser.Deserialize(input.BaseStream);
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
            return existingInstance;
        }
    }
}
