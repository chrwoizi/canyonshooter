using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CanyonShooter.Engine.Graphics
{
    public class Graphics : IGraphics
    {
        private static readonly int MAX_POINTLIGHT_COUNT = 5;

        private ICanyonShooterGame game;

        private GraphicsDeviceManager deviceManager;

        private ShaderSelector shaderSelector;

        static private bool fullscreen = false;
        static private int refreshrate = 60;
        static private int screenWidth = 1024;
        static private int screenHeight = 768;
        //static private int multiSampleQuality = 1;
        static private MultiSampleType multiSampleType = MultiSampleType.None;
        static private bool vsync = false;

        private bool shadowMappingSupportedOverride = false;
        private bool shadowMappingSupportedOverrideValue = false;

        private bool shaderModel3SupportedOverride = false;
        private bool shaderModel3SupportedOverrideValue = false;  // value auf true gesetzt, für cmdline parameter.

        private bool allowFogShaders = true;

        public Graphics(ICanyonShooterGame game, GraphicsDeviceManager _deviceManager)
        {
            this.game = game;
            deviceManager = _deviceManager;

            // Falls NVIDIA PerfHud aktiviert ist, soll der entsprechende Adapter gewählt werden.
            deviceManager.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(deviceManager_PreparingDeviceSettings);
            deviceManager.DeviceCreated += new EventHandler(deviceManager_DeviceCreated);
        }

        void deviceManager_DeviceCreated(object sender, EventArgs e)
        {
            updateLocals();
            InitializeShaderSelector(game);
        }

        private void InitializeShaderSelector(ICanyonShooterGame game)
        {
            // initialize shader selector
            shaderSelector = new ShaderSelector(game);
            shaderSelector.RegisterEffect("Color");
            shaderSelector.RegisterEffect("ColorConstantsInstancing");
            shaderSelector.RegisterEffect("ColorHardwareInstancing");
            shaderSelector.RegisterEffect("Depth");
            shaderSelector.RegisterEffect("DepthConstantsInstancing");
            shaderSelector.RegisterEffect("DepthHardwareInstancing");
            shaderSelector.RegisterEffect("Light");
            shaderSelector.RegisterEffect("LightConstantsInstancing");
            shaderSelector.RegisterEffect("LightHardwareInstancing");
            shaderSelector.RegisterEffect("Shadow");
            shaderSelector.RegisterEffect("ShadowConstantsInstancing");
            shaderSelector.RegisterEffect("ShadowHardwareInstancing");
            shaderSelector.RegisterEffect("Texture");
            shaderSelector.RegisterEffect("TextureConstantsInstancing");
            shaderSelector.RegisterEffect("TextureHardwareInstancing");
            shaderSelector.RegisterEffect("Wireframe");
            shaderSelector.RegisterEffect("WireframeConstantsInstancing");
            shaderSelector.RegisterEffect("WireframeHardwareInstancing");
            shaderSelector.RegisterEffect("NormalMapping");
            shaderSelector.InitializeDepthShaders();
        }

        private void updateLocals()
        {
            fullscreen          = deviceManager.IsFullScreen;
            refreshrate         = deviceManager.GraphicsDevice.DisplayMode.RefreshRate;
            screenWidth         = deviceManager.GraphicsDevice.DisplayMode.Width;
            screenHeight        = deviceManager.GraphicsDevice.DisplayMode.Height;
            //multiSampleQuality  = deviceManager.GraphicsDevice.PresentationParameters.MultiSampleQuality;
            multiSampleType     = deviceManager.GraphicsDevice.PresentationParameters.MultiSampleType;
            vsync               = deviceManager.SynchronizeWithVerticalRetrace;
        }

        private void updatePresentationParameters()
        {
            PresentationParameters pp = deviceManager.GraphicsDevice.PresentationParameters.Clone();

            pp.IsFullScreen = fullscreen;
            pp.FullScreenRefreshRateInHz = fullscreen ? refreshrate : 0;
            pp.MultiSampleType = multiSampleType;
            //pp.MultiSampleQuality = multiSampleType != MultiSampleType.None ? multiSampleQuality : 0;

            deviceManager.GraphicsDevice.Reset(pp);
        }

        /// <summary>
        /// Event. Wählt den NVIDIA PerfHUD Grafikadapter aus, falls er verfügbar ist.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void deviceManager_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            foreach (GraphicsAdapter curAdapter in GraphicsAdapter.Adapters)
            {
                if (curAdapter.Description.Contains("NVIDIA PerfHUD"))
                {
                    e.GraphicsDeviceInformation.Adapter = curAdapter;
                    e.GraphicsDeviceInformation.DeviceType = DeviceType.Reference;
                    break;
                }
            }

            e.GraphicsDeviceInformation.PresentationParameters.IsFullScreen = fullscreen;
            e.GraphicsDeviceInformation.PresentationParameters.FullScreenRefreshRateInHz = fullscreen ? refreshrate : 0;
            e.GraphicsDeviceInformation.PresentationParameters.MultiSampleType = multiSampleType;
            
            //e.GraphicsDeviceInformation.PresentationParameters.MultiSampleQuality = multiSampleType != MultiSampleType.None ? multiSampleQuality : 0;
        }

        #region IGraphics Members

        public int MaxPointLights
        {
            get { return MAX_POINTLIGHT_COUNT; }
        }
        
        public GraphicsDevice Device
        {
            get
            {
                return deviceManager.GraphicsDevice;
            }
        }

        public IShaderSelector ShaderSelector
        {
            get
            {
                return shaderSelector;
            }
        }

        public bool Fullscreen
        {
            get
            {
                return fullscreen;
            }
            set
            {
                fullscreen = value;
                deviceManager.IsFullScreen = fullscreen;
                deviceManager.ApplyChanges();
            }
        }

        public int Refreshrate
        {
            get
            {
                return refreshrate;
            }
            set
            {
                refreshrate = value;
                updatePresentationParameters();
            }
        }

        public int ScreenWidth
        {
            get
            {
                return screenWidth;
            }
        }

        public int ScreenHeight
        {
            get
            {
                return screenHeight;
            }
        }

        public bool SetScreenResolution(int width, int height)
        {
            try
            {
                deviceManager.PreferredBackBufferWidth = width;
                deviceManager.PreferredBackBufferHeight = height;
                deviceManager.ApplyChanges();
                screenWidth = width;
                screenHeight = height;
            }
            catch(Exception)
            {
                return false;
            }
            return true;
        }

        /*public int MultiSampleQuality
        {
            get
            {

                return multiSampleQuality;
            }
            set
            {
                multiSampleQuality = value;
                deviceManager.GraphicsDevice.Reset();
            }
        }*/

        public MultiSampleType MultiSampleType
        {
            get
            {
                return multiSampleType;
            }
            set
            {
                if (GraphicsAdapter.DefaultAdapter.CheckDeviceMultiSampleType(DeviceType.Hardware, deviceManager.GraphicsDevice.PresentationParameters.BackBufferFormat, fullscreen, value))
                {
                    multiSampleType = value;
                    deviceManager.PreferMultiSampling = multiSampleType != MultiSampleType.None;
                    deviceManager.ApplyChanges();
                }
            }
        }

        public bool VSync
        {
            get
            {
                return vsync;
            }
            set
            {
                deviceManager.SynchronizeWithVerticalRetrace = value;
                deviceManager.ApplyChanges();
            }
        }

        public bool ShaderModel3Supported
        {
            get
            {
                if(shaderModel3SupportedOverride) return shaderModel3SupportedOverrideValue; // more performance for debug mode
                ShaderProfile vs = deviceManager.GraphicsDevice.GraphicsDeviceCapabilities.MaxVertexShaderProfile;
                ShaderProfile ps = deviceManager.GraphicsDevice.GraphicsDeviceCapabilities.MaxPixelShaderProfile;
                if ((vs == ShaderProfile.VS_3_0) && (ps == ShaderProfile.PS_3_0)) return true;
                if ((vs == ShaderProfile.XVS_3_0) && (ps == ShaderProfile.XPS_3_0)) return true;
                return false;
            }
        }

        public bool ShaderModel3SupportedOverride
        {
            get { return shaderModel3SupportedOverride; }
            set { shaderModel3SupportedOverride = value;}
        }

        public bool ShadowMappingSupported
        {
            get
            {
                return ShadowMappingSupportedOverride ? ShadowMappingSupportedOverrideValue : ShaderModel3Supported;
            }
        }

        public bool ShadowMappingSupportedOverride
        {
            get
            {
                return shadowMappingSupportedOverride;
            }
            set
            {
                shadowMappingSupportedOverride = value;
            }
        }

        public bool ShadowMappingSupportedOverrideValue
        {
            get
            {
                return shadowMappingSupportedOverrideValue;
            }
            set
            {
                shadowMappingSupportedOverrideValue = value;
            }
        }

        #endregion

        #region IGraphics Members


        public bool AllowFogShaders
        {
            get
            {
                return allowFogShaders;
            }
            set
            {
                allowFogShaders = value;
            }
        }

        #endregion
    }
}
