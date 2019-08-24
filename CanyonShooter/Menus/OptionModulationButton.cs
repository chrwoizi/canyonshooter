using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using DistrictEnum = CanyonShooter.Menus.Option.DistrictEnum;

namespace CanyonShooter.Menus
{
    //Eventhändler
    public delegate void OptionModulationButtonEventHandler();

    /// <summary>
    /// Für jeden Button in der Buttonlist, also für jeden Button muss so ein Objekt erzeugt werden. 
    /// Name gibt den Schlüssel an. Damit kann ich besser steuern. Hochrunter is zeile auswählen und 
    /// links rechts wählt Plus minus aus. Enter funzt nur bei ausgewähltem Button. 
    /// Muss auch die Werte übergeben, denn muss bei true muss On stehen und bei false Off also muss hier 
    /// gezeichnet werden und im Dialog nur noch der Name des Attributs 
    /// </summary>
    /// 
    //Eventhändler
    public delegate void OptionEventHandler();
    class OptionModulationButton
    {
        #region static Data Members
        private static Vector2 _UNITPOS = new Vector2();
        private static Vector2 _PLUSPOS = new Vector2(5,4); // nur noch von Rechte Seite Status aus
        private static Vector2 _MINUSPOS = new Vector2(24, 4); // nur noch von Rechte Seite Status aus  Vector2(30,4)
        private static Vector2 _STATUSPOS = new Vector2(0, 4); //X-Koordinate nicht benutzt
        private const int _STATUSWIDTH = 60;
        private const int _STATUSHEIGHT = 30;
        private const int _TEXTOFFSET = 10;
        #endregion
        #region Data Members
        private ICanyonShooterGame game;
        
        private int district;
        private string name;
        
        private Vector2 Position;
        private Rectangle StatusPosition;

        private int StatusWidth;
        private int ButtonHighforContent;

        public Button plus;
        public Button minus;
        private Texture2D status;
        private SpriteFont font;

        private string currentString;
        private int currentNumber;
        private Dictionary<string, object> Values;
        private List<string> Names;

        private Button Caller;
        #endregion

        #region Properties
        public string Name
        {
            get { return name; }
        }
        #endregion


        /// <summary>
        /// Constructor
        /// </summary>


        public OptionModulationButton(ICanyonShooterGame game, Button but,Vector2 pos, int district, 
            string name, Dictionary<string,object> allowValues, List<string> Names, string StartValue)
        {
            this.game = game;
            this.district = district;
            this.name = name;
            this.Values = allowValues;
            this.Position = pos;
            this.Names = Names;
            this.Caller = but;

            // Determine the longest string to adjust the statusfield behind
            font = game.Content.Load<SpriteFont>("Arial"); 
            float longestWidth = 0;
            for (int i = 0; i<Names.Count; i++)
            {
                if (font.MeasureString(Names[i]).X > longestWidth)
                {
                    longestWidth = font.MeasureString(Names[i]).X;
                }
            }
            this.StatusWidth = (int)Math.Ceiling(longestWidth + 10)+1;
            this.StatusPosition = new Rectangle((int)(but.getPosition().Left + Position.X -(StatusWidth / 2)), 
                (int)(but.getPosition().Top + Position.Y + _STATUSPOS.Y), (int)StatusWidth, Caller.Position.Height - 8); 

            this.currentString = StartValue;
            for (int i=0; i<Names.Count; i++) 
            {
                if (Names[i].Equals(StartValue) )
                {
                    currentNumber = i;
                }
            }

            this.ButtonHighforContent =  (int) Math.Ceiling((double)(but.Height - 8));
            plus = new Button(game, name + "_plus", new Vector2(but.getPosition().Left + Position.X + (StatusWidth / 2) + _PLUSPOS.X,
                but.getPosition().Top + Position.Y + _PLUSPOS.Y), this.ButtonHighforContent, this.ButtonHighforContent,
                "butplus", true, this);
            plus.Buttonpressed += new ActionEventHandler(Pluspressed);    
            minus = new Button(game, name + "_minus", new Vector2(but.getPosition().Left + Position.X -(StatusWidth/2) - _MINUSPOS.X,
                but.getPosition().Top + Position.Y + _MINUSPOS.Y), this.ButtonHighforContent, this.ButtonHighforContent,
                "butminus", true, this);
            minus.Buttonpressed += new ActionEventHandler(Minuspressed);   
            
        }

        /// <summary>
        /// Draw the Buttons
        /// </summary>
        /// <param name="loader">Spritebatch to draw with</param>
        public void Draw(GameTime gametime,SpriteBatch loader)
        {
            plus.Draw(gametime, loader);
            minus.Draw(gametime, loader);
            DrawState(gametime, loader);
        }

        /// <summary>
        /// Draw the current state
        /// </summary>
        /// <param name="loader">Spritebatch to draw with</param>
        public void DrawState(GameTime gametime, SpriteBatch loader)
        {
            loader.Draw(status,StatusPosition , Color.White);
            loader.DrawString(font,""+ Names[currentNumber], calcMidLine(StatusPosition),Color.White);
        }
        
        /// <summary>
        /// Load the Content which to draw
        /// </summary>
        public void LoadContent(ICanyonShooterGame game)
        {
            plus.LoadContent(this.game);
            minus.LoadContent(this.game);
            status = game.Content.Load<Texture2D>("Content\\Textures\\Menu\\status");
            if (font == null)
            {
                font = game.Content.Load<SpriteFont>("Arial");
            }
        }

        /// <summary>
        /// Update the OptionModulationButtons
        /// </summary>
        public void Update(GameTime gametime)
        {
            plus.setPosition( new Rectangle((int)(Caller.getPosition().Left + Position.X + (StatusWidth/2) + _PLUSPOS.X),
                (int)(Caller.getPosition().Top + Position.Y +  _PLUSPOS.Y),(int) this.ButtonHighforContent,(int) this.ButtonHighforContent));
            minus.setPosition( new Rectangle((int)(Caller.getPosition().Left + Position.X -(StatusWidth/2) - _MINUSPOS.X),
                (int)(Caller.getPosition().Top + Position.Y + _MINUSPOS.Y), (int)this.ButtonHighforContent, (int)this.ButtonHighforContent));
            this.StatusPosition = new Rectangle((int)(Caller.getPosition().Left + Position.X - (StatusWidth / 2)),
                (int)(Caller.getPosition().Top + Position.Y + _STATUSPOS.Y)
                ,(int)StatusWidth,(int) this.ButtonHighforContent); 
            plus.Update(gametime);
            minus.Update(gametime);

            // EVT noch DRAW METHODE AUFRUFEN
        }

        private Vector2 calcMidLine(Rectangle Position)
        {
            float line = (int) Math.Ceiling((double)(Position.Height / 2));

            Vector2 fontsize = font.MeasureString(Names[currentNumber]);
            //int midofside = ((Caller.Position.Height - 8) * (_STATUSWIDTH / _STATUSHEIGHT)) / 2;

            return new Vector2(Position.X + (Position.Width/2) - (fontsize.X/2), Position.Y + line +1 - 
                (fontsize.Y/2));
        }

        public void Pluspressed()
        {
            if (currentNumber != (Names.Count - 1))
            {
                currentNumber++;
                currentString = Names[currentNumber];
            }
            else
            {
                currentNumber=0;
                currentString = Names[currentNumber];
            }
            OptionList call = (OptionList)Caller.caller;
            call.changed(name, true,false); 
        }

        public void Minuspressed()
        {
            if (currentNumber != 0)
            {
                currentNumber--;
                currentString = Names[currentNumber];
            }
            else
            {
                currentNumber = (Names.Count - 1);
                currentString = Names[currentNumber];
            }
            OptionList call = (OptionList)Caller.caller;
            call.changed(name, false, false);         
        }

        public object returnCurrentValue()
        {
            return Values[currentString];
        }

        public void setCurrentString(string value)
        {
            currentString = value;
            for (int i=0;i<Names.Count;i++)
            {
                if(Names[i]==value)
                {
                    currentNumber = i;
                }
            }
        }
    }
}
