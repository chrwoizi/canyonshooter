//
//  @ Project : CanyonShooter
//  @ File Name : IProfil.cs
//  @ Date : 15.01.2008
//  @ Author : Sascha Lity
//

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;

namespace CanyonShooter.Profils
{
    /// <summary>
    /// Interface to communicate with the Profil
    /// </summary>
    public interface IProfil : IGameComponent, IDrawable
    {
        bool Quick { get; set;}
        string QuickLevel { get; set;}
        /// <summary>
        /// Delete a Profil and set the standardvalues
        /// </summary>
        void deleteProfil();

        /// <summary>
        /// writeProfilName(string Name) set the Profil´s name
        /// </summary>
        /// <param name="Name">Profil´s name</param>
        void writeProfilName(string Name);
        
        /// <summary>
        /// Set the Profil´s Soundvalues
        /// </summary>
        /// <param name="parameter">Contains the Soundvalues</param>
        void writeProfilSound(Dictionary<string, object> parameter);

        /// <summary>
        /// Set the Profil´s Graphikvalues
        /// </summary>
        /// <param name="parameter">Contains the Graphikvalues</param>
        void writeProfilGrafik(Dictionary<string, object> parameter);

        /// <summary>
        /// Set the Profil´s Controlvalues
        /// </summary>
        /// <param name="parameter">Contains the Controlvalues</param>
        void writeProfilSteuerung(Dictionary<string, object> parameter);

        /// <summary>
        /// Set the Profil´s Playervalues
        /// </summary>
        /// <param name="parameter">Contains the Playervalues</param>
        void writeProfilSpielereinstellungen(Dictionary<string, object> parameter);

        /// <summary>
        /// Set the Profil´s Playermodel
        /// </summary>
        /// <param name="Name">Name of the Model</param>
        /// <param name="shield">Shieldvalue of the Model</param>
        /// <param name="speed">Speedvalue of the Model</param>
        /// <param name="health">Healthvalue of the Model</param>
        void writeProfilGleiter(string Name, int shield, int speed, int health, int fuel);

        ///<summary>
        ///Get/Set the current userprofil
        ///</summary>
        ProfilProperties CurrentProfil { get; set; }

        /// <summary>
        /// Get/Set the current number of a userprofil
        /// </summary>
        int CurrentNumber { get; set; }

        /// <summary>
        /// Get/Set the state if Profil is called from Menu\Option or not
        /// </summary>
        bool Menü { get; set; }

        /// <summary>
        /// Get/Set the state if you Exit the Game after pressing the ESC-Key
        /// Only false(you can quit the game) when there is no active Profil at 
        /// the beginning.
        /// </summary>
        bool Esc { get; set; }

        /// <summary>
        /// Get/Set the state if the Menu\Optionen is started after going back from
        /// the Profilchoice to Menu\Optionen.
        /// </summary>
        bool Opt { get; set; }

        /// <summary>
        /// Get/Set the Graphikvalues
        /// </summary>
        Dictionary<string, Object> GrafikList { get; set; }

        /// <summary>
        /// Get/Set the Soundvalues
        /// </summary>
        Dictionary<string, Object> SoundList { get; set; }

        /// <summary>
        /// Get/Set the Controlvalues
        /// </summary>
        Dictionary<string, Object> SteuerungList { get; set; }

        /// <summary>
        /// Get/Set the Playervalues
        /// </summary>
        Dictionary<string, Object> SpielereinstellungenList { get; set; }

        /// <summary>
        /// Get the name of Datatypes of Graphikvalues for valuecasting
        /// </summary>
        Dictionary<string, string> GrafikDat { get;}

        /// <summary>
        /// Get the name of Datatypes of Soundvalues for valuecasting
        /// </summary>
        Dictionary<string, string> SoundDat { get;}

        /// <summary>
        /// Get the name of Datatypes of Controlvalues for valuecasting
        /// </summary>
        Dictionary<string, string> SteuerungDat { get; }

        /// <summary>
        /// Get the name of Datatypes of Playervalues for valuecasting
        /// </summary>
        Dictionary<string, string> SpielereinstellungenDat { get;}

        /// <summary>
        /// Get list of Keys(Sound)
        /// </summary>
        List<string> NamenKeysSound { get; }

        /// <summary>
        /// Get list of Keys(Graphik)
        /// </summary>
        List<string> NamenKeysGrafik { get; }

        /// <summary>
        /// Get list of Keys(Graphik)
        /// </summary>
        List<string> NamenKeysSteuerung { get; }

        /// <summary>
        /// Get list of Keys(Player)
        /// </summary>
        List<string> NamenKeysSpielereinstellungen { get; }
    }
}
