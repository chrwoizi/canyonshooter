using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace DescriptionLibs.Credits
{
    [ContentTypeWriter]
    public class CreditDescriptionWriter : ContentTypeWriter<CreditDescription>
    {
        protected override void Write(ContentWriter output, CreditDescription value)
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(CreditDescription));
                ser.Serialize(output.BaseStream, value);
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(CreditDescriptionReader).AssemblyQualifiedName;
        }
    }
}
