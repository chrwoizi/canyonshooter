using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.Credits
{
    public class CreditDescriptionReader : ContentTypeReader<CreditDescription>
    {
        protected override CreditDescription Read(ContentReader input, CreditDescription existingInstance)
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(CreditDescription));
                existingInstance = (CreditDescription)ser.Deserialize(input.BaseStream);
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
            return existingInstance;
        }
    }
}
