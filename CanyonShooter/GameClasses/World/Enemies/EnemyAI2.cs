using System;
using System.Collections.Generic;
using System.Text;
using CanyonShooter.DataLayer.Descriptions;
using CanyonShooter.Engine.Physics;
using XnaDevRu.Physics;
using Microsoft.Xna.Framework;
using CanyonShooter.Engine.Graphics.Effects;
using DescriptionLibs.EnemyType;
using CanyonShooter.GameClasses.Items;
using CanyonShooter.GameClasses.Weapons;
using CanyonShooter.Engine.Helper;
using CanyonShooter.Engine.Graphics;
using CanyonShooter.Engine.Audio;

namespace CanyonShooter.GameClasses.World.Enemies
{
    public class EnemyAI2 : GameObject, IEnemy
    {
        private ICanyonShooterGame game;
        private EnemyTypeDescription typeDesc;
        private int itemChance = 80;
        private IList<string> itemList;
        private WeaponManager weapons;
        private IMesh modelAnimationPart;
         /// <param name="desc">The description data (i.e. healthpoints, strength).</param>
        public EnemyAI2(ICanyonShooterGame game, EnemyDescription desc)
            : base(game, "Enemy-AI2")
        {
            this.game = game;
            
            // Physics Config
            ConnectedToXpa = true;
            ContactGroup = ContactGroup.Enemies;
            InfluencedByGravity = false;
            LocalPosition = desc.RelativeSpawnLocation;
            typeDesc = game.Content.Load<EnemyTypeDescription>("Content\\Enemies\\" + desc.Type);
            currentHitpoints = typeDesc.MaxHitpoints;
            SetModel(typeDesc.Model);
            LocalScale = new Vector3(1.5f, 1.5f, 1.5f);
            itemChance = desc.ItemChance;
            itemList = desc.ItemList;

            if (game.Graphics.ShadowMappingSupported)
            {
                game.World.Sky.Sunlight.ShadowMapLow.Scene.AddDrawable(this);
                game.World.Sky.Sunlight.ShadowMapHigh.Scene.AddDrawable(this);
            }

            #region Init WeaponManager and add Weapons
            if (weapons == null)
            {
                weapons = new WeaponManager(game, Model, WeaponHolderType.Enemy);


                foreach (string weapon in typeDesc.Weapons)
                {
                    weapons.AddWeapon((WeaponType)Enum.Parse(typeof(WeaponType), weapon, true));
                }
            }
            #endregion
        }
        private ISound FxSound;
        public override void Dispose()
        {
            if (modelAnimationPart != null)
                modelAnimationPart.StopRotationAnimation();
            if (modelAnimationPart != null)
                modelAnimationPart.StopRotationAnimation();
            if (FxSound != null)
            {
                FxSound.Loop = false;
                FxSound.Stop();
            }
        }
        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here
            base.Initialize();
        }

        protected override void LoadContent()
        {

            base.LoadContent(); //Wichtig, base muss vorher aufgerufen werden,
            //damit die GameObject-Klasse das Model lädt
            #region Helicopter animation hack
            modelAnimationPart = this.Model.GetMesh("Rotor");
            if (modelAnimationPart != null)
            {

                //FxSound = game.Sounds.CreateSound("Heli");
                //FxSound.Loop = true;
                //FxSound.Parent = this;
                modelAnimationPart.PlayRotationAnimation();
            }
            #endregion
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

                case Engine.Physics.ContactGroup.Player:
                    if (collisionObject is Player2)
                    {
                        OnDeath();
                        ((Player2)collisionObject).ReceiveDamage(20);
                    }
                    break;

                default:
                    break;
            }

        }
        private bool playerPassed = false;
        private int id = 0;
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // base first because base handles collision events which might dispose "this"
            base.Update(gameTime);

            // update 1 time a second only
            if (Helper.WaitFor("EAI" + id, TimeSpan.FromSeconds(0.25)))
            {
                Helper.ResetWait("EAI");
                // Compute distance
                Vector3 a = GlobalPosition;
                Vector3 b = game.World.Players[0].LocalPosition;
                float distance;
                Vector3.Distance(ref a, ref b, out distance);

                // If player arrives aim the player
                if (distance < 210.0f)
                {
                    // Direction to player
                    Vector3 d;
                    Vector3.Subtract(ref b, ref a, out d);
                    Vector3.Negate(d);
                    
                    d.Normalize();

                    // Aim the weapons
                    //Weapons.SecondaryWeapon.AimAt(d);

                    // Fire to player
                    if (distance < 200.0f)
                    {
                        
                        // Fire a rocket only a few times
                        if (weapons.GetAmmo(AmmoType.ROCKETS) != 0)
                        {
                            // Aim the weapons
                      //      weapons.PrimaryWeapon.AimAt(d);
                        //    weapons.PrimaryWeapon.BeginFire();
                        }
                    }

                    // Notice if player passed the enemy
                    if (distance < 400.0f) playerPassed = true;
                }

                // Player passed its time to explode
                if ((distance > 1000.0f) && playerPassed)
                {
                    //if (weapons.PrimaryWeapon != null) weapons.PrimaryWeapon.EndFire();
                    //weapons = null;
                    game.World.RemoveObject(this);
                }
            }
        }

        public int Identifier
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public void ReceiveDamage(int damage)
        {
            currentHitpoints -= damage;
            if (currentHitpoints <= 0)
            {
                OnDeath();
            }
        }

        //private bool dying = false;
        /// <summary>
        /// Called when enemy dies.
        /// </summary>
        private void OnDeath()
        {
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

            // Score aktualisieren
            game.GameStates.Score.KilledEnemy++;
            game.GameStates.Score.AddPoints(typeDesc.ScorePoints);
            Explode();

            //TODO: Implement Item-List
            Item.CreateItem(game, Item.RandomItemName(itemList, itemChance), this);
        }
        int currentHitpoints;
        public int Health
        {
            get { return currentHitpoints; }
            set { currentHitpoints = value; }
        }

    }
}
