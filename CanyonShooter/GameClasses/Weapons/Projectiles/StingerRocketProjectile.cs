using System;
using CanyonShooter.Engine.Graphics.Effects;
using CanyonShooter.Engine.Graphics.Lights;
using CanyonShooter.Engine.Physics;
using CanyonShooter.GameClasses.World.Debris;
using CanyonShooter.GameClasses.World.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaDevRu.Physics;
using CanyonShooter.GameClasses.World;
using CanyonShooter.Engine.Helper;

namespace CanyonShooter.GameClasses.Weapons
{
    public class StingerRocketProjectile : BasicProjectile, IWeaponProjectile
    {
        private IPointLight light;
        private ICanyonShooterGame game;
        private Vector3 velocity;
        private StingerRocket owner;
        public RocketMode Mode = RocketMode.Homing;

        public StingerRocketProjectile(ICanyonShooterGame game, Vector3 startPosition, Vector3 direction, StingerRocket owner, WeaponHolderType weaponHolderType)
            : base(game, startPosition, direction, "RocketLaunch", weaponHolderType)
        {
            this.game = game;
            this.owner = owner;
            // assign a Model
            SetModel("Stinger");

            // effect light
            light = new PointLight(game,Color.Azure,1.0f);
            light.Parent = this;
            game.World.AddPointLight(light);



            // rotate the projectile into the direction
            Vector3 defaultDirection = new Vector3(0, 0, -1);

            if (defaultDirection != Direction && defaultDirection != -Direction)
            {
                Direction.Normalize();
                defaultDirection.Normalize();
                Vector3 rotationAxe = Vector3.Cross(defaultDirection, Direction);
                rotationAxe.Normalize();
                float angle = (float)Math.Acos(Vector3.Dot(Direction, defaultDirection));
                LocalRotation = Quaternion.CreateFromAxisAngle(rotationAxe, angle);

            }

            // set speed and direction of the projectile:
            velocity = Direction * owner.ProjectileSpeed + owner.Velocity;
            Velocity = velocity;

            InfluencedByGravity = false;
            
            // create smoke effect
            SmokeEmitter = game.Effects.CreateEffect("RocketSmoke");
            
            // connect the smoke to the this projectile-object:
            SmokeEmitter.Parent = this;
            
            // place the smoke at the end of the rocket:
            SmokeEmitter.LocalPosition = new Vector3(0,0,3.5f);
            
            // add smoke to the world
            game.World.AddEffect(SmokeEmitter);

            // Loop Smoke until Destruction
            SmokeEmitter.Play();
            

        }
        /// <summary>
        /// this var holds the object of the smoke emitter
        /// and is deleted on destruction.
        /// </summary>
        private IEffect SmokeEmitter = null;

        /// <summary>
        /// Called when the projectile collides with an other object.
        /// </summary>
        /// <param name="e">The event-data.</param>
        public override void OnCollision(CollisionEvent e)
        {
            // do not consider collision if there is no collision but an intersection
            if (game.Physics.CanSolidsCollide(e.ThisSolid, e.OtherSolid))
            {
                SmokeEmitter.Dispose();
                light.Parent = null;
                game.World.RemovePointLight(light);

                // Erzeugt Geröll bei Kollision mit dem Canyon
                if(e.OtherSolid.UserData is IGameObject)
                {
                    if ((e.OtherSolid.UserData as IGameObject).ContactGroup == Engine.Physics.ContactGroup.Canyon)
                    {
                        DebrisEmitter d = new DebrisEmitter(game, "debrisTest", 10, 0.5f, 1.0f);
                        // do not use e.Position in the next statement because it could ne NaN.
                        d.LocalPosition = GlobalPosition + e.Normal * 10.0f;
                        d.Type = new DebrisEmitterTypeCone(game, e.Normal, 20, 10, 50);
                        d.SelfDestructDelay = TimeSpan.FromSeconds(10);
                        d.Emit(5);
                        game.World.AddObject(d);
                    }
                }
            }

            base.OnCollision(e);
        }

        private bool motorActive = true;

        private IGameObject target = null;
        private bool targetLocked = false;

        public override void Update(GameTime gameTime)
        {
            if(isDisposed) return;

            if (motorActive)
                switch(Mode)
                {
                    
                    case RocketMode.Normal:
                        Velocity = velocity;
                        break;

                    case RocketMode.Homing:
                        // first time set target;
                        if (target == null)
                        {
                            if (this.ContactGroup == ContactGroup.PlayerProjectiles)
                                target = GetEnemyWithShortestDistance();
                            else
                                target = game.World.Players[0];
                        }
                        else
                        {
                            if (ConnectedToXpa)
                            {
                                if (targetLocked || Vector3.Distance(target.GlobalPosition, GlobalPosition) <= 200)
                                {
                                    targetLocked = true;
                                    // homing missle auto targetting:
                                    Vector3 targetDir = GetDirectionToTarget(target.GlobalPosition);
                                    // lets create these cool searching effect.
                                    Random rnd = new Random(gameTime.TotalGameTime.Seconds);
                                    targetDir.X += rnd.Next(-2, 2)/4f;
                                    targetDir.Y += rnd.Next(-2, 2)/4f;
                                    targetDir.Z += rnd.Next(-2, 2)/4f;
                                    LocalRotation = Helper.RotateTo(targetDir, new Vector3(0, 0, -1));
                                    Velocity = targetDir*owner.ProjectileSpeed + owner.Velocity;
                                    ;
                                }
                                else // fly normally
                                    Velocity = velocity;
                            }
                        }

                        break;

                    default:
                        Velocity = velocity;
                        break;
                }


            base.Update(gameTime);
        }

        /// <summary>
        /// Called when [auto destruction].
        /// </summary>
        public override void OnAutoDestruction()
        {
            motorActive = false;
            InfluencedByGravity = true;

            SmokeEmitter.Dispose();
            light.Parent = null;
            game.World.RemovePointLight(light);
        }

        private bool isDisposed = false;

        public override void Dispose()
        {
            isDisposed = true;
            base.Dispose();
        }

        /// <summary>
        /// Gets the enemy with shortest distance.
        /// </summary>
        /// <returns></returns>
        public IEnemy GetEnemyWithShortestDistance()
        {
            IEnemy shortest = null;
            float shortestDisctance = -1;
            foreach (IEnemy enemy in game.World.Enemies)
            {
                float distance = Vector3.Distance(enemy.GlobalPosition, GlobalPosition);
                if (shortest == null)
                {
                    shortest = enemy;
                    shortestDisctance = distance;
                }
                else
                {
                    if(distance < shortestDisctance)
                    {
                        shortest = enemy;
                        shortestDisctance = distance;
                    }
                }
            }
            return shortest;
        }

        public Vector3 GetDirectionToTarget(Vector3 target)
        {
            Vector3 dir = -GlobalPosition + target;
            dir.Normalize();
            return dir;
        }

        #region IWeaponProjectile Member

        private int damage = 500;
        public new int Damage
        {
            get
            {
                return damage;
            }
            set
            {
                damage = value;
            }
        }

        #endregion
    }


    public enum RocketMode
    {
        Normal,
        Homing,
    } ;
}
