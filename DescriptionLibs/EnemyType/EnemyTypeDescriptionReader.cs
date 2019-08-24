using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.EnemyType
{
    public class EnemyTypeDescriptionReader : ContentTypeReader<EnemyTypeDescription>
    {
        protected override EnemyTypeDescription Read(ContentReader input, EnemyTypeDescription existingInstance)
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(EnemyTypeDescription));
                existingInstance = (EnemyTypeDescription)ser.Deserialize(input.BaseStream);
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
            return existingInstance;
        }
    }
}
