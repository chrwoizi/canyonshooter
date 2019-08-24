// Zuständigkeit: Richard, Christian, Florian

#region Using Statements

using System.Collections.ObjectModel;
using CanyonShooter.Engine;
using CanyonShooter.Engine.Graphics.Models;
using CanyonShooter.Engine.Physics;
using CanyonShooter.GameClasses.World.Debris;
using DescriptionLibs.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaDevRu.Physics;
using Model=CanyonShooter.Engine.Graphics.Models.Model;

#endregion

namespace CanyonShooter.GameClasses.World
{
    /// <summary>
    /// Objekte wie z.B. Items, Enemies leiten von dieser Klasse ab.
    /// Diese Klasse ermöglicht z.B. das Einbinden in die Physik-Engine.
    /// </summary>
    public class GameObject : DrawableGameComponent, IGameObject
    {
        #region Private Data Members

        private Transformable transformable;
        private IModel model;
        private ICanyonShooterGame game;
        private string modelName = "";
        private string name;
        private bool autoApplyModelsCollisionShapes = true;
        private bool autoApplyModelsMass = true;
        private ContactGroup contactGroup = ContactGroup.Default;
        protected int canyonSegment = -1;
        private bool isExploded = false;

        #endregion

        /// <summary>
        /// Creates a new GameObject and adds it to the world
        /// </summary>
        /// <param name="game"></param>
        public GameObject(ICanyonShooterGame game, string name)
            : base(game as Game)
        {
            this.game = game;
            this.name = name;
            transformable = new Transformable(game, this, this);

            DrawOrder = (int)DrawOrderType.Default;
        }

        /// <summary>
        /// Creates a new GameObject and adds it to the world
        /// </summary>
        /// <param name="game"></param>
        public GameObject(ICanyonShooterGame game)
            : this(game, "")
        {
            
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

            if(model != null) model.Update(gameTime);

            transformable.Update(gameTime);
        }

        //
        // Zusammenfassung:
        //     Called when the DrawableGameComponent needs to be drawn.  Override this method
        //     with component-specific drawing code.
        //
        // Parameter:
        //   gameTime:
        //     Time passed since the last call to Microsoft.Xna.Framework.DrawableGameComponent.Draw(Microsoft.Xna.Framework.GameTime).
        public override void Draw(GameTime gameTime)
        {
            if (model != null) model.Draw(this);
            /*if(game.Renderer.DrawCollisionShapes)
            {
                foreach (ShapeData shape in transformable.Shapes)
                {
                    game.Renderer.DrawCollisionShape(shape, GlobalTransformation);
                } 
            }*/
        }

        //
        // Zusammenfassung:
        //     Called when the component needs to load graphics resources.  Override this
        //     method to load any component-specific graphics resources.
        //
        // Parameter:
        //   loadAllContent:
        //     true if all graphics resources need to be loaded; false if only manual resources
        //     need to be loaded.

        protected override void LoadContent()
        {
            if(modelName != "")
            {
                if (Model != null) Model.Parent = null;
                Model = new Model(game, modelName);
            }
        }

        public virtual void Explode()
        {
            if (!isExploded)
            {
                if (model != null)
                {
                    Vector3 velocity = Velocity;
                    if (velocity == Vector3.Zero) velocity = -Vector3.UnitZ;
                    DebrisEmitterTypeCone emitter = new DebrisEmitterTypeCone(game, Vector3.Normalize(velocity), 30, Velocity.Length() * 1.0f, velocity.Length() * 2.0f);

                    foreach (WreckageModel wreckageModel in model.WreckageModels)
                    {
                        Wreckage wreckage = new Wreckage(game);
                        wreckage.ContactGroup = ContactGroup;
                        wreckage.SetModel(wreckageModel.Model);
                        wreckage.Parent = Parent;
                        emitter.LocalPosition = LocalPosition + wreckageModel.Position;
                        emitter.Apply(wreckage);
                        game.World.AddObject(wreckage);
                    }
                }

                isExploded = true;

                if (!(this is IPlayer))
                {
                    Destroy();
                }
            }
        }

        #region IGameObject Members

        public virtual string Name
        {
            get
            {
                return name;
            }
        }

        public virtual IModel Model
        {
            get
            {
                return model;
            }
            set 
            {
                if (model != null)
                {
                    model.Parent = null;
                }

                model = value;

                if (model != null)
                {
                    model.Parent = this;
                    if (autoApplyModelsCollisionShapes) ApplyModelsCollisionShapes();
                }

                modelName = "";
            }
        }

        public ContactGroup ContactGroup
        {
            get { return contactGroup;  }
            set 
            { 
                contactGroup = value;
                if(autoApplyModelsCollisionShapes) ApplyModelsCollisionShapes();
            }
        }

        public virtual void SetModel(string name)
        {
            modelName = name;
            if (model != null)
            {
                model.Parent = null;
                model = new Model(game, modelName);
                model.Parent = this;
                if (autoApplyModelsCollisionShapes) ApplyModelsCollisionShapes();
                if (autoApplyModelsMass) ApplyModelsMass();
            }
        }

        public bool AutoApplyModelsCollisionShapes
        {
            get
            {
                return autoApplyModelsCollisionShapes;
            }
            set
            {
                autoApplyModelsCollisionShapes = value;
            }
        }

        public bool AutoApplyModelsMass
        {
            get
            {
                return autoApplyModelsMass;
            }
            set
            {
                autoApplyModelsMass = value;
            }
        }

        public void ApplyModelsCollisionShapes()
        {
            if (ConnectedToXpa && Model != null)
            {
                ClearShapes();
                foreach (ShapeData shape in Model.CollisionShapes)
                {
                    shape.ContactGroup = (int)contactGroup;
                    AddShape(shape);
                }
            }
        }

        public void ApplyModelsMass()
        {
            if (ConnectedToXpa && Model != null)
            {
                Mass = Model.MassInDescription;
            }
        }

        public void Destroy()
        {
            Enabled = false;
            Visible = false;

            game.World.RemoveObject(this);

            Dispose();
        }

        public new virtual void Dispose()
        {
            base.Dispose();

            if (game.Graphics.ShadowMappingSupported)
            {
                game.World.Sky.Sunlight.ShadowMapLow.Scene.RemoveDrawable(this);
                game.World.Sky.Sunlight.ShadowMapHigh.Scene.RemoveDrawable(this);
            }

            transformable.Dispose();
            
            if(Model != null)
                Model.Parent = null;
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
                if (autoApplyModelsCollisionShapes) ApplyModelsCollisionShapes();
                if (autoApplyModelsMass) ApplyModelsMass();
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

        public void AddShape(ShapeData shape, ContactGroup contact)
        {
            transformable.AddShape(shape, contact);
        }

        public ShapeData GetShape(int id)
        {
            return transformable.GetShape(id);
        }

        public void ClearShapes()
        {
            transformable.ClearShapes();
        }

        public void AddCollisionEvent(QueueableCollisionEvent e)
        {
            transformable.AddCollisionEvent(e);
        }

        public void AddForce(Force f)
        {
            transformable.AddForce(f);
        }

        public void BeforePhysicsSimulationStep() { transformable.BeforePhysicsSimulationStep(); }

        public void AfterPhysicsSimulationStep() { transformable.AfterPhysicsSimulationStep(); }

        #endregion

        #region ICollisionActor Member

        public virtual void OnCollision(CollisionEvent e)
        {

        }

        #endregion

        #region IShaderConstantsSetter Members

        public virtual void SetShaderConstant(EffectParameter constant)
        {
            
        }

        #endregion

        #region IGameObject Members


        public int CanyonSegment
        {
            get { return canyonSegment; }
            set { canyonSegment = value;}
        }

        public void AddShape(ShapeData shape)
        {
            transformable.AddShape(shape, contactGroup);
        }

        #endregion
    }
}


