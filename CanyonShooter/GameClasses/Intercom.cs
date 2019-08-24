using System;
using System.Collections.Generic;
using System.Text;
using CanyonShooter.GameClasses.Console;
using CanyonShooter.GameClasses.World.Enemies;
using SpeechLib;
using System.Threading;

namespace CanyonShooter.GameClasses
{
    public static class Intercom
    {
        public static ICanyonShooterGame Game;

        public static void SendMessage(string message, bool speak)
        {
            if (Game != null)
                GraphicalConsole.GetSingleton(Game).WriteLine(message, 1);
            if (speak)
            {
                Thread speakThread = new Thread(SpeakInternal);
                speakThread.Start(message);
            }
        }

        public static void Speak(string message)
        {
            Thread speakThread = new Thread(SpeakInternal);
            speakThread.Start(message);
        }

        private static void SpeakInternal(object text)
        {
            try
            {
                if (Game.GameStates.Profil.CurrentProfil.Sound)
                {
                    SpVoice voice = new SpVoice();
                    voice.Volume = (int)(Game.GameStates.Profil.CurrentProfil.Effect * 100);
                    voice.Speak(text.ToString(), SpeechVoiceSpeakFlags.SVSFDefault);
                }
            }
            catch
            {
                // suppress any errors
            }
        }

        public static void EnemWithStingerRocketsDetected(string name)
        {
            SendMessage("Our zonescanner detected " + name +".",false);
            SendMessage("Be carefull of Rockets!",true);
        }

        public static void PlayerIsInTargetOf(string name)
        {
            SendMessage("Warning! You are in target of " + name + "!", true);
        }

        public static void PlayerLeavingCanyon()
        {
            SendMessage("Don't violate the rules", true);
        }

        public static void GiveHealth()
        {
            SendMessage("Health", true);
        }

        public static void GiveShield()
        {
            SendMessage("Shield", true);
        }

        public static void GiveExtraLife()
        {
            SendMessage("Extra Life", true);
        }

        public static void GiveFuel()
        {
            SendMessage("Fuel", true);
        }

        public static void GiveWeapon()
        {
            SendMessage("Weapon", true);
        }

        public static void GiveAmmo()
        {
            SendMessage("Ammo", true);
        }

        public static void HurryUp()
        {
            SendMessage("Hurry Up !", true);
        }
    }
}
