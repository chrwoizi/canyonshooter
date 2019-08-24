using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;

namespace DescriptionLibs.EnemyType
{
    [Serializable]
    public class EnemyTypeDescription
    {
        #region Data Member

        public string Model;
        public int MaxHitpoints;
        public float Speed;
        public List<Vector3> Path;
        public List<string> Weapons;
        public List<Vector3> Formation;
        public int ScorePoints;
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
            EnemyTypeDescription tmp = new EnemyTypeDescription();
            tmp.Formation = new List<Vector3>();
            tmp.Formation.Add(new Vector3(1, 0, 0));
            tmp.Formation.Add(new Vector3(0, 2, 0));
            tmp.Formation.Add(new Vector3(0, 3, 0));
            tmp.OutputXml();
        }
    }
}
