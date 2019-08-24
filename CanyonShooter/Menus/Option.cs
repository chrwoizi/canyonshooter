
#region Using Statements

using DescriptionLibs.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CanyonShooter.Engine.Input;

#endregion


namespace CanyonShooter.Menus
{
    public class Option : DrawableGameComponent
    {
        #region DataMembers

        private ICanyonShooterGame game;
        private OptionDescription xml;
        private MenuDescription MenuXml;
        private int district;

        private Vector2 Resolution;
        private Vector2 oldResolution;

        private bool isActive = false;

        public bool Activity
        {
            get { return isActive; }
            set { isActive = value; }
        }

        private bool visible = false;

        public bool Visiblity
        {
            get { return visible; }
            set { visible = value; }
        }

        private Listbox listbox;
        private Dialog dialog;

        private Prompt prompt;
        int counter;

        internal Dialog Dialog
        {
            get { return dialog; }
            set { dialog = value; }
        }

        internal Listbox Listbox
        {
            get { return listbox; }
        }


        private MenuControl steuerung;

        #region Draw
        private Texture2D background;
        private Texture2D mouse;
        #endregion

        #endregion

        // Konstuktor
        // Dieser Konstruktor kann von außen nicht erreicht werden.
        public Option(ICanyonShooterGame game, MenuDescription Menuxml)
            : base(game as Game)
        {
            this.counter = 0;
            this.district = 99999999;
            this.game = game;
            this.xml = game.Content.Load<OptionDescription>("Content\\Menu\\OptionMenu");
            this.MenuXml = Menuxml;
            Resolution = new Vector2(game.Graphics.Device.Viewport.Width, game.Graphics.Device.Viewport.Height);

            steuerung = new MenuControl((Menu)this.game.GameStates.Menu, this.game);
        }

        public void initializeDistrict(int district)
        {
            this.visible = true;
            this.isActive = true;
            this.district = district;
            if (district == (int)DistrictEnum.Spielereinstellungen)
            {
                dialog = new Dialog(game, xml, district,
                    game.GameStates.Profil.SpielereinstellungenList,
                    game.GameStates.Profil.SpielereinstellungenDat,
                    game.GameStates.Profil.NamenKeysSpielereinstellungen);
            }
            else if (district == (int)DistrictEnum.Grafik)
            {
                dialog = new Dialog(game, xml, district,
                                  game.GameStates.Profil.GrafikList,
                                  game.GameStates.Profil.GrafikDat,
                                  game.GameStates.Profil.NamenKeysGrafik);
            }
            else if (district == (int)DistrictEnum.Sound)
            {
                dialog = new Dialog(game, xml, district,
                                   game.GameStates.Profil.SoundList,
                                   game.GameStates.Profil.SoundDat,
                                   game.GameStates.Profil.NamenKeysSound);
            }
            else if (district == (int)DistrictEnum.Steuerung)
            {
                listbox = new Listbox(game, xml.OptionParts[district].StartPosition, true, this.MenuXml, district,
                                   game.GameStates.Profil.SteuerungList,
                                   game.GameStates.Profil.SteuerungDat,
                                   game.GameStates.Profil.NamenKeysSteuerung);
                listbox.Buttons.Last.Previous.Value.resetActive();
                listbox.Buttons.Last.Value.resetActive();
                listbox.Buttons.First.Value.resetActive();
                listbox.List.First.Value.SetActive();
            }
            else
            {
                this.visible = false;
                this.isActive = false;
            }

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
            if (isActive && visible)
            {
                base.Update(gameTime);
                if (district < 3)
                {
                    dialog.Update(gameTime, Spritebatch);
                }
                else if (district == 3)
                {
                    listbox.Update(gameTime, Spritebatch);
                }

                #region Input

                if (district < 3)
                {
                    #region Mouse

                    #region Set Button active

                    #region Button Accept
                    if (isMouseInBox(this.dialog.accept) && new Vector2(game.Input.MousePosition.X,
                        game.Input.MousePosition.Y) != game.GameStates.Menu.Mouse)
                    {
                        counter = dialog.list.Count + 1;
                        this.dialog.accept.SetActive();
                        this.dialog.decline.resetActive();
                        dialog.list.currentNode.Value.resetActive();
                    }
                    #endregion
                    #region Button Decline
                    else if (isMouseInBox(this.dialog.decline) && new Vector2(game.Input.MousePosition.X, 
                        game.Input.MousePosition.Y) != game.GameStates.Menu.Mouse)
                    {
                        counter = dialog.list.Count + 1;
                        this.dialog.decline.SetActive();
                        this.dialog.accept.resetActive();
                        dialog.list.currentNode.Value.resetActive();
                    }
                    #endregion
                    else if (!isMouseInBox(this.dialog.accept) && !isMouseInBox(this.dialog.decline) && 
                        new Vector2(game.Input.MousePosition.X, game.Input.MousePosition.Y)!= game.GameStates.Menu.Mouse)

                    {
                        if (counter != dialog.list.Count + 1)
                        {
                            this.dialog.decline.resetActive();
                            this.dialog.accept.resetActive();
                            dialog.list.currentNode.Value.SetActive();
                        }
                        foreach (Button g in dialog.list)
                        {
                            if (isMouseInBox(g))
                            {
                                counter = 0;
                                resetButton();
                                while (!dialog.list.currentNode.Value.Equals(g))
                                {
                                    dialog.list.Next();
                                }
                                #region OptionModulationButton
                                if (this.dialog.OptModDic.ContainsKey(g.getButtonName()))
                                {
                                    if (isMouseInBox(this.dialog.OptModDic[g.getButtonName()].plus))
                                    {
                                        dialog.OptModDic[g.getButtonName()].plus.SetActive();
                                        dialog.OptModDic[g.getButtonName()].minus.resetActive();
                                    }
                                    else if (isMouseInBox(this.dialog.OptModDic[g.getButtonName()].minus))
                                    {
                                        dialog.OptModDic[g.getButtonName()].plus.resetActive();
                                        dialog.OptModDic[g.getButtonName()].minus.SetActive();
                                    }
                                }
                                #endregion
                                #region SlideBar
                                else if (dialog.SlideMods.ContainsKey(g.getButtonName()))
                                {
                                    if (isMouseInBox(dialog.SlideMods[g.getButtonName()].plus))
                                    {
                                        dialog.SlideMods[g.getButtonName()].plus.SetActive();
                                        dialog.SlideMods[g.getButtonName()].minus.resetActive();
                                    }
                                    else if (isMouseInBox(this.dialog.SlideMods[g.getButtonName()].minus))
                                    {
                                        dialog.SlideMods[g.getButtonName()].plus.resetActive();
                                        dialog.SlideMods[g.getButtonName()].minus.SetActive();
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                    #endregion

                    #region Leftbutton
                    if (game.Input.HasKeyJustBeenPressed("Menu.Select"))
                    {
                        #region Dialog Button
                        if (isMouseInBox(this.dialog.accept) && dialog.accept.GetActive())
                        {                            
                            if (Resolution != new Vector2(game.Graphics.Device.Viewport.Width,
                                game.Graphics.Device.Viewport.Height))
                            {
                                this.oldResolution = new Vector2(game.Graphics.Device.Viewport.Width,
                                          game.Graphics.Device.Viewport.Height);
                                prompt = new Prompt(game, "Resolution" + "1", game.Graphics.Device.Viewport.Width,
                                            game.Graphics.Device.Viewport.Height, new string[] { 
                                                    "You about to change the resolution.",
                                                    "If you decide to choose a new resolution",
                                                    "you will have 15 Seconds to accept the new","resolution!" },
                                            false);
                                prompt.accept += new PromptEventHandler(initiateResolutionAccept);
                                prompt.decline += new PromptEventHandler(ResCancel);
                                prompt.intiateButtons();
                                this.Activity = false;                           
                            }
                            else
                            {
                                steuerung.selection(this.dialog.accept.getButtonName(), "Optionen");
                            }
                        }
                        else if (isMouseInBox(this.dialog.decline) && dialog.decline.GetActive())
                        {
                            steuerung.selection(this.dialog.decline.getButtonName(), "Optionen");
                        }
                        #endregion
                        else
                        {
                            foreach (Button g in dialog.list)
                            {
                                string text = g.getButtonName();
                                if (isMouseInBox(g))
                                {
                                    if (!this.dialog.OptModDic.ContainsKey(text))
                                    {
                                        #region Slidebar
                                        if (dialog.SlideMods.ContainsKey(text))
                                        {
                                            #region Slidebar Plus
                                            if (isMouseInBox(dialog.SlideMods[text].plus) &&
                                                dialog.SlideMods[text].plus.GetActive())
                                            {
                                                dialog.SlideMods[text].plus.ButtonPressed();
                                                {
                                                    switch (text)
                                                    {
                                                        case "Music":
                                                            {
                                                                dialog.Data[text] = (float)dialog.SlideMods[text].CurrentValue;
                                                                if ((bool)dialog.OptModDic["Sound"].returnCurrentValue())
                                                                {
                                                                    game.Sounds.MusicVolume = (float)dialog.SlideMods[text].CurrentValue;
                                                                }
                                                                break;
                                                            }
                                                        case "Effect":
                                                            {
                                                                dialog.Data[text] = (float)dialog.SlideMods[text].CurrentValue;
                                                                if ((bool)dialog.OptModDic["Sound"].returnCurrentValue())
                                                                {
                                                                    game.Sounds.EffectVolume = (float)dialog.SlideMods[text].CurrentValue;
                                                                }
                                                                break;
                                                            }
                                                        case "Detail":
                                                            {
                                                                dialog.Data[text] = (float)dialog.SlideMods[text].CurrentValue;
                                                                break;
                                                            }
                                                        default:
                                                            break;
                                                    }
                                                }
                                            }
                                            #endregion
                                            #region Slidebar Minus
                                            else if (isMouseInBox(dialog.SlideMods[text].minus) &&
                                                dialog.SlideMods[text].minus.GetActive())
                                            {
                                                dialog.SlideMods[text].minus.ButtonPressed();
                                                switch (text)
                                                {
                                                    case "Music":
                                                        {
                                                            dialog.Data[text] = (float)dialog.SlideMods[text].CurrentValue;
                                                            if ((bool)dialog.OptModDic["Sound"].returnCurrentValue())
                                                            {
                                                                game.Sounds.MusicVolume = (float)dialog.SlideMods[text].CurrentValue;
                                                            }
                                                            break;
                                                        }
                                                    case "Effect":
                                                        {
                                                            dialog.Data[text] = (float)dialog.SlideMods[text].CurrentValue;
                                                            if ((bool)dialog.OptModDic["Sound"].returnCurrentValue())
                                                            {
                                                                game.Sounds.EffectVolume = (float)dialog.SlideMods[text].CurrentValue;
                                                            }
                                                            break;
                                                        }
                                                    case "Detail":
                                                        {
                                                            dialog.Data[text] = (float)dialog.SlideMods[text].CurrentValue;
                                                            break;
                                                        }
                                                    default:
                                                        break;
                                                }
                                            }
                                            #endregion
                                            #region BarPressed
                                            else if (isMouseInRec(dialog.SlideMods[text].BarRec) && g.GetActive())
                                            {
                                                dialog.SlideMods[text].BarPressed(game.GameStates.Menu.MousePosition);
                                                switch (text)
                                                {
                                                    case "Music":
                                                        {
                                                            dialog.Data[text] = (float)dialog.SlideMods[text].CurrentValue;
                                                            if ((bool)dialog.OptModDic["Sound"].returnCurrentValue())
                                                            {
                                                                game.Sounds.MusicVolume = (float)dialog.SlideMods[text].CurrentValue;
                                                            }
                                                            break;
                                                        }
                                                    case "Effect":
                                                        {
                                                            dialog.Data[text] = (float)dialog.SlideMods[text].CurrentValue;
                                                            if ((bool)dialog.OptModDic["Sound"].returnCurrentValue())
                                                            {
                                                                game.Sounds.EffectVolume = (float)dialog.SlideMods[text].CurrentValue;
                                                            }
                                                            break;
                                                        }
                                                    case "Detail":
                                                        {
                                                            dialog.Data[text] = (float)dialog.SlideMods[text].CurrentValue;
                                                            break;
                                                        }
                                                    default:
                                                        break;
                                                }
                                            }
                                            #endregion
                                            #region Slidebar marker not implement
                                            /*else if (isMouseInRec(dialog.SlideMods[text].MarkerRec) && 
                                                !game.Input.HasKeyJustBeenReleased("Menu.Select"))
                                            {
                                                dialog.SlideMods[text].BarScroll((int)game.GameStates.Menu.MousePosition.X);
                                                switch (text)
                                                {
                                                    case "Music":
                                                        {
                                                            dialog.Data[text] = (float)dialog.SlideMods[text].CurrentValue;
                                                            if ((bool)dialog.OptModDic["Sound"].returnCurrentValue())
                                                            {
                                                                game.Sounds.MusicVolume = (float)dialog.SlideMods[text].CurrentValue / 100;
                                                            }
                                                            break;
                                                        }
                                                    case "Effect":
                                                        {
                                                            dialog.Data[text] = (float)dialog.SlideMods[text].CurrentValue;
                                                            if ((bool)dialog.OptModDic["Sound"].returnCurrentValue())
                                                            {
                                                                game.Sounds.EffectVolume = (float)dialog.SlideMods[text].CurrentValue / 100;
                                                            }
                                                            break;
                                                        }
                                                    default:
                                                        break;
                                                }
                                            }*/
                                            #endregion
                                        }
                                        #endregion
                                        #region Other
                                        else
                                        {
                                            switch (g.getButtonName())
                                            {
                                                case "Playername":
                                                    {
                                                        if (g.GetActive())
                                                        {
                                                            steuerung.selection(g.getButtonName(),
                                                            "Optionen");
                                                        }
                                                        break;
                                                    }
                                                default:
                                                    break;
                                            }
                                        }
                                        #endregion
                                    }
                                    #region OptionModulationButton
                                    else if (this.dialog.OptModDic.ContainsKey(text))
                                    {
                                        if (isMouseInBox(dialog.OptModDic[text].plus) && 
                                            dialog.OptModDic[text].plus.GetActive())
                                        {
                                            dialog.OptModDic[text].plus.ButtonPressed();
                                            {
                                                switch (text)
                                                {
                                                    #region Resolution   
                                                    case "Resolution":
                                                        {
                                                            SetGameResolution((string)dialog.OptModDic[text].returnCurrentValue(), text);
                                                            break;
                                                        }
                                                    #endregion
                                                    #region Fullscreen
                                                    case "Fullscreen":
                                                        {
                                                            game.Graphics.Fullscreen = (bool)dialog.OptModDic[text].returnCurrentValue();
                                                            dialog.Data[text] = (bool)dialog.OptModDic[text].returnCurrentValue();
                                                            break;
                                                        }
                                                    #endregion
                                                    #region Anti Aliasing
                                                    case "Anti Aliasing":
                                                        {
                                                            dialog.Data[text] = (string)dialog.OptModDic[text].returnCurrentValue();
                                                            switch ((string)dialog.Data[text])
                                                            {
                                                                case "0x":
                                                                    {
                                                                        game.Graphics.MultiSampleType = MultiSampleType.None;
                                                                        break;
                                                                    }
                                                                case "2x":
                                                                    {
                                                                        game.Graphics.MultiSampleType = MultiSampleType.TwoSamples;
                                                                        break;
                                                                    }
                                                                case "4x":
                                                                    {
                                                                        game.Graphics.MultiSampleType = MultiSampleType.FourSamples;
                                                                        break;
                                                                    }
                                                                case "8x":
                                                                    {
                                                                        game.Graphics.MultiSampleType = MultiSampleType.EightSamples;
                                                                        break;
                                                                    }
                                                                case "16x":
                                                                    {
                                                                        game.Graphics.MultiSampleType = MultiSampleType.SixteenSamples;
                                                                        break;
                                                                    }
                                                                default:
                                                                    break;
                                                            }
                                                            break;
                                                        }
                                                    #endregion
                                                    #region Sound
                                                    case "Sound":
                                                        {
                                                            dialog.Data[text] = (bool)dialog.OptModDic[text].returnCurrentValue();
                                                            if ((bool)dialog.OptModDic[text].returnCurrentValue() == false)
                                                            {
                                                                game.Sounds.EffectVolume = 0f;
                                                                game.Sounds.MusicVolume = 0f;
                                                            }
                                                            else
                                                            {
                                                                game.Sounds.MusicVolume = (float)dialog.SlideMods["Music"].CurrentValue;
                                                                game.Sounds.EffectVolume = (float)dialog.SlideMods["Effect"].CurrentValue;
                                                            }
                                                            break;
                                                        }
                                                    #endregion
                                                    default:
                                                        SetDataValues(text, dialog.OptModDic[text].returnCurrentValue());
                                                        break;
                                                }
                                            }
                                        }
                                        else if (isMouseInBox(dialog.OptModDic[text].minus) && 
                                            dialog.OptModDic[text].minus.GetActive())
                                        {
                                            dialog.OptModDic[text].minus.ButtonPressed();
                                            switch (text)
                                            {

                                                #region Resolution
                                                case "Resolution":
                                                    {
                                                        SetGameResolution((string)dialog.OptModDic[text].returnCurrentValue(), text);
                                                        break;
                                                    }
                                                #endregion
                                                #region Fullscreen
                                                case "Fullscreen":
                                                    {
                                                        game.Graphics.Fullscreen = (bool)dialog.OptModDic[text].returnCurrentValue();
                                                        dialog.Data[text] = (bool)dialog.OptModDic[text].returnCurrentValue();
                                                        break;
                                                    }
                                                #endregion
                                                #region Anti Aliasing
                                                case "Anti Aliasing":
                                                    {
                                                        dialog.Data[text] = (string)dialog.OptModDic[text].returnCurrentValue();
                                                        switch ((string)dialog.Data[text])
                                                        {
                                                            case "0x":
                                                                {
                                                                    game.Graphics.MultiSampleType = MultiSampleType.None;
                                                                    break;
                                                                }
                                                            case "2x":
                                                                {
                                                                    game.Graphics.MultiSampleType = MultiSampleType.TwoSamples;
                                                                    break;
                                                                }
                                                            case "4x":
                                                                {
                                                                    game.Graphics.MultiSampleType = MultiSampleType.FourSamples;
                                                                    break;
                                                                }
                                                            case "8x":
                                                                {
                                                                    game.Graphics.MultiSampleType = MultiSampleType.EightSamples;
                                                                    break;
                                                                }
                                                            case "16x":
                                                                {
                                                                    game.Graphics.MultiSampleType = MultiSampleType.SixteenSamples;
                                                                    break;
                                                                }
                                                            default:
                                                                break;
                                                        }
                                                        break;
                                                    }
                                                #endregion
                                                #region Sound
                                                case "Sound":
                                                    {
                                                        dialog.Data[text] = (bool)dialog.OptModDic[text].returnCurrentValue();
                                                        if ((bool)dialog.OptModDic[text].returnCurrentValue() == false)
                                                        {
                                                            game.Sounds.EffectVolume = 0f;
                                                            game.Sounds.MusicVolume = 0f;
                                                        }
                                                        else
                                                        {
                                                            game.Sounds.MusicVolume = (float)dialog.SlideMods["Music"].CurrentValue;
                                                            game.Sounds.EffectVolume = (float)dialog.SlideMods["Effect"].CurrentValue;
                                                        }
                                                        break;
                                                    }
                                                #endregion
                                                default:
                                                    SetDataValues(text, dialog.OptModDic[text].returnCurrentValue());
                                                    break;
                                            }
                                        }
                                        else if (!isMouseInBox(dialog.OptModDic[text].minus) &&
                                            !isMouseInBox(dialog.OptModDic[text].plus) && g.GetActive())
                                        {
                                            if (Resolution != new Vector2(game.Graphics.Device.Viewport.Width, 
                                                game.Graphics.Device.Viewport.Height))
                                            {
                                                this.oldResolution = new Vector2(game.Graphics.Device.Viewport.Width,
                                                       game.Graphics.Device.Viewport.Height);
                                                prompt = new Prompt(game, g.getButtonName() + "1", game.Graphics.Device.Viewport.Width,
                                                game.Graphics.Device.Viewport.Height, new string[] { 
                                                    "You about to change the resolution.",
                                                    "If you decide to choose a new resolution",
                                                    "you will have 15 Seconds to accept the new","resolution!" },
                                                false);
                                                prompt.accept += new PromptEventHandler(initiateResolution);
                                                prompt.decline += new PromptEventHandler(ResCancel);
                                                prompt.intiateButtons();
                                                this.Activity = false;                                                
                                            }                                            
                                        }
                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                    #endregion
                    #endregion

                    #region Keyboard

                    #region Key Left
                    if (game.Input.HasKeyJustBeenPressed("Menu.Left"))
                    {
                        if (counter == this.dialog.list.Count + 1)
                        {
                            if (this.dialog.accept.GetActive())
                            {
                                this.dialog.accept.resetActive();
                                this.dialog.decline.SetActive();
                            }
                            else
                            {
                                this.dialog.accept.SetActive();
                                this.dialog.decline.resetActive();
                            }
                        }
                        else
                        {
                            string text = this.dialog.list.currentNode.Value.getButtonName();
                            #region OptionModulationButton
                            if (this.dialog.OptModDic.ContainsKey(text))
                            {
                                this.dialog.OptModDic[text].minus.SetActive();
                                this.dialog.OptModDic[text].plus.resetActive();
                                this.dialog.OptModDic[text].minus.ButtonPressed();
                                switch (dialog.list.currentNode.Value.getButtonName())
                                {
                                    #region Resolution
                                    case "Resolution":
                                        {
                                            SetGameResolution((string)dialog.OptModDic[text].returnCurrentValue(), text);
                                            break;
                                        }
                                    #endregion
                                    #region Fullscreen
                                    case "Fullscreen":
                                        {
                                            game.Graphics.Fullscreen = (bool)dialog.OptModDic[text].returnCurrentValue();
                                            dialog.Data[text] = (bool)dialog.OptModDic[text].returnCurrentValue();
                                            break;
                                        }
                                    #endregion
                                    #region Anti Aliasing
                                    case "Anti Aliasing":
                                        {
                                            dialog.Data[text] = (string)dialog.OptModDic[text].returnCurrentValue();
                                            switch ((string)dialog.Data[text])
                                            {
                                                case "0x":
                                                    {
                                                        game.Graphics.MultiSampleType = MultiSampleType.None;
                                                        break;
                                                    }
                                                case "2x":
                                                    {
                                                        game.Graphics.MultiSampleType = MultiSampleType.TwoSamples;
                                                        break;
                                                    }
                                                case "4x":
                                                    {
                                                        game.Graphics.MultiSampleType = MultiSampleType.FourSamples;
                                                        break;
                                                    }
                                                case "8x":
                                                    {
                                                        game.Graphics.MultiSampleType = MultiSampleType.EightSamples;
                                                        break;
                                                    }
                                                case "16x":
                                                    {
                                                        game.Graphics.MultiSampleType = MultiSampleType.SixteenSamples;
                                                        break;
                                                    }
                                                default:
                                                    break;
                                            }
                                            break;
                                        }
                                    #endregion
                                    #region Sound
                                    case "Sound":
                                        {
                                            dialog.Data[text] = (bool)dialog.OptModDic[text].returnCurrentValue();
                                            if ((bool)dialog.OptModDic[text].returnCurrentValue() == false)
                                            {
                                                game.Sounds.EffectVolume = 0f;
                                                game.Sounds.MusicVolume = 0f;
                                            }
                                            else
                                            {
                                                game.Sounds.MusicVolume = (float)dialog.SlideMods["Music"].CurrentValue;
                                                game.Sounds.EffectVolume = (float)dialog.SlideMods["Effect"].CurrentValue;
                                            }
                                            break;
                                        }
                                    #endregion
                                    default:
                                        SetDataValues(text, dialog.OptModDic[text].returnCurrentValue());
                                        break;
                                }
                            }
                            #endregion
                            #region SlideBar
                            else if (dialog.SlideMods.ContainsKey(text))
                            {
                                this.dialog.SlideMods[text] .minus.SetActive();
                                this.dialog.SlideMods[text].plus.resetActive();
                                this.dialog.SlideMods[text].minus.ButtonPressed();
                                switch (dialog.list.currentNode.Value.getButtonName())
                                {
                                    #region Sound
                                    case "Music":
                                        {
                                            dialog.Data[text] = (float)dialog.SlideMods[text].CurrentValue;
                                            if ((bool)dialog.OptModDic["Sound"].returnCurrentValue())
                                            {
                                                game.Sounds.MusicVolume = (float)dialog.SlideMods[text].CurrentValue;
                                            }
                                            break;
                                        }
                                    case "Effect":
                                        {
                                            dialog.Data[text] = (float)dialog.SlideMods[text].CurrentValue;
                                            if ((bool)dialog.OptModDic["Sound"].returnCurrentValue())
                                            {
                                                game.Sounds.EffectVolume = (float)dialog.SlideMods[text].CurrentValue;
                                            }
                                            break;
                                        }
                                    #endregion

                                    case "Detail":
                                        {
                                            dialog.Data[text] = (float)dialog.SlideMods[text].CurrentValue;
                                            break;
                                        }
                                    default:
                                        break;
                                }
                            }
                            #endregion
                        }

                    }
                    #endregion

                    #region Key Right
                    else if (game.Input.HasKeyJustBeenPressed("Menu.Right"))
                    {
                        if (counter == this.dialog.list.Count + 1)
                        {
                            if (this.dialog.accept.GetActive())
                            {
                                this.dialog.accept.resetActive();
                                this.dialog.decline.SetActive();
                            }
                            else
                            {
                                this.dialog.accept.SetActive();
                                this.dialog.decline.resetActive();
                            }
                        }
                        else
                        {
                            string text = this.dialog.list.currentNode.Value.getButtonName();
                            #region OptionModulationButton
                            if (this.dialog.OptModDic.ContainsKey(text))
                            {
                                this.dialog.OptModDic[text].plus.SetActive();
                                this.dialog.OptModDic[text].minus.resetActive();
                                this.dialog.OptModDic[text].plus.ButtonPressed();
                                switch (dialog.list.currentNode.Value.getButtonName())
                                {
                                    #region Resolution
                                    case "Resolution":
                                        {
                                            SetGameResolution((string)dialog.OptModDic[text].returnCurrentValue(), text);
                                            break;
                                        }
                                    #endregion
                                    #region Fullscreen
                                    case "Fullscreen":
                                        {
                                            game.Graphics.Fullscreen = (bool)dialog.OptModDic[text].returnCurrentValue();
                                            dialog.Data[text] = (bool)dialog.OptModDic[text].returnCurrentValue();
                                            break;
                                        }
                                    #endregion
                                    #region Anti Aliasing
                                    case "Anti Aliasing":
                                        {
                                            dialog.Data[text] = (string)dialog.OptModDic[text].returnCurrentValue();
                                            switch ((string)dialog.Data[text])
                                            {
                                                case "0x":
                                                    {
                                                        game.Graphics.MultiSampleType = MultiSampleType.None;
                                                        break;
                                                    }
                                                case "2x":
                                                    {
                                                        game.Graphics.MultiSampleType = MultiSampleType.TwoSamples;
                                                        break;
                                                    }
                                                case "4x":
                                                    {
                                                        game.Graphics.MultiSampleType = MultiSampleType.FourSamples;
                                                        break;
                                                    }
                                                case "8x":
                                                    {
                                                        game.Graphics.MultiSampleType = MultiSampleType.EightSamples;
                                                        break;
                                                    }
                                                case "16x":
                                                    {
                                                        game.Graphics.MultiSampleType = MultiSampleType.SixteenSamples;
                                                        break;
                                                    }
                                                default:
                                                    break;
                                            }
                                            break;
                                        }
                                    #endregion
                                    #region Sounds
                                    case "Sound":
                                        {
                                            dialog.Data[text] = (bool)dialog.OptModDic[text].returnCurrentValue();
                                            if ((bool)dialog.OptModDic[text].returnCurrentValue() == false)
                                            {
                                                game.Sounds.EffectVolume = 0f;
                                                game.Sounds.MusicVolume = 0f;
                                            }
                                            else
                                            {
                                                game.Sounds.MusicVolume = (float)dialog.SlideMods["Music"].CurrentValue;
                                                game.Sounds.EffectVolume = (float)dialog.SlideMods["Effect"].CurrentValue;
                                            }
                                            break;
                                        }
                                    #endregion
                                    default:
                                        SetDataValues(text, dialog.OptModDic[text].returnCurrentValue());
                                        break;
                                }
                            }
                            #endregion

                            #region Slidebar
                            else if (dialog.SlideMods.ContainsKey(text))
                            {
                                this.dialog.SlideMods[text].plus.SetActive();
                                this.dialog.SlideMods[text].minus.resetActive();
                                this.dialog.SlideMods[text].plus.ButtonPressed();
                                switch (dialog.list.currentNode.Value.getButtonName())
                                {
                                    #region Sound
                                    case "Music":
                                        {
                                            dialog.Data[text] = (float)dialog.SlideMods[text].CurrentValue;
                                            if ((bool)dialog.OptModDic["Sound"].returnCurrentValue())
                                            {
                                                game.Sounds.MusicVolume = (float)dialog.SlideMods[text].CurrentValue;
                                            }
                                            break;
                                        }
                                    case "Effect":
                                        {
                                            dialog.Data[text] = (float)dialog.SlideMods[text].CurrentValue;
                                            if ((bool)dialog.OptModDic["Sound"].returnCurrentValue())
                                            {
                                                game.Sounds.EffectVolume = (float)dialog.SlideMods[text].CurrentValue;
                                            }
                                            break;
                                        }
                                    #endregion

                                    case "Detail":
                                        {
                                            dialog.Data[text] = (float)dialog.SlideMods[text].CurrentValue;
                                            break;
                                        }
                                    default:
                                        break;
                                }
                            }
                            #endregion
                        }
                    }
                    #endregion

                    #region Key Up
                    if (game.Input.HasKeyJustBeenPressed("Menu.Up"))
                    {
                        if (dialog.list.currentNode.Equals(dialog.list.First)
                            && dialog.list.currentNode.Value.GetActive())
                        {
                            resetButton();
                            dialog.list.currentNode.Value.resetActive();
                            dialog.accept.SetActive();
                            counter = dialog.list.Count + 1;
                        }
                        else if (counter == dialog.list.Count + 1)
                        {
                            dialog.accept.resetActive();
                            dialog.decline.resetActive();
                            counter = 0;
                            dialog.list.currentNode = dialog.list.Last;
                            dialog.list.currentNode.Value.SetActive();
                        }
                        else
                        {
                            resetButton();
                            dialog.list.Previous();
                        }
                    }
                    #endregion

                    #region Key Down
                    if (game.Input.HasKeyJustBeenPressed("Menu.Down"))
                    {
                        if (dialog.list.currentNode.Equals(dialog.list.Last)
                            && dialog.list.currentNode.Value.GetActive())
                        {
                            resetButton();
                            dialog.list.currentNode.Value.resetActive();
                            dialog.accept.SetActive();
                            counter = dialog.list.Count + 1;
                        }
                        else if (counter == dialog.list.Count + 1)
                        {
                            dialog.accept.resetActive();
                            dialog.decline.resetActive();
                            counter = 0;
                            dialog.list.currentNode = dialog.list.First;
                            dialog.list.currentNode.Value.SetActive();
                        }
                        else
                        {
                            resetButton();
                            dialog.list.Next();
                        }
                    }
                    #endregion

                    #region Key Enter
                    else if (game.Input.HasKeyJustBeenPressed("Menu.Ok"))
                    {
                        string text = this.dialog.list.currentNode.Value.getButtonName();
                        if (counter == this.dialog.list.Count + 1)
                        {
                            if (this.dialog.accept.GetActive())
                            {
                                if (Resolution != new Vector2(game.Graphics.Device.Viewport.Width,
                                game.Graphics.Device.Viewport.Height))
                                {
                                    this.oldResolution = new Vector2(game.Graphics.Device.Viewport.Width,
                                          game.Graphics.Device.Viewport.Height);
                                    prompt = new Prompt(game, "Resolution" + "1", game.Graphics.Device.Viewport.Width,
                                                game.Graphics.Device.Viewport.Height, new string[] { 
                                                    "You about to change the resolution.",
                                                    "If you decide to choose a new resolution",
                                                    "you will have 15 Seconds to accept the new","resolution!" },
                                                false);
                                    prompt.accept += new PromptEventHandler(initiateResolutionAccept);
                                    prompt.decline += new PromptEventHandler(ResCancel);
                                    prompt.intiateButtons();
                                    this.Activity = false;
                                }
                                else
                                {
                                    steuerung.selection(this.dialog.accept.getButtonName(), "Optionen");    
                                }
                            }
                            else
                            {
                                steuerung.selection(this.dialog.decline.getButtonName(), "Optionen");
                            }
                        }
                        else if (!this.dialog.OptModDic.ContainsKey(text) &&
                            counter != this.dialog.list.Count + 1)
                        {
                            switch (dialog.list.currentNode.Value.getButtonName())
                            {
                                case "Playername":
                                    {
                                        steuerung.selection(this.dialog.list.currentNode.Value.getButtonName(),
                                            "Optionen");
                                        break;
                                    }
                                default:
                                    break;
                            }
                        }
                        else if (dialog.list.currentNode.Value.getButtonName().Equals("Resolution"))
                        {
                            if (Resolution != new Vector2(game.Graphics.Device.Viewport.Width,
                                                   game.Graphics.Device.Viewport.Height))
                            {
                                this.oldResolution = new Vector2(game.Graphics.Device.Viewport.Width,
                                game.Graphics.Device.Viewport.Height);                               
                                prompt = new Prompt(game,"Resolution" + "1", game.Graphics.Device.Viewport.Width,
                                                game.Graphics.Device.Viewport.Height, new string[] { 
                                                    "You about to change the resolution.",
                                                    "If you decide to choose a new resolution",
                                                    "you will have 15 Seconds to accept the new","resolution!" },
                                                false);
                                prompt.accept += new PromptEventHandler(initiateResolution);
                                prompt.decline += new PromptEventHandler(ResCancel);
                                prompt.intiateButtons();
                                this.Activity = false; 
                            }
                        }
                    }
                    #endregion

                    #region Key Escape
                    else if (game.Input.HasKeyJustBeenPressed("Game.Menu"))
                    {
                        steuerung.discardChanges();
                    }
                    #endregion
                    #endregion
                }
                else if (district == 3)
                {
                    #region Mouse

                    #region Buttonaktivierung
                    #region Listbox
                    #region Listbox Accept
                    if (isMouseInBox(listbox.Buttons.Last.Value) && new Vector2(game.Input.MousePosition.X,
                        game.Input.MousePosition.Y) != game.GameStates.Menu.Mouse)
                    {
                        counter = listbox.List.Count + 1;
                        listbox.Buttons.Last.Value.SetActive();
                        listbox.Buttons.Last.Previous.Value.resetActive();
                        listbox.Buttons.First.Value.resetActive();
                        listbox.List.currentNode.Value.resetActive();
                        listbox.Scrollbar.BackBarActive = false;
                    }
                    #endregion
                    #region Listbox Decline
                    else if (isMouseInBox(listbox.Buttons.First.Value) && new Vector2(game.Input.MousePosition.X,
                        game.Input.MousePosition.Y) != game.GameStates.Menu.Mouse)
                    {
                        counter = listbox.List.Count + 1;
                        listbox.Buttons.First.Value.SetActive();
                        listbox.Buttons.Last.Value.resetActive();
                        listbox.Buttons.Last.Previous.Value.resetActive();
                        listbox.List.currentNode.Value.resetActive();
                        listbox.Scrollbar.BackBarActive = false;
                    }
                    #endregion

                    #region Listbox Reset
                    else if(isMouseInBox(listbox.Buttons.Last.Previous.Value) && new Vector2(game.Input.MousePosition.X,
                        game.Input.MousePosition.Y) != game.GameStates.Menu.Mouse)
                    {
                        counter = listbox.List.Count + 1;
                        listbox.Buttons.Last.Previous.Value.SetActive();
                        listbox.Buttons.First.Value.resetActive();
                        listbox.Buttons.Last.Value.resetActive();
                        listbox.List.currentNode.Value.resetActive();
                        listbox.Scrollbar.BackBarActive = false;
                    }
                    #endregion
                    #endregion

                    #region Scrollbar
                    #region Scrollbar UP
                    else if (isMouseInBox(listbox.Scrollbar.scrollUp) && new Vector2(game.Input.MousePosition.X,
                        game.Input.MousePosition.Y) != game.GameStates.Menu.Mouse)
                    {
                        counter = listbox.List.Count + 2;
                        listbox.Buttons.Last.Value.resetActive();
                        listbox.Buttons.Last.Previous.Value.resetActive();
                        listbox.Buttons.First.Value.resetActive();
                        listbox.List.currentNode.Value.resetActive();
                        listbox.Scrollbar.scrollUp.SetActive();
                        listbox.Scrollbar.scrollDown.resetActive();
                        listbox.Scrollbar.BackBarActive = false;
                    }
                    #region Scrollbar BAR
                    else if (isMouseInRec(listbox.Scrollbar.BackBar) && new Vector2(game.Input.MousePosition.X,
                   game.Input.MousePosition.Y) != game.GameStates.Menu.Mouse)
                    {
                        counter = listbox.List.Count + 2;
                        listbox.Buttons.Last.Value.resetActive();
                        listbox.Buttons.Last.Previous.Value.resetActive();
                        listbox.Buttons.First.Value.resetActive();
                        listbox.List.currentNode.Value.resetActive();
                        listbox.Scrollbar.scrollUp.resetActive();
                        listbox.Scrollbar.scrollDown.resetActive();
                        listbox.Scrollbar.BackBarActive = true;
                    }
                        #endregion
                    #region Scrollbar DOWN
                    else if (isMouseInBox(listbox.Scrollbar.scrollDown) && new Vector2(game.Input.MousePosition.X,
                        game.Input.MousePosition.Y) != game.GameStates.Menu.Mouse)
                    {
                        counter = listbox.List.Count + 2;
                        listbox.Buttons.First.Value.resetActive();
                        listbox.Buttons.Last.Value.resetActive();
                        listbox.Buttons.Last.Previous.Value.resetActive();
                        listbox.List.currentNode.Value.resetActive();
                        listbox.Scrollbar.scrollUp.resetActive();
                        listbox.Scrollbar.scrollDown.SetActive();
                        listbox.Scrollbar.BackBarActive = false;
                    }
                    #endregion
                    #endregion
                    #endregion

                    else if (!isMouseInBox(listbox.Buttons.Last.Value) && !isMouseInBox(listbox.Buttons.First.Value)
                        && !isMouseInBox(listbox.Buttons.Last.Previous.Value) && !isMouseInBox(listbox.Scrollbar.scrollDown)
                        && !isMouseInRec(listbox.Scrollbar.BackBar) && !isMouseInBox(listbox.Scrollbar.scrollUp)
                        && new Vector2(game.Input.MousePosition.X, game.Input.MousePosition.Y) != game.GameStates.Menu.Mouse)
                    {
                        if (counter != listbox.List.Count + 1 && counter != listbox.List.Count+2)
                        {
                            listbox.Buttons.First.Value.resetActive();
                            listbox.Buttons.Last.Previous.Value.resetActive();
                            listbox.Buttons.Last.Value.resetActive();
                            listbox.List.currentNode.Value.SetActive();
                            listbox.Scrollbar.scrollUp.resetActive();
                            listbox.Scrollbar.scrollDown.resetActive();
                            listbox.Scrollbar.BackBarActive = false;
                        }
                        foreach (Button g in listbox.List)
                        {
                            string text = g.getButtonName();
                            if (isMouseInBox(g) && g.Visiblity)
                            {
                                counter = 0;
                                resetButton();
                                while (!listbox.List.currentNode.Value.Equals(g))
                                {
                                    listbox.List.Next();
                                }
                                if (listbox.SlideBars.ContainsKey(text))
                                {
                                    if (isMouseInBox(listbox.SlideBars[text].plus))
                                    {
                                        listbox.SlideBars[text].plus.SetActive();
                                        listbox.SlideBars[text].minus.resetActive();
                                    }
                                    else if (isMouseInBox(listbox.SlideBars[text].minus))
                                    {
                                        listbox.SlideBars[text].minus.SetActive();
                                        listbox.SlideBars[text].plus.resetActive();
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    #region Leftbutton
                    if (game.Input.HasKeyJustBeenPressed("Menu.Select"))
                    {
                        #region Button Reset, Accept and Decline
                        if (isMouseInBox(listbox.Buttons.Last.Value) && listbox.Buttons.Last.Value.GetActive())
                        {
                            steuerung.selection(listbox.Buttons.Last.Value.getButtonName(), "OptionenControl");
                        }
                        else if (isMouseInBox(listbox.Buttons.First.Value) && listbox.Buttons.First.Value.GetActive())
                        {
                            steuerung.selection(listbox.Buttons.First.Value.getButtonName(), "OptionenControl");
                        }
                        else if (isMouseInBox(listbox.Buttons.Last.Previous.Value) && listbox.Buttons.Last.Previous.Value.GetActive())
                        {
                            checkforReset();
                        }
                        #endregion
                        else 
                        #region Scrollbar
                        if (isMouseInBox(listbox.Scrollbar.scrollUp) && listbox.Scrollbar.scrollDown.GetActive())  //&& listbox.Buttons.Last.Value.GetActive())
                        {
                            listbox.Scrollbar.scrollUp.ButtonPressed();
                        }
                        else if (isMouseInBox(listbox.Scrollbar.scrollDown) && listbox.Scrollbar.scrollDown.GetActive())//&& listbox.Buttons.First.Value.GetActive())
                        {
                            listbox.Scrollbar.scrollDown.ButtonPressed();
                        }
                        else if (isMouseInRec(listbox.Scrollbar.BackBar) && listbox.Scrollbar.BackBarActive)
                        {
                            listbox.Scrollbar.BackBarPressed(new Vector2(game.Input.MousePosition.X,
                                                             game.Input.MousePosition.Y));
                        }
                        #endregion
                        else
                        {
                            foreach (Button g in listbox.List)
                            {
                                if ((g.getButtonName().Equals("Translation") || g.getButtonName().Equals("Acceleration Level") ||
                                    g.getButtonName().Equals("Brake") || g.getButtonName().Equals("Banking") ||
                                    g.getButtonName().Equals("Drift") || g.getButtonName().Equals("Auto Level") ||
                                    g.getButtonName().Equals("Rolling") || g.getButtonName().Equals("Mouse Intensity"))&&
                                    g.GetActive())
                                {
                                    string text = g.getButtonName();
                                    if (isMouseInBox(listbox.SlideBars[text].plus))
	                                {
                                        listbox.SlideBars[text].plus.SetActive();
                                        listbox.SlideBars[text].minus.resetActive();
                                        listbox.SlideBars[text].plus.ButtonPressed();
                                        SetDataValues(text, listbox.SlideBars[text].CurrentValue);
	                                }
                                    else if (isMouseInBox(listbox.SlideBars[text].minus))
                                    {
                                        listbox.SlideBars[text].minus.SetActive();
                                        listbox.SlideBars[text].plus.resetActive();
                                        listbox.SlideBars[text].minus.ButtonPressed();
                                        SetDataValues(text, listbox.SlideBars[text].CurrentValue);
                                    }
                                    #region Slidebar Barpressed
                                    else if (isMouseInRec(listbox.SlideBars[text].BarRec))
                                    {
                                        listbox.SlideBars[text].minus.SetActive();
                                        listbox.SlideBars[text].minus.resetActive();
                                        listbox.SlideBars[text].BarPressed(game.GameStates.Menu.MousePosition);
                                        SetDataValues(text, listbox.SlideBars[text].CurrentValue);
                                    }
                                    #endregion
                                    
                                }
                                else
                                {
                                    if (isMouseInBox(g))
                                    {
                                        if (!(g.getButtonName().Equals("Primary Fire") ||
                                            g.getButtonName().Equals("Secondary Fire")) && g.GetActive())
                                        {
                                            prompt = new Prompt(game, g.getButtonName()+"1", game.Graphics.Device.Viewport.Width,
                                              game.Graphics.Device.Viewport.Height, 
                                              new string[] { "Please press the new Key:" }, false, true);
                                                prompt.accept += new PromptEventHandler(checkforSaving);
                                                prompt.decline += new PromptEventHandler(Cancel);
                                                prompt.intiateButtons();
                                                this.Activity = false;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    #endregion
                    #endregion

                    #region Keyboard

                    #region Key Left
                    if (game.Input.HasKeyJustBeenPressed("Menu.Left"))
                    {
                        if (counter == listbox.List.Count + 1)
                        {
                            if (listbox.Buttons.Last.Value.GetActive())
                            {
                                listbox.Buttons.Last.Previous.Value.SetActive();
                                listbox.Buttons.Last.Value.resetActive();
                                listbox.Buttons.First.Value.resetActive();
                            }
                            else if (listbox.Buttons.Last.Previous.Value.GetActive())
                            {
                                listbox.Buttons.Last.Previous.Value.resetActive();
                                listbox.Buttons.Last.Value.resetActive();
                                listbox.Buttons.First.Value.SetActive();
                            }
                            else if (listbox.Buttons.First.Value.GetActive())
                            {
                                listbox.Buttons.Last.Value.SetActive();
                                listbox.Buttons.Last.Previous.Value.resetActive();
                                listbox.Buttons.First.Value.resetActive();
                            }
                        }
                        else if (counter != listbox.List.Count + 1)
                        {
                            listbox.Buttons.First.Value.resetActive();
                            listbox.Buttons.Last.Previous.Value.resetActive();
                            listbox.Buttons.Last.Value.resetActive();
                            listbox.List.currentNode.Value.SetActive();
                            foreach (Button g in listbox.List)
                            {
                                if ((listbox.List.currentNode.Value.Equals(g)) &&
                                    listbox.SlideBars.ContainsKey(g.getButtonName()))
                                {
                                    string text = g.getButtonName();
                                    this.listbox.SlideBars[text].minus.SetActive();
                                    listbox.SlideBars[text].plus.resetActive();
                                    listbox.SlideBars[text].minus.ButtonPressed();
                                    SetDataValues(text, listbox.SlideBars[text].CurrentValue);
                                }
                            }
                        }
                    }
                    #endregion

                    #region Key Right
                    else if (game.Input.HasKeyJustBeenPressed("Menu.Right"))
                    {
                        if (counter == listbox.List.Count + 1)
                        {
                            if (listbox.Buttons.Last.Value.GetActive())
                            {
                                listbox.Buttons.Last.Previous.Value.resetActive();
                                listbox.Buttons.Last.Value.resetActive();
                                listbox.Buttons.First.Value.SetActive();
                            }
                            else if (listbox.Buttons.Last.Previous.Value.GetActive())
                            {
                                listbox.Buttons.Last.Previous.Value.resetActive();
                                listbox.Buttons.Last.Value.SetActive();
                                listbox.Buttons.First.Value.resetActive();
                            }
                            else if (listbox.Buttons.First.Value.GetActive())
                            {
                                listbox.Buttons.Last.Value.resetActive();
                                listbox.Buttons.Last.Previous.Value.SetActive();
                                listbox.Buttons.First.Value.resetActive();
                            }
                        }
                        else if (counter != listbox.List.Count + 1)
                        {
                            listbox.Buttons.First.Value.resetActive();
                            listbox.Buttons.Last.Previous.Value.resetActive();
                            listbox.Buttons.Last.Value.resetActive();
                            listbox.List.currentNode.Value.SetActive();
                            foreach (Button g in listbox.List)
                            {
                                if ((listbox.List.currentNode.Value.Equals(g)) && 
                                    listbox.SlideBars.ContainsKey(g.getButtonName()))
                                {
                                    string text = g.getButtonName();
                                    this.listbox.SlideBars[text].plus.SetActive();
                                    listbox.SlideBars[text].minus.resetActive();
                                    listbox.SlideBars[text].plus.ButtonPressed();
                                    SetDataValues(text, listbox.SlideBars[text].CurrentValue);
                                }
                            }
                        }
                    }
                    #endregion

                    #region Key Up
                    if (game.Input.HasKeyJustBeenPressed("Menu.Up"))
                    {
                        if (listbox.List.currentNode.Equals(listbox.List.First)
                            && listbox.List.currentNode.Value.GetActive())
                        {
                            resetButton();
                            listbox.List.currentNode.Value.resetActive();
                            listbox.Buttons.Last.Value.SetActive();
                            counter = listbox.List.Count + 1;
                        }
                        else if (counter == listbox.List.Count + 1)
                        {
                            listbox.Buttons.Last.Value.resetActive();
                            listbox.Buttons.First.Value.resetActive();
                            listbox.Buttons.Last.Previous.Value.resetActive();
                            counter = 0;
                            Listbox.List.currentNode = listbox.List.Last;
                            listbox.List.currentNode.Value.SetActive();
                        }
                        else
                        {
                            listbox.Buttons.First.Value.resetActive();
                            listbox.Buttons.Last.Previous.Value.resetActive();
                            listbox.Buttons.Last.Value.resetActive();
                            resetButton();
                            listbox.List.Previous();
                        }
                    }
                    #endregion

                    #region Key Down
                    if (game.Input.HasKeyJustBeenPressed("Menu.Down"))
                    {
                        if (listbox.List.currentNode.Equals(listbox.List.Last)
                            && listbox.List.currentNode.Value.GetActive())
                        {
                            resetButton();
                            listbox.List.currentNode.Value.resetActive();
                            listbox.Buttons.Last.Value.SetActive();
                            counter = listbox.List.Count + 1;
                        }
                        else if (counter == listbox.List.Count + 1)
                        {
                            listbox.Buttons.Last.Value.resetActive();
                            listbox.Buttons.First.Value.resetActive();
                            listbox.Buttons.Last.Previous.Value.resetActive();
                            counter = 0;
                            listbox.List.currentNode = listbox.List.First;
                            listbox.List.currentNode.Value.SetActive();
                        }
                        else
                        {
                            listbox.Buttons.First.Value.resetActive();
                            listbox.Buttons.Last.Previous.Value.resetActive();
                            listbox.Buttons.Last.Value.resetActive();
                            resetButton();
                            listbox.List.Next();
                        }
                    }
                    #endregion

                    #region Key Enter
                    else if (game.Input.HasKeyJustBeenPressed("Menu.Ok"))
                    {
                        if (listbox.Buttons.Last.Value.GetActive())
                        {
                            steuerung.selection(listbox.Buttons.Last.Value.getButtonName(), "OptionenControl");
                        }
                        else if (listbox.Buttons.First.Value.GetActive())
                        {
                            steuerung.selection(listbox.Buttons.First.Value.getButtonName(), "OptionenControl");
                        }
                        else if (listbox.Buttons.Last.Previous.Value.GetActive())
                        {
                            checkforReset();
                        }
                        else
                        {
                            Button g = listbox.List.currentNode.Value;
                            if (!(g.getButtonName().Equals("Translation") || g.getButtonName().Equals("Acceleration Level") ||
                                    g.getButtonName().Equals("Brake") || g.getButtonName().Equals("Banking") ||
                                    g.getButtonName().Equals("Drift") || g.getButtonName().Equals("Auto Level") ||
                                    g.getButtonName().Equals("Rolling") || g.getButtonName().Equals("Mouse Intensity")))
                            {
                                if (!(g.getButtonName().Equals("Primary Fire") ||
                                    g.getButtonName().Equals("Secondary Fire")))
                                {
                                    prompt = new Prompt(game, g.getButtonName()+"1", game.Graphics.Device.Viewport.Width,
                                      game.Graphics.Device.Viewport.Height, new string[] { "Please press the new Key:" },
                                      false, true);
                                    prompt.accept += new PromptEventHandler(checkforSaving);
                                    prompt.decline += new PromptEventHandler(Cancel);
                                    prompt.intiateButtons();
                                    this.Activity = false;
                                }
                            }
                        }
                    }
                    #endregion

                    #region Key Escape
                    else if (game.Input.HasKeyJustBeenPressed("Game.Menu"))
                    {
                        steuerung.discardChanges();
                    }
                    #endregion

                    #endregion
                }

                #endregion

            }
        }

        /// <summary>
        /// Set the Values in Dictionary
        /// </summary>
        /// <param name="name">where the value is set</param>
        /// <param name="Value">Value which to set</param>
        private void SetDataValues(string name, object Value)
        {
            switch (name)
            {
                #region Grafik
                case "Shadow":
                    {
                        dialog.Data[name] = (bool)Value;
                        break;
                    }
                case "Fog":
                    {
                        dialog.Data[name] = (bool)Value;
                        break;
                    }
                #endregion
                #region Playersettings
                case "Difficult":
                    {
                        dialog.Data[name] = (int)Value;
                        break;
                    }
                #endregion
                #region Control
                case "Translation":
                    {
                        listbox.Data[name] = (float)Value;
                        break;
                    }
                case "Acceleration Level":
                    {
                        listbox.Data[name] = (float)Value;
                        break;
                    }
                case "Brake":
                    {
                        listbox.Data[name] = (float)Value;
                        break;
                    }
                case "Banking":
                    {
                        listbox.Data[name] = (float)Value / 100;
                        break;
                    }
                case "Drift":
                    {
                        listbox.Data[name] = (float)Value / 100;
                        break;
                    }
                case "Auto Level":
                    {
                        listbox.Data[name] = (float)Value / 10;
                        break;
                    }
                case "Rolling":
                    {
                        listbox.Data[name] = (float)Value;
                        break;
                    }
                case "Mouse Intensity":
                    {
                        listbox.Data[name] = ((float)Value - 5.0f) / 1000;
                        break;
                    }
                #endregion
                default:
                    break;
            }
        }

        #region Event Handler
        #region double Prompt for KeyCheck
        public void checkforSaving()
        {
            if (prompt.CurrentKey != Keys.None)
            {
                Keys chosen;
                string description, oldDescription;
                if (checkKey())
                {
                    chosen = prompt.CurrentKey;
                    description = listbox.List.currentNode.Value.getButtonName();
                    oldDescription = getoldDescitption();
                    prompt = new Prompt(game, description + "2", game.Graphics.Device.Viewport.Width,
                      game.Graphics.Device.Viewport.Height, new string[] { "The key \'"+chosen+"\' has already assigned to",
                          "\""+ oldDescription+"\"!","",
                          "Assign \""+ description+"\"","with the key \'" + chosen +"\'?"},
                      false, false);
                    this.Activity = false;
                    prompt.CurrentKey = chosen;
                    prompt.accept += new PromptEventHandler(SaveControl);
                    prompt.decline += new PromptEventHandler(Cancel);
                    prompt.intiateButtons();
                }
                else
                {
                    SaveControl();
                }
            }
            else
            {
                prompt.Text = new string[] { "You have not enter any key!", "Please enter a key or cancel this input" };
            }
        }

        #region helpMethods for checkforSaving
        private string getoldDescitption() 
         {
            foreach (string i in Listbox.Data.Keys)
            {
                if (prompt.CurrentKey.Equals(listbox.Data[i]))
                {
                   return i;
                }
            }
            return "";
        }
        #endregion

        private bool checkKey()
        {
            foreach (string i in Listbox.Data.Keys)
            {
                    if (prompt.CurrentKey.Equals(Listbox.Data[i]) && i!=prompt.Name.Substring(0,
                        listbox.List.currentNode.Value.getButtonName().Length))
                    {
                        return true;
                    }
            }
            return false;
        }
        #endregion
        public void SaveControl()
        {
            OptionList call = (OptionList)this.listbox.List;
            call.changed(listbox.List.currentNode.Value.getButtonName(), false, false);
            prompt.SetNewKey(listbox.Data, listbox.List.currentNode.Value.getButtonName());
            prompt.unloadPrompt();
            this.Activity = true;
            this.listbox.Activity = true;
        }

        public void checkResolutionAccept()
        {
            steuerung.Save();
            prompt.unloadPrompt();
            dialog.list.adjustButtons(dialog.startposition, "", false);
            game.GameStates.Menu.CurrentList.adjustButtons(new Vector2(), "Optionen", false);

            game.GameStates.Menu.Shifts.LeftShift.setPosition((new Vector2((int)((game.Graphics.Device.Viewport.Width
                        - 398 + 22)), (int)73)), 23, 72);
            game.GameStates.Menu.Shifts.RightShift.setPosition(new Vector2((int)game.Graphics.Device.Viewport.Width - 398 
                + 378 - 44,(int)73), 23, 72); 
        }

        public void checkResolution()
        {
            prompt.unloadPrompt();
            this.Activity = true;
            dialog.list.adjustButtons(dialog.startposition, "",false);
           // game.GameStates.Menu.CurrentList.adjustButtons(new Vector2(), "Optionen", false);
        }

        public void initiateResolutionAccept()
        {
            game.Graphics.SetScreenResolution((int)Resolution.X, (int)Resolution.Y);
            prompt = new Prompt(game, "Resolution" + "2", game.Graphics.Device.Viewport.Width,
                game.Graphics.Device.Viewport.Height, true, new string[] { "You have changed the resolution.",
                    "Do you want to use this resolution" , "and save your settings?","Time left to accept: "},
                     false, 15);
            this.Activity = false;
            prompt.accept += new PromptEventHandler(checkResolutionAccept);
            prompt.decline += new PromptEventHandler(ResCancel);
            prompt.intiateButtons();
        }

        public void initiateResolution()
        {
            game.Graphics.SetScreenResolution((int)Resolution.X, (int)Resolution.Y);
            prompt = new Prompt(game, "Resolution" + "2", game.Graphics.Device.Viewport.Width,
                game.Graphics.Device.Viewport.Height,true, new string[] { "You have changed the resolution.",
                    "Do you want to use this resolution?","Time left to accept: "},
                     false,15);
            this.Activity = false;
            prompt.accept += new PromptEventHandler(checkResolution);
            prompt.decline += new PromptEventHandler(ResCancel);
            prompt.intiateButtons();
        }
        

        public void Cancel()
        {
            prompt.unloadPrompt();
            this.Activity = true;
        }

        public void ResCancel()
        {
            //SetGameResolution((string)dialog.OptModDic[text].returnCurrentValue(), text); 
            game.Graphics.SetScreenResolution((int)oldResolution.X, (int)oldResolution.Y);
            dialog.OptModDic["Resolution"].setCurrentString("" + oldResolution.X + "x" + oldResolution.Y);
            prompt.unloadPrompt();
            this.Activity = true;
        }

        public void checkforReset()
        {
            prompt = new Prompt(game, "RESETCHECK", game.Graphics.Device.Viewport.Width,
                                             game.Graphics.Device.Viewport.Height,
                                             new string[] { "You are about to reset your settings!",
                                                 "All your settings will be lost after reseting.","",
                                                 "Do you want to reset your settings?" }, false);
            prompt.accept += new PromptEventHandler(SetControlStandard);
            prompt.decline += new PromptEventHandler(Cancel);
            prompt.intiateButtons();
            this.Activity = false;
        }

        /// <summary>
        /// Set the controlvalues to standard
        /// </summary>
        private void SetControlStandard()
        {
            if (listbox != null)
            {
                #region Back to Standard Values
                listbox.Data["Translation"] = 0.5f;
                listbox.SlideBars["Translation"].CurrentValue = 0.5f;
                listbox.SlideBars["Translation"].calculatingMakerPos(listbox.SlideBars["Translation"].CurrentValue);
                listbox.Data["Acceleration Level"] = 10f;
                listbox.SlideBars["Acceleration Level"].CurrentValue = 10f;
                listbox.SlideBars["Acceleration Level"].calculatingMakerPos(listbox.SlideBars["Acceleration Level"].CurrentValue);
                listbox.Data["Brake"] = 10f;
                listbox.SlideBars["Brake"].CurrentValue = 10f;
                listbox.SlideBars["Brake"].calculatingMakerPos(listbox.SlideBars["Brake"].CurrentValue);
                listbox.Data["Banking"] = 0.01f;
                listbox.SlideBars["Banking"].CurrentValue = 1.0f;
                listbox.SlideBars["Banking"].calculatingMakerPos(listbox.SlideBars["Banking"].CurrentValue);
                listbox.Data["Drift"] = 0.01f;
                listbox.SlideBars["Drift"].CurrentValue = 1.0f;
                listbox.SlideBars["Drift"].calculatingMakerPos(listbox.SlideBars["Drift"].CurrentValue);
                listbox.Data["Auto Level"] = 0.05f;
                listbox.SlideBars["Auto Level"].CurrentValue = 0.5f;
                listbox.SlideBars["Auto Level"].calculatingMakerPos(listbox.SlideBars["Auto Level"].CurrentValue);
                listbox.Data["Rolling"] = 0.8f;
                listbox.SlideBars["Rolling"].CurrentValue = 0.8f;
                listbox.SlideBars["Rolling"].calculatingMakerPos(listbox.SlideBars["Rolling"].CurrentValue);
                listbox.Data["Mouse Intensity"] = -0.001f;
                listbox.SlideBars["Mouse Intensity"].CurrentValue = 4.0f;
                listbox.SlideBars["Mouse Intensity"].calculatingMakerPos(listbox.SlideBars["Mouse Intensity"].CurrentValue);
                listbox.Data["Primary Fire"] = MouseButton.Left;
                listbox.Data["Secondary Fire"] = MouseButton.Right;
                listbox.Data["Left Drift"] = Keys.A;
                listbox.Data["Right Drift"] = Keys.D;
                listbox.Data["Brakes"] = Keys.S;
                listbox.Data["Acceleration"] = Keys.W;
                listbox.Data["Boost"] = Keys.Space;
                listbox.Data["Primary Weapon 1"] = Keys.D1;
                listbox.Data["Primary Weapon 2"] = Keys.D2;
                listbox.Data["Secondary Weapon 1"] = Keys.D3;
                listbox.Data["Secondary Weapon 2"] = Keys.D4;
                #endregion
                game.GameStates.Profil.writeProfilSteuerung(this.Listbox.Data);
                prompt.unloadPrompt();
                this.Activity = true;
            }
        }

        #endregion

        /// <summary>
        /// Reset the active of the OptionModulationButton and the Slidebars
        /// </summary>
        private void resetButton()
        {
            if (dialog != null)
            {
                string text = this.dialog.list.currentNode.Value.getButtonName();
                if (this.dialog.OptModDic.ContainsKey(text))
                {
                    this.dialog.OptModDic[text].plus.resetActive();
                    this.dialog.OptModDic[text].minus.resetActive();
                }
                else if (dialog.SlideMods.ContainsKey(text))
                {
                    this.dialog.SlideMods[text].plus.resetActive();
                    this.dialog.SlideMods[text].minus.resetActive();
                }
            }
            else if (listbox != null)
            {
                string text = listbox.List.currentNode.Value.getButtonName();
                if (listbox.SlideBars.ContainsKey(text))
                {
                    listbox.SlideBars[text].plus.resetActive();
                    listbox.SlideBars[text].minus.resetActive();
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
            if (visible)
            {
                if (SpriteBatch != null)
                {
                    foreach (OptionpartDescription i in xml.OptionParts)
                    {
                        if (i.Number == this.district)
                        {
                            SpriteBatch.Draw(background, new Rectangle(0, 0,
                                game.Graphics.Device.Viewport.Width, game.Graphics.Device.Viewport.Height),
                                Color.White);
                        }
                    }
                    if (district < 3)
                    {
                        dialog.Draw(gameTime, SpriteBatch);
                    }
                    else if (district == 3)
                    {
                        listbox.Draw(gameTime, SpriteBatch);
                    }
                    SpriteBatch.Draw(mouse, new Vector2(game.Input.MousePosition.X,
                        game.Input.MousePosition.Y), Color.White);
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
            foreach (OptionpartDescription i in xml.OptionParts)
            {
                if (i.Number == this.district)
                {
                    background = game.Content.Load<Texture2D>("Content\\Textures\\Menu\\" + i.BackgroundImage);
                    mouse = game.Content.Load<Texture2D>("Content\\Textures\\Menu\\" + i.Mouse);
                }
            }
            if (visible && isActive)
            {
                if (district < 3)
                {
                    dialog.LoadContent();
                }
                else if (district == 3)
                {
                    listbox.LoadContent(game);
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

        public bool isMouseInRec(Rectangle Check)
        {
            if ((game.GameStates.Menu.MousePosition.X >= Check.X) &&
                (game.GameStates.Menu.MousePosition.X <= Check.Right) &&
                (game.GameStates.Menu.MousePosition.Y >= Check.Y) &&
                (game.GameStates.Menu.MousePosition.Y <= Check.Bottom))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Set the value Resolution
        /// </summary>
        /// <param name="res">the resolution to set</param>
        private void SetGameResolution(string res, string text)
        {
            switch (res)
            {
                case "800x600":
                    {
                        if (Resolution != new Vector2(800, 600))
                        {
                            dialog.Data[text] = (string)dialog.OptModDic[text].returnCurrentValue();
                            Resolution = new Vector2(800, 600);
                        }
                        break;
                    }
                case "1024x600":
                    {
                        if (Resolution != new Vector2(1024, 600))
                        {
                            dialog.Data[text] = (string)dialog.OptModDic[text].returnCurrentValue();
                            Resolution = new Vector2(1024, 600);
                        }
                        break;
                    }
                case "1024x768":
                    {
                        if (Resolution != new Vector2(1024, 768))
                        {
                            dialog.Data[text] = (string)dialog.OptModDic[text].returnCurrentValue();
                            Resolution = new Vector2(1024, 768);
                        }
                        break;
                    }
                case "1280x768":
                    {
                        if (Resolution != new Vector2(1280, 768))
                        {
                            dialog.Data[text] = (string)dialog.OptModDic[text].returnCurrentValue();
                            Resolution = new Vector2(1280, 768);
                        }
                        break;
                    }
                case "1280x800":
                    {
                        if (Resolution != new Vector2(1280, 800))
                        {
                            dialog.Data[text] = (string)dialog.OptModDic[text].returnCurrentValue();
                            Resolution = new Vector2(1280, 800);
                        }
                        break;
                    }
                case "1280x960":
                    {
                        if (Resolution != new Vector2(1280, 960))
                        {
                            dialog.Data[text] = (string)dialog.OptModDic[text].returnCurrentValue();
                            Resolution = new Vector2(1280, 960);
                        }
                        break;
                    }
                case "1280x1024":
                    {
                        if (Resolution != new Vector2(1280, 1024))
                        {
                            dialog.Data[text] = (string)dialog.OptModDic[text].returnCurrentValue();
                            Resolution = new Vector2(1280, 1024);
                        }
                        break;
                    }
                case "1360x768":
                    {
                        if (Resolution != new Vector2(1360, 768))
                        {
                            dialog.Data[text] = (string)dialog.OptModDic[text].returnCurrentValue();
                            Resolution = new Vector2(1360, 768);
                        }
                        break;
                    }
                case "1440x900":
                    {
                        if (Resolution != new Vector2(1440, 900))
                        {
                            dialog.Data[text] = (string)dialog.OptModDic[text].returnCurrentValue();
                            Resolution = new Vector2(1440, 900);
                        }
                        break;
                    }
                case "1600x1200":
                    {
                        if (Resolution != new Vector2(1600, 1200))
                        {
                            dialog.Data[text] = (string)dialog.OptModDic[text].returnCurrentValue();
                            Resolution = new Vector2(1600, 1200);
                        }
                        break;
                    }
                case "1600x1280":
                    {
                        if (Resolution != new Vector2(1600, 1280))
                        {
                            dialog.Data[text] = (string)dialog.OptModDic[text].returnCurrentValue();
                            Resolution = new Vector2(1600, 1280);
                        }
                        break;
                    }
                case "1768x992":
                    {
                        if (Resolution != new Vector2(1768, 992))
                        {
                            dialog.Data[text] = (string)dialog.OptModDic[text].returnCurrentValue();
                            Resolution = new Vector2(1768, 992);
                        }
                        break;
                    }
                case "1856x1392":
                    {
                        if (Resolution != new Vector2(1856, 1392))
                        {
                            dialog.Data[text] = (string)dialog.OptModDic[text].returnCurrentValue();
                            Resolution = new Vector2(1856, 1392);
                        }
                        break;
                    }
                case "1920x1200":
                    {
                        if (Resolution != new Vector2(1920, 1200))
                        {
                            dialog.Data[text] = (string)dialog.OptModDic[text].returnCurrentValue();
                            Resolution = new Vector2(1920, 1200);
                        }
                        break;
                    }
                default:
                    break;
            }
        }

        //Für die Unteschiedlichen Untermenüs
        public enum DistrictEnum
        {
            Spielereinstellungen = 0,
            Grafik = 1,
            Sound = 2,
            Steuerung = 3
        }

    }
}