//
//
//  @ Project : CanyonShooter
//  @ File Name : ICanyon.cs
//  @ Date : 01.11.2007
//  @ Author : Christian Woizischke
//
//


using CanyonShooter.DataLayer.Level;
using Microsoft.Xna.Framework;

namespace CanyonShooter.GameClasses.World.Canyon
{
    /// <summary></summary>
    public interface ICanyon : IDrawable, IGameComponent
    {
        /*
        /// <summary>
        /// generates a new canyon segment from the description data.
        /// </summary>
        /// <param name="data">The description data.</param>
        /// <returns>True on success.</returns>
        bool AddSegment(SegmentDescription data);
        */

        void StreamUnload(int index);
        void StreamLoad(int index, Level level);

        Segment GetSegmentFromGlobalIndex(int index);

        void Dispose();
    }
}
