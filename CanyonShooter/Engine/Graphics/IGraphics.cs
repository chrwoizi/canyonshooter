using Microsoft.Xna.Framework.Graphics;

namespace CanyonShooter.Engine.Graphics
{
    public interface IGraphics
    {
        GraphicsDevice Device { get; }

        IShaderSelector ShaderSelector { get; }

        bool Fullscreen { get; set; }

        int Refreshrate { get; set; }

        bool VSync { get; set; }

        int ScreenWidth { get; }
        int ScreenHeight { get; }

        int MaxPointLights { get; }

        bool ShaderModel3Supported { get; }

        bool ShaderModel3SupportedOverride { get; set;}

        bool ShadowMappingSupported { get; }

        bool ShadowMappingSupportedOverride { get; set; }

        bool ShadowMappingSupportedOverrideValue { get; set; }

        bool AllowFogShaders { get; set; }

        bool SetScreenResolution(int width, int height);

        //int MultiSampleQuality { get; set; }
        MultiSampleType MultiSampleType { get; set; }
    }
}
