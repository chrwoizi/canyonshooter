using System;
using System.Collections.Generic;
using CanyonShooter.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CanyonShooter.GameClasses.Console
{
    class GraphicalConsole : DrawableGameComponent, IGameComponent, IDrawable, IGameConsole
    {
        #region Fields

        private static GraphicalConsole self = null;

        private ICanyonShooterGame game;

        private SpriteBatch spriteBatch;
        private SpriteFont font;

        private GameConsole gameConsole;

        private Texture2D background;

        private float propFontScale = 1.0f;
        private string propName;

        private bool visible;
        private string currentInput = "";
        private string[] Suggestions;
        private int chosenSuggestion;

        private int currentScrolling = 0;

        Queue<string>[] ConsoleHistory;
 

        #endregion

        #region Properties

        public string Name
        {
            get
            {
                return this.propName;
            }
            set
            {
                this.propName = value;
            }
        }

        public float FontScale
        {
            get
            {
                return propFontScale;
            }
            set
            {
                this.propFontScale = value;
            }
        }

        #endregion
        
        private GraphicalConsole(ICanyonShooterGame game, string name)
            : base(game as Game)
        {
            this.Name = name;
            this.DrawOrder = (int)DrawOrderType.Console;
            this.game = game;

            this.gameConsole = new GameConsole(true);
            this.ConsoleHistory = new Queue<string>[2];
            this.ConsoleHistory[0] = new Queue<string>();
            this.ConsoleHistory[1] = new Queue<string>();

            this.chosenSuggestion = 0;
            this.Suggestions = new string[0];

            this.visible = false;
            gameConsole.RegisterObjectProperty(this, "Console", "FontScale");
        }

        #region Drawable
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (game.Input.HasKeyJustBeenReleased("Console.Toggle"))
                visible = (visible) ? false : true;

            if (game.Input.HasKeyJustBeenReleased("Console.ScrollingDown") && currentScrolling > 0)
                currentScrolling--;
            if (game.Input.HasKeyJustBeenReleased("Console.ScrollingUp") && currentScrolling < ConsoleHistory[0].Count-20)
                currentScrolling++;
            if (visible)
            {
                if (game.Input.HasKeyJustBeenReleased("Console.Enter"))
                {
                    if ((Suggestions.Length != 0) && (currentInput != Suggestions[chosenSuggestion]))
                    {
                        currentInput = Suggestions[chosenSuggestion];
                    }
                    else
	                {
                        ConsoleHistory[0].Enqueue(currentInput);
                        string result = gameConsole.Execute(currentInput);
                        foreach (string s in result.Split(new Char[] { '\n'}))
                            ConsoleHistory[0].Enqueue(s);
                        currentInput = "";
                        Suggestions = gameConsole.GetSuggestions("");
                        chosenSuggestion = 0;
	                }
                }

                if (game.Input.HasKeyJustBeenReleased("Console.Back"))
                {
                    if (currentInput.Length >=1)
                        currentInput = currentInput.Substring(0, currentInput.Length - 1);
                    Suggestions = gameConsole.GetSuggestions(currentInput);
                }
                if (game.Input.HasKeyJustBeenReleased("Console.NextSuggestion"))
                {
                    if (Suggestions.Length != 0)
                    {
                        chosenSuggestion++;
                        chosenSuggestion %= Suggestions.Length;
                    }
                }
                
                string text = "";

                game.Input.HandleKeyboardInput( ref text);
                if (text.Length != 0)
                {
                    this.currentInput += text;
                    Suggestions = gameConsole.GetSuggestions(currentInput);
                    if (chosenSuggestion >= Suggestions.Length)
                        chosenSuggestion = 0;
                }
            }
           
            // Remove unvisible commands
            //while (commands.Count >= 20)
            //    commands.Dequeue();
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.SaveState);

            if (visible && spriteBatch != null)
            {
                spriteBatch.Draw(background, new Rectangle(0, 0, 800, 300), new Color(0, 0, 0, 225));
                float i = 300 - ((font.MeasureString("I").Y * FontScale) - 4) * ((ConsoleHistory[0].Count > 20) ? 21 : ConsoleHistory[0].Count + 1);
                int numCommands = 0;
                foreach (string str in ConsoleHistory[0])
                {
                    numCommands++;
                    if ((numCommands <= ConsoleHistory[0].Count - 20 - currentScrolling) ||
                        (numCommands > ConsoleHistory[0].Count - currentScrolling))
                        continue;
                    spriteBatch.DrawString(font, str, new Vector2(10, i), Color.White, 0, new Vector2(0, 0), FontScale, SpriteEffects.None, 0);
                    i +=  (font.MeasureString("I").Y * FontScale)-4;
                }
                // Aktuelle Eingabe
                spriteBatch.DrawString(font, currentInput, new Vector2(10, i), Color.Red, 0, new Vector2(0, 0), FontScale, SpriteEffects.None, 0);
                i += (font.MeasureString("I").Y * FontScale) - 4;
                float j = 50;
                int k = 0;
                foreach (string str in Suggestions)
                {
                    Vector2 measure = font.MeasureString(str + " | ");
                    Vector2 measure2 = font.MeasureString(" | ") * 0.5f;
                    spriteBatch.Draw(background, new Rectangle((int)(j - measure2.X), (int)i, (int)(measure.X), (int)(measure.Y)), new Color(0, 0, 127, 225));
                    spriteBatch.DrawString(font, str, new Vector2(j, i), (k == chosenSuggestion) ? Color.Beige : Color.Brown, 0, new Vector2(0, 0), FontScale, SpriteEffects.None, 0);
                    j += (measure.X * FontScale);
                    k++;
                }
            }
            else if (!visible && spriteBatch != null)
            {
                //spriteBatch.Draw(background, new Rectangle(0, 0, 800, 75), new Color(75, 75, 255, 128));
                float ii = 75 - ((font.MeasureString("I").Y * FontScale) - 4) * ((ConsoleHistory[1].Count > 5) ? 6 : ConsoleHistory[1].Count + 1);
                int numGameCommands = 0;
                foreach (string str in ConsoleHistory[1])
                {
                    numGameCommands++;
                    if (numGameCommands <= ConsoleHistory[1].Count - 5)
                        continue;
                    spriteBatch.DrawString(font, str, new Vector2(10, ii), Color.White, 0, new Vector2(0, 0), FontScale, SpriteEffects.None, 0);
                    ii += (font.MeasureString("I").Y * FontScale) - 4;
                }
            }
            spriteBatch.End();
        }


        //
        // Zusammenfassung:
        //     Called when the component needs to load graphics resources.  Override this
        //     method to load any component-specific graphics resources.
        //
        // Parameter:
        //   loadAllContent:
        //     true if all graphics resources need to be loaded; false if only manual resources
        //     need to be loaded.
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(game.Graphics.Device);

            font = game.Content.Load<SpriteFont>("Courier");
            background = game.Content.Load<Texture2D>("Content\\Textures\\Console1");
        }

        #endregion

        #region IGraphicalConsole

        public static GraphicalConsole GetSingleton(ICanyonShooterGame game)
        {
            if (self == null)
                self = new GraphicalConsole(game, "Console");

            return self;
        }

        public bool RegisterObjectFunction(object functionOwner, string objectName, string functionName)
        {
            return gameConsole.RegisterObjectFunction(functionOwner, objectName, functionName);
        }

        public bool RegisterObjectFunction(object functionOwner, string objectName, string functionName, Type[] parameterTypes)
        {
            return gameConsole.RegisterObjectFunction(functionOwner, objectName, functionName, parameterTypes);
        }

        public bool RegisterObjectProperty(object propertyOwner, string objectName, string propertyName)
        {
            return gameConsole.RegisterObjectProperty(propertyOwner, objectName, propertyName);
        }

        public bool RegisterStaticFunction(Type classType, string className, string functionName)
        {
            return gameConsole.RegisterStaticFunction(classType, className, functionName);
        }

        public bool RegisterStaticFunction(Type classType, string className, string functionName, Type[] parameterTypes)
        {
            return gameConsole.RegisterStaticFunction(classType, className, functionName, parameterTypes);
        }

        public bool RegisterStaticProperty(Type classType, string className, string propertyName)
        {
            return gameConsole.RegisterStaticProperty(classType, className, propertyName);
        }

        public bool RegisterTypeParser(GameConsoleTypeParser typeParser)
        {
            return gameConsole.RegisterTypeParser(typeParser);
        }

         /// <summary>
         /// adds a line to console
         /// </summary>
         /// <param name="value">string to add</param>
         /// <param name="console">destination console, 0 for debug, 1 for gameinfo</param>
         /// <returns></returns>
        public bool WriteLine(string value, int console)
        {
            if (console < ConsoleHistory.Length)
            {
                ConsoleHistory[console].Enqueue(value);
                return true;
            }
            return false;
        }
        #endregion
    }
}
