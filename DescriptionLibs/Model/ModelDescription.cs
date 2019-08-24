using System;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using System.Collections.Generic;

namespace DescriptionLibs.Model
{
    [Serializable]
    public class ModelDescription
    {
        #region Data Member

        public string BaseFBX;
        public float BaseRotationAngle;
        public Vector3 BaseRotationAxis;
        public RandomRotationConstraints RandomRotationConstraints;
        public List<MeshAnimation> Animations;
        public List<MeshMaterial> Materials;
        public List<WeaponDescription> Weapons;
        public List<ParticleEffectDescription> ParticleEffects;
        public List<AfterBurnerEffectDescription> AfterBurnerEffects;
        public CollisionShapes CollisionShapes;
        public List<WreckageModel> Wreckages;
        public float Mass;
        public float Bounciness;
        public float Friction;
        public float Hardness;

        #endregion

        public void OutputXml()
        {
            MemoryStream ms = new MemoryStream();
            XmlTextWriter w = new XmlTextWriter(ms, Encoding.UTF8);
            w.Formatting = Formatting.Indented;
            IntermediateSerializer.Serialize(w, this, "simple");
            w.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            TextReader r = new StreamReader(ms);
            Console.Out.Write(r.ReadToEnd());
            Console.Out.WriteLine();
        }

        public static void OutputDefaultXml()
        {
            ModelDescription tmp = new ModelDescription();
            tmp.OutputXml();
        }
    }
}
