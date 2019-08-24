using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;

namespace DescriptionLibs.Weapon
{
    [Serializable]
    public class WeaponDescription
    {
        #region Data Member

        public string Model;
        public string Icon;
        public int Damage;
        public float ProjectileSpeed;
        public float FirePause;
        public bool LoopedFire;
        public string AmmoType;
        public int AmmoReloadCount;
        public int DefaultAmmo;
        public int AmmoPerShoot;
        public bool AutoReload;
        public Vector3 ProjectileOffset;

        // Sound-Config:
        public string SoundFiring;
        public string SoundAmmoEmpty;
        public float SoundAmmoEmptyMilliseconds;
        public string SoundReload;
        public float SoundReloadMilliseconds;
        public string SoundStartFire;
        public float SoundStartMilliseconds;
        public string SoundStopFire;
        public float SoundStopMilliseconds;

        // CrossHairs:
        public string CrossHair;
        public string CrossHairFiring;
        public string CrossHairReloading;

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
            WeaponDescription tmp = new WeaponDescription();
            tmp.OutputXml();
        }
    }
}
