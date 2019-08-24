using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace DescriptionLibs.Model
{
    [ContentTypeWriter]
    public class ModelDescriptionWriter : ContentTypeWriter<ModelDescription>
    {
        protected override void Write(ContentWriter output, ModelDescription value)
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(ModelDescription));
                ser.Serialize(output.BaseStream, value);
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(ModelDescriptionReader).AssemblyQualifiedName;
        }
    }
}
