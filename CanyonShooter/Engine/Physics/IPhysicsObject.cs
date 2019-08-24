using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using XnaDevRu.Physics;

namespace CanyonShooter.Engine.Physics
{
    public interface IPhysicsObject : IDisposable
    {
        void UpdateToXpa();
        void UpdateFromXpa();

        void AddForce(Force f);

        void AddShape(ShapeData data);
        ShapeData GetShape(int id);
        void ClearShapes();

        Vector3 Position { get; set; }
        Quaternion Orientation { get; set; }
        Vector3 GlobalLinearVel { get; set; }
        Vector3 GlobalAngularVel { get; set; }

        void SetMass(Mass newmass, Matrix offset);
        float Mass { get; }

        bool InfluencedByGravity { get; set; }
        bool Static { get; set; }

        CollisionEventProcessor CollisionEventProcessor { get; set; }
        object UserData { get; set; }

        Solid Solid { get; set; }
    }
}
