using Microsoft.Xna.Framework;
using XnaDevRu.Physics;

namespace CanyonShooter.Engine.Physics
{
    public class Blast
    {
        public Blast(ICanyonShooterGame game, ContactGroup contactGroup, Vector3 position, float force, float radius)
        {
            // TODO fix ode errors on multithreading

            if (!game.Physics.MultiThreading)
            {
                SphereShapeData sphere = new SphereShapeData();
                sphere.Radius = radius;
                sphere.ContactGroup = (int) contactGroup;

                ShapeData[] shapes = new ShapeData[1];
                shapes[0] = sphere;

                VolumeQueryResult result = game.Physics.VolumeQuery(
                    Matrix.CreateTranslation(position), // transform
                    "Blast Area", // name
                    shapes);

            /*    for (int i = 0; i < result.NumSolids; i++)
                {
                    Solid s = result.GetSolid(i);
                    ITransformable t = s.UserData as ITransformable;

                    if (t != null)
                    {
                        Vector3 direction = t.GlobalPosition - position;

                        float distance = direction.Length();

                        if (distance < radius)
                        {
                            float attenuation = 1.0f - distance/radius;

                            Force f = new Force();

                            // TODO make the force single step.
                            //f.SingleStep = true;
                            f.Duration = 0.1f;

                            f.Type = ForceType.GlobalForce;
                            f.Direction = attenuation*attenuation*force*Vector3.Normalize(direction);

                            t.AddForce(f);
                        }
                    }
                }*/            
            }
        }
    }
}
