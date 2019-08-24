
#region Using Statements

using DescriptionLibs.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace CanyonShooter.Menus
{
    /// <summary>
    /// Diese Klasse soll zeigen, dass die Highscore-Liste in der Komponente Highscore gespeichert werden 
    /// soll und durch die Klasse HighscoreMenu angeteigt wird.
    /// Ob diese Klasse von Menu erbt oder eine eigenständige GameComponent ist, bleibt den zuständigen
    /// Personen überlassen.
    /// </summary>
    public class HighscoreMenu : DrawableGameComponent
    {
        #region DataMembers

        private ICanyonShooterGame game;
        private Listbox Scorelist;
        private MenuDescription xml;
        private string currentmenu;

        private MenuControl steuerung;

        private bool active;

        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        public int Difficult
        {
            get { return this.Scorelist.Difficulty;}
        }

        bool visible;

        public bool Visiblity
        {
            get { return visible; }
            set { visible = value; }
        }

        #region Draw
        private Texture2D background;
        private Texture2D mouse;
        #endregion

        #endregion

        // Konstuktor
        // Dieser Konstruktor kann von außen nicht erreicht werden.
        public HighscoreMenu(ICanyonShooterGame game, string MenuName, int difficulty)
            : base(game as Game)
        {
            this.game = game;
            this.visible = true;
            this.Active = true;
            this.xml = game.Content.Load<MenuDescription>("Content\\Menu\\Hauptmenu");
            this.currentmenu = MenuName;
            foreach (MenupartDescription i in xml.MenuParts)
            {
                if (i.Name == MenuName)
                {
                    this.Scorelist = new Listbox(game, new Vector2(i.MenuPosition.X, i.MenuPosition.Y), true, xml, difficulty);
                }
            }
            steuerung = new MenuControl((Menu)game.GameStates.Menu, game);
        }


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
            base.Update(gameTime);
            Scorelist.Update(gameTime, Spritebatch);

            if (Active)
            {
                if (this.Scorelist.Buttons.Count == 2)
                {
                    #region Mouse
                    //Auswahl tätigen
                    if (isMouseInBox(this.Scorelist.Buttons.First.Value) &&
                        new Vector2(game.Input.MousePosition.X, game.Input.MousePosition.Y) != game.GameStates.Menu.Mouse)
                    {
                        this.Scorelist.Buttons.First.Value.SetActive();
                        this.Scorelist.Buttons.Last.Value.resetActive();
                    }
                    else if (isMouseInBox(this.Scorelist.Buttons.Last.Value) &&
                        new Vector2(game.Input.MousePosition.X, game.Input.MousePosition.Y) != game.GameStates.Menu.Mouse)
                    {
                        this.Scorelist.Buttons.Last.Value.SetActive();
                        this.Scorelist.Buttons.First.Value.resetActive();
                    }

                    if (game.Input.HasKeyJustBeenPressed("Menu.Select"))
                    {
                        if (isMouseInBox(this.Scorelist.Buttons.First.Value) && Scorelist.Buttons.First.Value.GetActive())
                        {
                            steuerung.selection(this.Scorelist.Buttons.First.Value.getButtonName(), "Highscore");
                        }
                        if (isMouseInBox(this.Scorelist.Buttons.Last.Value) && Scorelist.Buttons.Last.Value.GetActive())
                        {
                            steuerung.selection(this.Scorelist.Buttons.Last.Value.getButtonName(), "Highscore");
                        }
                    }

                    #endregion

                    #region Keyboard
                    if (game.Input.HasKeyJustBeenPressed("Menu.Left"))
                    {
                        if (this.Scorelist.Buttons.First.Value.GetActive())
                        {
                            this.Scorelist.Buttons.First.Value.resetActive();
                            this.Scorelist.Buttons.Last.Value.SetActive();
                        }
                        else
                        {
                            this.Scorelist.Buttons.First.Value.SetActive();
                            this.Scorelist.Buttons.Last.Value.resetActive();
                        }
                    }
                    else if (game.Input.HasKeyJustBeenPressed("Menu.Right"))
                    {
                        if (this.Scorelist.Buttons.First.Value.GetActive())
                        {
                            this.Scorelist.Buttons.First.Value.resetActive();
                            this.Scorelist.Buttons.Last.Value.SetActive();
                        }
                        else
                        {
                            this.Scorelist.Buttons.First.Value.SetActive();
                            this.Scorelist.Buttons.Last.Value.resetActive();
                        }
                    }
                    else if (game.Input.HasKeyJustBeenPressed("Menu.Ok"))
                    {
                        if (this.Scorelist.Buttons.First.Value.GetActive())
                        {
                            steuerung.selection(this.Scorelist.Buttons.First.Value.getButtonName(), "Highscore");
                        }
                        else
                        {
                            steuerung.selection(this.Scorelist.Buttons.Last.Value.getButtonName(), "Highscore");
                        }
                    }
                    else if (game.Input.HasKeyJustBeenPressed("Game.Menu"))
                    {
                        steuerung.BackHighscore();
                    }
                    #endregion
                }
            }
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
                foreach (MenupartDescription i in xml.MenuParts)
                {
                    if (i.Name.Equals(currentmenu))
                    {
                        SpriteBatch.Draw(background, new Rectangle(0, 0,
                            game.Graphics.Device.Viewport.Width, game.Graphics.Device.Viewport.Height),
                            Color.White);
                    }
                }
                Scorelist.Draw(gameTime, SpriteBatch);
                SpriteBatch.Draw(mouse, new Vector2(game.Input.MousePosition.X,
                    game.Input.MousePosition.Y), Color.White);
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
            Scorelist.LoadContent(game);
            foreach (MenupartDescription i in xml.MenuParts)
            {
                if (i.Name.Equals(currentmenu))
                {
                    background = game.Content.Load<Texture2D>("Content\\Textures\\Menu\\" + i.BackgroundImage);
                    mouse = game.Content.Load<Texture2D>("Content\\Textures\\Menu\\" + i.Mouse);
                }
            }
        }

        public bool isMouseInBox(Button Check)
        {
            if (this.Visiblity)
            {
                if ((game.GameStates.Menu.MousePosition.X >= Check.getPosition().X) &&
                    (game.GameStates.Menu.MousePosition.X <= Check.getPosition().Right) &&
                    (game.GameStates.Menu.MousePosition.Y >= Check.getPosition().Y) &&
                    (game.GameStates.Menu.MousePosition.Y <= Check.getPosition().Bottom))
                {
                    return true;
                }
            }
            return false;
        }

    }
}


