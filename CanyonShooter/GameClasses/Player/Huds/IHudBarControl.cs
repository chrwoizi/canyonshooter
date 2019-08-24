using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CanyonShooter.GameClasses.Huds
{
    public interface IHudBarControl : IHudControl
    {
        string Text { get; set;}
        int Value{ get; set;}
        void Draw(SpriteBatch sb, SpriteFont font);

        Rectangle MinRect { get; set; }
        Rectangle MaxRect { get; set; }

    }
}
