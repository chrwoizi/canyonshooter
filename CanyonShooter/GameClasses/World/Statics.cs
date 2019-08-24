// Zuständigkeit: Christian

#region Using Statements

using System;
using CanyonShooter.Engine.Graphics;
using CanyonShooter.Engine.Graphics.Models;
using CanyonShooter.Engine.Physics;
using Microsoft.Xna.Framework;
using XnaDevRu.Physics;
using CanyonShooter.GameClasses.World.Canyon;

#endregion

namespace CanyonShooter.GameClasses.World
{
    /// <summary>
    /// Diese Klasse stellt mehrere statische Objekte (z.B. Bäume, Steine, etc.) mit dem instancing-verfahren dar
    /// </summary>
    public class Statics : GameObject
    {
        #region Private Fields

        private ICanyonShooterGame game;

        private string modelName;

        private ITransformable[] instances;

        private Vector3[] positions;
        private Quaternion[] rotations;
        private float[] scales;

        private bool physics;

        #endregion


        public Statics(ICanyonShooterGame game, Segment canyonSegment, string modelName, Vector3[] positions, Quaternion[] rotations, float[] scales, bool physics)
            : base(game, "statics " + modelName)
        {
            this.canyonSegment = canyonSegment.GlobalIndex;
            this.physics = physics;
            this.game = game;
            this.modelName = modelName;

            if (rotations != null)
            {
                if (positions.Length != rotations.Length)
                {
                    throw new Exception();
                }
            }
            if (scales != null)
            {
                if (positions.Length != scales.Length)
                {
                    throw new Exception();
                }
            }

            instances = new ITransformable[positions.Length];
            for (int i = 0; i < instances.Length; ++i)
            {
                instances[i] = new Transformable(game);

                if (physics)
                {
                    instances[i].ConnectedToXpa = true;
                    instances[i].Static = true;
                }
            }

            this.positions = positions;
            this.rotations = rotations;
            this.scales = scales;
        }

        protected override void LoadContent()
        {
            Model = new StaticInstancingModel(game, modelName);

            for (int i = 0; i < instances.Length; ++i)
            {
                instances[i].LocalPosition = positions[i];
                instances[i].LocalRotation = rotations != null ? Quaternion.Concatenate(Model.GlobalRotation, rotations[i]) : Model.GlobalRotation;
                instances[i].LocalScale = scales != null ? Vector3.Multiply(new Vector3(scales[i]), Model.GlobalScale) : Model.GlobalScale;
            }

            foreach (HardwareInstancingModelMeshAdapterMesh mesh in Model.Meshes)
            {
                mesh.InitializeInstancing(instances);
            }

            if (physics)
            {
                foreach (ShapeData shape in Model.CollisionShapes)
                {
                    shape.ContactGroup = (int)ContactGroup.Statics;

                    for (int i = 0; i < instances.Length; ++i)
                    {
                        instances[i].AddShape(shape, ContactGroup);
                    }
                }
            }
        }

        public override void SetModel(string name)
        {
            throw new Exception("Cannot change the model.");
        }
    }
}


