// Zuständigkeit: Florian
#region Using Statements

using System;
using System.Collections.ObjectModel;
using CanyonShooter.Engine.Physics;
using CanyonShooter.GameClasses.World;
using CanyonShooter.ParticleEngine;
using DescriptionLibs.ParticleEffect;
using Microsoft.Xna.Framework;
using XnaDevRu.Physics;

#endregion

namespace CanyonShooter.Engine.Graphics.Effects
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public abstract class Effect : ParticleSystem, IEffect
    {
        #region Private Data Members

        private Transformable transformable;
        private ICanyonShooterGame game;
        #endregion

        public ParticleSettings Settings
        {
            get
            {
                return this.settings;
            }
        }

        /// <summary>
        /// loads the effect ressources from files.
        /// </summary>
        /// <param name="type">The type of effect (i.e. explosion).</param>
        public Effect(ICanyonShooterGame game, EffectType type, ParticleSettings settings)
            : base(game as Game,game.Content)
        {
            transformable = new Transformable(game, this, null);
            this.game = game;
            this.type = type;
            this.settings = settings;
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

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            transformable.Update(gameTime);
        }



        #region IEffect Member

        private EffectType type;

        public EffectType Type
        {
            get { return type; }
        }

        public abstract void Play(TimeSpan duration, bool destroy);

        public virtual void Play()
        {
            game.World.AddEffect(this);
        }
        #endregion

        #region ITransformable Member

        public void RecalculateGlobalValues()
        {
            transformable.RecalculateGlobalValues();
        }

        public void AddChild(ITransformable c)
        {
            transformable.AddChild(c);
        }

        public void RemoveChild(ITransformable c)
        {
            transformable.RemoveChild(c);
        }

        public Matrix GlobalTransformation
        {
            get { return transformable.GlobalTransformation; }
        }

        public Matrix LocalTransformation
        {
            get { return transformable.LocalTransformation; }
        }

        public Vector3 GlobalPosition
        {
            get { return transformable.GlobalPosition; }
        }

        public Vector3 LocalPosition
        {
            get
            {
                return transformable.LocalPosition;
            }
            set
            {
                transformable.LocalPosition = value;
            }
        }

        public Quaternion GlobalRotation
        {
            get { return transformable.GlobalRotation; }
        }

        public Quaternion LocalRotation
        {
            get
            {
                return transformable.LocalRotation;
            }
            set
            {
                transformable.LocalRotation = value;
            }
        }

        public Vector3 GlobalScale
        {
            get { return transformable.GlobalScale; }
        }

        public Vector3 LocalScale
        {
            get
            {
                return transformable.LocalScale;
            }
            set
            {
                transformable.LocalScale = value;
            }
        }

        public void Move(Vector3 v)
        {
            transformable.Move(v);
        }

        public void Rotate(Quaternion q)
        {
            transformable.Rotate(q);
        }

        public void Scale(Vector3 v)
        {
            transformable.Scale(v);
        }

        public ITransformable Parent
        {
            get
            {
                return transformable.Parent;
            }
            set
            {
                transformable.Parent = value;
            }
        }

        public ReadOnlyCollection<ITransformable> Childs
        {
            get { return transformable.Childs; }
        }

        public bool ConnectedToXpa
        {
            get
            {
                return transformable.ConnectedToXpa;
            }
            set
            {
                transformable.ConnectedToXpa = value;
            }
        }

        public Vector3 Velocity
        {
            get
            {
                return transformable.Velocity;
            }
            set
            {
                transformable.Velocity = value;
            }
        }

        public Vector3 AngularVelocity
        {
            get
            {
                return transformable.AngularVelocity;
            }
            set
            {
                transformable.AngularVelocity = value;
            }
        }

        public ITransformable Self
        {
            get
            {
                return transformable.Self;
            }
        }

        public void BeforePhysicsSimulationStep() { transformable.BeforePhysicsSimulationStep(); }

        public void AfterPhysicsSimulationStep() { transformable.AfterPhysicsSimulationStep(); }

        public ICollisionActor CollisionActor 
        {
            get { return transformable.CollisionActor; }
            set { transformable.CollisionActor = value; }
        }

        public bool InfluencedByGravity
        {
            get { return transformable.InfluencedByGravity; }
            set { transformable.InfluencedByGravity = value; }
        }

        public bool Static
        {
            get { return transformable.Static; }
            set { transformable.Static = value; }
        }

        public float Mass
        {
            get { return transformable.Mass; }
            set { transformable.Mass = value; }
        }

        public void AddShape(ShapeData shape, ContactGroup group)
        {
            transformable.AddShape(shape, group);
        }

        public ShapeData GetShape(int id)
        {
            return transformable.GetShape(id);
        }

        public void ClearShapes()
        {
            transformable.ClearShapes();
        }

        public void AddForce(Force f)
        {
            transformable.AddForce(f);
        }

        #endregion

        public virtual void OnCollision(IGameObject other)
        {

        }

        protected override void Dispose(bool disposing)
        {
            Parent = null;
            if(game.World != null)
                game.World.RemoveEffect(this);
            base.Dispose(disposing);
        }

        #region ITransformable Members


        public void AddCollisionEvent(QueueableCollisionEvent e)
        {
            transformable.AddCollisionEvent(e);
        }

        #endregion
    }
}


