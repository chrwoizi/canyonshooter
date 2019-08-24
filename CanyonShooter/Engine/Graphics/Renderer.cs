// Zuständigkeit: Christian

#region Using Statements

using System;
using System.Collections.ObjectModel;
using CanyonShooter.Engine.Graphics.Cameras;
using CanyonShooter.Engine.Graphics.Lights;
using CanyonShooter.Engine.Graphics.Models;
using CanyonShooter.Engine.Graphics.PostProcessing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaDevRu.Physics;
using Model=Microsoft.Xna.Framework.Graphics.Model;

#endregion

namespace CanyonShooter.Engine.Graphics
{
    /// <summary>
    /// Zeigt Models auf dem Bildschirm an oder zeichnet in eine Textur.
    /// 
    /// </summary>
    public class Renderer : IRenderer
    {
        private ICanyonShooterGame game;
        private ICamera camera;
        private Model sphereMesh = null;
        private Model cubeMesh = null;
        private BasicEffect collisionShapeEffect = null;
        private bool drawCollisionShapes = false;
        private PostProcessType postProcessType = PostProcessType.None;
        private IPostProcess postProcess;
        private GameComponentCollection gameComponents = null;
        private bool renderDepth = false;

        public Renderer(ICanyonShooterGame game, GameComponentCollection gameComponents)
        {
            this.game = game;
            this.gameComponents = gameComponents;
            camera = new PerspectiveCamera(game);
        }

        private void InitCollisionShapeVisualization()
        {
            collisionShapeEffect = new BasicEffect(game.Graphics.Device, null);
            collisionShapeEffect.EmissiveColor = Color.White.ToVector3();
            collisionShapeEffect.Alpha = 0.2f;
            sphereMesh = game.Content.Load<Model>("Content\\Models\\SphereLowPoly");
            cubeMesh = game.Content.Load<Model>("Content\\Models\\Cube");
        }

        private void renderToScreen(IModel model, IShaderConstantsSetter shaderConstantsSetter)
        {
            if (model.Meshes.Count != model.Materials.Count) throw new Exception("Model " + model.Name + " has " + model.Meshes.Count + " meshes and " + model.Materials.Count + " materials. These numbers must be the same.");

            //Camera = game.World.Sky.Sunlight.ShadowMapLow.Scene.Camera;
            for (int i = 0; i < model.Meshes.Count; i++)
            {
                // current mesh
                IMesh mesh = model.Meshes[i];

                // current material
                IMaterial material = model.Materials[i];

                // calculate the matrices
                Matrix world = mesh.GlobalTransformation;
                Matrix view = camera.ViewMatrix;
                Matrix projection = camera.ProjectionMatrix;
                Matrix wvp = world * view * projection;
                Matrix viewProjection = view * projection;

                SetDefaultRenderStates();

                material.SetupForRendering();

                Effect effect = renderDepth ? game.Graphics.ShaderSelector.GetSuitableDepthShader(material.Effect.CurrentTechnique) : material.Effect;
                
                // set effect parameters
                if (effect.GetType().Name == "BasicEffect")
                {
                    if(renderDepth)
                    {
                        throw new Exception("There is no depth technique in BasicEffect.");
                    }

                    BasicEffect beffect = (material.Effect as BasicEffect);
                    beffect.World = world;
                    beffect.View = view;
                    beffect.Projection = projection;
                    beffect.EnableDefaultLighting();
                }
                else foreach (EffectParameter parameter in effect.Parameters)
                {
                    if (parameter.Semantic != null)
                    {
                        if (parameter.Semantic == "WORLDVIEWPROJECTION")
                        {
                            parameter.SetValue(wvp);
                        }
                        else if (parameter.Semantic == "WORLD")
                        {
                            parameter.SetValue(world);
                        }
                        else if (parameter.Semantic == "VIEW")
                        {
                            parameter.SetValue(view);
                        }
                        else if (parameter.Semantic == "PROJECTION")
                        {
                            parameter.SetValue(projection);
                        }
                        else if (parameter.Semantic == "VIEWPROJECTION")
                        {
                            parameter.SetValue(viewProjection);
                        }

                        else if (parameter.Semantic == "CAMERA_POSITION")
                        {
                            parameter.SetValue(camera.GlobalPosition);
                        }

                        else if (parameter.Semantic == "MATERIAL_EMISSIVE")
                        {
                            parameter.SetValue(
                                new Vector3(material.EmissiveColor.R/255.0f, material.EmissiveColor.G/255.0f,
                                            material.EmissiveColor.B/255.0f));
                        }
                        else if (parameter.Semantic == "MATERIAL_DIFFUSE")
                        {
                            parameter.SetValue(
                                new Vector4(material.DiffuseColor.R/255.0f, material.DiffuseColor.G/255.0f,
                                            material.DiffuseColor.B/255.0f, material.DiffuseColor.A/255.0f));
                        }
                        else if (parameter.Semantic == "MATERIAL_SPECULAR_AND_SHININESS")
                        {
                            parameter.SetValue(
                                new Vector4(material.SpecularColor.R/255.0f, material.SpecularColor.G/255.0f,
                                            material.SpecularColor.B/255.0f, material.Shininess));
                        }
                        else if (parameter.Semantic == "AMBIENT_LIGHT")
                        {
                            parameter.SetValue(
                                new Vector3(game.World.AmbientLight.R/255.0f, game.World.AmbientLight.G/255.0f,
                                            game.World.AmbientLight.B/255.0f));
                        }

                        else if (parameter.Semantic == "SUNLIGHT_DIRECTION")
                        {
                            parameter.SetValue(game.World.Sky.Sunlight.Direction);
                        }
                        else if (parameter.Semantic == "SUNLIGHT_COLOR")
                        {
                            parameter.SetValue(
                                new Vector3(game.World.Sky.Sunlight.Color.R/255.0f,
                                            game.World.Sky.Sunlight.Color.G/255.0f,
                                            game.World.Sky.Sunlight.Color.B/255.0f));
                        }
                        else if (parameter.Semantic == "SUNLIGHT_SHADOWMAP_LOW")
                        {
                            parameter.SetValue(game.World.Sky.Sunlight.ShadowMapLow.Texture);
                        }
                        else if (parameter.Semantic == "SUNLIGHT_SHADOWMAP_LOW_TEXELSIZE")
                        {
                            Texture2D shadowMap = game.World.Sky.Sunlight.ShadowMapLow.Texture;
                            if (shadowMap != null)
                            {
                                parameter.SetValue(
                                    new Vector2(1.0f / shadowMap.Width,
                                                1.0f / shadowMap.Height));
                            }
                        }
                        else if (parameter.Semantic == "SUNLIGHT_SHADOWMAP_LOW_MATRIX")
                        {
                            parameter.SetValue(game.World.Sky.Sunlight.ShadowMapLow.ShadowMatrix);
                        }
                        else if (parameter.Semantic == "SUNLIGHT_SHADOWMAP_HIGH")
                        {
                            parameter.SetValue(game.World.Sky.Sunlight.ShadowMapHigh.Texture);
                        }
                        else if (parameter.Semantic == "SUNLIGHT_SHADOWMAP_HIGH_TEXELSIZE")
                        {
                            Texture2D shadowMap = game.World.Sky.Sunlight.ShadowMapHigh.Texture;
                            if (shadowMap != null)
                            {
                                parameter.SetValue(
                                    new Vector2(1.0f / shadowMap.Width,
                                                1.0f / shadowMap.Height));
                            }
                        }
                        else if (parameter.Semantic == "SUNLIGHT_SHADOWMAP_HIGH_MATRIX")
                        {
                            parameter.SetValue(game.World.Sky.Sunlight.ShadowMapHigh.ShadowMatrix);
                        }

                        else if (parameter.Semantic == "POINTLIGHT_COUNT")
                        {
                            parameter.SetValue(Math.Min(game.World.PointLights.Count, game.Graphics.MaxPointLights));
                        }
                        else if (parameter.Semantic == "POINTLIGHT_POSITIONS")
                        {
                            Vector3[] positions = new Vector3[game.Graphics.MaxPointLights];
                            int currentPositionId = 0;

                            ReadOnlyCollection<IPointLight> lights = game.World.PointLights;

                            for (int k = 0; k < Math.Min(lights.Count, positions.Length); k++)
                            {
                                positions[currentPositionId++] = lights[k].GlobalPosition;
                            }

                            for (int n = currentPositionId; n < positions.Length; n++)
                            {
                                positions[n] = new Vector3(0, 0, 0);
                            }

                            parameter.SetValue(positions);
                        }
                        else if (parameter.Semantic == "POINTLIGHT_COLORS")
                        {
                            Vector3[] colors = new Vector3[game.Graphics.MaxPointLights];
                            int currentColorId = 0;

                            ReadOnlyCollection<IPointLight> lights = game.World.PointLights;

                            for (int k = 0; k < Math.Min(lights.Count, colors.Length); k++)
                            {
                                colors[currentColorId++] = lights[k].Color.ToVector3();
                            }

                            for (int n = currentColorId; n < colors.Length; n++)
                            {
                                colors[n] = new Vector3(0, 0, 0);
                            }

                            parameter.SetValue(colors);
                        }
                        else if (parameter.Semantic == "POINTLIGHT_ATTENUATIONS")
                        {
                            Vector2[] attenuations = new Vector2[game.Graphics.MaxPointLights];
                            int currentAttenuationId = 0;

                            ReadOnlyCollection<IPointLight> lights = game.World.PointLights;

                            for (int k = 0; k < Math.Min(lights.Count, attenuations.Length); k++)
                            {
                                attenuations[currentAttenuationId++] =
                                    new Vector2(lights[k].SquaredAttenuation, lights[k].LinearAttenuation);
                            }

                            for (int n = currentAttenuationId; n < attenuations.Length; n++)
                            {
                                attenuations[n] = new Vector2(0, 0);
                            }

                            parameter.SetValue(attenuations);
                        }

                        else if (parameter.Semantic.Length > 7 && parameter.Semantic.Substring(0, 7) == "TEXTURE")
                        {
                            int id = Convert.ToInt32(parameter.Semantic.Substring(7, 1));
                            parameter.SetValue(material.Textures[id]);
                        }

                        else if (parameter.Semantic == "FOG")
                        {
                            parameter.SetValue(game.Graphics.AllowFogShaders);
                        }

                        else if(shaderConstantsSetter != null) shaderConstantsSetter.SetShaderConstant(parameter);
                    }
                }

                mesh.Effect = effect;

                mesh.Draw(game.Graphics.Device);   
            }
        }

        public void SetDefaultRenderStates()
        {
            game.Graphics.Device.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
            game.Graphics.Device.RenderState.DepthBufferEnable = true;
            game.Graphics.Device.RenderState.FillMode = FillMode.Solid;
            game.Graphics.Device.RenderState.DepthBufferWriteEnable = true;
            game.Graphics.Device.RenderState.DepthBias = 0;
        }

        /// <summary>
        /// Draws a MeshShapeData.
        /// </summary>
        /// <param name="worldMatrix"></param>
        public void DrawCollisionMeshShape(Matrix worldMatrix, MeshShapeData meshData)
        {
            Vector3[] vertices = meshData.VertexArray;
            int[] indices = meshData.TriangleArray;

            VertexElement[] elements = {
                new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0)
            };
            VertexDeclaration vdecl = new VertexDeclaration(game.Graphics.Device, elements);

            game.Graphics.Device.VertexDeclaration = vdecl;

            SetDefaultRenderStates();

            FillMode backup = game.Graphics.Device.RenderState.FillMode;
            game.Graphics.Device.RenderState.FillMode = FillMode.WireFrame;

            collisionShapeEffect.View = camera.ViewMatrix;
            collisionShapeEffect.Projection = camera.ProjectionMatrix;
            collisionShapeEffect.World = worldMatrix;

            collisionShapeEffect.Begin(SaveStateMode.None);

            for (int i = 0; i < collisionShapeEffect.CurrentTechnique.Passes.Count; i++)
            {
                collisionShapeEffect.CurrentTechnique.Passes[i].Begin();

                game.Graphics.Device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3);

                collisionShapeEffect.CurrentTechnique.Passes[i].End();
            }

            collisionShapeEffect.End();

            game.Graphics.Device.RenderState.FillMode = backup;
        }

        /// <summary>
        /// Draws a collision shape. Mesh draw code is taken from the XNA Vertex Lighting Sample
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="modelMatrix"></param>
        public void DrawCollisionShape(ShapeData shape, Matrix modelMatrix)
        {
            if (collisionShapeEffect == null)
            {
                InitCollisionShapeVisualization();
            }

            if (shape is SphereShapeData)
            {
                SphereShapeData sphereData = (shape as SphereShapeData);
                Matrix worldMatrix = Matrix.CreateScale(sphereData.Radius) * sphereData.Offset *
                              modelMatrix;
                DrawCollisionShapeMesh(worldMatrix, sphereMesh);
            }
            else if (shape is BoxShapeData)
            {
                BoxShapeData boxData = (shape as BoxShapeData);
                Matrix worldMatrix = Matrix.CreateScale(boxData.Dimensions * 0.7f) * Matrix.CreateFromQuaternion(Quaternion.Inverse(Quaternion.CreateFromRotationMatrix(boxData.Offset))) * Matrix.CreateTranslation(boxData.Offset.Translation) * modelMatrix;
                DrawCollisionShapeMesh(worldMatrix, cubeMesh);
            }
            else if (shape is MeshShapeData)
            {
                MeshShapeData meshData = (shape as MeshShapeData);
                Matrix worldMatrix = meshData.Offset * modelMatrix;
                DrawCollisionMeshShape(worldMatrix, meshData);
            }
        }

        public void DrawCollisionShapeMesh(Matrix worldMatrix, Model model)
        {
            //our sample meshes only contain a single part, so we don't need to bother
            //looping over the ModelMesh and ModelMeshPart collections. If the meshes
            //were more complex, we would repeat all the following code for each part
            ModelMesh mesh = model.Meshes[0];
            ModelMeshPart meshPart = mesh.MeshParts[0];

            SetDefaultRenderStates();

            FillMode backup = game.Graphics.Device.RenderState.FillMode;
            game.Graphics.Device.RenderState.FillMode = FillMode.WireFrame;

            collisionShapeEffect.View = camera.ViewMatrix;
            collisionShapeEffect.Projection = camera.ProjectionMatrix;
            collisionShapeEffect.World = worldMatrix;

            meshPart.Effect = collisionShapeEffect;

            mesh.Draw(SaveStateMode.SaveState);
            
            game.Graphics.Device.RenderState.FillMode = backup;
        }

        #region IRenderer Member

        public ICamera Camera 
        {
            get
            {
                return camera;
            }
            set
            {
                camera = value;
            }
        }

        public void Draw(IModel model, IShaderConstantsSetter shaderConstantsSetter)
        {
            renderToScreen(model, shaderConstantsSetter);

            if (drawCollisionShapes && !renderDepth)
            {
                if (sphereMesh == null) InitCollisionShapeVisualization();

                foreach (ShapeData shape in model.CollisionShapes)
                {
                    DrawCollisionShape(shape, model.GlobalTransformation);
                }
            }
        }

        public PostProcessType PostProcessType
        {
            get
            {
                return postProcessType;
            }
            set
            {
                postProcessType = value;

                if (postProcess != null)
                {
                    gameComponents.Remove(postProcess);
                    postProcess.Dispose();
                }

                switch (postProcessType)
                {
                    case PostProcessType.None:
                        postProcess = null;
                        break;
                    case PostProcessType.Bloom:
                        BloomPostProcess pp = new BloomPostProcess(game);
                        pp.Settings = BloomPostProcess.BloomSettings.PresetSettings[0];
                        postProcess = pp;
                        break;
                    case PostProcessType.RadialBlur:
                        postProcess = new RadialBlurPostProcess(game);
                        break;
                    case PostProcessType.BloomAndBlur:
                        postProcess = new BloomAndRadialBlurPostProcess(game);
                        break;
                }

                if(postProcess != null)
                {
                    if(postProcessType == PostProcessType.BloomAndBlur)
                    {
                        (postProcess as BloomAndRadialBlurPostProcess).addToComponents(gameComponents);
                    }
                    gameComponents.Add(postProcess);
                }
            }
        }

        public bool RenderDepth
        {
            get
            {
                return renderDepth;
            }
            set
            {
                renderDepth = value;
            }
        }

        public bool DrawCollisionShapes
        {
            get
            {
                return drawCollisionShapes;
            }
            set
            {
                drawCollisionShapes = value;
            }
        }

        #endregion
    }
}


