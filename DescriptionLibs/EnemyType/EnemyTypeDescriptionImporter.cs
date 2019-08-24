using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.EnemyType
{
    [ContentImporter(".model", DefaultProcessor = "EnemyTypeDescriptionProcessor")]
    public class EnemyTypeDescriptionImporter : ContentImporter<EnemyTypeDescription>
    {
        public override EnemyTypeDescription Import(string filename, ContentImporterContext context)
        {
            try
            {
                EnemyTypeDescription desc = new EnemyTypeDescription();

                XmlSerializer ser = new XmlSerializer(typeof(EnemyTypeDescription));
                FileStream str = new FileStream(filename, FileMode.Open);

                desc = (EnemyTypeDescription)ser.Deserialize(str);
                return desc;
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
        }
    }
}
