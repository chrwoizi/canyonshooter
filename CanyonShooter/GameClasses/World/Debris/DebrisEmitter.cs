using System;
using CanyonShooter.Engine.Graphics;
using CanyonShooter.Engine.Graphics.Models;
using CanyonShooter.Engine.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaDevRu.Physics;

namespace CanyonShooter.GameClasses.World.Debris
{
    public class DebrisEmitter : GameObject, IDebrisEmitter
    {
        private IDebrisEmitterType type;
        private ICanyonShooterGame game;
        private string modelName;
        private Matrix[] instanceData;
        private ITransformable[] instances;
        private int currentFreeInstance = 0;
        private float minSize = 1.0f;
        private float maxSize = 1.0f;
        private Random random = new Random();
        private TimeSpan selfDestructDelay;
        private TimeSpan selfDestructStart;
        private bool selfDestruct = false;

        public DebrisEmitter(ICanyonShooterGame game, string modelName, int maxInstanceCount, float minSize, float maxSize) : base(game, "debris emitter")
        {
            this.game = game;
            this.modelName = modelName;
            this.minSize = minSize;
            this.maxSize = maxSize;

            if (maxInstanceCount > 50) throw new Exception("Constants instancing doesn't support more than 50 instances.");

            instanceData = new Matrix[maxInstanceCount];
            instances = new ITransformable[maxInstanceCount];
            for (int i = 0; i < maxInstanceCount; i++)
            {
                instanceData[i] = Matrix.Identity;
                instances[i] = null;
            }

            Type = new DebrisEmitterTypeSphere(game, 100, 200);

            if(game.Graphics.Device != null)
            {
                LoadContent();
            }
        }

        protected override void LoadContent()
        {
            Model = new DynamicInstancingModel(game, modelName);
            foreach (ConstantsInstancingModelMeshAdapterMesh mesh in Model.Meshes)
            {
                mesh.InitializeInstancing(instanceData.Length);
            }
            foreach (ShapeData shape in Model.CollisionShapes)
            {
                shape.ContactGroup = (int)ContactGroup.Debris;
            }
        }

        public override void SetModel(string name)
        {
            throw new Exception("Cannot change the model.");
        }

        #region IDebrisEmitter Members

        public IDebrisEmitterType Type
        {
            get { return type; }
            set
            {
                if(type != null) type.Parent = null;
                type = value;
                type.Parent = this;
            }
        }

        public TimeSpan SelfDestructDelay
        {
            get { return selfDestructDelay; }
            set 
            {
                selfDestruct = true;
                selfDestructDelay = value;
                selfDestructStart = new TimeSpan();
            }
        }

        public bool SelfDestruct
        {
            get
            {
                return selfDestruct;
            }
            set
            {
                selfDestruct = value;
            }
        }

        public override void SetShaderConstant(EffectParameter constant)
        {
            if (constant.Semantic == "INSTANCE_DATA")
            {
                constant.SetValue(instanceData);
            }
            else base.SetShaderConstant(constant);
        }

        private int previousCount = 0;
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(selfDestruct)
            {
                if (selfDestructStart.CompareTo(new TimeSpan()) == 0)
                {
                    selfDestructStart = gameTime.TotalGameTime;
                }
                else
                {
                    if (selfDestructDelay.CompareTo(gameTime.TotalGameTime-selfDestructStart) < 0)
                    {
                        Destroy();
                    }
                }
            }

            int id = 0;
            for (; id < instances.Length; ++id)
            {
                if (instances[id] != null)
                {
                    instances[id].Update(gameTime);

                    instanceData[id] = Matrix.Transpose(instances[id].GlobalTransformation);

                    // set visible flag
                    instanceData[id].M41 = 1.0f;
                }
            }
            for (int i = id; i < previousCount; ++i)
            {
                // set visible flag
                instanceData[i].M41 = 0.0f;
            }

            previousCount = instances.Length;
        }

        public static Transformable ttt;

        public void Emit(int count)
        {
            for(int i = 0; i < count; ++i)
            {
                ITransformable t;
                if(ttt == null)
                {
                    t = ttt = new Transformable(game);
                }
                else
                {
                    t = new Transformable(game);
                }
                t.ConnectedToXpa = true;

                float size = minSize + (maxSize - minSize) * (float)random.NextDouble();
                t.LocalScale = new Vector3(size, size, size);

                foreach (ShapeData shape in Model.CollisionShapes)
                {
                    t.AddShape(shape, ContactGroup);
                }

                t.Mass = Model.MassInDescription;

                type.Apply(t);

                if(instances[currentFreeInstance] != null) instances[currentFreeInstance].Dispose();
                instances[currentFreeInstance] = t;
                currentFreeInstance = (currentFreeInstance + 1) % instances.Length;
            }
        }

        #endregion

        #region IDisposable Members

        public override void Dispose()
        {
            for (int i = 0; i < instances.Length; ++i )
            {
                if (instances[i] != null)
                {
                    instances[i].Dispose();
                    instances[i] = null;
                }
            }

            base.Dispose();
        }

        #endregion
    }
}
