//
//
//  @ Project : CanyonShooter
//  @ File Name : ISky.cs
//  @ Date : 01.11.2007
//  @ Author : Christian Woizischke
//
//


using CanyonShooter.Engine.Graphics.Lights;
using Microsoft.Xna.Framework;

namespace CanyonShooter.GameClasses.World
{
    /// <summary></summary>
    public interface ISky : IGameComponent, IDrawable
    {
        ISunlight Sunlight { get; set; }
    }
}
