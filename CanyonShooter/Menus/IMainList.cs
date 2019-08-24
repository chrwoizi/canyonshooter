using System;
using System.Collections.Generic;
using System.Text;

namespace CanyonShooter.Menus
{
    public interface IMainList
    {
        #region Shifts
        ///<summary>
        ///Zugriff auf die Shifts der MainListe gewähren
        ///</summary>
        Button LeftShift { get;  }

        Button RightShift { get; }
        #endregion

        void init(string menuName, bool hori);
    }
}
