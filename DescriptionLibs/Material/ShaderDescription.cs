using System;
using System.Collections.Generic;
using System.Text;

namespace DescriptionLibs.Material
{
    public enum SurfaceType
    {
        Wireframe,
        Color,
        Texture
    }

    public class ShaderDescription
    {
        public SurfaceType SurfaceType;
        public bool Light;
        public bool ReceiveShadows;
        public bool NormalMapping;
        public bool WireframeOverlay;
    }
}
