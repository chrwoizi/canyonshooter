using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.Material
{
    [ContentImporter(".model", DefaultProcessor = "MaterialDescriptionProcessor")]
    public class MaterialDescriptionImporter : ContentImporter<MaterialDescription>
    {
        public override MaterialDescription Import(string filename, ContentImporterContext context)
        {
            try
            {
                MaterialDescription desc = new MaterialDescription();

                XmlSerializer ser = new XmlSerializer(typeof(MaterialDescription));
                FileStream str = new FileStream(filename, FileMode.Open);

                desc = (MaterialDescription)ser.Deserialize(str);
                return desc;
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
        }
    }
}
