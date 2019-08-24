using CanyonShooter.Engine.Physics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;

namespace CanyonShooter.Engine.Audio
{
    /// <summary>
    /// This class implements the sound object.
    /// @author: M.Rodriguez
    /// @date: 02.12.2007
    /// </summary>
    public class Sound : Transformable, ISound
    {
        #region Sound Members
        private string name = "";
        private float volume = 0.0f;
        private bool loop = false;
        private Cue cue;
        private AudioEngine engine;
        private SoundBank bank;
        private WaveBank wave;
        private SoundType type;
        private SoundListener box;
        private AudioEmitter emitter = new AudioEmitter();
        private ISoundSystem soundSystem;
        private AudioListener al = new AudioListener();
        #endregion Sound Members

        /// <summary>
        /// Ordinary constructor.
        /// </summary>
        /// <param name="game">Main game object</param>
        /// <param name="list">Game sound listener</param>
        /// <param name="sname">Sound name</param>
        /// <param name="e">XACT audio engine</param>
        /// <param name="banks">XACT sound banks</param>
        /// <param name="waves">XACT wave banks</param>
        /// <param name="stype">Sound type</param>
        /// <param name="soundSystem">Current sound system</param>
        public Sound(ICanyonShooterGame game, SoundListener list, string sname, 
            AudioEngine e, SoundBank banks, WaveBank waves, SoundType stype, ISoundSystem soundSystem) 
            : base(game)
        {
            this.soundSystem = soundSystem;
            engine = e;
            bank = banks;
            wave = waves;
            box = list;
            name = sname;
            cue = bank.GetCue(Name);
            type = stype;
        }

        #region ISound Member

        /// <summary>
        /// The sound type of this instance.
        /// </summary>
        public SoundType Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }

        /// <summary>
        /// Each sound has a volume property.
        /// </summary>
        public float Volume
        {
            get
            {
                return volume;
            }
            set
            {
                volume = value;
            }
        }

        /// <summary>
        /// The unique sound name property.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        /// <summary>
        /// Property to set loopable sound.
        /// </summary>
        public bool Loop
        {
            get
            {
                return loop;
            }
            set
            {
                loop = value;
            }
        }

        /// <summary>
        /// Play this sound now.
        /// </summary>
        public void Play()
        {
            // Dispose cue because can only played once
            // see: http://forums.xna.com/thread/11094.aspx
            if(!cue.IsDisposed) cue.Dispose();
            cue = bank.GetCue(Name);
            switch (Type)
            {
                case SoundType.Effect:
                    al.Position = box.ListenerPosition;
                    emitter.Position = this.GlobalPosition;
                    if((al != null) && (emitter != null))
                    try
                    {
                        cue.Apply3D(al, emitter);
                    }
                    catch (System.Exception)
                    {
                        // Catch XNA Sound error. One member of the development team
                        // get errors if multiple sound device are attached.
                    }
                    break;
            }
            // Wait for stopping cue.
            while (cue.IsStopping) { }
            if (!cue.IsPlaying)
            {
                cue.Play();
            }
        }

        /// <summary>
        /// Directly stop the playback.
        /// </summary>
        public void Stop()
        {
            cue.Stop(AudioStopOptions.Immediate);
        }

        /// <summary>
        /// Pause the current sound playback.
        /// </summary>
        public void Pause()
        {
            cue.Pause();
        }

        /// <summary>
        /// Get the current sound playback status.
        /// </summary>
        /// <returns>True if the instance plays a sound.</returns>
        public bool Playing()
        {
            return cue.IsPlaying;
        }

        /// <summary>
        /// Transform the sound emitter positions for 3D sound.
        /// </summary>
        protected override void OnTransform()
        {
            // Calculate new emitter and listener position.
            /*switch (Type)
            {
                case SoundType.Effect:
                    al.Position = box.ListenerPosition;
                    //al.Up = Vector3.TransformNormal(new Vector3(0, 1, 0), Matrix.CreateFromQuaternion(
                    //    new Quaternion(al.Position,1.0f)));
                    //al.Forward = (new Vector3(0,0,1)) * this.GlobalRotation;
                    //al.Velocity = 
                    emitter.Position = this.GlobalPosition;
                    //al.Up = Vector3.TransformNormal(new Vector3(0, 1, 0), Matrix.CreateFromQuaternion(
                    //    new Quaternion(this.GlobalPosition, 1.0f)));
                    //emitter.Forward = (new Vector3(0, 0, 1)) * this.GlobalRotation;
                    //emitter.Velocity = 
                    try 
                    {
                        cue.Apply3D(al,emitter);
                    }
                    catch (System.Exception)
                    {
                        // Catch XNA Sound error. One member of the development team
                        // get errors if multiple sound device are attached.
                    }
                    break;
            }*/
        }

        #endregion

        #region IDisposeable member

        /// <summary>
        /// Dispose the Sound for advanced memory usage.
        /// </summary>
        public override void Dispose()
        {
            soundSystem.RemoveSoundReference(this);
 	        base.Dispose();
        }
        
        #endregion

    }
}
