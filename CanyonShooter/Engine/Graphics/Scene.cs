using System.Collections.Generic;
using CanyonShooter.Engine.Graphics.Cameras;
using Microsoft.Xna.Framework;

namespace CanyonShooter.Engine.Graphics
{
    public class Scene : IScene
    {
        class Comparer : IComparer<IDrawable>
        {
            #region IComparer<IDrawable> Members

            public int Compare(IDrawable x, IDrawable y)
            {
                return x.DrawOrder - y.DrawOrder;
            }

            #endregion
        }

        private List<IDrawable> drawables = new List<IDrawable>();
        private bool sortNextTime = false;
        private ICamera camera;

        public Scene()
        {
            
        }

        #region IScene Members

        public bool isEmpty
        {
            get
            {
                return drawables.Count == 0;
            }
        }

        public void AddDrawable(IDrawable d)
        {
            if (!drawables.Contains(d))
            {
                drawables.Add(d);
                drawables.Sort(new Comparer());
            }
        }

        public void RemoveDrawable(IDrawable d)
        {
            if (drawables.Contains(d))
            {
                drawables.Remove(d);
            }
        }

        public void Draw(GameTime gameTime)
        {
            if(sortNextTime)
            {
                drawables.Sort(new Comparer());
                sortNextTime = false;
            }

            int currentDrawOrder = 0;

            foreach (IDrawable drawable in drawables)
            {
                if (drawable.Visible)
                {
                    if (currentDrawOrder > drawable.DrawOrder)
                    {
                        sortNextTime = true;
                    }

                    drawable.Draw(gameTime);

                    currentDrawOrder = drawable.DrawOrder;
                }
            }
        }

        #endregion

        #region IScene Members


        public ICamera Camera
        {
            get
            {
                return camera;
            }
            set
            { 
                camera = value;
            }
        }

        #endregion
    }
}
