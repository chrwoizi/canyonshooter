#region using
using System;
using System.Collections.Generic;
using DescriptionLibs.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace CanyonShooter.Menus
{
    class OptionList : ButtonList
        {
        private object caller;
        private Vector2 space;
        private Vector2 Position;
        private Vector2 currentPosition;
        private int width = 0;
        private Rectangle BoundingBox;
        private int district;

        private Dictionary<string, object> Data;
        private Dictionary<string, string> Format;
        private List<string> Names;
        private Dictionary<string, int> changes;
        private const int INF = 99999999;

        public OptionDescription xml;

      
        public OptionList(ICanyonShooterGame game, OptionDescription xml, int district, Vector2 StartPosition, Dictionary<string, object> Data,Dictionary<string, string> Format, List<string> Names, bool hori,object caller)
            : base(game as ICanyonShooterGame)
        {
            this.caller = caller;
            this.xml = xml;
            this.Data = Data;
            this.Format = Format;
            this.Names = Names;
            this.startposition = StartPosition;
            this.district = district;
            this.height = 0;
            init("", hori);

            changes = new Dictionary<string, int>();
            this.setChanges();
            
        }

        public OptionList(ICanyonShooterGame game, OptionDescription xml, int district, Dictionary<string, object> Data, Dictionary<string, string> Format, List<string> Names, bool hori, object caller)
            : base(game as ICanyonShooterGame)
        {
            this.caller = caller;
            this.xml = xml;
            this.Data = Data;
            this.Format = Format;
            this.Names = Names;
            this.district = district;
            this.height = 0;
            init("", hori);

            changes = new Dictionary<string, int>();
            this.setChanges(); 
        }


        public override void init(string disctrict, bool hori)
        {
            this.Clear();
            foreach (OptionpartDescription i in xml.OptionParts)
            {
                if (i.Number == this.district)
                {
                    this.space = i.Spacer;
                    if (this.startposition.Equals(new Vector2()))
                    {
                        this.Position = new Vector2(game.Graphics.Device.Viewport.Width - 398 + (i.StartPosition.X - 398),
                              i.StartPosition.Y);
                    }
                    else
                    {
                        this.Position = this.startposition;
                    }
                    this.currentPosition = Position;
                    foreach (TextureDescription g in i.Textures)
                    {
                        if (g.Name == "OptionButton")
                        {
                            foreach (string str in this.Names)
                            {
                                if (this.Count == 0)
                                {
                                    this.AddFirst(new Button(game, str, this.currentPosition, g.Height, g.Width,
                                    "OptionDialog\\" + g.Image, true, this));
                                }
                                else
                                {
                                    this.AddLast(new Button(game, str, this.currentPosition, g.Height, g.Width,
                                    "OptionDialog\\" + g.Image, true, this));
                                }
                                calculateDisplacement(false, (int)g.Height, (int)g.Width);
                            }
                        }
                    }

                }
            }
            if (this.Count != 0)
            {
                currentNode = this.First;
                currentNode.Value.SetActive();
            }
            this.height = (int)(this.currentPosition.Y - this.Position.Y);
        }

        
        private void calculateDisplacement(bool horizont, int butheight, int butwidth)
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
        }//ENDE

        #region Chnages
        public void changed(string Name, bool up, bool inf)
        {  
            if (!inf)
            {
                if (up)
                {
                    changes[Name]++;
                }
                else
                {
                    changes[Name]--;
                }
            }
            else
            {
                changes[Name] = INF;
            }
        }      
            
        

        public bool controlChanges()
        {
            foreach (string i in changes.Keys)
            {
                if (changes[i] != 0)
                {
                    return true;
                }
            }
            return false;
        }

        private void setChanges()
        {
            this.changes = new Dictionary<string, int>();
            foreach (string i in Names)
            {
                changes.Add(i, 0);
            }
        }
        #endregion 
    }
}
