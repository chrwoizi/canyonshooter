using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace DescriptionLibs.Menu
{
    [ContentTypeWriter]
    public class OptionDescriptionWriter : ContentTypeWriter<OptionDescription>
    {
        protected override void Write(ContentWriter output, OptionDescription value)
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(OptionDescription));
                ser.Serialize(output.BaseStream, value);
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(OptionDescriptionReader).AssemblyQualifiedName;
        }
    }
}
