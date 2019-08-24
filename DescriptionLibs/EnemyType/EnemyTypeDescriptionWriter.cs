using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace DescriptionLibs.EnemyType
{
    [ContentTypeWriter]
    public class EnemyTypeDescriptionWriter : ContentTypeWriter<EnemyTypeDescription>
    {
        protected override void Write(ContentWriter output, EnemyTypeDescription value)
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(EnemyTypeDescription));
                ser.Serialize(output.BaseStream, value);
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(EnemyTypeDescriptionReader).AssemblyQualifiedName;
        }
    }
}
