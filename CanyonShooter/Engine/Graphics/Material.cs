using System;
using System.Collections.Generic;
using CanyonShooter.Engine.Graphics.Models;
using DescriptionLibs.Material;
using Microsoft.Xna.Framework.Graphics;

namespace CanyonShooter.Engine.Graphics
{
    /// <summary>
    /// Texturen und Shader
    /// </summary>
    public class Material : IMaterial
    {
        private const int MAX_TEXTURE_COUNT = 8;

        private ICanyonShooterGame game;
        private string name = "";
        private Effect effect = null;
        private Texture[] textures = new Texture[MAX_TEXTURE_COUNT];
        private Color emissiveColor = Color.Black;
        private Color diffuseColor = Color.White;
        private Color specularColor = Color.White;
        private float shininess = 40.0f;
        private EffectTechnique currentTechnique = null;
        static private List<Material> materials = new List<Material>();
        private InstancingType instancing;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">material description file name or "" for basic material</param>
        private Material(ICanyonShooterGame game, string name, InstancingType instancing)
        {
            this.game = game;
            this.instancing = instancing;

            load(name, instancing);
            this.name = name;
        }

        static public Material Create(ICanyonShooterGame game, string name, InstancingType instancing)
        {
            foreach (Material material in materials)
            {
                if (material.name == name && material.instancing == instancing)
                {
                    return material;
                }
            }
            Material n = new Material(game, name, instancing);
            materials.Add(n);
            return n;
        }

        static public void ClearSharedPool()
        {
            materials.Clear();
        }

        private void load(string name, InstancingType instancing)
        {
            if (name == "")
            {
                effect = new BasicEffect(game.Graphics.Device, null);
            }
            else
            {
                MaterialDescription desc = game.Content.Load<MaterialDescription>("Content\\Materials\\" + name);

                this.diffuseColor = desc.DiffuseColor;
                this.specularColor = desc.SpecularColor;
                this.shininess = desc.Shininess;
                this.emissiveColor= desc.EmissiveColor;

                if (desc.Effect == "")
                {
                    effect = game.Graphics.ShaderSelector.CreateEffect(desc.ShaderDescription, instancing);
                }
                else
                {
                    if (desc.Effect == "basic")
                    {
                        if(instancing != InstancingType.None)
                        {
                            throw new Exception("The basic shader does not support instancing.");
                        }

                        effect = new BasicEffect(game.Graphics.Device, null);
                    }
                    else
                    {
                        string shaderModel = game.Graphics.ShaderModel3Supported ? "_3_0" : "_2_0";
                        string techniqueName = desc.Technique + shaderModel;
                        effect = game.Content.Load<Effect>("Content\\Effects\\" + desc.Effect);
                        bool set = false;
                        foreach (EffectTechnique technique in effect.Techniques)
                        {
                            if (technique.Name == techniqueName)
                            {
                                effect.CurrentTechnique = technique;
                                set = true;
                                break;
                            }
                        }
                        if(!set)
                        {
                            string techniqueName2 = desc.Technique + "_2_0";
                            foreach (EffectTechnique technique in effect.Techniques)
                            {
                                if (technique.Name == techniqueName2)
                                {
                                    effect.CurrentTechnique = technique;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (desc.Texture0 != "") textures[0] = game.Content.Load<Texture>("Content\\Textures\\" + desc.Texture0);
                if (desc.Texture1 != "") textures[1] = game.Content.Load<Texture>("Content\\Textures\\" + desc.Texture1);
                if (desc.Texture2 != "") textures[2] = game.Content.Load<Texture>("Content\\Textures\\" + desc.Texture2);
                if (desc.Texture3 != "") textures[3] = game.Content.Load<Texture>("Content\\Textures\\" + desc.Texture3);
                if (desc.Texture4 != "") textures[4] = game.Content.Load<Texture>("Content\\Textures\\" + desc.Texture4);
                if (desc.Texture5 != "") textures[5] = game.Content.Load<Texture>("Content\\Textures\\" + desc.Texture5);
                if (desc.Texture6 != "") textures[6] = game.Content.Load<Texture>("Content\\Textures\\" + desc.Texture6);
                if (desc.Texture7 != "") textures[7] = game.Content.Load<Texture>("Content\\Textures\\" + desc.Texture7);
            }
            currentTechnique = effect.CurrentTechnique;

            this.name = name;
        }

        #region IMaterial Member

        public Texture[] Textures
        {
            get { return textures; }
        }

        public Color EmissiveColor 
        {
            get { return emissiveColor; }
            set { emissiveColor = value; }
        }

        public Color DiffuseColor 
        {
            get { return diffuseColor; }
            set { diffuseColor = value; }
        }

        public Color SpecularColor
        {
            get { return specularColor; }
            set { specularColor = value; }
        }

        public float Shininess
        {
            get { return shininess; }
            set { shininess = value; }
        }

        public Effect Effect
        {
            get { return effect;  }
        }

        public string Name
        {
            get { return name; }
        }

        public void SetupForRendering()
        {
            effect.CurrentTechnique = currentTechnique;
        }

        #endregion
    }
}
