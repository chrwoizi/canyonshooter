#region Using Statements
using System;
using System.Collections.Generic;
using DescriptionLibs.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using DifficultEnum = CanyonShooter.Menus.Menu.DifficultEnum;

#endregion

namespace CanyonShooter.Menus
{
    class Dialog : DrawableGameComponent
    {
        #region Data Members
        private ICanyonShooterGame game;
        private OptionDescription xml;
        private int district;

        private Rectangle Position;

        public Vector2 startposition;
        private Vector2 space;

        public Dictionary<string, object> Data;
        private Dictionary<string, string> Format;
        private List<string> Names;

        //test
        private Dictionary<string, OptionModulationButton> OptMod;
        private Dictionary<string, HSlideBar> SlideMod;
        /*
        private int height;
        private int width;
        */
        #region Graphic
        private Texture2D titel;
        private Texture2D gap;
        private Texture2D dialogtop;
        private Texture2D dialogleft;
        private Texture2D dialogright;
        private Texture2D dialogmid;
        private Texture2D dialogbottom;
        private SpriteFont font;
        private bool visible;

        #endregion

        public ButtonList list;
        public Button accept;
        public Button decline;
        #endregion

        #region Properties

        public Dictionary<string, OptionModulationButton> OptModDic
        {
            get { return OptMod; }
        }
        
        public Dictionary<string, HSlideBar> SlideMods
        {
            get { return SlideMod; }
        }

        public bool Visiblity
        {
            get { return visible; }
            set { visible = value; }
        }

        public int District
        {
            get { return district; }
        }
        #endregion


        public Dialog(ICanyonShooterGame game, OptionDescription xml, int district, 
            Dictionary<string, object> Data,Dictionary<string, string> Format, List<string> Names)
            : base(game as Game)
        {
            this.game = game;
            this.xml = xml;
            this.Data =Data;
            this.Format = Format;
            this.Names = Names;
            this.district = district;
            this.visible = true;
            this.list = new OptionList(game,xml,district,Data,Format,Names,false, this);

            OptMod = new Dictionary<string,OptionModulationButton>();
            SlideMod = new Dictionary<string, HSlideBar>();
            int counter = Names.Count;
            foreach (Button g in this.list)
            {
                OptionModulationButton opt = getOptionModulationButton(g, Names[Names.Count - counter]);
                if (opt != null)
                {
                    OptMod.Add(g.getButtonName(), opt);
                }
                else
                {
                    HSlideBar hsb = getSlideBar(g,Names[Names.Count - counter]);
                    if (hsb != null)
                    {
                        SlideMod.Add(g.getButtonName(),hsb);
                    }
                }
                counter--;
            }

            foreach (OptionpartDescription i in xml.OptionParts)
            {
                if (i.Number == this.district)
                {
                    this.Position = new Rectangle(game.Graphics.Device.Viewport.Width - 398 + (i.Position.X - 398),
                           i.Position.Y, i.Position.Width, list.Height + i.Position.Height);
                    this.startposition = new Vector2(game.Graphics.Device.Viewport.Width - 398 + (i.StartPosition.X - 398),
                           i.StartPosition.Y);
                    foreach (OptionButtonDescription g in i.Buttons)
                    {
                        if (g.ButtonName == "Akzeptieren")
                        {
                            accept = new Button(game, g.ButtonName, startposition, g.height, g.width,
                                "OptionDialog\\" + g.ButtonImage, true, this);
                        }
                        else if (g.ButtonName == "Ablehnen")
                        {
                            decline = new Button(game, g.ButtonName, startposition, g.height, g.width,
                                "OptionDialog\\" + g.ButtonImage, true, this);
                        }
                    }
                }
            }
        }
        
     
       /* private void intializeButtons()
        {
            foreach (OptionpartDescription i in xml.OptionParts)
            {
                if (i.Number == this.district)
                {
                    this.space = i.Spacer;
                    this.startposition = new Vector2(game.Graphics.Device.Viewport.Width - 398 + (i.StartPosition.X - 398),
                              i.StartPosition.Y);
                    this.currentPosition = startposition;
                    foreach (TextureDescription g in i.Textures)
                    {
                        if (g.Name == "OptionButton")
                        {
                            foreach (string str in  this.Names) 
                            {
                                list.Add(new Button(game, str, this.currentPosition, g.Height, g.Width,
                                g.Image, true));
                                calculateDisplacement(false, (int)g.Height, (int)g.Width);
                            }
                        }
                    }

                }
            }

        }*/

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime, SpriteBatch Spritebatch)
        {
            //// TODO: Add your update code here
            base.Update(gameTime);
            foreach (Button g in list)
            {
                g.Update(gameTime, Spritebatch);
            }
            accept.Update(gameTime, Spritebatch);
            decline.Update(gameTime, Spritebatch);
        }

        //
        // Zusammenfassung:
        //     Called when the DrawableGameComponent needs to be drawn.  Override this method
        //     with component-specific drawing code.
        //
        // Parameter:
        //   gameTime:
        //     Time passed since the last call to Microsoft.Xna.Framework.DrawableGameComponent.Draw(Microsoft.Xna.Framework.GameTime).
        public void Draw(GameTime gameTime, SpriteBatch SpriteBatch)
        {
            if (SpriteBatch != null)
            {
                foreach (OptionpartDescription i in xml.OptionParts)
                {
                    if (i.Number == this.district)
                    {
                       int titelHeight=0,titelWidth=0, gapWidth=0, gapHeight=0;
                       foreach (TextureDescription g in i.Textures)
                       {
                           switch (g.Name)
                           {
                               case "Titel":
                                   titelHeight=g.Height;
                                   titelWidth = g.Width;
                                   break;
                               case "Gap":
                                   gapHeight = g.Height;
                                   gapWidth = g.Width;
                                   break;
                           }
                       }
                        SpriteBatch.Draw(titel,new Rectangle((int)startposition.X,(int)startposition.Y-titelHeight-gapHeight,titelWidth,titelHeight),Color.White);
                        foreach(Button g in list)
                        {
                            SpriteBatch.Draw(gap, new Rectangle(g.Position.X, g.Position.Y - gapHeight, gapWidth, gapHeight), Color.White);
                            g.Draw(gameTime, SpriteBatch);
                        }
                        this.drawDialog(gameTime, SpriteBatch, new Vector2(startposition.X,startposition.Y  + list.Height)-i.Spacer);
                        this.drawInfo(gameTime, SpriteBatch);
                    }
                }
            }
        }

        //
        // Summary:
        //     Called when the component needs to load graphics resources.  Override to
        //     load any component specific graphics resources.
        //
        // Parameters:
        //   loadAllContent:
        //     true if all graphics resources need to be loaded;false if only manual resources
        //     need to be loaded.
        public new void LoadContent()
        {
           font = game.Content.Load<SpriteFont>("Arial");
           foreach (OptionpartDescription i in xml.OptionParts)
           {
               if (i.Number == this.district)
               {
                   foreach (TextureDescription g in i.Textures)
                   {
                       switch (g.Name)
                       {
                           case "Titel":
                               titel = game.Content.Load<Texture2D>("Content\\Textures\\Menu\\OptionDialog\\" + g.Image);
                               break;
                           case "Gap":
                               gap = game.Content.Load<Texture2D>("Content\\Textures\\Menu\\OptionDialog\\" + g.Image);
                               break;
                           case "DialogTop":
                               dialogtop = game.Content.Load<Texture2D>("Content\\Textures\\Menu\\OptionDialog\\" + g.Image);
                               break;
                           case "DialogLeft":
                               dialogleft = game.Content.Load<Texture2D>("Content\\Textures\\Menu\\OptionDialog\\" + g.Image);
                               break;
                           case "DialogMid":
                               dialogmid = game.Content.Load<Texture2D>("Content\\Textures\\Menu\\OptionDialog\\" + g.Image);
                               break;
                           case "DialogRight":
                               dialogright = game.Content.Load<Texture2D>("Content\\Textures\\Menu\\OptionDialog\\" + g.Image);
                               break;
                           case "DialogBottom":
                               dialogbottom = game.Content.Load<Texture2D>("Content\\Textures\\Menu\\OptionDialog\\" + g.Image);
                               break;
                       }
                   }
               }
           }
           foreach (Button i in this.list)
           {
               i.LoadContent(game);
               if (OptMod.ContainsKey(i.getButtonName()))
               {
                   OptMod[i.getButtonName()].LoadContent(game);
               }
               else if (SlideMod.ContainsKey(i.getButtonName()))
               {
                   SlideMod[i.getButtonName()].LoadContent();
               }
           }
           accept.LoadContent(game);
           decline.LoadContent(game);
        }

        private void drawInfo(GameTime gameTime, SpriteBatch loader)
        {
            int counter = 0;
            foreach (Button i in list)
            {
                if (i.Visiblity)
                {            
                    i.Draw(gameTime, loader);
                    loader.DrawString(font, i.getButtonName(), calcMidLine(i.Position, true, i.getButtonName()), Color.White);

                        if (OptMod.ContainsKey(i.getButtonName()))
                        {
                            OptMod[i.getButtonName()].Draw(gameTime, loader);
                        }
                        else if (!OptMod.ContainsKey(i.getButtonName()))
                        {
                            if ( i.getButtonName().Equals("Playername"))                 
                            {
                                loader.DrawString(font, ""+ Data[i.getButtonName()],
                                    calcMidLine(i.Position, false, "" + Data[i.getButtonName()]), Color.White);
                            } 
                            else if (SlideMod.ContainsKey(i.getButtonName()))
                            {
                                SlideMod[i.getButtonName()].Draw(gameTime,loader);
                            }
                
                    }
                        //loader.DrawString(font, ""+ Data[i.getButtonName()], calcMidLine(i.Position, false), Color.White);
                        counter++;
                }
            }
        }

        private void drawDialog(GameTime gameTime, SpriteBatch loader, Vector2 Position)
        {
            if (loader != null)
            {
               
               foreach (OptionpartDescription i in xml.OptionParts)
               {
                   if (i.Number == this.district)
                   {
                       int[,] offset = new int[i.Textures.Count,2];
                       foreach (TextureDescription g in i.Textures)
                       {
                           switch (g.Name)
                           {
                               case "DialogTop":
                                   offset[0,0] = g.Width;
                                   offset[0,1] = g.Height;
                                   break;
                               case "DialogLeft":
                                   offset[1,0] = g.Width;
                                   offset[1,1] = g.Height;
                                   break;
                               case "DialogMid":
                                   offset[2,0] = g.Width;
                                   offset[2,1] = g.Height;
                                   break;
                               case "DialogRight":
                                   offset[3,0] = g.Width;
                                   offset[3,1] = g.Height;
                                   break;
                               case "DialogBottom":
                                   offset[4,0] = g.Width;
                                   offset[4,1] = g.Height;
                                   break;
                               case "Gap":
                                   offset[5,0] = g.Width;
                                   offset[5,1] = g.Height;
                                   break;
                               default:
                                   for (int tex = 6; tex<offset.GetLength(0);tex++) {
                                       if (offset[tex,0]==0 && offset[tex,1]==0) {
                                       offset[tex,0]=g.Width;
                                       offset[tex,1] = g.Height;
                                       break;
                                       }
                                   }
                                   break;                                   
                           }
                       }
                       
                       loader.Draw(this.gap, new Rectangle((int)Position.X,(int)( Position.Y), offset[5,0], offset[5,1]), Color.White);
                       loader.Draw(dialogtop, new Rectangle((int)Position.X,(int)( Position.Y+offset[5,1]), offset[0,0], offset[0,1]), Color.White);
                       loader.Draw(dialogleft, new Rectangle((int)Position.X,(int)( Position.Y+offset[5,1] + offset[0,1]), offset[1,0], offset[1,1]), Color.White);
                       decline.setPosition(new Vector2(Position.X+offset[1,0], Position.Y+offset[5,1] + offset[0,1]), decline.Width, decline.Height);
                       decline.Draw(gameTime,loader);
                       loader.Draw(dialogmid, new Rectangle((int)(Position.X+offset[1,0]+decline.Width),(int)( Position.Y+offset[5,1] + offset[0,1]), offset[2,0], offset[2,1]), Color.White);
                       accept.setPosition(new Vector2(Position.X + offset[1,0] + decline.Width + offset[2,0],Position.Y+offset[5,1] + offset[0,1]),accept.Width,accept.Height);
                       accept.Draw(gameTime,loader);
                       loader.Draw(dialogright, new Rectangle((int)(Position.X + offset[1,0] + decline.Width + offset[2,0]+accept.Width),(int)( Position.Y+offset[5,1]+ offset[0,1]), offset[3,0], offset[3,1]), Color.White);
                       loader.Draw(dialogbottom, new Rectangle((int)Position.X,(int)( Position.Y+offset[5,1]+ offset[0,1]+ offset[1,1]), offset[4,0], offset[4,1]), Color.White);

                           
                   }
               }
            }
        }
        /*
        private Vector2 calcMidLine(Rectangle Position, bool first)
        {
            float line = (Position.Height / 2);
            Vector2 fontsize = font.MeasureString("TESTSTRING");

            if (first)
            {
                return new Vector2(Position.X + 10, Position.Y + line - (fontsize.Y / 2));
            }
            else 
            { 
                float score = Position.Width * 75 / 100;
                return new Vector2(Position.X + score, Position.Y + line - (fontsize.Y / 2));
            }
         
        }*/
       
        private Vector2 calcMidLine(Rectangle Position,bool first, string output)
        {
            float line = (Position.Height / 2);
            Vector2 fontsize = font.MeasureString(output);
            if (first)
            {
                return new Vector2(Position.X + 10, Position.Y + line - (fontsize.Y / 2)+1);
            }
            else
            {          
                float score = Position.Width * 75 / 100;
                return new Vector2(Position.X + score - (fontsize.X / 2), Position.Y + line - (fontsize.Y / 2)+1);            
            }
        }

        /// <summary>
        /// Creates the needed OptionModulationButton
        /// </summary>
        /// <param name="but">The currentButton</param>
        /// <param name="Name">KeyValue for Data</param>
        /// <returns>needed OptionModulationButton</returns>
        private OptionModulationButton getOptionModulationButton(Button but, string Name)
        {
            Vector2 space = new Vector2(but.Width * 75 / 100, 0);
            Dictionary<string, object> dic = new Dictionary<string, object>();
            List<string> list= new List<string>();
            switch (Name)
            {
                #region Difficult
                case "Difficult":
                    {
                        dic.Add("Rooky", (int)DifficultEnum.leicht);
                        dic.Add("Standard", (int)DifficultEnum.mittel);
                        dic.Add("Elite", (int)DifficultEnum.schwer);
                        list.Add("Rooky");
                        list.Add("Standard");
                        list.Add("Elite");
                        switch ((int)Data[Name])
                        {
                            case (int)DifficultEnum.leicht:
                                {
                                    return new OptionModulationButton(game, but, space, 
                                        this.district, Name, dic, list, "Rooky");
                                }
                            case (int)DifficultEnum.mittel:
                                {
                                    return new OptionModulationButton(game, but, space, 
                                        this.district, Name, dic, list, "Standard");
                                }
                            case (int)DifficultEnum.schwer:
                                {
                                    return new OptionModulationButton(game, but, space, 
                                        this.district, Name, dic, list, "Elite");
                                }
                            default:
                                return null;
                        }
                    }
                #endregion

                #region Resolution
                case "Resolution":
                    {
                        dic.Add("800x600", "800x600");
                        dic.Add("1024x600", "1024x600");
                        dic.Add("1024x768", "1024x768");
                        dic.Add("1280x768", "1280x768");
                        dic.Add("1280x800", "1280x800");
                        dic.Add("1280x960", "1280x960");
                        dic.Add("1280x1024", "1280x1024");
                        dic.Add("1360x768", "1360x768");
                        dic.Add("1440x900", "1440x900");
                        dic.Add("1600x1200", "1600x1200");
                        dic.Add("1600x1280", "1600x1280");
                        dic.Add("1768x992", "1768x992");
                        dic.Add("1856x1392", "1856x1392");
                        dic.Add("1920x1200", "1920x1200");
                        list.Add("800x600");
                        list.Add("1024x600");
                        list.Add("1024x768");
                        list.Add("1280x768");
                        list.Add("1280x800");
                        list.Add("1280x960");
                        list.Add("1280x1024");
                        list.Add("1360x768");
                        list.Add("1440x900");
                        list.Add("1600x1200");
                        list.Add("1600x1280");
                        list.Add("1768x992");
                        list.Add("1856x1392");
                        list.Add("1920x1200");
                        return new OptionModulationButton(game, but, space,
                            this.district, Name, dic, list, (string)this.Data["Resolution"]);
                    }
                #endregion

                #region Shadow
                case "Shadow":
                    {
                        dic.Add("On", true);
                        dic.Add("Off", false);
                        list.Add("On");
                        list.Add("Off");
                        if ((bool)Data[Name])
                        {
                            return new OptionModulationButton(game, but, space,
                            this.district, Name, dic, list, "On");
                        }
                        else
                        {
                            return new OptionModulationButton(game, but, space,
                            this.district, Name, dic, list, "Off");
                        }
                    }
                #endregion

                #region Fog
                case "Fog":
                    {
                        dic.Add("On", true);
                        dic.Add("Off", false);
                        list.Add("On");
                        list.Add("Off");
                        if ((bool)Data[Name])
                        {
                            return new OptionModulationButton(game, but, space,
                            this.district, Name, dic, list, "On");
                        }
                        else
                        {
                            return new OptionModulationButton(game, but, space,
                            this.district, Name, dic, list, "Off");
                        }
                    }
                #endregion

                #region Fullscreen
                case "Fullscreen":
                    {
                        dic.Add("On", true);
                        dic.Add("Off", false);
                        list.Add("On");
                        list.Add("Off");
                        if ((bool)Data[Name])
                        {
                            return new OptionModulationButton(game, but, space,
                            this.district, Name, dic, list, "On");
                        }
                        else
                        {
                            return new OptionModulationButton(game, but, space,
                            this.district, Name, dic, list, "Off");
                        }
                    }
                #endregion

                #region Anti Aliasing
                case "Anti Aliasing":
                    {
                        dic.Add("Off", "0x");
                        dic.Add("2x", "2x");
                        dic.Add("4x", "4x");
                        dic.Add("8x", "8x");
                        dic.Add("16x", "16x");
                        list.Add("Off");
                        list.Add("2x");
                        list.Add("4x");
                        list.Add("8x");
                        list.Add("16x");
                        return new OptionModulationButton(game, but, space,
                            this.district, Name, dic, list, (string)Data[Name]);
                    }
                #endregion

                #region Sound
                case "Sound":
                    {
                        dic.Add("On", true);
                        dic.Add("Off", false);
                        list.Add("On");
                        list.Add("Off");
                        if ((bool)Data[Name])
                        {
                            return new OptionModulationButton(game, but, space,
                            this.district, Name, dic, list, "On");
                        }
                        else
                        {
                            return new OptionModulationButton(game, but, space,
                            this.district, Name, dic, list, "Off");
                        }                        
                    }
                #endregion
                default:
                    return null;
            }
        }

        private HSlideBar getSlideBar(Button but, string Name)
        {
            Vector2 space = new Vector2(but.Width * 45 / 100, 0);
            switch (Name)
            {
                case "Detail":
                    {
                       return new HSlideBar(game, but,new Vector2(but.Position.X+space.X,but.Position.Y+space.Y),0.0f,1.0f,
                           (float)Data[Name], 0.1f, 2, 10);                           
                    }
                case "Music":
                    {
                        return new HSlideBar(game, but, new Vector2(but.Position.X + space.X, but.Position.Y + space.Y), 
                            0.0f, 1.0f, (float)Data[Name], 0.05f, 5, 100);                            
                    }
                case "Effect":
                    {
                        return new HSlideBar(game, but, new Vector2(but.Position.X + space.X, but.Position.Y + space.Y), 
                            0.0f, 1.0f, (float)Data[Name], 0.05f, 5, 100);
                    }
                default:
                    return null;
                // STEUERUNG noch einfügen
            }

        }

         /*private void calculateDisplacement(bool horizont, int butheight, int butwidth)
        {
            // Ermittlung der Verschiebung !!!TODO: Verschiebung nach letztem bzw vor erstem heraus bekommen !!!
            if (horizont)
            {
                this.currentPosition.X = this.currentPosition.X + this.space.X + butwidth;
                if (butheight > this.height)
                {
                    this.height = (int)butheight;
                }

            }
            else
            {
                this.currentPosition.Y = this.currentPosition.Y + this.space.Y + butheight;
                if (butwidth > this.width)
                {
                    this.width = (int)butwidth;
                }
            }
        }//ENDE*/
    }
}



