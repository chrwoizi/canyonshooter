using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using CanyonShooter.Engine.Helper;
using CanyonShooter.GameClasses.World;
using DescriptionLibs.EnemyType;
using DescriptionLibs.Weapon;
using CanyonShooter.Engine.Audio;
using Microsoft.Xna.Framework;
using CanyonShooter.GameClasses.Console;
using CanyonShooter.GameClasses.Huds;
using Microsoft.Xna.Framework.Graphics;

namespace CanyonShooter.GameClasses.Weapons
{


    public enum WeaponState
    {
        START_FIRE,
        STOP_FIRE,
        FIRING,
        FIRING_LOOP,
        RELOADING,
        AMMO_EMPTY,
        NOTHING,
        FIRE_PAUSED,
    }


    public class BaseWeapon : GameObject, IWeapon
    {
        private ICanyonShooterGame game;
        private WeaponDescription desc;
        private WeaponManager manager;

        private ISound soundFiring;
        private ISound soundStartFire;
        private ISound soundStopFire;
        private ISound soundReload;
        private ISound soundAmmoEmpty;

        private WeaponState state = WeaponState.FIRING;
        private AmmoType ammoType;

        private Texture2D icon;

        private Texture2D crossHair;
        private Texture2D crossHairFiring;
        private Texture2D crossHairReloading;

        public WeaponType DualWeapon = WeaponType.NO_WEAPON;
        public bool IsTriggeredDual = false;

        /// <summary>
        /// Gets the icon of the weapon.
        /// </summary>
        /// <value>The icon.</value>
        public Texture2D Icon
        {
            get{ return icon;}
        }

        /// <summary>
        /// Gets the crosshair for the state
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>the crosshair for the state</returns>
        public Texture2D GetCrossHair(WeaponState state)
        {
            switch(state)
            {
                case WeaponState.FIRING:
                case WeaponState.FIRING_LOOP:
                case WeaponState.FIRE_PAUSED:
                    return crossHairFiring;

                case WeaponState.RELOADING:
                case WeaponState.AMMO_EMPTY:
                    return crossHairReloading;

                default:
                    return crossHair;
            }
        }

        /// <summary>
        /// Gets the cross hair.
        /// </summary>
        /// <returns></returns>
        public Texture2D GetCrossHair()
        {
            switch (state)
            {
                case WeaponState.FIRING:
                case WeaponState.FIRING_LOOP:
                    return crossHairFiring;

                case WeaponState.RELOADING:
                    return crossHairReloading;

                default:
                    return crossHair;
            }
        }

        public WeaponHolderType WeaponHolderType;

        public WeaponState State
        {
            get { return state;}
            set { state = value; }
        }

        public float ProjectileSpeed
        {
            get { return desc.ProjectileSpeed; }
        }

        public TimeSpan FirePause
        {
            get { return TimeSpan.FromSeconds(desc.FirePause); }
            set { desc.FirePause = (float)value.TotalMilliseconds; }
        }

        public int Damage
        {
            get { return desc.Damage; }
            set { desc.Damage = value; }
        }

        public WeaponDescription Desc
        {
            get { return desc;}
        }

        public ICanyonShooterGame Game
        {
            get { return this.game; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseWeapon"/> class.
        /// </summary>
        /// <param name="game">The game to spawn this weapon in.</param>
        /// <param name="weaponDescription">The weapon description.</param>
        /// <param name="name">The name of the weapon</param>
        public BaseWeapon(ICanyonShooterGame game,WeaponHolderType weaponHolderType, string weaponDescription,WeaponManager weaponManager, string name):base(game, name)
        {
            this.game = game;
            manager = weaponManager;
            WeaponHolderType = weaponHolderType;

            if (weaponDescription == String.Empty)
                return;

            // Load Weapon Description
            desc = game.Content.Load<WeaponDescription>("Content\\Weapons\\" + weaponDescription);
            
            // Load Model
            if(desc.Model != String.Empty)
#pragma warning disable DoNotCallOverridableMethodsInConstructor
                SetModel(desc.Model);
#pragma warning restore DoNotCallOverridableMethodsInConstructor

            // Load AmmoType
            ammoType = (AmmoType)Enum.Parse(typeof(AmmoType), desc.AmmoType);

            // Load Ammo
            Ammo += desc.DefaultAmmo;
            //if (desc.AmmoReloadCount > desc.DefaultAmmo)
            //{
            //    ammoInMagazine = desc.DefaultAmmo;
            //    Ammo = 0;
            //}
            //else
            //{
            //    ammoInMagazine = desc.AmmoReloadCount;
            //    Ammo += desc.DefaultAmmo - desc.AmmoReloadCount;
            //}

            // Load Sounds:
            if (desc.SoundFiring != String.Empty)
            {
                soundFiring = game.Sounds.CreateSound(desc.SoundFiring);
                soundFiring.Parent = this;
            }
            if (desc.SoundStartFire != String.Empty)
            {
                soundStartFire = game.Sounds.CreateSound(desc.SoundStartFire);
                soundStartFire.Parent = this;
            }
            if (desc.SoundStopFire != String.Empty)
            {
                soundStopFire = game.Sounds.CreateSound(desc.SoundStopFire);
                soundStopFire.Parent = this;
            }
            if (desc.SoundReload != String.Empty)
            {
                soundReload = game.Sounds.CreateSound(desc.SoundReload);
                soundReload.Parent = this;
            }
            if (desc.SoundAmmoEmpty != String.Empty)
            {
                soundAmmoEmpty = game.Sounds.CreateSound(desc.SoundAmmoEmpty);
                soundAmmoEmpty.Parent = this;
            }
            state = WeaponState.NOTHING;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            icon = game.Content.Load<Texture2D>(desc.Icon);

            crossHair = game.Content.Load<Texture2D>(desc.CrossHair);
            crossHairFiring = game.Content.Load<Texture2D>(desc.CrossHairFiring);
            crossHairReloading = game.Content.Load<Texture2D>(desc.CrossHairReloading);

        }

        /// <summary>
        /// Called when [fire starts].
        /// </summary>
        public virtual void OnFireStart(Vector3 startPosition, Vector3 direction)
        {
            
        }

        /// <summary>
        /// Called when [fire started].
        /// </summary>
        public virtual void OnFireStarted(Vector3 startPosition, Vector3 direction)
        {

        }

        /// <summary>
        /// Called when [fire stops].
        /// </summary>
        public virtual void OnFireStop(Vector3 startPosition, Vector3 direction)
        {
            
        }

        /// <summary>
        /// Called when [firing].
        /// </summary>
        public virtual void OnFiring(Vector3 startPosition, Vector3 direction)
        {

        }

        #region Helpers

        /// <summary>
        /// Dictionary containing the wait-times
        /// </summary>
        private Dictionary<string, DateTime> waitingFor = new Dictionary<string, DateTime>();

        /// <summary>
        /// Waits for a durtation
        /// </summary>
        /// <param name="name">The namespace to wait</param>
        /// <param name="duration">The duration to wait.</param>
        /// <returns>true if duration timed out, else false (still waiting)</returns>
        public bool WaitFor(string name, TimeSpan duration)
        {
            
            if (waitingFor.ContainsKey(name)) //this lock already exists
            {
                if (DateTime.Now >= waitingFor[name]) //lock duration timed out
                {
                    return true;
                }
                else
                    return false;
            }
            else
            {   // add lock
                waitingFor.Add(name, DateTime.Now.Add(duration));
                return false;
            }
        }

        /// <summary>
        /// Resets the wait.
        /// </summary>
        /// <param name="name">The name.</param>
        public void ResetWait(string name)
        {
            if (waitingFor.ContainsKey(name)) //this lock already exists
                waitingFor.Remove(name);
        }

        #endregion
        
        #region IWeapon Member

        private Vector3 direction = Vector3.Zero;
        private bool isStarted = false;
        private bool isLooping = false;
        private bool isEmpty = false;
        private bool isReloading = false;
        private bool isStopping = false;

        private bool NeedReload()
        {
            return state == WeaponState.AMMO_EMPTY;
        }

        private bool StartSoundEnabled()
        {
            return desc.SoundStartFire != String.Empty && (desc.SoundStartMilliseconds > 0);
        }

        private bool StopSoundEnabled()
        {
            return desc.SoundStopFire != String.Empty && (desc.SoundStopMilliseconds > 0);
        }

        private bool ReloadEnabled()
        {
            return desc.SoundReload != String.Empty && (desc.SoundReloadMilliseconds > 0);
        }

        public bool CanReload()
        {
            return Ammo > 0 && state != WeaponState.RELOADING;
        }

        public void Reload()
        {
            if (CanReload())
                state = WeaponState.RELOADING;
            if (state == WeaponState.AMMO_EMPTY)
                state = WeaponState.NOTHING;
        }

        public virtual void Fire(Vector3 startPosition, Vector3 direction)
        {
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            #region Update HUD with AmmoInfo
            if (this == manager.PrimaryWeapon)
            {
                IHudTextControl control = game.GameStates.Hud.GetControl("weapon1_magazine") as IHudTextControl;
                if (control != null)
                {
                    control.Text = AmmoInMagazine.ToString();
                    if (state == WeaponState.FIRING || state == WeaponState.FIRING_LOOP)
                        control.Effect = HUDEffectType.PULSE;
                    else
                        control.Effect = HUDEffectType.NONE;

                }
                    

                IHudTextControl control2 = game.GameStates.Hud.GetControl("weapon1_ammo") as IHudTextControl;
                if (control2 != null)
                    control2.Text = ammoType.ToString() + ": " + Ammo.ToString();
            }
            
            if (this == manager.SecondaryWeapon)  //TODO: Irgendwie updated der das nicht richtig
            {
                IHudTextControl control = game.GameStates.Hud.GetControl("weapon2_magazine") as IHudTextControl;
                if (control != null)
                {
                    control.Text = AmmoInMagazine.ToString();
                    if (state == WeaponState.FIRING || state == WeaponState.FIRING_LOOP)
                        control.Effect = HUDEffectType.PULSE;
                    else
                        control.Effect = HUDEffectType.NONE;

                }

                IHudTextControl control2 = game.GameStates.Hud.GetControl("weapon2_ammo") as IHudTextControl;
                if (control2 != null)
                    control2.Text = ammoType.ToString() + ": "  + Ammo.ToString();
            }
            #endregion

            #region WEAPON_STATE_MACHINE
            switch (state)
            {
                case WeaponState.NOTHING:
                    break;

                case WeaponState.AMMO_EMPTY:
                    // Stop Firing-Loop Sound if playing
                    if(soundFiring != null)
                    {
                        soundFiring.Loop = false;
                        soundFiring.Stop();
                        isLooping = false;
                    }
                    
                    if (isEmpty)
                    {
                        if (!WaitFor("AmmoEmpty", TimeSpan.FromMilliseconds(desc.SoundAmmoEmptyMilliseconds)))
                            break;
                        else
                            isEmpty = false;
                    }
                    if (!isEmpty)
                    {
                        isEmpty = true;
                        ResetWait("AmmoEmpty");
                        // Play AmmoEmpty Sound
                        if (soundAmmoEmpty != null)
                            soundAmmoEmpty.Play();
                        OnAmmoEmpty();
                    }
                    
                    break;

                case WeaponState.RELOADING:
                    if (isReloading)
                    {
                        if (!WaitFor("Reloading", TimeSpan.FromMilliseconds(desc.SoundReloadMilliseconds)))
                            break;
                        else
                        {
                            isReloading = false;
                            if (desc.LoopedFire)
                                state = WeaponState.FIRING_LOOP;
                            else
                                state = WeaponState.FIRING;
                            OnReload();
                        }
                    }
                    if (!isReloading)
                    {
                        isReloading = true;
                        ResetWait("Reloading");
                        // Play AmmoEmpty Sound
                        if (soundReload != null)
                            soundReload.Play();
                    }
                    // Update player HUD crosshair by ELC
                    break;

                case WeaponState.START_FIRE:
                    // Update player HUD crosshair by ELC
                    if (isStarted)
                    {
                        if (!WaitFor("StartFire", TimeSpan.FromMilliseconds(desc.SoundStartMilliseconds)))
                            break;
                        else
                        {
                            if (desc.LoopedFire)
                                state = WeaponState.FIRING_LOOP;
                            else
                                state = WeaponState.FIRING;
                            isStarted = false;
                            OnFireStarted(Vector3.Transform(desc.ProjectileOffset, GlobalTransformation), direction);
                            break;
                        }
                    }
                    if (!isStarted)
                    {
                        isStarted = true;
                        ResetWait("StartFire");
                        // Play StartFire Sound
                        if (soundStartFire != null)
                            soundStartFire.Play();
                        OnFireStart(Vector3.Transform(desc.ProjectileOffset, GlobalTransformation), direction);
                        
                    }
                    break;
                case WeaponState.FIRING_LOOP:
                    if(!DescreaseAmmo())
                        break;

                    if (!isLooping)
                    {
                        // Play Firing-Loop Sound
                        isLooping = true;
                        if (soundFiring != null)
                            soundFiring.Loop = true;
                    }
                    OnFiring(Vector3.Transform(desc.ProjectileOffset, GlobalTransformation), direction);
                    state = WeaponState.FIRE_PAUSED;
                    break;



                case WeaponState.FIRING:
                    if(!DescreaseAmmo())
                        break;

                    if (soundFiring != null)
                    {
                        soundFiring.Stop();
                        soundFiring.Play();
                    }
                    OnFiring(Vector3.Transform(desc.ProjectileOffset, GlobalTransformation), direction);
                    state = WeaponState.FIRE_PAUSED;
                    break;



                case WeaponState.FIRE_PAUSED:
                    if (WaitFor("FirePause", TimeSpan.FromMilliseconds(desc.FirePause)))
                    {
                        ResetWait("FirePause");
                        if(desc.LoopedFire)
                            state = WeaponState.FIRING_LOOP;
                        else
                            state = WeaponState.FIRING;
                    }

                    break;



                case WeaponState.STOP_FIRE:
                    isStarted = false;
                    if(desc.LoopedFire && isLooping)
                    {
                        // Stop Firing-Loop Sound
                        if (soundFiring != null)
                        {
                            soundFiring.Loop = false;
                            soundFiring.Stop();
                        }
                        isLooping = false;  
                    }

                    if (isStopping)
                    {
                        if (!WaitFor("StopFire", TimeSpan.FromMilliseconds(desc.SoundStopMilliseconds)))
                            break;
                        else
                        {
                            state = WeaponState.NOTHING;
                            isStopping = false;
                            break;
                        }
                    }
                    if (!isStopping)
                    {
                        isStopping = true;
                        ResetWait("StopFire");
                        //Play StartFire Sound
                        if (soundStopFire != null)
                            soundStopFire.Play();
                        OnFireStop(GlobalPosition,direction);
                    }
                    break;


                default:
                    break;
            }
            #endregion
        }

        /// <summary>
        /// Descreases the ammo.
        /// </summary>
        /// <returns>false, if ammo is empty, otherwise true</returns>
        private bool DescreaseAmmo()
        {
            if (ammoInMagazine <= 0)
            {
                state = WeaponState.AMMO_EMPTY;
                return false;
            }
            else  //Decrease Ammo
                ammoInMagazine -= desc.AmmoPerShoot;
            return true;
        }

        public virtual void OnAmmoEmpty()
        {
            if(desc.AutoReload)
                Reload();
        }

        public virtual void OnReload()
        {
            // Enough Ammo avaible
            if (desc.AmmoReloadCount <= Ammo)
            {
                Ammo -= desc.AmmoReloadCount;
                ammoInMagazine = desc.AmmoReloadCount;
            }
            // Not Enough
            else
            {
                ammoInMagazine = Ammo;
                Ammo = 0;
            }


        }

        public int Ammo
        {
            get
            {
               return manager.GetAmmo(ammoType);
            }
            set
            {
                
                manager.Ammo[ammoType] = value;
            }
        }

        public bool CanFire
        {
            get
            {
                return true;
            }
        }



        #endregion

        #region IWeapon Member


        public void BeginFire()
        {
            if(DualWeapon != WeaponType.NO_WEAPON)
            {
                BaseWeapon weapon = manager.GetWeapon(DualWeapon) as BaseWeapon;

                if(weapon != null && weapon != this)
                {
                    weapon.AimAt();
                    weapon.BeginFire();
                    weapon.IsTriggeredDual = true;
                }
            }
                
                

            if(state == WeaponState.NOTHING)
                state = WeaponState.START_FIRE;
        }

        public void EndFire()
        {
            if(IsTriggeredDual)
                return;
            if (DualWeapon != WeaponType.NO_WEAPON)
            {
                BaseWeapon weapon = manager.GetWeapon(DualWeapon) as BaseWeapon;
                if (weapon != null && weapon != this)
                {
                    weapon.IsTriggeredDual = false;
                    weapon.EndFire();
                }
            
            }

            if (IsFiring && state != WeaponState.RELOADING)
            {
                state = WeaponState.STOP_FIRE;
            }
        }

        public bool IsFiring
        {
            get 
            { 
                return state == WeaponState.FIRING || 
                        state == WeaponState.FIRING_LOOP ||
                        state == WeaponState.FIRE_PAUSED ||
                        state == WeaponState.START_FIRE; 
            }
        }

        /// <summary>
        /// Aims with weapon direction
        /// </summary>
        public void AimAt()
        {
            direction = Vector3.Transform(-Vector3.UnitZ, GlobalRotation);
        }

        /// <summary>
        /// Aims at the given direction.
        /// The direction will be normalized.
        /// </summary>
        /// <param name="direction">The direction.</param>
        public void AimAt(Vector3 direction)
        {
            this.direction = direction;
            Debug.Assert(direction != Vector3.Zero);
            Debug.Assert(!float.IsNaN(direction.X));
        }

        /// <summary>
        /// Aimts at the given target [IGameObject].
        /// The direction will be normalized.
        /// </summary>
        /// <param name="target">The target.</param>
        public void AimAt(IGameObject target)
        {
            this.direction = -target.GlobalPosition + (GlobalPosition + desc.ProjectileOffset);
            Debug.Assert(direction != Vector3.Zero);
            Debug.Assert(!float.IsNaN(direction.X));
            direction.Normalize(); ;
        }


        #endregion

        #region IWeapon Member

        private int ammoInMagazine = 0;

        public int AmmoInMagazine
        {
            get { return ammoInMagazine; }
            set { ammoInMagazine = value;}
        }

        #endregion

        public string GetMagazineInfo()
        {
            return String.Format("{0}/{1}", ammoInMagazine, desc.AmmoReloadCount);
        }

        public WeaponType GetWeaponType()
        {
            return manager.GetWeaponType(this);
        }
    }
}
