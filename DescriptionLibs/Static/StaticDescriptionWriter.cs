using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace DescriptionLibs.Static
{
    [ContentTypeWriter]
    public class StaticDescriptionWriter : ContentTypeWriter<StaticDescription>
    {
        protected override void Write(ContentWriter output, StaticDescription value)
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(StaticDescription));
                ser.Serialize(output.BaseStream, value);
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(StaticDescriptionReader).AssemblyQualifiedName;
        }
    }
}
