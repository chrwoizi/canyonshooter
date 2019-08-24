// Zuständigkeit: Richard

#region Using Statements

using System;
using CanyonShooter.DataLayer.Level;
using CanyonShooter.Engine.Graphics.Cameras;
using CanyonShooter.Engine.Graphics.Effects;
using CanyonShooter.Engine.Helper;
using CanyonShooter.Engine.Physics;
using CanyonShooter.GameClasses.Items;
using CanyonShooter.GameClasses.Scores;
using CanyonShooter.GameClasses.Weapons;
using CanyonShooter.GameClasses.World;
using Microsoft.Xna.Framework;
// For audio and mesh animation by M.R.
using CanyonShooter.Engine.Audio;
using CanyonShooter.Engine.Graphics;
// ------------------------------------
using XnaDevRu.Physics;

#endregion

namespace CanyonShooter.GameClasses
{
    /// <summary>
    /// Verwaltet die Eigenschaften, Scores und Items eines Spielers.
    /// </summary>
    public class Player : GameObject, IPlayer
    {
        public Score score;
        private ICanyonShooterGame game = null;
        private int playerID;
        private int health = 100;
        private float angHorz = 0;
        private float angVert = 0;
        private float angHorzMin = 0;
        private float angHorzMax = 0;
        private float angRoll = 0;
        private float angHorzCamera = 0;
        private float angVertCamera = 0;
        private float WeaponX = 0.5f;
        private float WeaponY = 0.5f;
        private float speed = 0;
        private float SpeedF = 0; // 0 = Normal, 1 = Turbo
        // Sound Listener by M.R. The player position is the music box position.
        private SoundListener listener;
        private ISound meshSoundEfx;
        private IMesh modelAnimationPart;
        // -------------------------------------------------------------------
        private PerspectiveCamera camera;

        public ICamera Camera
        {
            get
            {
                return camera;
            }
        }

        public Player(ICanyonShooterGame game, int PlayerID)
            : base(game, "player"+PlayerID)
        {
            // TODO: Construct any child components here
            this.game = game;
            playerID = PlayerID;

            camera = new PerspectiveCamera(game);

            // Sound Listener by M.R. Set SoundSystem listener.
            listener = new SoundListener(game);
            game.Sounds.SoundListener = listener;
            // -----------------------------------------------
            SetModel(game.GameStates.Profil.CurrentProfil.Model);
            //SetModel("F35eger");
            //SetModel("Starfighter");
            //SetModel("Fighter");
            //SetModel("Drone");
            //SetModel("Car");
            //SetModel("G35hopper");
            LocalPosition = /*this.LocalPosition + */ new Vector3(0, 0, 0);
            //LocalScale = new Vector3(0.15f, 0.4f, 0.6f);
            //LocalScale = new Vector3(0.5f, 0.5f, 0.5f);
            //Scale(new Vector3(0.5f, 0.5f, 0.5f));
            ConnectedToXpa = true;
            ContactGroup = ContactGroup.Player;
            InfluencedByGravity = false;
            //Velocity = new Vector3(0, 0, -20.0f);

            //this.Solid.AddShape(t11);
            //this.Solid.AddShape(t12);
            //this.Solid.UserData = this;

            /*
            SphereShapeData t11 = new SphereShapeData();
            t11.Radius = 20.0f;
            t11.Material.Hardness = 0.2f;
            t11.Material.Bounciness = 0.6f;
            t11.Material.Friction = 1.0f;
            t11.Material.Density = 0.4f;
            t11.Offset.Translation = new Vector3(5, 4, 0);
            SphereShapeData t12 = new SphereShapeData();
            t12.Radius = 20.0f;
            t12.Material.Hardness = 0.2f;
            t12.Material.Bounciness = 0.6f;
            t12.Material.Friction = 1.0f;
            t12.Material.Density = 0.4f;
            t12.Offset.Translation = new Vector3(-5, 0, 0);

            obj = new GameObject[2];

            obj[0] = new GameObject(this, "0");
            obj[0].SetModel("Drone");
            obj[0].LocalPosition = obj[0].LocalPosition + new Vector3(0, 0, -50);
            obj[0].ConnectedToXpa = true;
            obj[0].Solid.AddShape(t11);
            obj[0].Solid.AddShape(t12);
            obj[0].Solid.UserData = obj[0];
            obj[1] = new GameObject(this, "1");
            Components.Add(obj[1]);
            obj[1].SetModel("Roket");
            obj[1].LocalPosition = obj[1].LocalPosition + new Vector3(-10, 0, 0);
            //obj[1].Solid.UserData = obj[1];
            obj[1].Parent = obj[0];
            
            Components.Add(obj[0]);

            Solid platte = physics.CreateSolid();
            PlaneShapeData planeData = new PlaneShapeData(new Plane(new Vector3(0, 1, 0), -30));
            planeData.Material.Hardness = 1f;
            planeData.Material.Bounciness = 0.9f;
            planeData.Material.Friction = 1.0f;
            planeData.Material.Density = 0.8f;
            platte.Tag = "Bodenplatte";
            platte.AddShape(planeData);
            platte.Static = true;
            */

        }


        protected override void LoadContent()
        {
            base.LoadContent();
            if (Weapons == null)
            {
                //Initialize WeaponManager
                Weapons = new WeaponManager(game, Model, WeaponHolderType.Player);
                
                // Equip some cool weapons:
                //TODO: remove this later.
                Weapons.AddWeapon(WeaponType.ROCKET_STINGER);
                Weapons.AddWeapon(WeaponType.ULTRA_PHASER);
                Weapons.AddWeapon(WeaponType.MINIGUN);
                Weapons.AddWeapon(WeaponType.MINIGUN2);
                Weapons.AddWeapon(WeaponType.PLASMAGUN);
                
            }
            else
            {
                Weapons.WeaponHolder = Model;
            }
            // For audio and mesh animation by M.R.
            modelAnimationPart = Model.GetMesh("Rotor");
            if (modelAnimationPart != null)
            {
                meshSoundEfx = game.Sounds.CreateSound("Heli");
                meshSoundEfx.Play();
                modelAnimationPart.PlayRotationAnimation();
            }
            // -------------------------------------
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here);

            base.Initialize();
            LocalPosition = game.World.Level.Cache[1].APos / 2; // In der Mitte des ersten Segments starten
        }

        private int canyonPosition = 0;
        public int CanyonPosition { get { return canyonPosition; } }
        private void UpdatePosition()
        {
            if (game.World.Level.GetDistanceToSegmentConnection(LocalPosition,canyonPosition+1) > 0)
            {
                canyonPosition++;
                UpdatePosition();
            }
        }

        private const float MaxMove = 0.02f;//0.03f;
        private const float NormSpeed = 150;
        private const float BoostSpeed = 500;
        private const float MaxHorzAngleDiff = (float)Math.PI / 6; // 30°

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            float steuerVert = 0;
            float steuerHorz = 0;
            bool booster = false;
            bool firePrimary = false;
            bool fireSecondary = false;

            if (game.GameStates.InputFocus == game.World && health > 0)
            {
                if (game.Input.IsKeyDown("Player" + playerID + ".Up"))
                {
                    steuerVert = -MaxMove;
                }
                if (game.Input.IsKeyDown("Player" + playerID + ".Down"))
                {
                    steuerVert = MaxMove;
                }
                if (game.Input.IsKeyDown("Player" + playerID + ".Left"))
                {
                    steuerHorz = MaxMove;
                }
                if (game.Input.IsKeyDown("Player" + playerID + ".Right"))
                {
                    steuerHorz = -MaxMove;
                }
                booster = game.Input.IsKeyDown("Player" + playerID + ".Boost");
                if (game.Input.IsKeyDown("Player" + playerID + ".PrimaryFire"))
                {
                    firePrimary = true;
                }
                if (game.Input.IsKeyDown("Player" + playerID + ".SecondaryFire"))
                {
                    fireSecondary = true;
                }
                if (game.Input.HasKeyJustBeenPressed("Player" + playerID + ".PrimarySwitchUp"))
                {
                    Weapons.SwitchUp(true);
                }
                if (game.Input.HasKeyJustBeenPressed("Player" + playerID + ".PrimarySwitchDown"))
                {
                    Weapons.SwitchDown(true);
                }
                if (game.Input.HasKeyJustBeenPressed("Player" + playerID + ".SecondarySwitchUp"))
                {
                    Weapons.SwitchUp(false);
                }
                if (game.Input.HasKeyJustBeenPressed("Player" + playerID + ".SecondarySwitchDown"))
                {
                    Weapons.SwitchDown(false);
                }
                Vector2 mm = game.Input.MouseMovement;
                WeaponX += mm.X / 500f;
                WeaponY += mm.Y / 500f;
                if (WeaponX < 0) WeaponX = 0;
                if (WeaponY < 0) WeaponY = 0;
                if (WeaponX > 1) WeaponX = 1;
                if (WeaponY > 1) WeaponY = 1;

            }
            if (booster)
            {
                speed = speed * 0.95f + BoostSpeed * 0.05f;
                SpeedF = SpeedF * 0.95f + 1 * 0.05f;
            }
            else
            {
                speed = speed * 0.95f + NormSpeed * 0.05f;
                SpeedF = SpeedF * 0.95f + 0 * 0.05f;
            }

            angHorz += steuerHorz;
            angVert += steuerVert;

            angRoll += steuerHorz;
            angRoll *= 0.95f;

            // Amounts der Canyon Teile bestimmen

            //TODO: Richard - Fix Crash wenn canyonPosition = 99: +1 -> out of range
          //  System.IndexOutOfRangeException wurde nicht behandelt.
          //Message="Der Index war außerhalb des Arraybereichs."
          //Source="Canyon Shooter"
          //StackTrace:
          //     bei CanyonShooter.GameClasses.Player.Update(GameTime gameTime) in C:\DATEN\WORK\CanyonShooter\svn\CS_XNA2.0\CanyonShooter\GameClasses\Player\Player.cs:Zeile 269.
          //     bei Microsoft.Xna.Framework.Game.Update(GameTime gameTime)
          //     bei CanyonShooter.CanyonShooterGame.Update(GameTime gameTime) in C:\DATEN\WORK\CanyonShooter\svn\CS_XNA2.0\CanyonShooter\CanyonShooterGame.cs:Zeile 155.
          //     bei Microsoft.Xna.Framework.Game.Tick()
          //     bei Microsoft.Xna.Framework.Game.HostIdle(Object sender, EventArgs e)
          //     bei Microsoft.Xna.Framework.GameHost.OnIdle()
          //     bei Microsoft.Xna.Framework.WindowsGameHost.ApplicationIdle(Object sender, EventArgs e)
          //     bei System.Windows.Forms.Application.ThreadContext.System.Windows.Forms.UnsafeNativeMethods.IMsoComponent.FDoIdle(Int32 grfidlef)
          //     bei System.Windows.Forms.Application.ComponentManager.System.Windows.Forms.UnsafeNativeMethods.IMsoComponentManager.FPushMessageLoop(Int32 dwComponentID, Int32 reason, Int32 pvLoopData)
          //     bei System.Windows.Forms.Application.ThreadContext.RunMessageLoopInner(Int32 reason, ApplicationContext context)
          //     bei System.Windows.Forms.Application.ThreadContext.RunMessageLoop(Int32 reason, ApplicationContext context)
          //     bei System.Windows.Forms.Application.Run(Form mainForm)
          //     bei Microsoft.Xna.Framework.WindowsGameHost.Run()
          //     bei Microsoft.Xna.Framework.Game.Run()
          //     bei CanyonShooter.Program.Main(String[] args) in C:\DATEN\WORK\CanyonShooter\svn\CS_XNA2.0\CanyonShooter\Program.cs:Zeile 12.

            Vector2 tmp2 = new Vector2(game.World.Level.Cache[canyonPosition].ADir.Z, game.World.Level.Cache[canyonPosition].ADir.X);
            Vector2 tmp3 = new Vector2(game.World.Level.Cache[canyonPosition+1].ADir.Z, game.World.Level.Cache[canyonPosition+1].ADir.X);
            float n1,n2,n3;
            game.World.Level.GetSegmentAmounts(LocalPosition, canyonPosition, out n1, out n2, out n3);
            Vector2 tmp = tmp2 * n2 + tmp3 * n3;            

            float angHorzCanyon;
            float angVertCanyon;
            if (tmp.X == 0)
                angHorzCanyon = 0;
            else if (tmp.X > 0)
                angHorzCanyon = (float)Math.PI + (float)Math.Atan(tmp.Y / tmp.X);
            else
                angHorzCanyon = (float)Math.Atan(tmp.Y / tmp.X);
            angVertCanyon = 0;

            // Bewegungsbereich maximal einschränken, jedoch nur wenn er nicht gerade ausgeschöpft wird
            if (angHorz > angHorzMin)
                angHorzMin = angHorz;
            if (angHorz < angHorzMax)
                angHorzMax = angHorz;

            // Bewegungsbereich je nach Canyon Beschaffenheit ausdehnen
            if (angHorzMin > angHorzCanyon - MaxHorzAngleDiff)
                angHorzMin = angHorzCanyon - MaxHorzAngleDiff;
            if (angHorzMax < angHorzCanyon + MaxHorzAngleDiff)
                angHorzMax = angHorzCanyon + MaxHorzAngleDiff;

            // Wenn wir über dem Bewegungsbereich sind, dann korrigieren
            if (angHorz < angHorzMin)
                angHorz = (angHorz - angHorzMin) * 0.90f + angHorzMin;
            if (angHorz > angHorzMax)
                angHorz = (angHorz - angHorzMax) * 0.90f + angHorzMax;

            //angHorz *= 0.95f;
            angVert *= 0.97f;
            //if (health > 0)
            {
                LocalRotation = Quaternion.CreateFromYawPitchRoll(angHorz, angVert, angRoll);
                //Move(Vector3.Transform(new Vector3(0, 0, -Speed*2), LocalRotation));
                Velocity = Vector3.Transform(new Vector3(0, 0, -speed), LocalRotation);
                //Velocity = game.World.Level.Cache[canyonPosition].ADir;
            }
            /*else
            {
                Velocity = new Vector3();
            }*/
            UpdatePosition();

            // Bounding Box Test
            //Vector2 Pfadabstand = game.World.Level.GetDistanceToSegment(LocalPosition, canyonPosition);
            //if (Math.Abs(Pfadabstand.X) > Level.CanyonSpanFactor || Math.Abs(Pfadabstand.Y) > Level.CanyonSpanFactor)
            //{
            //    health = 0;
            //}

            // Kameraeinstellungen
            angHorzCamera = angHorzCamera * 0.97f + angHorzCanyon * 0.03f;
            angVertCamera = angVertCamera * 0.97f + angVertCanyon * 0.03f; 
            Quaternion camq = Quaternion.CreateFromYawPitchRoll(angHorzCamera, 0.0f, 0.0f);
            camera.LocalPosition = LocalPosition + Vector3.Transform(
                new Vector3(0, 12 * 2 / 1.7f * SpeedF + 12 * 2 * (1 - SpeedF), 30 * 2 / 1.7f * SpeedF + 30 * 2 * (1 - SpeedF)), camq);
            camera.LocalRotation = camq;
            camera.Fov = 130* SpeedF + 90 * (1 - SpeedF);

            // Lokale Waffenrotation
            Quaternion WR = Quaternion.CreateFromYawPitchRoll(-(WeaponX - 0.5f), -(WeaponY - 0.5f), 0);
            Weapons.PrimaryWeapon.Parent.LocalRotation = WR;
            //Weapons.SecondaryWeapon.Parent.LocalRotation = WR;
            // Globale Waffenrotation
            WR = Quaternion.Concatenate(WR, camq);
            Vector3 WF = Vector3.Transform(new Vector3(0, 0, -speed), WR);

            // Waffenaiming
            Weapons.PrimaryWeapon.AimAt(WF);
            //Weapons.SecondaryWeapon.AimAt(WF);
            // Waffenfeuer
            if (firePrimary)
            {
                    Weapons.PrimaryWeapon.BeginFire();
            }
            else
            {
                    Weapons.PrimaryWeapon.EndFire();
            }

            if (fireSecondary)
            {
                    //Weapons.SecondaryWeapon.BeginFire();
            }
            else
            {
                    //Weapons.SecondaryWeapon.EndFire();
            }

            // For audio and mesh animation by M.R.
            if (modelAnimationPart != null)
            {
                if (!meshSoundEfx.Playing())
                    meshSoundEfx.Play();
            }
            // -------------------------------------
        }

        public WeaponManager Weapons = null;

        public override void Explode()
        {
            base.Explode();
            // For audio and mesh animation by M.R.
            if (modelAnimationPart != null)
            {
                meshSoundEfx.Stop();
                meshSoundEfx.Dispose();
            }
            // ------------------------------------
            Visible = false;
        }



        #region Static Player Helper Functions
        // Added by Florian

        /// <summary>
        /// Aims at player.
        /// Calculates the promising direction in reference to the speed
        /// of the target and the projectile to shoot.
        /// Warning. Use this function with care.
        /// </summary>
        /// <param name="fromPosition">From position.</param>
        /// <param name="weaponSpeed">The weapon speed.</param>
        /// <param name="playerId">The player id.</param>
        /// <returns>probably best direction to hit the target</returns>
        public static Vector3 AimAtPlayer(ICanyonShooterGame game, Vector3 fromPosition, float weaponSpeed, int playerId)
        {
            #region How it works
            /* Fire Direction
            dg = (Z-G+t*vz*dz)/(vg*t)
            t = |Z-G| / (vg-vz)

            dg: richtung des projektils (länge 1)
            dz: richgun des ziels (länge 1)
            vg: geschwindigkeit des projektils (float)
            vz: geschwindigkeit des ziels (float)
            Z: position des ziels
            G: startpunkt des projektils
            */

            //Vector3 pZ = game.World.Players[0].GlobalPosition;
            //Vector3 pG = GlobalPosition;
            //float vZ = game.World.Players[0].Velocity.Length();
            //float vG = 500;
            //Vector3 dirZ = game.World.Players[0].Velocity;
            //dirZ.Normalize();

            //float t = (pZ - pG).Length() / (vG - vZ);
            //Vector3 dg = (pZ - pG + t * vZ * dirZ) / (vG * t);
            //weapons.SecondaryWeapon.Fire(LocalPosition, dg);
            #endregion

            Vector3 pZ = game.World.Players[playerId].GlobalPosition;
            float vZ = game.World.Players[0].Velocity.Length();
            Vector3 dirZ = game.World.Players[0].Velocity;
            dirZ.Normalize();

            float t = (pZ - fromPosition).Length() / (weaponSpeed - vZ);
            return (pZ - fromPosition + t * vZ * dirZ) / (weaponSpeed * t);
        }

        /// <summary>
        /// Gets the direction to the player.
        /// </summary>
        /// <param name="fromPosition">From position.</param>
        /// <param name="playerId">The player id.</param>
        /// <returns>A normalized direction Vector3</returns>
        public static Vector3 GetDirectionToPlayer(ICanyonShooterGame game, Vector3 fromPosition, int playerId)
        {
            Vector3 dir = -fromPosition + game.World.Players[playerId].GlobalPosition;
            if(dir != Vector3.Zero)
                dir.Normalize();
            return dir;
        }


        /// <summary>
        /// Gets the distance to the player.
        /// </summary>
        /// <param name="fromPosition">From position.</param>
        /// <param name="playerId">The player id.</param>
        /// <returns>The distance to the player.</returns>
        public static float GetDistanceToPlayer(ICanyonShooterGame game, Vector3 fromPosition, int playerId)
        {
            return (-fromPosition + game.World.Players[playerId].GlobalPosition).Length();
        }

        #endregion



        #region IPlayer Members

        public override string Name
        {
            get { return "Player " + playerID; }
        }

        public int Health
        {
            get { return health; }
        }

        public IScore Score
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public void ReceiveDamage(int value)
        {
            if (value < 0)
                throw new Exception("Not allowed");
            health -= value;

            if(health <= 0)
            {
                Explode();
            }
        }

        public void GiveItem(IItem item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool HasItem(IItem item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void RemoveItem(IItem item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        public override void OnCollision(CollisionEvent e)
        {
            base.OnCollision(e);

            //if(e.OtherSolid.UserData == null)
            //    GraphicalConsole.GetSingleton(game).WriteLine("Collision!!!");
            //else
            //    GraphicalConsole.GetSingleton(game).WriteLine("Collision mit " + e.OtherSolid.UserData.ToString());

            // do not burst in flames on collisions with own projectiles :)
            if (!game.Physics.CanSolidsCollide(e.ThisSolid, e.OtherSolid)) return;

            if (!Helper.Lock("Player1.Collision", TimeSpan.FromSeconds(1)))
            {
                game.Sounds.CreateSound("Desruptor").Play();
                IEffect fx = game.Effects.CreateEffect("Explosion");
                fx.Play(TimeSpan.FromSeconds(2),true);
                game.World.AddEffect(fx);
            }
        }

        #region IPlayer Member


        public CanyonShooter.GameClasses.Huds.IHudControl HUDCrosshair
        {
            get { throw new Exception("The method or operation is not implemented."); }
            set { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region IPlayer Member

        int IPlayer.Health
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public int Shield
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion

        #region IPlayer Member


        public double Distance
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion

        #region IPlayer Member


        public int Lifes
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion

        #region IPlayer Member


        public int BoosterHeat
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion

        #region IPlayer Member


        public int RemainingTime
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion

        #region IPlayer Member



        #endregion

        #region IPlayer Member


        public float Speed
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion

        #region IPlayer Member


        public float Fuel
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public float MaxFuel
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion

        #region ITransformable Member


        public void AddShape(ShapeData shape, ContactGroup group)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public new ShapeData GetShape(int id)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void AddForce(Force f)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IPlayer Members


        public float RelativeSpeed
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region IPlayer Member


        public int MaxBoosterHeat
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion
    }
}


