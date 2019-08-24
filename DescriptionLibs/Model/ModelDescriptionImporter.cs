using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.Model
{
    [ContentImporter(".model", DefaultProcessor = "ModelDescriptionProcessor")]
    public class ModelDescriptionImporter : ContentImporter<ModelDescription>
    {
        public override ModelDescription Import(string filename, ContentImporterContext context)
        {
            try
            {
                ModelDescription desc = new ModelDescription();

                XmlSerializer ser = new XmlSerializer(typeof(ModelDescription));
                FileStream str = new FileStream(filename, FileMode.Open);

                desc = (ModelDescription)ser.Deserialize(str);
                return desc;
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
        }
    }
}
