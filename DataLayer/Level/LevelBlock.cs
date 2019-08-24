using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using CanyonShooter.DataLayer.Descriptions;

namespace CanyonShooter.DataLayer.Level
{
    [Serializable]
    public struct Vec2 // Damit das PropertyGrid damit klar kommt
    {
        [NonSerialized]
        public Vector2 v;

        public float X
        {
            get { return v.X; }
            set { v.X = value; }
        }
        public float Y
        {
            get { return v.Y; }
            set { v.Y = value; }
        }
    }
    /*
    [Serializable]
    public struct Vec2
    {
        private float x, y;
        public float X
        {
            get { return x; }
            set { x = value; }
        }
        public float Y
        {
            get { return y; }
            set { y = value; }
        }
    }
    [Serializable]
    public struct Vec3
    {
        private float x, y, z;
        public float X
        {
            get { return x; }
            set { x = value; }
        }
        public float Y
        {
            get { return y; }
            set { y = value; }
        }
        public float Z
        {
            get { return z; }
            set { z = value; }
        }
    }
    */

    public struct LevelCache
    {
        public Vector3 X;   // Verbindungsebene: Spannvektor X
        public Vector3 Y;   // Verbindungsebene: Spannvektor Y
        public Vector3 Z;   // Verbindungsebene: Normalisierte Normale der Ebene
        public Vector3 APos; // Wegpunkt: Absoluter Mittelpunkt (der Verbindungsebene)
        public Vector3 ADir;// Webpunkt: Absolute Richtung zum nächsten Wegpunkt (ungleich Z!) (nicht normalisiert)
        public float APosX { get { return APos.X; } set { APos.X = value; } }
        public float APosY { get { return APos.Y; } set { APos.Y = value; } }
        public float APosZ { get { return APos.Z; } set { APos.Z = value; } }
        public float ADirX { get { return ADir.X; } set { ADir.X = value; } }
        public float ADirY { get { return ADir.Y; } set { ADir.Y = value; } }
        public float ADirZ { get { return ADir.Z; } set { ADir.Z = value; } }
    }

    [Serializable]
    [XmlInclude(typeof(EnemyDescription))]
    [XmlInclude(typeof(StaticObjectDescription))]
    [XmlInclude(typeof(WaypointObjectDescription))]
    [XmlInclude(typeof(SunlightObjectDescription))]
    public struct LevelBlock
    {
        private Vector3 direction;

        [Browsable(false)]
        public Vector3 Direction // Zum einfacheren Zugriff
        {
            get { return direction; }
        }
        
        [Category("Canyon"), Description("Canyon Richtung (relativ zum Vorgänger)")]
        public float DirectionX
        {
            get { return direction.X; }
            set { direction.X = value; }
        }
        [Category("Canyon"), Description("Canyon Richtung (relativ zum Vorgänger)")]
        public float DirectionY
        {
            get { return direction.Y; }
            set { direction.Y = value; }
        }
        [Category("Canyon"), Description("Canyon Richtung (relativ zum Vorgänger)")]
        public float DirectionZ
        {
            get { return direction.Z; }
            set { direction.Z = value; }
        }

        private Vec2[] walls;
        [Category("Canyon"), Description("Canyon Wände")]
        public Vec2[] Walls
        {
            get { return walls; }
            set { walls = value; }
        }

        private EditorDescription[] objects;
        [Category("Canyon"), Description("Objekte hier im Canyon")]
        public EditorDescription[] Objects
        {
            get { return objects; }
            set { objects = value; }
        }

        private string decoModelName;
        [Category("Decoration"), Description("Modelname für Dekorationsobjekte")]
        public string DecoModelName
        {
            get { return decoModelName; }
            set { decoModelName = value; }
        }
        private string decoDensity;
        [Category("Decoration"), Description("Dichte der Dekorationsobjekte")]
        public string DecoDensity
        {
            get { return decoDensity; }
            set { decoDensity = value; }
        }
        private string decoMinSize;
        [Category("Decoration"), Description("Minimale Größe der Dekorationsobjekte")]
        public string DecoMinSize
        {
            get { return decoMinSize; }
            set { decoMinSize = value; }
        }
        private string decoMaxSize;
        [Category("Decoration"), Description("Maximale Größe der Dekorationsobjekte")]
        public string DecoMaxSize
        {
            get { return decoMaxSize; }
            set { decoMaxSize = value; }
        }
    }
}
