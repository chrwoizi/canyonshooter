using System;
using System.Collections.Generic;
using System.Text;
using CanyonShooter.Engine.Graphics.Models;
using CanyonShooter.GameClasses.Scores;
using CanyonShooter.GameClasses.Items;
using CanyonShooter.Engine.Graphics.Cameras;
using CanyonShooter.Engine.Physics;
using XnaDevRu.Physics;
using Microsoft.Xna.Framework;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework.Graphics;
using CanyonShooter.GameClasses.World;
using CanyonShooter.GameClasses.Console;
using CanyonShooter.GameClasses.Weapons;
using CanyonShooter.Engine.Graphics;
using CanyonShooter.Engine.Audio;
using CanyonShooter.GameClasses.Huds;
using CanyonShooter.Engine.Helper;
using CanyonShooter.Engine.Graphics.Effects;
using CanyonShooter.GameClasses.World.Enemies;
using System.Threading;

namespace CanyonShooter.GameClasses
{
    class Ghost: GameObject,  IPlayer
    {
        #region Player profile control fields
        private float translationGap = 0.5f;            // Translation level
        private float maxSpeed = 1000;                  // Maximal speed
        private float minSpeed = 500;                   // Minimal speed
        private float speedGap = 10.0f;                 // Acceleration level
        private float boosterGap = 0.01f;               // Booster acceleration level
        private float minBooster = 0.25f;               // Minimal booster distance
        private float maxBooster = 1.0f;                // Maximal booster distance
        private float maxDecay = 10.0f;                 // Brake level
        private float bankingFactor = 0.01f;            // Banking intensity
        private float driftFactor = 0.01f;              // Drift intensity
        private float autoLevel = 0.05f;                // Auto correction intensity
        private float rollFactor = 0.8f;                // Rolling factor
        private float mouseIntensity = -0.001f;         // Mouse rotation intensity
        private float cameraZoom = 0.5f;                // Camera zoom
        private int remainingTime = 60;                // Remaining time for the player
        private double distance = 0;                    // Current Player distance
        private int boosterHeat = 0;                    // Temperature of the booster
        private int maxBoosterHeat = 100;               // Maximal temperature of the booster
        private int minBoosterHeat = 0;                 // Minimal temperature of the booster
        private int boosterHeatGap = 10;                // Booster heat steps
        private float maxFuel = 1000;                   // Maximum Fuel Tanksize
        private float fuel = 1000; //500;                       // 30% BoosterFuel
        #endregion Player profile control fields

        #region Player profile properties
        /// <summary>
        /// Set the maximal booster heat.
        /// </summary>
        public int MaxBoosterHeat
        {
            get { return maxBoosterHeat; }
            set { maxBoosterHeat = value; }
        }
        /// <summary>
        /// Translation factor for left and right translation
        /// </summary>
        public float TranslationGap
        {
            get { return translationGap; }
            set { translationGap = value; }
        }
        /// <summary>
        /// Maximal speed of the starship
        /// </summary>
        public float MaxSpeed
        {
            get { return maxSpeed; }
            set { maxSpeed = value; }
        }
        /// <summary>
        /// Minimal speed of the starship
        /// </summary>
        public float MinSpeed
        {
            get { return minSpeed; }
            set { minSpeed = value; }
        }
        /// <summary>
        /// Acceleration gap for each frame
        /// </summary>
        public float SpeedGap
        {
            get { return speedGap; }
            set { speedGap = value; }
        }
        /// <summary>
        /// Maximal brake decay
        /// </summary>
        public float MaxDecay
        {
            get { return maxDecay; }
            set { maxDecay = value; }
        }
        /// <summary>
        /// Banking level of the starship during left
        /// and right flight
        /// </summary>
        public float BankingFactor
        {
            get { return bankingFactor; }
            set { bankingFactor = value; }
        }
        /// <summary>
        /// Special drift factor for the left and right 
        /// panning
        /// </summary>
        public float DriftFactor
        {
            get { return driftFactor; }
            set { driftFactor = value; }
        }
        /// <summary>
        /// Auto pilot leveling factor
        /// </summary>
        public float AutoLevel
        {
            get { return autoLevel; }
            set { autoLevel = value; }
        }
        /// <summary>
        /// Starship rolling factor during banking
        /// </summary>
        public float RollFactor
        {
            get { return rollFactor; }
            set { rollFactor = value; }
        }
        /// <summary>
        /// The global mouse rotation intensity
        /// </summary>
        public float MouseIntensity
        {
            get { return mouseIntensity; }
            set { mouseIntensity = value; }
        }
        /// <summary>
        /// Camera zoom level for the render camera
        /// </summary>
        public float CameraZoom
        {
            get { return cameraZoom; }
            set { cameraZoom = value; }
        }
        /// <summary>
        /// Players current fuel for boost
        /// </summary>
        public float Fuel
        {
            get{ return fuel;}
            set
            {

                fuel = value;
                if (fuel > maxFuel)
                    fuel = maxFuel;
            }
            
        }
        /// <summary>
        /// Maximal fuel of the starship
        /// </summary>
        public float MaxFuel
        {
            get{ return maxFuel;}
            set{ maxFuel = value;}
        }
        #endregion Player profile properties

        #region Working fields
        private bool isFinish = false;                  // Finished the player...
        private float speed = 1;                      // Current player speed
        private float decay = 0;                        // Current speed decay
        private float angVert = 0;                      // Current vertical angle
        private float angRoll = 0;                      // Current rolling level
        private float angHorz = 0;                      // Current horizontal angle
        private float angHorzTemp = 0;                  // Preframe value
        private float angVertTemp = 0;                  // Preframe value
        private float angRollTemp = 0;                  // Preframe value
        private int health = 100;                       // Players health
        private int shield = 50;                        // Players shield
        private int lifes = 3;                          // Players lifes
        private float shieldDamageAbsorbtion = 0.5f;    // Shield takes only 50% of the damage
        private int canyonPosition = 0;                 // Current canyon segment
        private int firstTime = 2;                      // Frame pre initializing time
        private PerspectiveCamera cam;                  // Player render camera
        private ICanyonShooterGame game;                // Main game class -> world
        public WeaponManager Weapons;                   // Dr. Dooms weapon manager
        private SoundListener listener;                 // Each player has a sound listener
        private ISound meshSoundEfx;                    // Sound for animation parts or propulsion
        private ISound hurryUpEfx;                      // A clock is playing for the last 10 sec.
        private ISound gameOverEfx;                     // Game over sound
        private IMesh modelAnimationPart;               // Looped animation part
        private Vector3 FRONT = new Vector3(0, 0, -1);  // Global rotation oriantation
        private Vector3 RIGHT = new Vector3(1, 0, 0);   // Global rotation oriantation
        private Vector3 UP = new Vector3(0, 1, 0);      // Global rotation oriantation
        private List<IItem> items = new List<IItem>();  // Players items
        private GameTime time;                          // Some time temps
        private bool respawn = false;                   // Temp for respawn state
        private bool gameOver = false;                  // Temp for gameOver state
        #endregion Working fields

        /// <summary>
        /// Set the Player position in the player.
        /// </summary>
        /// <param name="segmentId">Current Canyon ID</param>
        public void SetPositionInCanyon(int segmentId)
        {
            // Get the position from current canyon segment
            LocalPosition = game.World.Level.Cache[canyonPosition].APos;
            Vector3 canyonDir = game.World.Level.Cache[canyonPosition].ADir;
            canyonDir.Normalize();
            
            // Rotate the player to zero
            LocalRotation = Quaternion.Identity;
            LocalRotation = Helper.RotateTo(canyonDir, new Vector3(0, 0, -100));
            Velocity = Vector3.Zero;

            // Set the minimal player speed for start
            speed = minSpeed;
        }

        /// <summary>
        /// Executes if the player has finished the level
        /// </summary>
        private void Finish() 
        {
            isFinish = true;

            // Stop weapon fireing
            Weapons.PrimaryWeapon.EndFire();
            //Weapons.SecondaryWeapon.EndFire();
            
            if (game.World.Finish == null)
            {
                // Create finish object
                game.World.Finish = new Finish(game);
                game.World.AddObject(game.World.Finish);
                //game.GameStates.Gameplayed = true;
                // Set the flag position
                Vector3 v = Velocity;
                if (Velocity.Length() != 0)
                {
                    v.Normalize();
                }
                game.World.Finish.LocalPosition = Vector3.Add(LocalPosition, v * 160);
                game.World.Finish.LocalRotation = Quaternion.CreateFromAxisAngle(v, 45);

                // Slow down the speed of the player
                this.Velocity = Vector3.Zero;
            }
            if (hurryUpEfx != null)
            {
                hurryUpEfx.Loop = false;
                hurryUpEfx.Stop();
                hurryUpEfx.Dispose();
            }
            if (meshSoundEfx != null)
            {
                meshSoundEfx.Loop = false;
                meshSoundEfx.Stop();
                meshSoundEfx.Dispose();
            }
            Enabled = false;
            game.GameStates.Gameplayed = true;
            //if (Helper.Lock("DIEorDIE", TimeSpan.FromSeconds(7)))
            //{
               // game.GameStates.Reset();
                //game.GameStates.Gameplayed = true;
              // game.GameStates.SetStateMenu();
            //}                   
        }

        void Player2_EnabledChanged(object sender, EventArgs e)
        {
            if(Enabled)
            {
                //LocalPosition = Vector3.Zero;
                Velocity = Vector3.Zero;
            }
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            #region Player old
            float mouseX = ((Player2)game.World.Players[0]).mouseX;
            float mouseY = ((Player2)game.World.Players[0]).mouseY;

            /*
            float mouseX = 50*(float)gameTime.ElapsedGameTime.TotalSeconds * game.Input.MouseMovement.X;
            float mouseY = 50*(float)gameTime.ElapsedGameTime.TotalSeconds * game.Input.MouseMovement.Y;

            #region Finsh if time remaining is zero
            // Temp fix because we have no finish yet.
            if (remainingTime > 0)
            {
                if (Helper.WaitFor("RemainingTime", TimeSpan.FromSeconds(1)))
                {
                    remainingTime--;
                    Helper.ResetWait("RemainingTime");
                }
                // Play the hurry up sound
                if (remainingTime <= 10)
                {
                    if (!hurryUpEfx.Playing())
                    {
                        if (Helper.Lock("Hurry Up Intercom", TimeSpan.FromSeconds(10)))
                        {
                            Intercom.HurryUp();
                        }

                        hurryUpEfx.Loop = true;
                        hurryUpEfx.Play();
                    }
                }
            }
            else
            {
                hurryUpEfx.Loop = false;
                hurryUpEfx.Stop();
                hurryUpEfx.Dispose();
                game.Sounds.MusicBox(MusicBoxStatus.Pause);
                if (gameOverEfx.Playing() == false) gameOverEfx.Play();

                if (Helper.WaitFor("FlagDisplayTime", TimeSpan.FromSeconds(4)))
                {
                    game.Sounds.MusicBox(MusicBoxStatus.Play);
                    Helper.ResetWait("FlagDisplayTime");
                    Explode();
                    game.GameStates.Gameplayed = true;                    
                }
                Finish();
                return;
            }
            #endregion Finsh if time remaining is zero

            #region Game over and respawn
            time = gameTime;
            if(gameOver)
            {
                if (Helper.WaitFor("PlayerGameOver", TimeSpan.FromSeconds(4)))
                {
                    Helper.ResetWait("PlayerGameOver");
                    this.Enabled = false;
                    game.GameStates.Gameplayed = true;

                    
                }
                //Finish();
                return;
            }
            if(respawn)
            {
                if(Helper.WaitFor("PlayerRespawn",TimeSpan.FromSeconds(4)))
                {
                    //Reset Helpers
                    respawn = false;
                    // (REPAWN NOW!)
                    //canyonPosition++;

                    health = 100;
                    shield = 50;
                    Model.AfterBurnerLength = minBooster;

                    SetPositionInCanyon(canyonPosition);
                    game.GameStates.Hud.DisplayScrollingText(lifes + " Lifes remaining!", gameTime);
                    Visible = true;
                    Weapons.WeaponsVisible = true;
                }
                else
                    return;
            }
            #endregion Game over and respawn

            #region Speed management
            if (game.Input.IsKeyDown("Player1.Up") && fuel > 0)  // do we have enough fuel ?
            {
                // Booster temperature control
                if (Helper.WaitFor("BoosterHeat", TimeSpan.FromSeconds(0.5f)))
                {
                    if (boosterHeat == maxBoosterHeat)
                    {
                        if (Helper.WaitFor("Overheat", TimeSpan.FromSeconds(3)))
                        {
                            Helper.ResetWait("Overheat");
                            Explode();
                        }
                    }
                    else
                    {
                        boosterHeat += boosterHeatGap;
                    }
                    Helper.ResetWait("BoosterHeat");
                }
                decay = 0;
                if (speed < MaxSpeed)
                    speed += SpeedGap;
                if (Model.AfterBurnerLength < maxBooster)
                    Model.AfterBurnerLength += boosterGap;

                // Fuel Check
                fuel -= (int)((speed / (MaxSpeed / 100)) * 0.02f);
            }
            else
            {
                // Booster temperature control
                if (Helper.WaitFor("BoosterHeat", TimeSpan.FromSeconds(2)))
                {
                    if (boosterHeat > minBoosterHeat)
                    {
                        boosterHeat -= boosterHeatGap;
                    }
                    Helper.ResetWait("BoosterHeat");
                }
                if (game.Input.IsKeyDown("Player1.Down"))
                {
                    if (speed > MinSpeed)
                        speed -= SpeedGap;
                    if (Model.AfterBurnerLength > minBooster)
                        Model.AfterBurnerLength -= boosterGap;
                }
                else
                {
                    if (speed > MinSpeed)
                    {
                        if (decay < MaxDecay)
                            decay += 0.05f;
                        speed -= (int) decay;
                        if (Model.AfterBurnerLength > minBooster)
                            Model.AfterBurnerLength -= boosterGap;
                    }
                }
            }

            // Check for a negative fuel value and correct it:
            if (fuel < 0)
                fuel = 0;

            // Calculate new velocity
            Velocity = Vector3.Transform(new Vector3(0, 0, -speed), LocalRotation);
            #endregion Speed management

            #region Set the flight distance
            if (Helper.WaitFor("Distance", TimeSpan.FromSeconds(0.001f)))
            {
                distance += (double)(speed * 0.0006f);
                Helper.ResetWait("Distance");
            }
            #endregion Set the flight distance

            #region Compute banking and drift
            // Get direction for driftig
            Vector3 direction = Velocity;
            Vector3 translationDirection = Vector3.Cross(this.UP, direction);
            
            // Handle inputs and compute drift
            if (game.Input.IsKeyDown("Player1.Left"))
            {
                translationDirection *= translationGap;
                Velocity += translationDirection;
                angHorz += driftFactor;
            }

            if (game.Input.IsKeyDown("Player1.Right"))
            {
                translationDirection *= -translationGap;
                Velocity += translationDirection;
                angHorz -= DriftFactor;
            }

            // Compute banking angle
            angHorz += -(float)Math.Atan(mouseX) * BankingFactor;
            angHorz -= angHorz * AutoLevel;
            angVert += -(float)Math.Atan(mouseY) * BankingFactor;
            angVert -= angVert * AutoLevel;

            angRoll += angHorz;
            angRoll *= RollFactor; 
            #endregion Compute banking and drift

            #region Update model and camera view
            if (firstTime == 0)
            {
                // Banking
                Quaternion banking = Quaternion.CreateFromYawPitchRoll(angHorz, angVert, angRoll);
                // Rotation
                Vector3 up = Vector3.Transform(UP, Quaternion.Conjugate(LocalRotation));
                Quaternion horizontal = Quaternion.CreateFromAxisAngle(up, MouseIntensity * mouseX);
                Quaternion vertical = Quaternion.CreateFromAxisAngle(RIGHT, MouseIntensity * mouseY);
                Vector3 directionTemp = Vector3.Transform(direction, 
                    Matrix.CreateFromQuaternion(Quaternion.Concatenate(horizontal, banking)));
                if (IsFlightDirectionOK(directionTemp))
                {
                    this.Model.LocalRotation = banking;
                }
                else 
                {
                    horizontal = Quaternion.CreateFromAxisAngle(up, 0);
                    vertical = Quaternion.CreateFromAxisAngle(RIGHT, 0);                
                }
                Rotate(Quaternion.Concatenate(vertical, horizontal));
            }
            else
            {
                // Banking
                this.Model.LocalRotation = Quaternion.CreateFromYawPitchRoll((angHorzTemp+angHorz)/2, 
                    (angVertTemp + angVert)/2, (angRollTemp + angRoll)/2);
                firstTime--;
                LocalRotation = Quaternion.Identity;
            }
            #endregion Update model and camera view

            #region camera fov for boost

            // interpolate camera fov between 90° and 120° depending on current speed
            cam.Fov = 90 + 30 * (Math.Max(0,speed - minSpeed) / (maxSpeed - minSpeed));

            #endregion

            // Update others
            UpdatePosition();
            UpdateCameraZoom();
            if(!isFinish)
                Weapons.UpdatePlayerWeapons(gameTime);

            UpdateAudio();

            AngularVelocity = Vector3.Zero;
            */
            //Velocity *= 2;
            //UpdatePosition();
            if (respawn)
            {
                if (Helper.WaitFor("PlayerRespawn", TimeSpan.FromSeconds(4)))
                {
                    //Reset Helpers
                    respawn = false;
                    // (REPAWN NOW!)
                    //canyonPosition++;

                    health = 100;
                    shield = 50;
                    Model.AfterBurnerLength = minBooster;

                    SetPositionInCanyon(canyonPosition);
                    game.GameStates.Hud.DisplayScrollingText(lifes + " Lifes remaining!", gameTime);
                    Visible = true;
                    Weapons.WeaponsVisible = true;
                }
                else
                    return;
            }
            
            Vector3 direction = game.World.Players[0].Velocity;
            if (firstTime == 0)
            {
                // Banking
                Quaternion banking = Quaternion.CreateFromYawPitchRoll(((Player2)game.World.Players[0]).angHorz,
                    ((Player2)game.World.Players[0]).angVert,
                    ((Player2)game.World.Players[0]).angRoll);
                // Rotation
                Vector3 up = Vector3.Transform(UP, Quaternion.Conjugate(LocalRotation));
                Quaternion horizontal = Quaternion.CreateFromAxisAngle(up, MouseIntensity * mouseX);
                Quaternion vertical = Quaternion.CreateFromAxisAngle(RIGHT, MouseIntensity * mouseY);
                Vector3 directionTemp = Vector3.Transform(direction,
                    Matrix.CreateFromQuaternion(Quaternion.Concatenate(horizontal, banking)));
                if (IsFlightDirectionOK(directionTemp))
                {
                    this.Model.LocalRotation = banking;
                }
                else
                {
                    horizontal = Quaternion.CreateFromAxisAngle(up, 0);
                    vertical = Quaternion.CreateFromAxisAngle(RIGHT, 0);
                }
                Rotate(Quaternion.Concatenate(vertical, horizontal));
            }
            else
            {
                // Banking
                this.Model.LocalRotation = Quaternion.CreateFromYawPitchRoll((angHorzTemp + ((Player2)game.World.Players[0]).angHorz) / 2,
                    (angVertTemp + ((Player2)game.World.Players[0]).angVert) / 2, (angRollTemp + ((Player2)game.World.Players[0]).angRoll) / 2);
                firstTime--;
                LocalRotation = Quaternion.Identity;
            }


            Velocity = game.World.Players[0].Velocity;
            LocalPosition = game.World.Players[0].LocalPosition;
            //LocalRotation = game.World.Players[0].LocalRotation;
            //LocalTransformation = game.World.Players[0].LocalTransformation;
            //GlobalPosition = game.World.Players[0].GlobalPosition;
            //GlobalRotation = game.World.Players[0].GlobalRotation;
            //UpdateAudio();

            //AngularVelocity = Vector3.Zero;
            //UpdatePosition();

            #endregion Player old

            #region Pre-Frame Buffer
            angHorzTemp = angHorz;
            angRollTemp = angRoll;
            angVertTemp = angVert;
            #endregion Pre-Frame Buffer
        }

        /// <summary>
        /// Determines whether [is flight direction OK] [the specified direction].
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <returns>
        /// 	<c>true</c> if [is flight direction OK] [the specified direction]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsFlightDirectionOK(Vector3 direction)
        {
            Vector3 canyonDir = game.World.Level.Cache[canyonPosition].ADir;   
            canyonDir.Normalize();
            direction.Normalize();
            float angle = (float)Math.Acos(Vector3.Dot(canyonDir, direction));

            // The player can fly with an horizon of 180 degree
            if (Math.Abs(angle) > 1.8) return false;
            return true;
        }

        /// <summary>
        /// Update the player position and the current canyon segment.
        /// </summary>
        private void UpdatePosition()
        {
            if (game.World.Level.GetDistanceToSegmentConnection(LocalPosition, canyonPosition + 1) > 0)
            {
                canyonPosition++;
                UpdatePosition();
            }
        }

        /// <summary>
        /// Update the audio effects.
        /// </summary>
        private void UpdateAudio()
        {
            if (modelAnimationPart != null)
            {
                if (!meshSoundEfx.Playing())
                    meshSoundEfx.Play();
            }
        }

        /// <summary>
        /// Update the camera zoom.
        /// </summary>
        private void UpdateCameraZoom()
        {
            if (game.Input.IsKeyDown("Camera.ZoomIn")) // ZoomOut
            {
                cam.ZoomIn(cameraZoom);
            }
            if (game.Input.IsKeyDown("Camera.ZoomOut")) // ZoomOut
            {
                cam.ZoomOut(cameraZoom);
            }
        }

        /// <summary>
        /// Explode this player if health is zero
        /// </summary>
        public override void Explode()
        {
            base.Explode();
            this.Velocity = Vector3.Zero;
            this.BoosterHeat = 0;
            
            //Explosion
            if (!respawn)
            {
                Helper.ResetWait("PlayerRespawn");
                game.Sounds.CreateSound("Explosion").Play();
                IEffect fx = game.Effects.CreateEffect("ExplosionBig");
                fx.Play(TimeSpan.FromSeconds(2.5), true);
                fx.Parent = this;
                game.World.AddEffect(fx);
                Visible = false;
                Weapons.WeaponsVisible = false;

                lifes--;

                if (lifes <= 0)
                {
                    base.Explode();
                    gameOver = true;
                    game.GameStates.Hud.DisplayScrollingText("G A M E   O V E R ! !",time);
                    game.GameStates.Gameplayed = true;
                    if (Helper.Lock("ByeByeandie", TimeSpan.FromSeconds(7)))
                    {
                        Finish();
                    }                    
                }
                else
                {
                    respawn = true;
                }
            }
        }

        /// <summary>
        /// Collision handling.
        /// </summary>
        /// <param name="e">The collision event thrown</param>
        public override void OnCollision(CollisionEvent e)
        {
            base.OnCollision(e);

            /*
            IGameObject collisionObject = e.OtherSolid.UserData as IGameObject;

            if (collisionObject == null)
                return;

            if(health <= 0) return;
            
            // Checkpoints! (Waypoints) add Time
            if (collisionObject.ContactGroup == Engine.Physics.ContactGroup.Waypoint)
            {
                if (((WaypointObject)collisionObject).SecondsToAdd > 0)
                {
                    Intercom.SendMessage(
                        String.Format("You past a Checkpoint! {0} seconds remaining!",
                                      remainingTime), true);
                    remainingTime += ((WaypointObject)collisionObject).SecondsToAdd;
                    ((WaypointObject)collisionObject).SecondsToAdd = 0;
                }
            }

            if (!game.Physics.CanSolidsCollide(e.ThisSolid, e.OtherSolid))
                return;
            switch (collisionObject.ContactGroup)
            {
                case Engine.Physics.ContactGroup.Canyon:
                    if(!Helper.Lock("Collision.Player.With.Canyon",TimeSpan.FromSeconds(1)))
                        ReceiveDamage((int) speed / 2);

                    // mirror velocity to "bounce off"
                    Vector3 normal = Vector3.Normalize(e.Normal);
                    Vector3 dir = Vector3.Normalize(Velocity);
                    Vector3 reflect = dir - 2 * Vector3.Dot(dir, normal) * normal;
                    //Velocity = Velocity.Length() * reflect;
                    //LocalRotation = Helper.RotateTo(Velocity, -Vector3.UnitZ);

                    LocalPosition += 10*normal;
                    //angHorz = angHorzTemp = angVert = angVertTemp = 0;
                    //angHorz = 360 - angHorz;
                    //angVert = 360 - angVert;
                    //angRoll = 360 - angRoll;
                    //GraphicalConsole.GetSingleton(game).WriteLine("Horz:"+angHorz+" Vert:"+angVert+" Roll:"+angRoll,1);

                    break;
                case Engine.Physics.ContactGroup.Sky:
                    game.GameStates.Hud.DisplayScrollingText("Do not leave the canyon !", new GameTime());
                    Intercom.PlayerLeavingCanyon();
                    ReceiveDamage(1000);
                    break;
                    
                default:
                    break;
            }*/

            AngularVelocity = Vector3.Zero;

        }

        /// <summary>
        /// Ordinary constructor.
        /// </summary>
        /// <param name="game">The main CanyonShooter game object</param>
        public Ghost(ICanyonShooterGame game)
            : base(game, "PlayerElcode")
        {
            EnabledChanged += (Player2_EnabledChanged);

            // Set player profile values
            SetModel(game.GameStates.Profil.CurrentProfil.Model);
            BankingFactor = game.GameStates.Profil.CurrentProfil.Banking;
            DriftFactor = game.GameStates.Profil.CurrentProfil.Drift;
            TranslationGap = game.GameStates.Profil.CurrentProfil.Translation;
            RollFactor = game.GameStates.Profil.CurrentProfil.Rolling;
            MaxDecay = game.GameStates.Profil.CurrentProfil.Brake;
            AutoLevel = game.GameStates.Profil.CurrentProfil.AutoLevel;
            Health = game.GameStates.Profil.CurrentProfil.Health;
            MinSpeed = game.GameStates.Profil.CurrentProfil.Speed;
            Shield = game.GameStates.Profil.CurrentProfil.Shield;
            SpeedGap = game.GameStates.Profil.CurrentProfil.Acceleration;

            // Initialize player environment
            LocalPosition = new Vector3(0, 0, 0);
            ConnectedToXpa = true;
            ContactGroup = ContactGroup.None;
            InfluencedByGravity = false;
            this.game = game;
            //SetCamera(game);
            
            // Set sound listener for 3D sound
            listener = new SoundListener(game);
            game.Sounds.SoundListener = listener;
        }

        private void SetCamera(ICanyonShooterGame game)
        {
            cam = new PerspectiveCamera(game);
            cam.Parent = this;
            cam.LocalPosition = new Vector3(0, 9, 24);
            cam.Fov = 90;
        }

        /// <summary>
        /// Load the player contents like model, weapons, ...
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
            Model.AfterBurnerLength = minBooster;
            //Initialize WeaponManager
            if (Weapons == null)
            {
                Weapons = new WeaponManager(game, Model, WeaponHolderType.Player);

                // Equip standard weapons:
                Weapons.AddWeapon(WeaponType.MINIGUN);
                Weapons.AddWeapon(WeaponType.MINIGUN2);
                Weapons.AddWeapon(WeaponType.ULTRA_PHASER);
                Weapons.AddWeapon(WeaponType.ROCKET_STINGER);
                Weapons.SetPrimaryWeapon(WeaponType.ULTRA_PHASER);
                Weapons.SetSecondaryWeapon(WeaponType.ROCKET_STINGER);

            }
            else
            {
                Weapons.WeaponHolder = Model;
            }

            // Helicopter animation hack
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

            base.Initialize();
            // Start at the median of the first canyon segment.
            LocalPosition = game.World.Level.Cache[1].APos / 2; 
        }

        /// <summary>
        /// Adds health.
        /// </summary>
        /// <param name="value">The value.</param>
        public void AddHealth(int value)
        {
            health += value;
            if (health > 100)
                health = 100;
        }

        /// <summary>
        /// Adds shield.
        /// </summary>
        /// <param name="value">The value.</param>
        public void AddShield(int value)
        {
            shield += value;
            if (shield > 200)
                shield = 200;
        }

        #region IPlayer Member

        /// <summary>
        /// Inflicts damage.
        /// </summary>
        /// <param name="value">The amount of damage.</param>
        public void ReceiveDamage(int value)
        {
            if (value < 0)
                throw new Exception("Not allowed");
            GraphicalConsole.GetSingleton(game).WriteLine(string.Format("You got {0} Damage!", value), 1);
            if (shield > 0)
            {
                int shieldDamage = (int) (value*shieldDamageAbsorbtion);
                int newShield = shield - shieldDamage;
                if (newShield < 0)
                {
                    int remaining = (int)(-newShield / shieldDamageAbsorbtion);
                    health -= remaining;
                    newShield = 0;
                }
                shield = newShield;
            }
            else
                health -= value;    // no shield, apply pure damage

            // handle health-state
            if (health <= 0)
            {
                Explode();
                health = 0;
            }
        }

        /// <summary>
        /// The player adds the item to his inventory. the item will not be removed from the world.
        /// </summary>
        /// <param name="item">The item to give to this.</param>
        public void GiveItem(IItem item)
        {
            items.Add(item);
        }

        /// <summary>
        /// Checks if the player has the item in his inventory
        /// </summary>
        /// <param name="item">Which Item to look for.</param>
        /// <returns>True if the item is in the inventory.</returns>
        public bool HasItem(IItem item)
        {
            return items.Contains(item);
        }

        /// <summary>
        /// removes an item from the inventory (for scripting/story purposes)
        /// </summary>
        /// <param name="item">Which Item to remove. Doesn't need to be in the player's inventory.</param>
        public void RemoveItem(IItem item)
        {
            items.Remove(item);
        }

        /// <summary>
        /// The camera following the player
        /// </summary>
        public ICamera Camera
        {
            get { return cam; }
        }

        /// <summary>
        /// Property for position in the canyon;
        /// </summary>
        public int CanyonPosition
        {
            get { return canyonPosition; }
        }

        /// <summary>
        /// Enables or disables the complete player
        /// </summary>
        public new bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                base.Enabled = value;
            }
        }

        /// <summary>
        /// Players current lifes.
        /// </summary>
        public int Lifes
        {
            get
            {
                return lifes;
            }
            set
            {
                lifes = value;
            }
        }

        /// <summary>
        /// Health property of player
        /// </summary>
        public int Health
        {
            get
            {
                return health;
            }
            set
            {
                health = value;
            }
        }

        /// <summary>
        /// Shield property of player
        /// </summary>
        /// <value>The shield.</value>
        public int Shield
        {
            get
            {
                return shield;
            }
            set
            {
                shield = value;
            }
        }

        /// <summary>
        /// Current distance of the player
        /// </summary>
        public double Distance
        {
            get
            {
                return distance;
            }
            set
            {
                distance = value;
            }
        }

        /// <summary>
        /// Temperature of the booster
        /// </summary>
        public int BoosterHeat
        {
            get
            {
                return boosterHeat;
            }
            set
            {
                boosterHeat = value;
            }
        }

        /// <summary>
        /// The Level remaining time in seconds.
        /// </summary>
        public int RemainingTime
        {
            get
            {
                return remainingTime;
            }
            set
            {
                remainingTime = value;
            }
        }

        /// <summary>
        /// Current player speed.
        /// </summary>
        public float Speed
        {
            get
            {
                return speed * 2;
            }
            set
            {
                speed = value;
            }
        }

        public float RelativeSpeed
        {
            get { return (Math.Max(0, speed - minSpeed) / (maxSpeed - minSpeed)); }
        }

        #endregion
    }
}
