#region Using Statements

using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Input;
using CanyonShooter.Engine.Input;

#endregion

namespace CanyonShooter.Profils
{
    public class ProfilProperties
    {

        #region Data Members
        
        #region Allgemein
        public int ProfilNummer;
        public string ProfilName;
        public bool Active;
        public int PositionX;
        public int PositionY;
        public int ActionrecX;
        public int ActionrecY;
        public int ActionrecWidth;
        public int ActionrecHeigth;

        public bool OnlineHighscore;
        public bool OnlineQuestion;
        #endregion

        #region Parameter

        #region Grafik
        public string Resolution;
        public bool Shadow;
        public bool Fog;
        public float Detail; //Between 0 and 1
        public bool Fullscreen;
        public string AntiAliasing;
        #endregion

        #region Sound
        public float Music;
        public float Effect;
        public bool Sound;
        #endregion

        #region Spielereinstellungen

        public int Difficult;
        public string Playername;
        public string Model;
        public int Health;
        public int Shield;
        public int Speed;
        public int Fuel;

        #endregion
        
        #region Steuerung
        
        public float Translation;  //Translations Level
        public float Acceleration; // Acceleration Level
        public float Brake; //Brake Level
        public float Banking; //Banking Level
        public float Drift; //Drift Level
        public float AutoLevel; //Auto Level 
        public float Rolling; //Rolling Level
        public float mouseIntensity;// Mouse rotation intensity

        public MouseButton PlayerFirePrim;
        public MouseButton PlayerFireSek;
        public Keys PlayerLeftDrift;
        public Keys PlayerRightDrift;
        public Keys PlayerBrake;
        public Keys PlayerAcceleration;
        public Keys PlayerBoost;
        public Keys PlayerPrimWeapon1;
        public Keys PlayerPrimWeapon2;
        public Keys PlayerSekWeapon1;
        public Keys PlayerSekWeapon2;

        #endregion

        #endregion

        #endregion

        #region Load/Save code
        /// <summary>
        /// Saves the current settings
        /// </summary>
        /// <param name="filename">The filename to save to</param>
        public void Save(string filename)
        {
            Stream stream = File.Open(filename, FileMode.Truncate);

            XmlSerializer serializer = new XmlSerializer(typeof(ProfilProperties));
            serializer.Serialize(stream, this);
            stream.Close();
        }

        /// <summary>
        /// Loads settings from a file
        /// </summary>
        /// <param name="filename">The filename to load</param>
        public static ProfilProperties Load(string filename)
        {
            Stream stream = File.OpenRead(filename);
            XmlSerializer serializer = new XmlSerializer(typeof(ProfilProperties));
            ProfilProperties test = new ProfilProperties();
            test = (ProfilProperties)serializer.Deserialize(stream);
            stream.Close();
            return test;
        }
        #endregion
    }
}
