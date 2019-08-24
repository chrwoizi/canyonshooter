using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace CanyonShooter.DataLayer.Level
{
    public class Level
    {
        public LevelBlock[] Blocks;
        public LevelCache[] Cache;

        public Level()
            : this(4)
        {
        }
        public Level(int SegmentCount)
        {
            Blocks = new LevelBlock[SegmentCount];
            for (int i = 0; i < Blocks.Length; i++)
            {
                Blocks[i] = GetStandardBlock();
            }
            Cache = null;
        }

        public static LevelBlock GetStandardBlock()
        {
            LevelBlock t;
            t = new LevelBlock();
            t.DirectionZ = -50;
            t.Walls = new Vec2[4];
            t.Walls[0].v = new Vector2(-0.8f, 0.8f);
            t.Walls[1].v = new Vector2(-0.8f, -0.8f);
            t.Walls[2].v = new Vector2(0.8f, -0.8f);
            t.Walls[3].v = new Vector2(0.8f, 0.8f);
            return t;
        }

        // Rechnet die neue absolute Richtung aus
        // Bitte mit PosDir=(0,0,-1) starten
        public static Vector3 GetCanyonNextDirection(Vector3 OldAbsDir, Vector3 NewRelDir)
        {
            Vector3 z = Vector3.Normalize(OldAbsDir); // -z ins Bild
            Vector3 x = Vector3.Normalize(Vector3.Cross(z, new Vector3(0, 1, 0))); // x nach rechts
            Vector3 y = Vector3.Cross(x, z); // y nach oben
            return NewRelDir.X*x + NewRelDir.Y*y - NewRelDir.Z*z;
        }

        // Gibt die Normale der Verbindungsebene zwischen zwei Canyonstücken zurück
        public static Vector3 GetCanyonConnectionNormal(Vector3 Dir1, Vector3 Dir2)
        {
            return Vector3.Normalize(Vector3.Normalize(Dir1) + Vector3.Normalize(Dir2));
        }

        // Wie weit der Canyon aufgespannt werden kann
        // Horizontal und vertikal von -CanyonSpanFactor bis +CanyonSpanFactor
        public const float CanyonSpanFactor = 250;

        // Gibt die Projektion der Verbindungsstücke zurück
        public static void GetCanyonConnectionAbsProjection(Vector3 Dir1, Vector3 Dir2, out Vector3 x, out Vector3 y)
        {
            GetCanyonConnectionAbsProjection(GetCanyonConnectionNormal(Dir1, Dir2), out x, out y);
        }
        public static void GetCanyonConnectionAbsProjection(Vector3 Normal, out Vector3 x, out Vector3 y)
        {
            Vector3 z;
            z = Normal; // -z ins Bild
            x = Vector3.Normalize(Vector3.Cross(z, new Vector3(0, 1, 0))); // x nach rechts
            y = Vector3.Cross(x, z); // y nach oben

            x *= CanyonSpanFactor;
            y *= CanyonSpanFactor;
        }

        public void CalcCache()
        {
            Cache = new LevelCache[Blocks.Length];
            Cache[0].X = new Vector3(CanyonSpanFactor, 0, 0);
            Cache[0].Y = new Vector3(0, CanyonSpanFactor, 0);
            Cache[0].Z = new Vector3(0, 0, -1);
            Cache[0].APos = new Vector3(0, 0, 0);
            Cache[0].ADir = Blocks[0].Direction;
            for (int i = 1; i < Cache.Length; i++) // ersten Auslassen
            {
                Cache[i].APos = Cache[i - 1].APos + Cache[i - 1].ADir;
                Cache[i].ADir = GetCanyonNextDirection(Cache[i - 1].ADir, Blocks[i].Direction);
                Cache[i].Z = GetCanyonConnectionNormal(Cache[i - 1].ADir, Cache[i].ADir);
                GetCanyonConnectionAbsProjection(Cache[i].Z, out Cache[i].X, out Cache[i].Y);
            }
        }

        // Extern benötigte Funktionen

        // Bechechnet die Distanz zum Verbindungsstück
        // Rückgabewert Positiv: Man hat es bereits passiert
        // Rückgabewert Negativ: Man ist noch davor
        // (Rückgabewert Null  : Man ist genau drauf)
        public float GetDistanceToSegmentConnection(Vector3 LocalPosition, int Segment)
        {
            if (Segment < 0)
                return float.PositiveInfinity;
            if (Segment >= Cache.Length)
                return float.NegativeInfinity;

            return Vector3.Dot(LocalPosition, Cache[Segment].Z) - Vector3.Dot(Cache[Segment].APos, Cache[Segment].Z);
        }

        // Bechechnet die Distanz zur Hauptlinie des Canyons in X und Y Richtung
        public Vector2 GetDistanceToSegment(Vector3 LocalPosition, int Segment)
        {
            if (Segment < 0 || Segment >= Cache.Length)
                return new Vector2(float.PositiveInfinity, float.PositiveInfinity);

            Vector3 z = Vector3.Normalize(Cache[Segment].ADir); // -z ins Bild
            Vector3 x = Vector3.Normalize(Vector3.Cross(z, new Vector3(0, 1, 0))); // x nach rechts
            Vector3 y = Vector3.Cross(x, z); // y nach oben

            return new Vector2(
                Vector3.Dot(LocalPosition, x) - Vector3.Dot(Cache[Segment].APos, x),
                Vector3.Dot(LocalPosition, y) - Vector3.Dot(Cache[Segment].APos, y)
                );
        }

        // Gibt die Anteile der Segmente zurück um möglichst weiche Übergänge zu bekommen
        // Mit diesen Werten können dann die Elemente multipliziert werden
        // PrevAmount + ActAmount + NextAmount = 1
        public void GetSegmentAmounts(Vector3 LocalPosition, int Segment, out float PrevAmount, out float ActAmount, out float NextAmount)
        {
            if (Segment < 0) { PrevAmount = 0; ActAmount = 0; NextAmount = 1; }
            if (Segment >= Cache.Length) { PrevAmount = 1; ActAmount = 0; NextAmount = 0; }

            PrevAmount = 0; ActAmount = 1; NextAmount = 0;
            //float s1 = GetDistanceToSegmentConnection(LocalPosition, Segment-1);
            float s2 = GetDistanceToSegmentConnection(LocalPosition, Segment);
            float s3 = GetDistanceToSegmentConnection(LocalPosition, Segment+1);
            //float s4 = GetDistanceToSegmentConnection(LocalPosition, Segment+2);

            ActAmount = Math.Abs(s3) / (Math.Abs(s2) + Math.Abs(s3));
            NextAmount = Math.Abs(s2) / (Math.Abs(s2) + Math.Abs(s3));

            if (NextAmount < 0.5f)
            {
                NextAmount = 0;
                ActAmount = 1;
            }
            else
            {
                NextAmount = (NextAmount - 0.5f) * 2;
                ActAmount = 1f - NextAmount;
            }
        }

        // setzt die absolute Position eines Blocks neu
        // braucht einen gültigen Cache
        // erfordert eine Neubrechnung des Caches hinterher
        public void SetBlockAbsolutePosition(float AbsolutePositionX, float AbsolutePositionY, float AbsolutePositionZ, int Segment)
        {
            // Das 0te Segment darf nicht verschoben werden!
            if (Segment < 1 || Segment >= Cache.Length) { return; }

            Vector3 OldAbsDir;
            if (Segment == 1)
                OldAbsDir = new Vector3(0,0,-1);
            else
                OldAbsDir = Cache[Segment - 2].ADir;
            Vector3 z = Vector3.Normalize(OldAbsDir); // -z ins Bild
            Vector3 x = Vector3.Normalize(Vector3.Cross(z, new Vector3(0, 1, 0))); // x nach rechts
            Vector3 y = Vector3.Cross(x, z); // y nach oben

            Vector3 NewAbsDir = new Vector3(AbsolutePositionX, AbsolutePositionY, AbsolutePositionZ) - Cache[Segment - 1].APos;
            if (NewAbsDir.Length() == 0) return;

            Blocks[Segment - 1].DirectionX = Vector3.Dot(NewAbsDir, x);
            Blocks[Segment - 1].DirectionY = Vector3.Dot(NewAbsDir, y);
            Blocks[Segment - 1].DirectionZ = -Vector3.Dot(NewAbsDir, z);
        }
    }

    public static class LevelManager
    {
        public static void Save(string Filename, Level Level)
        {
            XmlSerializer x = new XmlSerializer(typeof(Level));
            Stream s = File.Create(Filename);
            x.Serialize(s, (Object)Level);
            s.Close();
            
        }

        public static Level LoadFromString(String s)
        {
            Level Level;
            XmlSerializer x = new XmlSerializer(typeof(Level));
            Level = (Level)x.Deserialize(new StringReader(s));
            Level.CalcCache();
            return Level;
        }

        public static Level Load(String Filename)
        {
            Level Level;
            XmlSerializer x = new XmlSerializer(typeof(Level));
            Stream s = File.OpenRead(Filename);
            Level = (Level)x.Deserialize(s);
            s.Close();
            Level.CalcCache();
            return Level;
        }
    }
}
