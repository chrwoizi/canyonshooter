using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace CanyonShooter.GameClasses.ModelChooser
{
    class GameModel
    {
        public Dictionary<int, Model> parts = new Dictionary<int, Model>();
        public Dictionary<int, string[]> description = new Dictionary<int, string[]>();

        public GameModel(Dictionary<int, Model> p, Dictionary<int, string[]> d)
        {
            parts = p;
            description = d;
        }
    }
}
