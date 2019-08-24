// Zuständigkeit: ????

#region Using directives

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

#endregion

namespace CanyonShooter.Engine.Input
{
    /// <summary>
    /// Input helper class, captures all the mouse, keyboard and XBox 360
    /// controller input and provides some nice helper methods and properties.
    /// Will also keep track of the last frame states for comparison if
    /// a button was just pressed this frame, but not already in the last frame.
    /// </summary>
    public class Input : IInput
    {

        #region Als Beispiel für die Implementierung

        #region GamePad

        /*
        /// <summary>
        /// GamePad state, set every frame in the Update method.
        /// </summary>
        private GamePadState gamePadState,
            gamePadStateLastFrame;

        /// <summary>
        /// Start dragging pos, will be set when we just pressed the left
        /// mouse button. Used for the MouseDraggingAmount property.
        /// </summary>
        private Point startDraggingPos;

		/// <summary>
		/// Game pad
		/// </summary>
		/// <returns>Game pad state</returns>
		public GamePadState GamePad
		{
			get
			{
				return gamePadState;
			} // get
		} // GamePad

		/// <summary>
		/// Is game pad connected
		/// </summary>
		/// <returns>Bool</returns>
		public bool IsGamePadConnected
		{
			get
			{
				return gamePadState.IsConnected;
			} // get
		} // IsGamePadConnected

		/// <summary>
		/// Game pad start pressed
		/// </summary>
		/// <returns>Bool</returns>
		public bool GamePadStartPressed
		{
			get
			{
				return gamePadState.Buttons.Start == ButtonState.Pressed;
			} // get
		} // GamePadStartPressed

		/// <summary>
		/// Game pad a pressed
		/// </summary>
		/// <returns>Bool</returns>
		public bool GamePadAPressed
		{
			get
			{
				return gamePadState.Buttons.A == ButtonState.Pressed;
			} // get
		} // GamePadAPressed

		/// <summary>
		/// Game pad b pressed
		/// </summary>
		/// <returns>Bool</returns>
		public bool GamePadBPressed
		{
			get
			{
				return gamePadState.Buttons.B == ButtonState.Pressed;
			} // get
		} // GamePadBPressed

		/// <summary>
		/// Game pad x pressed
		/// </summary>
		/// <returns>Bool</returns>
		public bool GamePadXPressed
		{
			get
			{
				return gamePadState.Buttons.X == ButtonState.Pressed;
			} // get
		} // GamePadXPressed

		/// <summary>
		/// Game pad y pressed
		/// </summary>
		/// <returns>Bool</returns>
		public bool GamePadYPressed
		{
			get
			{
				return gamePadState.Buttons.Y == ButtonState.Pressed;
			} // get
		} // GamePadYPressed

		/// <summary>
		/// Game pad left pressed
		/// </summary>
		/// <returns>Bool</returns>
		public bool GamePadLeftPressed
		{
			get
			{
				return gamePadState.DPad.Left == ButtonState.Pressed ||
					gamePadState.ThumbSticks.Left.X < -0.75f;
			} // get
		} // GamePadLeftPressed

		/// <summary>
		/// Game pad right pressed
		/// </summary>
		/// <returns>Bool</returns>
		public bool GamePadRightPressed
		{
			get
			{
				return gamePadState.DPad.Left == ButtonState.Pressed ||
					gamePadState.ThumbSticks.Left.X > 0.75f;
			} // get
		} // GamePadRightPressed

		/// <summary>
		/// Game pad left just pressed
		/// </summary>
		/// <returns>Bool</returns>
		public bool GamePadLeftJustPressed
		{
			get
			{
				return (gamePadState.DPad.Left == ButtonState.Pressed &&
					gamePadStateLastFrame.DPad.Left == ButtonState.Released) ||
					(gamePadState.ThumbSticks.Left.X < -0.75f &&
					gamePadStateLastFrame.ThumbSticks.Left.X > -0.75f);
			} // get
		} // GamePadLeftJustPressed

		/// <summary>
		/// Game pad right just pressed
		/// </summary>
		/// <returns>Bool</returns>
		public bool GamePadRightJustPressed
		{
			get
			{
				return (gamePadState.DPad.Right == ButtonState.Pressed &&
					gamePadStateLastFrame.DPad.Right == ButtonState.Released) ||
					(gamePadState.ThumbSticks.Left.X > 0.75f &&
					gamePadStateLastFrame.ThumbSticks.Left.X < 0.75f);
			} // get
		} // GamePadRightJustPressed

		/// <summary>
		/// Game pad up just pressed
		/// </summary>
		/// <returns>Bool</returns>
		public bool GamePadUpJustPressed
		{
			get
			{
				return (gamePadState.DPad.Up == ButtonState.Pressed &&
					gamePadStateLastFrame.DPad.Up == ButtonState.Released) ||
					(gamePadState.ThumbSticks.Left.Y > 0.75f &&
					gamePadStateLastFrame.ThumbSticks.Left.Y < 0.75f);
			} // get
		} // GamePadUpJustPressed

		/// <summary>
		/// Game pad down just pressed
		/// </summary>
		/// <returns>Bool</returns>
		public bool GamePadDownJustPressed
		{
			get
			{
				return (gamePadState.DPad.Down == ButtonState.Pressed &&
					gamePadStateLastFrame.DPad.Down == ButtonState.Released) ||
					(gamePadState.ThumbSticks.Left.Y < -0.75f &&
					gamePadStateLastFrame.ThumbSticks.Left.Y > -0.75f);
			} // get
		} // GamePadDownJustPressed
		
		/// <summary>
		/// Game pad up pressed
		/// </summary>
		/// <returns>Bool</returns>
		public bool GamePadUpPressed
		{
			get
			{
				return gamePadState.DPad.Down == ButtonState.Pressed ||
					gamePadState.ThumbSticks.Left.Y > 0.75f;
			} // get
		} // GamePadUpPressed

		/// <summary>
		/// Game pad down pressed
		/// </summary>
		/// <returns>Bool</returns>
		public bool GamePadDownPressed
		{
			get
			{
				return gamePadState.DPad.Up == ButtonState.Pressed ||
					gamePadState.ThumbSticks.Left.Y < -0.75f;
			} // get
		} // GamePadDownPressed

		/// <summary>
		/// Game pad a just pressed
		/// </summary>
		/// <returns>Bool</returns>
		public bool GamePadAJustPressed
		{
			get
			{
				return gamePadState.Buttons.A == ButtonState.Pressed &&
					gamePadStateLastFrame.Buttons.A == ButtonState.Released;
			} // get
		} // GamePadAJustPressed

		/// <summary>
		/// Game pad b just pressed
		/// </summary>
		/// <returns>Bool</returns>
		public bool GamePadBJustPressed
		{
			get
			{
				return gamePadState.Buttons.B == ButtonState.Pressed &&
					gamePadStateLastFrame.Buttons.B == ButtonState.Released;
			} // get
		} // GamePadBJustPressed
		
		/// <summary>
		/// Game pad x just pressed
		/// </summary>
		/// <returns>Bool</returns>
		public bool GamePadXJustPressed
		{
			get
			{
				return gamePadState.Buttons.X == ButtonState.Pressed &&
					gamePadStateLastFrame.Buttons.X == ButtonState.Released;
			} // get
		} // GamePadXJustPressed

		/// <summary>
		/// Game pad y just pressed
		/// </summary>
		/// <returns>Bool</returns>
		public bool GamePadYJustPressed
		{
			get
			{
				return gamePadState.Buttons.Y == ButtonState.Pressed &&
					gamePadStateLastFrame.Buttons.Y == ButtonState.Released;
			} // get
		} // GamePadYJustPressed

		/// <summary>
		/// Game pad back just pressed
		/// </summary>
		/// <returns>Bool</returns>
		public bool GamePadBackJustPressed
		{
			get
			{
				return gamePadState.Buttons.Back == ButtonState.Pressed &&
					gamePadStateLastFrame.Buttons.Back == ButtonState.Released;
			} // get
		} // GamePadBackJustPressed
*/
        #endregion

        #region Keyboard

        
        private bool IsSpecialKey(Keys key)
        {
            // All keys except A-Z, 0-9 and `-\[];',./= (and space) are special keys.
            // With shift pressed this also results in this keys:
            // ~_|{}:"<>? !@#$%^&*().
            int keyNum = (int)key;
            if ((keyNum >= (int)Keys.A && keyNum <= (int)Keys.Z) ||
                (keyNum >= (int)Keys.D0 && keyNum <= (int)Keys.D9) ||
                key == Keys.Space || // well, space ^^
                key == Keys.OemTilde || // `~
                key == Keys.OemMinus || // -_
                key == Keys.OemPipe || // \|
                key == Keys.OemOpenBrackets || // [{
                key == Keys.OemCloseBrackets || // ]}
                key == Keys.OemSemicolon || // ;:
                key == Keys.OemQuotes || // '"
                key == Keys.OemComma || // ,<
                key == Keys.OemPeriod || // .>
                key == Keys.OemQuestion || // /?
                key == Keys.OemPlus) // =+
                return false;

            // Else is is a special key
            return true;
        } // static bool IsSpecialKey(Keys key)
 
        /// <summary>
        /// Key to char helper conversion method.
        /// Note: If the keys are mapped other than on a default QWERTY
        /// keyboard, this method will not work properly. Most keyboards
        /// will return the same for A-Z and 0-9, but the special keys
        /// might be different. Sorry, no easy way to fix this with XNA ...
        /// For a game with chat (windows) you should implement the
        /// windows events for catching keyboard input, which are much better!
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Char</returns>
        private char KeyToChar(Keys key, bool shiftPressed)
        {
            // If key will not be found, just return space
            char ret = ' ';
            int keyNum = (int)key;
            if (keyNum >= (int)Keys.A && keyNum <= (int)Keys.Z)
            {
                if (shiftPressed)
                    ret = key.ToString()[0];
                else
                    ret = key.ToString().ToLower()[0];
            } // if (keyNum)
            else if (keyNum >= (int)Keys.D0 && keyNum <= (int)Keys.D9 &&
                shiftPressed == false)
            {
                ret = (char)((int)'0' + (keyNum - Keys.D0));
            } // else if
            else if (key == Keys.D1 && shiftPressed)
                ret = '!';
            else if (key == Keys.D2 && shiftPressed)
                ret = '"';
            else if (key == Keys.D3 && shiftPressed)
                ret = '§';
            else if (key == Keys.D4 && shiftPressed)
                ret = '$';
            else if (key == Keys.D5 && shiftPressed)
                ret = '%';
            else if (key == Keys.D6 && shiftPressed)
                ret = '&';
            else if (key == Keys.D7 && shiftPressed)
                ret = '/';
            else if (key == Keys.D8 && shiftPressed)
                ret = '(';
            else if (key == Keys.D9 && shiftPressed)
                ret = ')';
            else if (key == Keys.D0 && shiftPressed)
                ret = '=';
            else if (key == Keys.OemTilde)
                ret = shiftPressed ? '~' : '`';
            else if (key == Keys.OemMinus)
                ret = shiftPressed ? '_' : '-';
            //else if (key == Keys.OemPipe)     NOT ALLOWED, BUTTON FOR TOGGLE CONSOLE
            //    ret = shiftPressed ? '°' : '^';
            else if (key == Keys.OemOpenBrackets)
                ret = shiftPressed ? '?' : 's';
            else if (key == Keys.OemCloseBrackets)
                ret = shiftPressed ? '`' : '´';
            else if (key == Keys.OemSemicolon)
                ret = shiftPressed ? ':' : ';';
            else if (key == Keys.OemQuotes)
                ret = shiftPressed ? '"' : '\'';
            else if (key == Keys.OemComma)
                ret = shiftPressed ? ';' : ',';
            else if (key == Keys.OemPeriod)
                ret = shiftPressed ? ':' : '.';
            else if (key == Keys.OemQuestion)
                ret = shiftPressed ? '\'' : '#';
            else if (key == Keys.OemPlus)
                ret = shiftPressed ? '*' : '+';

            // Return result
            return ret;
        } // KeyToChar(key)
         
        /// <summary>
        /// Handle keyboard input helper method to catch keyboard input
        /// for an input text. Only used to enter the player name in the game.
        /// </summary>
        /// <param name="inputText">Input text</param>
        public void HandleKeyboardInput(ref string inputText)
        {
            // Is a shift key pressed (we have to check both, left and right)
            bool isShiftPressed =
                keyboardState.IsKeyDown(Keys.LeftShift) ||
                keyboardState.IsKeyDown(Keys.RightShift);

            // Go through all pressed keys
            foreach (Keys pressedKey in keyboardState.GetPressedKeys())
                // Only process if it was not pressed last frame
                if (keysPressedLastFrame.Contains(pressedKey) == false)
                {
                    // No special key?
                    if (IsSpecialKey(pressedKey) == false &&
                        // Max. allow 32 chars
                        inputText.Length < 32)
                    {
                        // Then add the letter to our inputText.
                        // Check also the shift state!
                        inputText += KeyToChar(pressedKey, isShiftPressed);
                    } // if (IsSpecialKey)
                    else if (pressedKey == Keys.Back &&
                        inputText.Length > 0)
                    {
                        // Remove 1 character at end
                        inputText = inputText.Substring(0, inputText.Length - 1);
                    } // else if
                } // foreach if (WasKeyPressedLastFrame)
        } // HandleKeyboardInput(inputText)

        #endregion

        #endregion


        #region Mouse Properties

        /// <summary>
        /// Mouse state, set every frame in the Update method.
        /// </summary>
        private MouseState mouseState,
            mouseStateLastFrame;

        /// <summary>
        /// Mouse wheel delta this frame. XNA does report only the total
        /// scroll value, but we usually need the current delta!
        /// </summary>
        /// <returns>0</returns>
        private int mouseWheelDelta = 0, mouseWheelValue = 0;

        /// <summary>
        /// X and y movements of the mouse this frame
        /// </summary>
        private float mouseXMovement, mouseYMovement,
            lastMouseXMovement, lastMouseYMovement;

        /// <summary>
        /// Mouse left button pressed
        /// </summary>
        /// <returns>Bool</returns>
        private bool MouseLeftButtonPressed
        {
            get
            {
                return mouseState.LeftButton == ButtonState.Pressed;
            } // get
        } // MouseLeftButtonPressed

        /// <summary>
        /// Mouse right button pressed
        /// </summary>
        /// <returns>Bool</returns>
        private bool MouseRightButtonPressed
        {
            get
            {
                return mouseState.RightButton == ButtonState.Pressed;
            } // get
        } // MouseRightButtonPressed

        /// <summary>
        /// Mouse middle button pressed
        /// </summary>
        /// <returns>Bool</returns>
        private bool MouseMiddleButtonPressed
        {
            get
            {
                return mouseState.MiddleButton == ButtonState.Pressed;
            } // get
        } // MouseMiddleButtonPressed

        /// <summary>
        /// Mouse left button just pressed
        /// </summary>
        /// <returns>Bool</returns>
        private bool MouseLeftButtonJustPressed
        {
            get
            {
                return mouseState.LeftButton == ButtonState.Pressed &&
                    mouseStateLastFrame.LeftButton == ButtonState.Released;
            } // get
        } // MouseLeftButtonJustPressed

        /// <summary>
        /// Mouse right button just pressed
        /// </summary>
        /// <returns>Bool</returns>
        private bool MouseRightButtonJustPressed
        {
            get
            {
                return mouseState.RightButton == ButtonState.Pressed &&
                    mouseStateLastFrame.RightButton == ButtonState.Released;
            } // get
        } // MouseRightButtonJustPressed

        /// <summary>
        /// Mouse wheel delta
        /// </summary>
        /// <returns>Int</returns>
        private int MouseWheelDelta
        {
            get
            {
                return mouseWheelDelta;
            } // get
        } // MouseWheelDelta
  
        #endregion

         
        #region Keyboard Properties

        /// <summary>
        /// Keyboard state, set every frame in the Update method.
        /// Note: KeyboardState is a class and not a struct,
        /// we have to initialize it here, else we might run into trouble when
        /// accessing any keyboardState data before BaseGame.Update() is called.
        /// We can also NOT use the last state because everytime we call
        /// Keyboard.GetState() the old state is useless (see XNA help for more
        /// information, section Input). We store our own array of keys from
        /// the last frame for comparing stuff.
        /// </summary>
        private KeyboardState keyboardState = Microsoft.Xna.Framework.Input.Keyboard.GetState();

        /// <summary>
        /// Keys pressed last frame, for comparison if a key was just pressed.
        /// </summary>
        private List<Keys> keysPressedLastFrame = new List<Keys>();

        /// <summary>
        /// Keyboard
        /// </summary>
        /// <returns>Keyboard state</returns>
        private KeyboardState Keyboard
        {
            get
            {
                return keyboardState;
            }
        }
       
        #endregion


        #region private fields

        /// <summary>
        /// true when movement shall be captured,
        /// false when position shall be captured.
        /// set by get mousemovement and get mousepos (predicts what is needed next)
        /// </summary>
        private bool captureMouseMovement = false;

        private ICanyonShooterGame game;

        private readonly Dictionary<string, Keys> BindingsKeyboard = new Dictionary<string, Keys>();
        private readonly Dictionary<string, MouseButton> BindingsMouse = new Dictionary<string, MouseButton>();

        #endregion



        public Input(ICanyonShooterGame game)
        {
            this.game = game;
        }

        #region IInput Member

        public bool Init()
        {
            return LoadFromFile("DUMMY");
        }

        public bool IsKeyDown(string name)
        {
            if (BindingsMouse.ContainsKey(name))
            {
                switch(BindingsMouse[name])
                {
                    case MouseButton.Left:
                        if (mouseState.LeftButton == ButtonState.Pressed) return true;
                        break;
                    case MouseButton.Middle:
                        if (mouseState.MiddleButton == ButtonState.Pressed) return true;
                        break;
                    case MouseButton.Right:
                        if (mouseState.RightButton == ButtonState.Pressed) return true;
                        break;
                }
            }

            if (BindingsKeyboard.ContainsKey(name))
            {
                if(keyboardState.IsKeyDown(BindingsKeyboard[name])) return true;
            }

            return false;
        }

        public bool HasKeyJustBeenPressed(string name)
        {
            if (BindingsMouse.ContainsKey(name))
            {
                switch (BindingsMouse[name])
                {
                    case MouseButton.Left:
                        if (mouseStateLastFrame.LeftButton == ButtonState.Released
                            && mouseState.LeftButton == ButtonState.Pressed) return true;
                        break;
                    case MouseButton.Middle:
                        if (mouseStateLastFrame.MiddleButton == ButtonState.Released
                            && mouseState.MiddleButton == ButtonState.Pressed) return true;
                        break;
                    case MouseButton.Right:
                        if (mouseStateLastFrame.RightButton == ButtonState.Released
                            && mouseState.RightButton == ButtonState.Pressed) return true;
                        break;
                }
            }

            if (BindingsKeyboard.ContainsKey(name))
            {
                return keyboardState.IsKeyDown(BindingsKeyboard[name]) &&
                        keysPressedLastFrame.Contains(BindingsKeyboard[name]) == false;
            }

            return false;
        }

        public bool HasKeyJustBeenReleased(string name)
        {
            if (BindingsMouse.ContainsKey(name))
            {
                switch (BindingsMouse[name])
                {
                    case MouseButton.Left:
                        if (mouseStateLastFrame.LeftButton == ButtonState.Pressed
                            && mouseState.LeftButton == ButtonState.Released) return true;
                        break;
                    case MouseButton.Middle:
                        if (mouseStateLastFrame.MiddleButton == ButtonState.Pressed
                            && mouseState.MiddleButton == ButtonState.Released) return true;
                        break;
                    case MouseButton.Right:
                        if (mouseStateLastFrame.RightButton == ButtonState.Pressed
                            && mouseState.RightButton == ButtonState.Released) return true;
                        break;
                }
            }

            if (BindingsKeyboard.ContainsKey(name))
            {
                return !keyboardState.IsKeyDown(BindingsKeyboard[name]) &&
                        keysPressedLastFrame.Contains(BindingsKeyboard[name]) == true;
            }

            return false;
        }

        public Vector2 MouseMovement
        {
            get
            {
                captureMouseMovement = true;
                return new Vector2(mouseXMovement, mouseYMovement);
            }
            set
            {
                mouseXMovement = value.X;
                mouseYMovement = value.Y;
            }
        }


        public void Bind(string name, string key)
        {
            try
            {
                Bind(name, (Keys)Enum.Parse(typeof(Keys), key, true));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void Bind(string name, Keys keyToBind)
        {
            if(!BindingsKeyboard.ContainsKey(name))
                BindingsKeyboard.Add(name, keyToBind);
            BindingsKeyboard[name] = keyToBind;
        }

        public void Bind(string name, MouseButton buttonToBind)
        {
            if(!BindingsMouse.ContainsKey(name))
                BindingsMouse.Add(name, buttonToBind);
            BindingsMouse[name] = buttonToBind;
        }

        public bool LoadFromFile(string name)
        {
            //TODO: Load from Configfile
            Bind("Menu.Left", Keys.Left);
            Bind("Menu.Right", Keys.Right);
            Bind("Menu.Up", Keys.Up);
            Bind("Menu.Down", Keys.Down);
            Bind("Menu.Ok", Keys.Enter);
            Bind("Menu.Select", MouseButton.Left);
            Bind("Menu.Back", Keys.Back);
            Bind("Menu.F1", Keys.F1);
            Bind("Menu.F2", Keys.F2);
            
            Bind("Camera.Forward", Keys.W);
            Bind("Camera.Backward", Keys.S);
            Bind("Camera.Left", Keys.A);
            Bind("Camera.Right", Keys.D);
            Bind("Camera.Up", Keys.Space);
            Bind("Camera.Down", Keys.LeftShift);
            Bind("Camera.ZoomIn", Keys.PageUp);
            Bind("Camera.ZoomOut",Keys.PageDown);

            if (game.DebugMode)
            {
                Bind("Debug.SwitchCameras", Keys.C);

                Bind("Console.Toggle", Keys.OemPipe);
                Bind("Console.Back", Keys.Back);
                Bind("Console.Enter", Keys.Enter);
                Bind("Console.NextSuggestion", Keys.Tab);
                Bind("Console.ScrollingUp", Keys.PageUp);
                Bind("Console.ScrollingDown", Keys.PageDown);
            }

            Bind("Game.Menu", Keys.Escape);
            Bind("Player1.Left", Keys.A);
            Bind("Player1.Right", Keys.D);
            Bind("Player1.Up", Keys.W);
            Bind("Player1.Down", Keys.S);
            Bind("Player1.Boost", Keys.Space);
            Bind("Player1.PrimaryFire",MouseButton.Left);
            Bind("Player1.SecondaryFire", MouseButton.Right);
            Bind("Player1.PrimaryWeapon.Select1", Keys.D1);
            Bind("Player1.PrimaryWeapon.Select2", Keys.D2);
            Bind("Player1.SecondaryWeapon.Select3", Keys.D3);
            Bind("Player1.SecondaryWeapon.Select4", Keys.D4);

            if (game.DebugMode)
            {
                Bind("QuickStart", Keys.Enter);
            }
            Bind("Post.Bug", Keys.F12);

            return true;
        }

        public Point MousePosition
        {
            get
            {
                captureMouseMovement = false;
                return new Point(mouseState.X, mouseState.Y);
            }
        }

        public void Update()
        {
            // Handle mouse input variables
            mouseStateLastFrame = mouseState;
            mouseState = Mouse.GetState();

            // Update mouseXMovement and mouseYMovement
            /*if(captureMouseMovement)
            {
                Mouse.SetPosition(
                    game.Graphics.Device.Viewport.Width / 2, 
                    game.Graphics.Device.Viewport.Height / 2
                );

                lastMouseXMovement += mouseState.X - game.Graphics.Device.Viewport.Width / 2;
                lastMouseYMovement += mouseState.Y - game.Graphics.Device.Viewport.Height / 2;
            }
            else
            {
                lastMouseXMovement += mouseState.X - mouseStateLastFrame.X;
                lastMouseYMovement += mouseState.Y - mouseStateLastFrame.Y;
            }
            mouseXMovement = lastMouseXMovement / 2.0f;
            mouseYMovement = lastMouseYMovement / 2.0f;
            lastMouseXMovement -= lastMouseXMovement / 2.0f;
            lastMouseYMovement -= lastMouseYMovement / 2.0f;*/

            mouseXMovement = mouseState.X - game.Graphics.Device.Viewport.Width / 2;
            mouseYMovement = mouseState.Y - game.Graphics.Device.Viewport.Height / 2;

            /*lastMouseXMovement = mouseXMovement;
            lastMouseYMovement = mouseYMovement;*/

            if (captureMouseMovement)
            {
                Mouse.SetPosition(game.Graphics.Device.Viewport.Width / 2, game.Graphics.Device.Viewport.Height / 2);
            }


            // Handle keyboard input
            keysPressedLastFrame = new List<Keys>(keyboardState.GetPressedKeys());
            keyboardState = Microsoft.Xna.Framework.Input.Keyboard.GetState();
        }


        #endregion

    } // class
} // namespace 
