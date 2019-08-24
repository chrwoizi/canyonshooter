using System;
using System.Collections.Generic;
using System.Text;
using CanyonShooter.Engine.Graphics.Models;
using DescriptionLibs.Material;
using Microsoft.Xna.Framework.Graphics;

namespace CanyonShooter.Engine.Graphics
{
    public class ShaderSelector : IShaderSelector
    {
        private ICanyonShooterGame game;
        private Dictionary<string, EffectTechniqueDescription> validShaders = new Dictionary<string, EffectTechniqueDescription>();
        private Effect depthEffect;
        private Effect depthConstantsInstancingEffect;
        private Effect depthHardwareInstancingEffect;

        public ShaderSelector(ICanyonShooterGame game)
        {
            this.game = game;
        }

        public void InitializeDepthShaders()
        {
            List<string> depth = new List<string>(); 
            depth.Add("depth");

            List<string> depthConstantsInstancing = new List<string>();
            depthConstantsInstancing.Add("depth");
            depthConstantsInstancing.Add("constantsinstancing");

            List<string> depthHardwareInstancing = new List<string>();
            depthHardwareInstancing.Add("depth");
            depthHardwareInstancing.Add("hardwareinstancing");

            depthEffect = CreateEffect(depth);
            depthConstantsInstancingEffect = CreateEffect(depthConstantsInstancing);
            depthHardwareInstancingEffect = CreateEffect(depthHardwareInstancing);
        }

        public Effect CreateEffect(ShaderDescription shaderDescription, InstancingType instancing)
        {
            EffectTechniqueDescription sd = GetEffectTechniqueDescription(shaderDescription, instancing);
            return CreateEffect(sd);
        }

        public Effect CreateEffect(List<string> shaderDescription)
        {
            EffectTechniqueDescription sd = GetEffectTechniqueDescription(shaderDescription);
            return CreateEffect(sd);
        }

        public Effect CreateEffect(EffectTechniqueDescription sd)
        {
            Effect result = game.Content.Load<Effect>("Content\\Effects\\" + sd.Effect);
            foreach (EffectTechnique technique in result.Techniques)
            {
                if (technique.Name == sd.Technique)
                {
                    result.CurrentTechnique = technique;
                    break;
                }
            }

            return result;
        }

        #region IShaderSelector Members

        public void RegisterEffect(string name)
        {
            Effect effect = game.Content.Load<Effect>("Content\\Effects\\" + name);

            foreach (EffectTechnique tech in effect.Techniques)
            {
                // remove begin and end of technique name
                string lowerTechName = tech.Name.ToLower();
                if (!(lowerTechName.StartsWith("t_") && (lowerTechName.EndsWith("_2_0") || lowerTechName.EndsWith("_3_0"))))
                {
                    throw new Exception("Effect file " + name + " has an invalid technique name. Techniques must start with \"t_\" and end with \"_2_0\" or \"_3_0\".");
                }
                string techNamePartsAsString = lowerTechName.Substring(2, lowerTechName.Length - 6);

                // split technique name to parts
                string[] techNameParts = techNamePartsAsString.Split(("_").ToCharArray());

                // sort tech name parts
                StringBuilder sb = new StringBuilder();
                SortedList<string, string> sl = new SortedList<string, string>(techNameParts.Length);
                bool continueWithNextTechnique = false;
                foreach (string part in techNameParts)
                {
                    if (part == "basic")
                    {
                        continueWithNextTechnique = true;
                        break;
                    }
                    sl.Add(part, part);
                }
                if (continueWithNextTechnique) continue;

                // build tech desc string
                string shaderModel = lowerTechName.EndsWith("_3_0") ? "SM3" : "SM2";
                sb.Append(shaderModel);
                foreach (KeyValuePair<string, string> part in sl)
                {
                    sb.Append("," + part.Value);
                }

                // create ShaderDescription
                EffectTechniqueDescription sd = new EffectTechniqueDescription();
                sd.Effect = name;
                sd.Technique = tech.Name;

                // ad to valid
                validShaders.Add(sb.ToString(), sd);
            }
        }

        public EffectTechniqueDescription GetEffectTechniqueDescription(ShaderDescription description, InstancingType instancing)
        {
            List<string> descParts = new List<string>();

            switch (description.SurfaceType)
            {
                case SurfaceType.Color:
                    descParts.Add("color");
                    break;
                case SurfaceType.Texture:
                    descParts.Add("texture");
                    break;
                case SurfaceType.Wireframe:
                    descParts.Add("wireframe");
                    break;
            }

            if (description.Light)
            {
                if (game.Graphics.ShaderModel3Supported)
                {
                    descParts.Add("light");

                    if (game.Graphics.ShadowMappingSupported)
                    {
                        if (description.ReceiveShadows)
                        {
                            descParts.Add("sunlightshadow");

                            if (description.NormalMapping)
                            {
                                descParts.Add("normalmapping");
                            }
                        }
                    }
                }
                else
                {
                    descParts.Add("light");
                }
            }

            if(description.WireframeOverlay)
            {
                descParts.Add("wireframeoverlay");
            }

            switch(instancing)
            {
                case InstancingType.Constants:
                    descParts.Add("constantsinstancing");
                    break;
                case InstancingType.Hardware:
                    descParts.Add("hardwareinstancing");
                    break;
            }

            return GetEffectTechniqueDescription(descParts);
        }

        public EffectTechniqueDescription GetEffectTechniqueDescription(List<string> descParts)
        {
            // sort keywords
            StringBuilder sb = new StringBuilder();

            SortedList<string, string> sl = new SortedList<string, string>(descParts.Count);

            foreach (string part in descParts)
            {
                sl.Add(part, part);
            }

            // build technique desc (internal format)
            string shaderModel = game.Graphics.ShaderModel3Supported ? "SM3" : "SM2";
            sb.Append(shaderModel);
            foreach (KeyValuePair<string,string> part in sl)
            {
                sb.Append("," + part.Value);
            }

            // look for technique desc
            string techName = sb.ToString();
            EffectTechniqueDescription sd;
            if(validShaders.TryGetValue(techName, out sd))
            {
                // technique found.
                return sd;
            }
            else
            {
                if (shaderModel == "SM3")
                {
                    // build technique desc (internal format)
                    shaderModel = "SM2";
                    sb = new StringBuilder();
                    sb.Append(shaderModel);
                    foreach (KeyValuePair<string, string> part in sl)
                    {
                        sb.Append("," + part.Value);
                    }

                    // look for technique desc
                    techName = sb.ToString();
                    if (validShaders.TryGetValue(techName, out sd))
                    {
                        // technique found.
                        return sd;
                    }
                    else throw new Exception("There is no shader with the keywords (" + descParts + ") for shader model 2.0 or 3.0.");
                }
                else throw new Exception("There is no shader with the keywords (" + descParts + ") for shader model 2.0 or 3.0.");
            }
        }

        #endregion

        #region IShaderSelector Members


        public Effect GetSuitableDepthShader(EffectTechnique e)
        {
            if (e.Name.Contains("constantsinstancing"))
            {
                return depthConstantsInstancingEffect;
            }
            else if (e.Name.Contains("hardwareinstancing"))
            {
                return depthHardwareInstancingEffect;
            }
            else return depthEffect;
        }

        #endregion
    }
}
