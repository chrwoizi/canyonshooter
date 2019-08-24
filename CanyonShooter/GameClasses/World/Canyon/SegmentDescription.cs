using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace CanyonShooter.GameClasses.World.Canyon
{
    public class SegmentDescription
    {

        

        public SegmentDescription(Vector3 planeX, Vector3 planeY, Vector3 direction, List<Vector2> canyonForm)
        {
            this.planeX = planeX;
            this.planeY = planeY;
            this.direction = direction;
            this.canyonForm = canyonForm;
        }
        
        Vector3 planeX;

        public Vector3 PlaneX
        {
            get { return planeX; }
            set { planeX = value; }
        }
        Vector3 planeY;

        public Vector3 PlaneY
        {
            get { return planeY; }
            set { planeY = value; }
        }
        Vector3 direction;

        public Vector3 Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        List<Vector2> canyonForm;

        public List<Vector2> CanyonForm
        {
            get { return canyonForm; }
            set { canyonForm = value; }
        }

    }
}
