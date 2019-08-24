using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace DescriptionLibs.Menu
{
    [ContentImporter(".model", DefaultProcessor = "MenuDescriptionProcessor")]
    public class MenuDescriptionImporter : ContentImporter<MenuDescription>
    {
        public override MenuDescription Import(string filename, ContentImporterContext context)
        {
            try
            {
                MenuDescription desc = new MenuDescription();

                XmlSerializer ser = new XmlSerializer(typeof(MenuDescription));
                FileStream str = new FileStream(filename, FileMode.Open);

                desc = (MenuDescription)ser.Deserialize(str);
                return desc;
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
        }
    }
}
