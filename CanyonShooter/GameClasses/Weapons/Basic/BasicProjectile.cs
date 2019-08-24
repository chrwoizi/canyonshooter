using System;
using System.Diagnostics;
using CanyonShooter.Engine.Audio;
using CanyonShooter.Engine.Graphics.Effects;
using CanyonShooter.Engine.Physics;
using CanyonShooter.GameClasses.World;
using CanyonShooter.GameClasses.World.Enemies;
using Microsoft.Xna.Framework;
using XnaDevRu.Physics;

namespace CanyonShooter.GameClasses.Weapons
{
    public class BasicProjectile : GameObject
    {
        private ICanyonShooterGame game = null;

        private bool destroyNextUpdate = false;
        
        public Vector3 FromPosition;
        public Vector3 Direction;
        public bool EnableExplosion = true;

        public bool AutoDestruction = true;

        private ISound sound;

        public TimeSpan AutoDestructionTime
        {
            set
            {
                autoDestructionTime = DateTime.Now.Add(value);
            }
        }

        private DateTime autoDestructionTime = DateTime.Now.Add(TimeSpan.FromSeconds(5));

        public BasicProjectile(ICanyonShooterGame game, Vector3 startPos, Vector3 direction, string soundName, WeaponHolderType weaponHolderType)
            : base(game)
        {
            if(game != null)
                this.game = game;
            else 
                throw new Exception("game can't be null!");

            ConnectedToXpa = true;
            if (weaponHolderType == WeaponHolderType.Player) 
                ContactGroup = ContactGroup.PlayerProjectiles;
            if (weaponHolderType == WeaponHolderType.Enemy) 
                ContactGroup = ContactGroup.EnemyProjectiles;

            Direction = direction;

            LocalPosition = startPos;
            FromPosition = startPos;

            if (soundName != String.Empty || soundName != "")
            {
                sound = game.Sounds.CreateSound(soundName);
                sound.Play();
            }
        }

        /// <summary>
        /// Called when the projectile collides with an other object.
        /// </summary>
        /// <param name="e">The event-data.</param>
        public override void OnCollision(CollisionEvent e)
        {
            base.OnCollision(e);

            // do not consider collision if there is no collision but an intersection
            if (!game.Physics.CanSolidsCollide(e.ThisSolid, e.OtherSolid)) return;

            if (EnableExplosion)
                OnExplosion(e.OtherSolid.UserData as ITransformable);

            destroyNextUpdate = true;
        }

        private bool explosion = true;
        public float ExplosionForce = 50000.0f;
        public float ExplosionRadius = 200.0f;

        /// <summary>
        /// Called when [explosion].
        /// </summary>
        /// <param name="other">The other.</param>
        public virtual void OnExplosion(ITransformable other)
        {
            if (other == null)
                return;

            // Apply Damage to Hit-Object

            //if(other is IEnemy)
            //    ((IEnemy)other).ReceiveDamage(Damage);

            if (explosion)
            {
                IEffect fx = game.Effects.CreateEffect("Explosion");
                fx.LocalPosition = LocalPosition;
                fx.Play(TimeSpan.FromSeconds(2), true);
                game.World.AddEffect(fx);
                game.Sounds.CreateSound("Explosion").Play();
                explosion = false;
                
                // assign a smoke plume to the hit target 

                IEffect smoke = game.Effects.CreateEffect("SmokePlume");
                smoke.Play(TimeSpan.FromSeconds(3), true);
                smoke.Parent = other; // Animation an Enemy anhängen
                game.World.AddEffect(smoke);

                // create blast effect
                new Blast(game, ContactGroup.PlayerProjectiles, GlobalPosition, ExplosionForce, ExplosionRadius);
            }

        }

        private bool autodestructed = false;

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if(destroyNextUpdate)
            {
                Destroy();
                return;
            }
            base.Update(gameTime);
            if (AutoDestruction)
            {
                if (autoDestructionTime <= DateTime.Now && !autodestructed)
                {
                    // Autodestroy the rocket, because no target is hit.
                    // this is possible e.g. if the rocket is fired in the sky.
                    OnAutoDestruction();
                    autodestructed = true;

                    Destroy();
                }
            }
        }

        /// <summary>
        /// Called when [auto destruction].
        /// </summary>
        public virtual void OnAutoDestruction()
        {
        }

        ~BasicProjectile()
        {
            //TODO: this should be fired on autodestruction, but is not
            // so this object does not get garbage collected, when it should be
            //TODO: FIX THIS!!!!
            Debug.Print("Projektil wird gelöscht!");
        }

        public override void Dispose()
        {
            if(sound != null)
                sound.Dispose();

            base.Dispose();
        }
    }
}
