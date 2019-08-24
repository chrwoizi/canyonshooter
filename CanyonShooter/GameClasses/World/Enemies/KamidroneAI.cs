// Zuständigkeit: Bettina

#region Using Statements

using System;
using CanyonShooter.DataLayer.Descriptions;
using CanyonShooter.Engine.Graphics.Effects;
using CanyonShooter.Engine.Helper;
using CanyonShooter.Engine.Physics;
using CanyonShooter.GameClasses.Console;
using CanyonShooter.GameClasses;
using CanyonShooter.GameClasses.Items;
using CanyonShooter.GameClasses.Weapons;
using CanyonShooter.GameClasses.World.Canyon;
using DescriptionLibs.EnemyType;
using Microsoft.Xna.Framework;
using XnaDevRu.Physics;
using CanyonShooter.Engine.AI;
using CanyonShooter.GameClasses.World.Enemies.StatesAI2;
using CanyonShooter.GameClasses.World.Enemies.StatesKamidroneAI;
using System.Collections.Generic;

#endregion

namespace CanyonShooter.GameClasses.World.Enemies
{
    /// <summary>
    /// 1 Gegner inklusive AI. Im besten Fall als Basisklasse für spezielle Gegnertypen verwenden.
    /// </summary>
    public class KamidroneAI : GameObject, IEnemy
    {
        #region Enemy Attributes and Properties

        // System
        private ICanyonShooterGame game;
        private EnemyTypeDescription typeDesc;

        public static int EnemiesCreated = 0;


        // Combat
        private WeaponManager weapons;
        private int currentHitpoints;

        // Travel

        private WaypointManager waypointManager;

        #endregion

        private int EnemyNr;

        /// <param name="desc">The description data (i.e. healthpoints, strength).</param>
        public KamidroneAI(ICanyonShooterGame game, EnemyDescription desc)
            : base(game, "Enemy-KamidroneAI")
        {
            EnemyNr = EnemiesCreated++;
            this.game = game;

            // Physics Config
            ConnectedToXpa = true;
            ContactGroup = ContactGroup.Enemies;
            InfluencedByGravity = false;


            InfoMessage(string.Format("Created at: {0}", desc.RelativeSpawnLocation));

            currentHitpoints = desc.MaxHitpoints;
            LocalPosition = desc.RelativeSpawnLocation;
            // System
            
            typeDesc = game.Content.Load<EnemyTypeDescription>("Content\\Enemies\\" + desc.Type);
            SetModel(typeDesc.Model);

            // Initiate AI-System:
            AI = new AIStateMachine(game, this);
            AI.DebugAI = false;

            // Add States to Machine:

            AI.AddState(new Patrol()); // Init-State
            AI.AddState(new FlyToWaypoint(desc.SegmentId));
            AI.AddState(new FlyToPlayer());
            
            
            // SquadronFormations

            if (desc.SquadronCount > 1) //Enemy-Squadron erzeugen.
            {
                
                // Create EnemyFormation and join it
                EnemyFormation formation = new EnemyFormation(game,LocalPosition,desc.SegmentId, desc.Speed,typeDesc.Formation);
                JoinFormation(formation);
                desc.SquadronCount = 1;
                int count = typeDesc.Formation.Count -1; // dieser enemy hat sich bereits selbst hinzugefügt.
                for (int i = 0; i < count; i++)
                {
                    // Versezte den nächsten Enemy in Richtung des Canyons
                    Vector3 canyonDirection = game.World.Level.Cache[desc.SegmentId].ADir;
                    canyonDirection.Normalize();
                    desc.RelativeSpawnLocation = desc.RelativeSpawnLocation + canyonDirection*100;

                    // Nächsten Enemy im Squadron erzeugen:

                    KamidroneAI enemyForFormation = new KamidroneAI(game, desc);
                    //enemyFollowing.FollowEnemy(this); // follow ME!
                    enemyForFormation.JoinFormation(formation);
                    game.World.AddObject(enemyForFormation);

                }
            }

           
        }

        public float Speed
        {
            get{ return typeDesc.Speed;}
        }

        public AIStateMachine AI;

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


        }

        /// <summary>
        /// Follows the enemy until it dies.
        /// The enemy returns to the default waypoint route after that.
        /// </summary>
        /// <param name="enemy">The enemy.</param>
        public void FollowEnemy(IEnemy enemy)
        {
            //if(AI.States["FLY_TO_WAYPOINT"].GetType() == typeof(AI2FlyToWaypoint))
            //    ((AI2FlyToWaypoint)AI.States["FLY_TO_WAYPOINT"]).Waypoints.FollowObject = enemy;
        }
        
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // base first because base handles collision events which might dispose "this"
            base.Update(gameTime);

            if (isDisposed)
                return;
            
            // Update AI
            AI.UpdateAI(gameTime);
            
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
                    InfoMessage("Getroffen!");
                    break;

                case Engine.Physics.ContactGroup.Player:
                    if (collisionObject is IPlayer)
                    {
                        InfoMessage("Explode at Player!");
                        //((IPlayer)collisionObject).ReceiveDamage(1);
                        OnDeath();
                    }
                    break;

                default:
                    //InfoMessage("No Action for Collision Group: " + collisionObject.ContactGroup);
                    break;
            }

        }

        #region IEnemy Members

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

        /// <summary>
        /// Called when enemy dies.
        /// </summary>
        private void OnDeath()
        {
            // Hier stirbt der Enemy

            //Explosions-Effekt
            IEffect fx;
            Random rnd = new Random((int)GlobalPosition.Z);
            int w = rnd.Next(0, 100);
            if(w <= 10)
                fx = game.Effects.CreateEffect("ExplosionBig"); // BIG 10%
            else if(w <= 40)
                fx = game.Effects.CreateEffect("ExplosionMiddle"); // MIDDLE 30%
            else
                fx = game.Effects.CreateEffect("Explosion"); // NORMAL 60%

            fx.Parent = this;
            game.World.AddEffect(fx);
            fx.Play(TimeSpan.FromSeconds(1.5f), true);
            // Score aktualisieren
            game.GameStates.Score.KilledEnemy++;
            game.GameStates.Score.AddPoints(20);
            Explode();
            //Item.CreateItem(game, "", this);
            
        }

        public int Health
        {
            get { return currentHitpoints; }
            set { currentHitpoints = value; }
        }

        #endregion

        public static bool ShowEnemyInfo = true;

        private void InfoMessage(string message)
        {
            if(ShowEnemyInfo)
                GraphicalConsole.GetSingleton(game).WriteLine(String.Format("Enemy{0}: {1}", EnemyNr, message),0);
        }

        private bool isDisposed = false;
        public override void Dispose()
        {
            base.Dispose();
            if(AI != null)
            {
                AI.States.Clear();
            }
            AI = null;

            FollowEnemy(null);
            isDisposed = true;
            InfoMessage("Disposed.");
            
        }

        ~KamidroneAI()
        {
            InfoMessage("Removed from memory.");
        }



        #region Formations

        public bool IsInFormation = false;
        public void JoinFormation(EnemyFormation formation)
        {
            if (formation == null)
                return;
            AI.AddState(new FlyInFormation(formation));
            if (formation.RegisterEnemy(EnemyNr))
                IsInFormation = true;
            else
                IsInFormation = false;
        }

        public void UpdateFormation(EnemyFormation formation)
        {
            if(formation == null)
                return;

            LocalPosition = formation.GetPosition(EnemyNr);
            LocalRotation = formation.GetRotation(EnemyNr);
        }

        #endregion


    }
}