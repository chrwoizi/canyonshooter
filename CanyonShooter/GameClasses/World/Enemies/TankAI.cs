using System;
using System.Collections.Generic;
using System.Text;
using CanyonShooter.DataLayer.Descriptions;
using XnaDevRu.Physics;
using CanyonShooter.Engine.Physics;
using CanyonShooter.GameClasses.Console;
using Microsoft.Xna.Framework;
using CanyonShooter.Engine.Graphics;
using CanyonShooter.GameClasses.Weapons;
using CanyonShooter.Engine.Graphics.Effects;
using CanyonShooter.Engine.Helper;

namespace CanyonShooter.GameClasses.World.Enemies
{
    public class TankAI : GameObject, IEnemy
    {
        private int id = 0;
        private int health;
        private float lastAngle = 0.0f;
        private Vector3 dir;
        private ICanyonShooterGame game;
        public WeaponManager Weapons;
        private bool playerPassed = false;
        private static int createdTanks = 0;

        public TankAI(ICanyonShooterGame game, EnemyDescription desc)
            : base(game, "Enemy-TankAI")
        {
            this.game = game;
            id = createdTanks++;

            // Set the tank model
            SetModel(desc.Model);

            LocalScale = new Vector3(8f, 8f, 8f);
            LocalPosition = desc.RelativeSpawnLocation;
            dir = new Vector3(0, 0, -1);

            // Physics Config
            ConnectedToXpa = true;
            ContactGroup = ContactGroup.Enemies;
            InfluencedByGravity = false;
            LocalPosition = desc.RelativeSpawnLocation;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // base first because base handles collision events which might dispose "this"
            base.Update(gameTime);

            // update 1 time a second only
            if (Helper.WaitFor("TankAI" + id, TimeSpan.FromSeconds(0.25)))
            {
                Helper.ResetWait("TankAI");
                // Compute distance
                Vector3 a = GlobalPosition;
                Vector3 b = game.World.Players[0].LocalPosition;
                float distance;
                Vector3.Distance(ref a, ref b, out distance);

                // If player arrives aim the player
                if (distance < 2100.0f)
                {
                    // Direction to player
                    Vector3 d;
                    Vector3.Subtract(ref b, ref a, out d);
                    Vector3.Negate(d);
                    float delta = 0.0f;
                    d.Normalize();

                    // Turret mesh rotation

                    float angle = (float) Math.Acos(Vector3.Dot(d, dir));
                    Quaternion q;

                    delta = angle - lastAngle;
                    q = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), angle);

                    lastAngle = angle;

                    //GraphicalConsole.GetSingleton(game).WriteLine(""+angle,1);
                    IMesh turret = Model.GetMesh("Turret");
                    if (turret != null)
                        turret.LocalRotation = q;

                    // Aim the weapons

                    // Fire to player
                    if (distance < 2000.0f)
                    {
                        // Fire a rocket only a few times
                        if (Weapons.GetAmmo(AmmoType.ROCKETS) != 0)
                        {
                            // Aim the weapons
                            Weapons.PrimaryWeapon.AimAt(d);
                            Weapons.PrimaryWeapon.BeginFire();
                        }
                    }

                    // Notice if player passed the enemy
                    if (distance < 400.0f) playerPassed = true;
                }

                // Player passed its time to explode
                if ((distance > 1000.0f) && playerPassed)
                {
                    Weapons.SecondaryWeapon.EndFire();
                    game.World.RemoveObject(this);
                }
            }
        }

        #region IEnemy Member

        public int Identifier
        {
            get { return id; }
        }

        public int Health
        {
            get { return health; }
        }

        public void ReceiveDamage(int value)
        {
            if (value < 0)
                throw new Exception("Not allowed");
            health -= value;

            if (health <= 0)
            {
                Explode();
            }
        }

        #endregion

        /// <summary>
        /// Load the player contents like model, weapons, ...
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            //Initialize WeaponManager
            if (Weapons == null)
            {
                Weapons = new WeaponManager(game, Model, WeaponHolderType.Enemy);

                // Equip some cool weapons:
                Weapons.AddWeapon(WeaponType.ROCKET_STINGER);
                Weapons.AddWeapon(WeaponType.ULTRA_PHASER);
                Weapons.SetPrimaryWeapon(WeaponType.ROCKET_STINGER);
                Weapons.SetSecondaryWeapon(WeaponType.ULTRA_PHASER);
                Weapons.SecondaryWeapon.FirePause = TimeSpan.FromMilliseconds(700);
                Weapons.SetAmmo(AmmoType.ROCKETS, 3);
            }
            else
            {
                Weapons.WeaponHolder = Model;
            }
        }

        /// <summary>
        /// Called when [collision].
        /// </summary>
        /// <param name="e">The e.</param>
        public override void OnCollision(CollisionEvent e)
        {
            base.OnCollision(e);

            if (!game.Physics.CanSolidsCollide(e.ThisSolid, e.OtherSolid))
                return;

            IGameObject collisionObject = e.OtherSolid.UserData as IGameObject;

            if (collisionObject == null)
                return;

            switch (collisionObject.ContactGroup)
            {
                case Engine.Physics.ContactGroup.PlayerProjectiles:
                    if (collisionObject is IWeaponProjectile)
                        ReceiveDamage((collisionObject as IWeaponProjectile).Damage);
                    break;
                default:
                    break;
            }

        }

        /// <summary>
        /// Explode this enemie if health is zero
        /// </summary>
        public override void Explode()
        {
            base.Explode();

            //Explosions-Effekt
            IEffect fx;
            Random rnd = new Random((int)GlobalPosition.Z);
            int w = rnd.Next(0, 100);
            if (w <= 10)
                fx = game.Effects.CreateEffect("ExplosionBig"); // BIG 10%
            else if (w <= 40)
                fx = game.Effects.CreateEffect("ExplosionMiddle"); // MIDDLE 30%
            else
                fx = game.Effects.CreateEffect("Explosion"); // NORMAL 60%

            fx.Parent = this;
            game.World.AddEffect(fx);
            fx.Play(TimeSpan.FromSeconds(1.5f), true);

            Visible = false;
        }
    }
}
