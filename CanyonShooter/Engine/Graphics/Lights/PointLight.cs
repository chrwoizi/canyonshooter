using System;
using System.Collections.ObjectModel;
using CanyonShooter.Engine.Graphics.Cameras;
using CanyonShooter.Engine.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaDevRu.Physics;

namespace CanyonShooter.Engine.Graphics.Lights
{
    class PointLight : GameComponent, IPointLight
    {
        private ICanyonShooterGame game;
        private Color color = new Color(255, 255, 255);
        protected float linearAttenuation = 0.00f;
        protected float squaredAttenuation = 0.00005f;
        private bool shadows = false;
        private ShadowMap shadowMap = null;
        private Transformable transformable;

        public PointLight(ICanyonShooterGame game)
            : base(game as Game)
        {
            transformable = new Transformable(game, this, null);
            this.game = game;
        }

        public PointLight(ICanyonShooterGame game, Color color)
            : base(game as Game)
        {
            transformable = new Transformable(game, this, null);
            this.game = game;
            this.color = color;
        }

        public PointLight(ICanyonShooterGame game, float r, float g, float b)
            : base(game as Game)
        {
            transformable = new Transformable(game, this, null);
            this.game = game;
            color = new Color(new Vector3(r,g,b));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="game"></param>
        /// <param name="color"></param>
        /// <param name="intensity"></param>
        public PointLight(ICanyonShooterGame game, Color color, float intensity)
            : base(game as Game)
        {
            transformable = new Transformable(game, this, null);
            this.game = game;

            float multiplier;
            if(color.R > color.G)
            {
                if(color.R > color.B)
                {
                    // max R
                    multiplier = 255.0f / (float)color.R;
                }
                else
                {
                    // max B
                    multiplier = 255.0f / (float)color.B;
                }
            }
            else
            {
                if(color.G > color.B)
                {
                    // max G
                    multiplier = 255.0f / (float)color.G;
                }
                else
                {
                    // max B
                    multiplier = 255.0f / (float)color.B;
                }
            }
            multiplier *= intensity;
            this.color = new Color(
                (byte)Math.Min(((float)color.R * multiplier), 255.0f),
                (byte)Math.Min(((float)color.G * multiplier), 255.0f),
                (byte)Math.Min(((float)color.B * multiplier), 255.0f)
                );
        }

        #region ILight Members

        public float Relevance(ICamera camera)
        {
            Vector3 v = camera.GlobalPosition - GlobalPosition;
            return v.Length();
        }

        public void On()
        {
            game.World.AddPointLight(this);
        }

        public void Off()
        {
            game.World.RemovePointLight(this);
        }

        #endregion

        #region IPointLight Members

        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        public float LinearAttenuation
        {
            get
            {
                return linearAttenuation;
            }
            set
            {
                linearAttenuation = value;
            }
        }

        public float SquaredAttenuation
        {
            get
            {
                return squaredAttenuation;
            }
            set
            {
                squaredAttenuation = value;
            }
        }

        public bool Shadows
        {
            get
            {
                return shadows;
            }
            set
            {
                if(shadows == value) return;

                shadows = value;

                if (shadowMap != null)
                {
                    shadowMap.Dispose();
                }

                if(shadows)
                {
                    shadowMap = new ShadowMap(game, ShadowMap.DefaultSize, ShadowMap.DefaultSize);
                    UpdateOrder = (int)UpdateOrderType.ShadowMap;
                    Enabled = true;
                }
                else
                {
                    Enabled = false;
                }
            }
        }

        public ShadowMap ShadowMap
        {
            get
            {
                return shadowMap;
            }
        }

        #endregion

        #region IUpdateable Members

        public override void Update(GameTime gameTime)
        {
 	        base.Update(gameTime);

            if (shadowMap != null)
            {
                shadowMap.Draw(gameTime);
            }

            transformable.Update(gameTime);
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

        public void BeforePhysicsSimulationStep() { transformable.BeforePhysicsSimulationStep(); }

        public void AfterPhysicsSimulationStep() { transformable.AfterPhysicsSimulationStep(); }

        #endregion

        #region ITransformable Members


        public void AddCollisionEvent(QueueableCollisionEvent e)
        {
            transformable.AddCollisionEvent(e);
        }

        #endregion
    }
}
