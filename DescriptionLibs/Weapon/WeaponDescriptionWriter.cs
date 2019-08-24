using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace DescriptionLibs.Weapon
{
    [ContentTypeWriter]
    public class WeaponDescriptionWriter : ContentTypeWriter<WeaponDescription>
    {
        protected override void Write(ContentWriter output, WeaponDescription value)
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(WeaponDescription));
                ser.Serialize(output.BaseStream, value);
            }
            catch (Exception e)
            {
                throw new InvalidContentException(e.Message);
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(WeaponDescriptionReader).AssemblyQualifiedName;
        }
    }
}
