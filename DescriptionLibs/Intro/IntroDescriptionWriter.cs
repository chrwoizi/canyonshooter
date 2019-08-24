using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace DescriptionLibs.Intro
{
    [ContentTypeWriter]
    public class IntroDescriptionWriter : ContentTypeWriter<IntroDescription>
    {
        protected override void Write(ContentWriter output, IntroDescription value)
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(IntroDescription));
                ser.Serialize(output.BaseStream, value);
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(IntroDescriptionReader).AssemblyQualifiedName;
        }
    }
}
