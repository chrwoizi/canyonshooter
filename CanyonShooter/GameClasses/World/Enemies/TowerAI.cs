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
using CanyonShooter.Engine.AI;
using CanyonShooter.GameClasses.World.Enemies.StatesTowerAI;

namespace CanyonShooter.GameClasses.World.Enemies
{
    public class TowerAI : GameObject, IEnemy
    {
        private int id = 0;
        private int health;
        private float lastAngle = 0.0f;
        private Vector3 dir;
        private ICanyonShooterGame game;
        public WeaponManager Weapons;
        //private bool playerPassed = false;
        private static int createdTanks = 0;

        private AIStateMachine AIStates;

        public TowerAI(ICanyonShooterGame game, EnemyDescription desc)
            : base(game, "Enemy-TankAI")
        {
            this.game = game;
            id = createdTanks++;
            
            // Set the tank model
            SetModel(desc.Model);

            LocalScale = new Vector3(4f, 4f, 4f);
            LocalPosition = desc.RelativeSpawnLocation;
            dir = new Vector3(0, 0, -1);

            // Physics Config
            ConnectedToXpa = true;
            this.Static = true; 
            ContactGroup = ContactGroup.Enemies;
            LocalPosition = desc.RelativeSpawnLocation;

            // Initialize AI-States of TowerAI
            AIStates = new AIStateMachine(game,this);
            AIStates.AddState(new TowerAIObserving(game.World.Players[0]));
            AIStates.AddState(new TowerAIAttackPlayer(game.World.Players[0]));
            AIStates.DebugAI = false;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // base first because base handles collision events which might dispose "this"
            base.Update(gameTime);

            AIStates.UpdateAI(gameTime);
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
                Weapons.SetAmmo(AmmoType.ROCKETS, 0);
                Weapons.SetAmmo(AmmoType.LASERCELLS, 9999);
                
                Weapons.SetPrimaryWeapon(WeaponType.ULTRA_PHASER);
                Weapons.PrimaryWeapon.FirePause = TimeSpan.FromMilliseconds(2000);

                Weapons.SetSecondaryWeapon(WeaponType.ROCKET_STINGER);
                Weapons.SecondaryWeapon.FirePause = TimeSpan.FromMilliseconds(3000);
                Weapons.SecondaryWeapon.AmmoInMagazine = 3;
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

            // do not check for collisions. the tower must not be destroyed
            /*if (!game.Physics.CanSolidsCollide(e.ThisSolid, e.OtherSolid))
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
            }*/

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






        public void AimToPlayer()
        {
            Vector3 a = GlobalPosition;
            Vector3 b = game.World.Players[0].LocalPosition;
            float distance;
            Vector3.Distance(ref a, ref b, out distance);

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

            IMesh turret = Model.GetMesh("TBarrel");
            if (turret != null)
                turret.LocalRotation = q;
        }
    }
}
