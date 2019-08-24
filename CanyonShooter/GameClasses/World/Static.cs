// Zuständigkeit: Christian

#region Using Statements

using CanyonShooter.Engine.Physics;

#endregion

namespace CanyonShooter.GameClasses.World
{
    /// <summary>
    /// Diese Klasse und alle abgeleiteten Klassen stellen statische Objekte dar, wie z.B. Bäume, Steine, etc.
    /// </summary>
    public class Static : GameObject, IStatic
    {
        #region Private Fields

        private string name;

        #endregion


        public Static(ICanyonShooterGame game, string name)
            : base(game, name)
        {
            ConnectedToXpa = true;
            ContactGroup = ContactGroup.Statics;
            Static = true; // Objekt an diese Stelle festnageln!!!
            SetModel(this.name = name);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        #region IStatic Members

        public override string Name
        {
            get { return name; }
        }

        #endregion
    }
}


