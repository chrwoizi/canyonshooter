using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace DescriptionLibs.Item
{
    [ContentTypeWriter]
    public class ItemDescriptionWriter : ContentTypeWriter<ItemDescription>
    {
        protected override void Write(ContentWriter output, ItemDescription value)
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(ItemDescription));
                ser.Serialize(output.BaseStream, value);
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(ItemDescriptionReader).AssemblyQualifiedName;
        }
    }
}
