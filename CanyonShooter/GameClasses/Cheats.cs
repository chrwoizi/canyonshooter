using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using CanyonShooter.Engine.Audio;
using CanyonShooter.GameClasses.Console;

namespace CanyonShooter.GameClasses
{
    public class Cheats : GameComponent
    {
        public Cheats(ICanyonShooterGame game):base(game as Game)
        {
            this.game = game;
            godmodeSound = game.Sounds.CreateSound("GodMode");
        }

        private ICanyonShooterGame game;
        private ISound godmodeSound;
        private bool godMode;
        public bool GodMode
        {
            get
            {
                return godMode;
            }
            set
            {
                if (value == true && !godMode)
                {
                    // Here is Godmode
                    game.World.Players[0].SetModel("MegaWumma");
                    game.World.Players[0].Fuel = 100000;
                    game.World.Players[0].MaxBoosterHeat = 100000;
                    if (godmodeSound != null)
                    {
                        GraphicalConsole.GetSingleton(game).WriteLine(
                            "God Mode ON !!! in honor of Edwin van Santen - 'Layla Mix'", 0);
                        game.Sounds.MusicBox(MusicBoxStatus.Stop);
                        godmodeSound.Stop();
                        godmodeSound.Loop = true;
                        godmodeSound.Play();
                    }
                }
                else
                {
                    if (godmodeSound != null)
                    {
                        godmodeSound.Stop();
                        godmodeSound.Loop = false;
                    }
                    game.Sounds.MusicBox(MusicBoxStatus.Play);
                    GraphicalConsole.GetSingleton(game).WriteLine(
                            "God Mode OFF", 0);
                }
                godMode = value;
            }
        }


    }
}
