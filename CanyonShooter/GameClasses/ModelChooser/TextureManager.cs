using System;
using System.Collections.Generic;
using System.Text;
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
    public class TextureManager
    {
        static Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();

        public static Texture2D LoadTexture(GraphicsDevice device, string fileName)
        {
            if (!Textures.ContainsKey(fileName))
            {
                Textures.Add(fileName, Texture2D.FromFile(device, fileName));
            }
            return Textures[fileName];
        }

        public static void Dispose()
        {
            Dictionary<string, Texture2D>.Enumerator enumer = Textures.GetEnumerator();

            while (enumer.MoveNext())
            {
                enumer.Current.Value.Dispose();
            }
            Textures.Clear();
        }
    }

}
