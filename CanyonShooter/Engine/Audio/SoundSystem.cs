using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace CanyonShooter.Engine.Audio
{
    /// <summary>
    /// This is the main Sound System class.
    /// @author: M.Rodriguez
    /// @date: 02.12.2007
    /// </summary>
    public class SoundSystem : ISoundSystem
    {
        #region Sound System Members
        private AudioEngine engine;
        private WaveBank wavebank;
        private SoundBank soundbank;
        private SoundListener listener;
        private float mvolume = 0.0f;
        private float evolume = 0.0f;
        private ICanyonShooterGame game;
        private List<ISound> sounds = new List<ISound>();
        private Dictionary<string, SoundType> lib = new Dictionary<string, SoundType>();
        private static Random rnd = new Random();
        private ISound currentTrack;
        private MusicBoxStatus currentStatus;
        private int mtracks = 0;
        private int midx = 0;
        private AudioCategory category;
        #endregion Sound System Members

        /// <summary>
        /// Ordinary constructor.
        /// </summary>
        /// <param name="theGame">The main game object</param>
        public SoundSystem(ICanyonShooterGame theGame)
        {
            game = theGame;
            Initialize();
        }

        /// <summary>
        /// The sound listener property.
        /// </summary>
        public SoundListener SoundListener
        {
            get
            {
                return listener;
            }
            set
            {
                listener = value;
            }
        }

        #region IGameComponent Member

        /// <summary>
        /// Initialize all the sound and wave banks. Start the sound engine and
        /// create a sound listener for 3d sound.
        /// </summary>
        public void Initialize()
        {
            // Initialize the main XNA sound system.
            engine = new AudioEngine(@"Content\Audio\CanyonShooter.xgs");
            wavebank = new WaveBank(engine, @"Content\Audio\Waves.xwb");
            soundbank = new SoundBank(engine, @"Content\Audio\Sounds.xsb");

            // Set the default listener. Maybe changed by player to inherit the
            // music box position.
            SoundListener = new SoundListener(game);

            // initialize sound matching
            lib.Add("Lasergun", SoundType.Effect);
            lib.Add("LasergunMiddle", SoundType.Effect);
            lib.Add("LasergunSlow", SoundType.Effect);
            lib.Add("LasergunHigh", SoundType.Effect);
            lib.Add("Fire", SoundType.Effect);
            lib.Add("Thunder", SoundType.Effect);
            lib.Add("RocketLaunch", SoundType.Effect);
            lib.Add("ReleaseMines", SoundType.Effect);
            lib.Add("Heli", SoundType.Effect);
            lib.Add("Desruptor", SoundType.Effect);
            lib.Add("Clock", SoundType.Effect);
            lib.Add("Explosion", SoundType.Effect);
            lib.Add("Ammo", SoundType.Effect);
            lib.Add("Beam", SoundType.Effect);
            lib.Add("Phaser", SoundType.Effect);
            lib.Add("UFO", SoundType.Effect);
            lib.Add("UFOLoop", SoundType.Effect);
            lib.Add("UFOStop", SoundType.Effect);
            lib.Add("UFOStart", SoundType.Effect);
            lib.Add("HeliAlternative", SoundType.Effect);
            lib.Add("MFireLoop", SoundType.Effect);
            lib.Add("MFireStart", SoundType.Effect);
            lib.Add("MFireStop", SoundType.Effect);
            lib.Add("GodMode", SoundType.Effect);
            lib.Add("Alert", SoundType.Effect);
            lib.Add("GameOver", SoundType.Effect);
            lib.Add("ExtraLife", SoundType.Effect);
            lib.Add("Button", SoundType.Effect);
            lib.Add("Welcome", SoundType.Effect);

            // Here comes the music library
            lib.Add("Trace", SoundType.Music);
            lib.Add("Past", SoundType.Music);
            lib.Add("Match", SoundType.Music);
            lib.Add("Energized", SoundType.Music);
            lib.Add("BritishAirwaves", SoundType.Music);
            lib.Add("GlobalPlayers", SoundType.Music);
            lib.Add("CriticalOrder", SoundType.Music);
            lib.Add("Dystopia", SoundType.Music);
            lib.Add("Respective", SoundType.Music);
            lib.Add("Trancefer", SoundType.Music);
            lib.Add("OutOfDimension", SoundType.Music);
            
            // Total music tracks used by music box
            mtracks = 11;
        }

        #endregion

        private void DisplayArtistName() 
        {
            string msg = GetMusicName();
            

            // Display title on hud or game console
            if (game.GameStates.Hud != null)
            {
                game.GameStates.Hud.DisplaySoundTitle(msg);
            }
            else 
            {
                CanyonShooter.GameClasses.Console.GraphicalConsole.GetSingleton(game).WriteLine(msg, 0);
            }
        }

        #region ISoundSystem Member
        /// <summary>
        /// Update the sound engine playbacks.
        /// </summary>
        public void Update()
        {
            // Refresh the music box state machine.
            if (currentStatus == MusicBoxStatus.Play && currentTrack.Playing() == false) 
            {
                MusicBox(MusicBoxStatus.Play);
            }

            // Refresh all the loopable sounds.
            for (int i = 0; i < sounds.Count; i++)
            {
                if (sounds[i].Loop)
                {
                    if(!sounds[i].Playing())
                    {
                        sounds[i].Play();
                    }
                }
            }
            // Update the main engine.
            engine.Update();
        }

        /// <summary>
        /// Property of the main music volume.
        /// </summary>
        public float MusicVolume
        {
            get
            {
                return mvolume;
            }
            set
            {
                mvolume = value;
                if (!engine.IsDisposed)
                {
                    category = engine.GetCategory("Music");
                    category.SetVolume(mvolume);
                }
            }
        }

        /// <summary>
        /// Property of the effect music volume.
        /// </summary>
        public float EffectVolume
        {
            get
            {
                return evolume;
            }
            set
            {
                evolume = value;
                category = engine.GetCategory("Effects");
                category.SetVolume(evolume);
            }
        }

        /// <summary>
        /// Get the current sound playback status.
        /// </summary>
        /// <returns>True if the instance plays a sound.</returns>
        public bool Playing()
        {
            for (int i = 0; i < sounds.Count; i++)
            {
                if (sounds[i].Playing()) return true;
            }
            return false;
        }

        /// <summary>
        /// Turn on the music box and play randomized
        /// background music.
        /// </summary>
        /// <param name="status">The new music box status</param>
        public void MusicBox(MusicBoxStatus status) 
        {
            currentStatus = status;
            int  i = 0;
            switch (currentStatus)
            {
                case MusicBoxStatus.Play:
                    // Statemachine PLAY
                    if (currentTrack != null && currentTrack.Playing() == true)
                    {
                        currentTrack.Stop();
                    }
                    // if no music is playing, create random and play
                    if (currentTrack == null || currentTrack.Playing() == false)
                    {
                        // New index not the same
                        int t;
                        while( (t = rnd.Next(0, mtracks)) == midx) { }
                        midx = t;
                        // Load new track
                        foreach (KeyValuePair<string, SoundType> s in lib)
                        {
                            if (s.Value == SoundType.Music)
                            {
                                if (midx == i)
                                {
                                    currentTrack = CreateSound(s.Key);
                                    break;
                                }
                                i++;
                            }
                        }
                    }
                    if (!currentTrack.Playing())
                    {
                        currentTrack.Play();
                        DisplayArtistName();
                    }
                    break;
                case MusicBoxStatus.Stop:
                    // Statemachine STOP
                    currentTrack.Stop();
                    break;
                case MusicBoxStatus.Pause:
                    // Statemachine PAUSE
                    currentTrack.Pause();
                    break;
            }
        }

        /// <summary>
        /// Create new sounds with this Sound Factory Method. The
        /// new sound will appear in the soundqueue.
        /// Choose one of the follwing effects:
        /// Lasergun, LasergunMiddle, LasergunSlow, LasergunHigh,
        /// Thunder, RocketLaunch, ReleaseMines, Heli, Desruptor
        /// Clock, Explosion, Ammo, Beam, Phaser, UFO, UFOLoop, 
        /// UFOStop, UFOStart, HeliAlternative, MFireLoop, MFireStart,
        /// MFireStop, Malt, MSelect, GodMode, Alert, GameOver,
        /// ExtraLife, Button
        /// <param name="soundName">The unique identification name of a sound.</param>
        /// <returns>An instance of the sound object.</returns>
        public ISound CreateSound(string soundName)
        {
            ISound snd = null;

            // Set initial audio subsystem
            foreach (KeyValuePair<string, SoundType> s in lib)
            {
                if (s.Key == soundName) 
                {
                    // Create sound
                    snd = new Sound(game, SoundListener, soundName,
                        engine, soundbank, wavebank, s.Value, this);

                    //  Set initial volume
                    switch(s.Value) 
                    {
                        case SoundType.Music:
                            snd.Volume = this.MusicVolume;
                            // Add sound to queue
                            sounds.Add(snd);
                            break;
                        case SoundType.Effect:
                            snd.Volume = this.EffectVolume;
                            // Add sound to queue
                            sounds.Add(snd);
                            break;
                    }
                }
            }

            return snd;
        }

        /// <summary>
        /// Unload all music content.
        /// </summary>
        public void Shutdown()
        {
            engine.Dispose();
        }

        /// <summary>
        /// Stop the playback of all sounds imidiatly.
        /// </summary>
        public void AllStop()
        {
            for (int i = 0; i < sounds.Count; i++)
            {
                sounds[i].Stop();
            }
        }

        /// <summary>
        /// Remove the sound reference from playing queue.
        /// </summary>
        /// <param name="sound"></param>
        public void RemoveSoundReference(ISound sound)
        {
            sounds.Remove(sound);
        }

        /// <summary>
        /// Get the current musicbox track name.
        /// </summary>
        /// <returns>String identificating the current track name</returns>
        public string GetMusicName()
        {
            string msg = "";
            switch (currentTrack.Name)
            {
                case ("OutOfDimension"):
                    msg = "Music: Out Of Dimension";
                    break;
                case ("Trancefer"):
                    msg = "Music: Trancefer";
                    break;
                case ("Respective"):
                    msg = "Music: Respective";
                    break;
                case ("Dystopia"):
                    msg = "Music: Dystopia";
                    break;
                case ("Trace"):
                    msg = "Music: Trace";
                    break;
                case ("Past"):
                    msg = "Music: Past";
                    break;
                case ("Match"):
                    msg = "Music: Match";
                    break;
                case ("Energized"):
                    msg = "Music: Energized";
                    break;
                case ("BritishAirwaves"):
                    msg = "Music: British Airwaves";
                    break;
                case ("GlobalPlayers"):
                    msg = "Music: Global Players";
                    break;
                case ("CriticalOrder"):
                    msg = "Music: Critical Order";
                    break;
                default:
                    break;
            }
            return msg;
        }

        #endregion
    }
}
