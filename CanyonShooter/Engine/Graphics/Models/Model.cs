using System;
using System.Collections.Generic;
using CanyonShooter.Engine.Graphics.Effects;
using CanyonShooter.Engine.Physics;
using CanyonShooter.GameClasses.Weapons;
using DescriptionLibs.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaDevRu.Physics;

namespace CanyonShooter.Engine.Graphics.Models
{
    public enum InstancingType
    {
        None,
        Hardware,
        Constants
    }

    /// <summary>
    /// Die Geometrie eines Objekts
    /// </summary>
    public class Model : Transformable, IModel
    {
        protected List<IMesh> meshes = new List<IMesh>();
        protected List<IMaterial> materials = new List<IMaterial>();
        protected List<WeaponSlot> weaponSlots = new List<WeaponSlot>();
        protected List<ShapeData> collisionShapes = new List<ShapeData>();
        protected List<WreckageModel> wreckageModels = new List<WreckageModel>();
        protected List<IEffect> particleEffects = new List<IEffect>();
        protected List<AfterBurner> afterBurnerEffects = new List<AfterBurner>();
        protected bool showAfterBurnerEffects = true;
        protected ICanyonShooterGame game;
        protected string name = "";
        protected float mass = 1.0f;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">model description file (mesh name, material name, bounds, ...)</param>
        public Model(ICanyonShooterGame game, string name) : base(game, null)
        {
            this.game = game;

            if (name != "")
            {
                Load(name, InstancingType.None);
            }
        }

        public float AfterBurnerLength
        {
            set
            {
                foreach (AfterBurner a in afterBurnerEffects)
                {
                    float xy = 1.0f + Math.Max(0, value - 0.8f);
                    a.LocalScale = new Vector3(a.MaxSize.X * xy, a.MaxSize.Y * xy, value * a.MaxSize.Z);
                    a.Power = value;
                }
            }
            get
            {
                return afterBurnerEffects[0].LocalScale.Z / afterBurnerEffects[0].MaxSize.Z;
            }
        }

        /// <summary>
        /// Creates a model which is made of one or multiple meshes.
        /// meshes[i] will be drawn with materials[i]
        /// </summary>
        /// <param name="game">the owner</param>
        /// <param name="meshes">meshes to add to the new Model</param>
        /// <param name="materials">materials. each material in that array can be null to load a default materials</param>
        /// <param name="name">just to identify this</param>
        public Model(ICanyonShooterGame game, IMesh[] meshes, IMaterial[] materials, string name) : base(game, null)
        {
            this.game = game;

            if (meshes != null)
            foreach (IMesh mesh in meshes)
            {
                mesh.Parent = this;
                this.meshes.Add(mesh);
            }

            if (materials != null)
            foreach (IMaterial material in materials)
            {
                if (material != null)
                {
                    this.materials.Add(material);
                }
                else
                {
                    InstancingType instancing = InstancingType.None;

                    if (meshes[0] is ConstantsInstancingModelMeshAdapterMesh)
                    {
                        instancing = InstancingType.Constants;
                    }
                    else if (meshes[0] is HardwareInstancingModelMeshAdapterMesh)
                    {
                        instancing = InstancingType.Hardware;
                    }
                    
                    this.materials.Add(Material.Create(game, "", instancing));
                }
            }

            this.name = name;
        }

        protected void Load(string name, InstancingType instancing)
        {
            if(name == null || name == string.Empty)
                return;

            // load description
            ModelDescription desc = game.Content.Load<ModelDescription>(".\\Content\\Models\\" + name + "Desc");

            LocalRotation = Quaternion.CreateFromAxisAngle(desc.BaseRotationAxis, MathHelper.ToRadians(desc.BaseRotationAngle));

            // load fbx
            Microsoft.Xna.Framework.Graphics.Model model = game.Content.Load<Microsoft.Xna.Framework.Graphics.Model>(".\\Content\\Models\\" + desc.BaseFBX);

            // process meshes
            foreach (ModelMesh modelmesh in model.Meshes)
            {
                // wrap ModelMesh in adapter
                IMesh mesh;
                switch(instancing)
                {
                    case InstancingType.None:
                        mesh = new ModelMeshAdapterMesh(game, modelmesh);
                        break;
                    case InstancingType.Constants:
                        mesh = new ConstantsInstancingModelMeshAdapterMesh(game, modelmesh);   
                        break;
                    case InstancingType.Hardware:
                        mesh = new HardwareInstancingModelMeshAdapterMesh(game, modelmesh);   
                        break;
                    default:
                        throw new Exception();
                }
                mesh.Parent = this;
                meshes.Add(mesh);

                // get material name. prefer the one with the mesh's name before *
                string materialName = "";
                foreach(MeshMaterial matdesc in desc.Materials)
                {
                    if(matdesc.MeshName == modelmesh.Name)
                    {
                        materialName = matdesc.MaterialName;
                        break;
                    }
                    if(matdesc.MeshName == "*")
                    {
                        if(materialName == "")
                        {
                            materialName = matdesc.MaterialName;
                        }
                    }
                }

                // load material
                Material material = Material.Create(game, materialName, instancing);
                materials.Add(material);

                // tell the mesh which effect to use
                mesh.Effect = material.Effect;
            }

            // get weapon slots
            foreach (WeaponDescription weapon in desc.Weapons)
            {
                WeaponSlot slot = new WeaponSlot(
                    (WeaponType)Enum.Parse(typeof(WeaponType), weapon.WeaponType, false),
                    weapon.Position,
                    weapon.RotationAxis,
                    weapon.RotationAngle,
                    weapon.Scaling);

                weaponSlots.Add(slot);
            }

            // get collision shapes
            foreach (Box box in desc.CollisionShapes.Boxes)
            {
                BoxShapeData shape = new BoxShapeData();
                shape.Dimensions = box.Size;
                Matrix m = Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(box.OffsetRotationAxis, box.OffsetRotationAngle));
                m.Translation = box.OffsetPosition;
                shape.Offset = m;
                shape.Material.Friction = desc.Friction;
                shape.Material.Bounciness = desc.Bounciness;
                shape.Material.Hardness = desc.Hardness;
                collisionShapes.Add(shape);
            }
            foreach (Capsule capsule in desc.CollisionShapes.Capsules)
            {
                CapsuleShapeData shape = new CapsuleShapeData();
                shape.Radius = capsule.Radius;
                shape.Length = capsule.Length;
                Matrix m = Matrix.CreateFromQuaternion(Quaternion.CreateFromAxisAngle(capsule.OffsetRotationAxis, capsule.OffsetRotationAngle));
                m.Translation = capsule.OffsetPosition;
                shape.Offset = m;
                shape.Material.Friction = desc.Friction;
                shape.Material.Bounciness = desc.Bounciness;
                shape.Material.Hardness = desc.Hardness;
                collisionShapes.Add(shape);
            }
            foreach (Sphere sphere in desc.CollisionShapes.Spheres)
            {
                SphereShapeData shape = new SphereShapeData();
                shape.Radius = sphere.Radius;
                Matrix m = Matrix.CreateTranslation(sphere.OffsetPosition);
                shape.Offset = m;
                shape.Material.Friction = desc.Friction;
                shape.Material.Bounciness = desc.Bounciness;
                shape.Material.Hardness = desc.Hardness;
                collisionShapes.Add(shape);
            }

            wreckageModels = desc.Wreckages;

            // get particle effects
            if (game.World != null)
            {
                foreach (ParticleEffectDescription effect in desc.ParticleEffects)
                {
                    IEffect fx = game.Effects.CreateEffect(effect.EffectName);
                    fx.Parent = this;
                    fx.LocalPosition = effect.Position;
                    fx.LocalRotation = Quaternion.CreateFromAxisAngle(effect.RotationAxis, effect.RotationAngle);
                    fx.LocalScale = effect.Scaling;
                    game.World.AddEffect(fx);
                    particleEffects.Add(fx); // for calling Destroy() later
                    fx.Play();
                }

            }

            // afterburner
            foreach (AfterBurnerEffectDescription ab in desc.AfterBurnerEffects)
            {
                AfterBurner a  = new AfterBurner(game);

                a.Parent = this;

                a.LocalPosition = ab.Position;
                a.LocalScale = ab.Scaling;
                a.LocalRotation = Quaternion.CreateFromAxisAngle(ab.RotationAxis, ab.RotationAngle);

                a.LoadTheContent();

                afterBurnerEffects.Add(a);
            }
            
            // get animations
            foreach (MeshAnimation anim in desc.Animations)
            {
                foreach (IMesh mesh in meshes)
                {
                    if (mesh.Name == anim.MeshName)
                    {
                        mesh.Animation = anim;

                        break; // inner foreach
                    }
                }
            }

            // mass
            mass = desc.Mass;

            // base rotation
            LocalRotation = Quaternion.CreateFromAxisAngle(desc.BaseRotationAxis, desc.BaseRotationAngle);

            // save name
            this.name = name;
        }


        #region IModel Members

        public string Name
        {
            get { return name; }
        }

        public List<IMesh> Meshes
        {
            get { return meshes; }
        }

        public List<IMaterial> Materials
        {
            get { return materials; }
        }

        public List<WeaponSlot> WeaponSlots
        {
            get { return weaponSlots; }
        }

        public List<ShapeData> CollisionShapes
        {
            get { return collisionShapes; }
        }

        public float MassInDescription
        {
            get { return mass; }
        }

        public IMesh GetMesh(string name)
        {
            foreach (IMesh mesh in meshes)
            {
                if (mesh.Name == name)
                {
                    return mesh;
                }
            }
            // Fix to get null if no part exists. Nessecery in the player
            // class to identify if an animation part exists or not.
            // Changes by M.R.
            //throw new Exception("Mesh named " + name + " does not exist.");
            return null;
        }

        public void Draw(IShaderConstantsSetter shaderConstantsSetter)
        {
            game.Renderer.Draw(this, shaderConstantsSetter);

            if (showAfterBurnerEffects)
            {
                foreach (AfterBurner afterBurner in afterBurnerEffects)
                {
                    afterBurner.Draw(null);
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (IMesh mesh in meshes)
            {
                mesh.Update(gameTime);
            }

            if (showAfterBurnerEffects)
            {
                foreach (AfterBurner afterBurner in afterBurnerEffects)
                {
                    afterBurner.Update(gameTime);
                }
            }
        }

        #endregion

        public override void Dispose()
        {
            foreach (IEffect effect in particleEffects)
            {
                effect.Dispose();
            }
            base.Dispose();
        }

        #region IModel Members


        public List<WreckageModel> WreckageModels
        {
            get { return wreckageModels; }
        }

        #endregion

        #region IModel Member


        public List<IEffect> ParticleEffects
        {
            get { return particleEffects; }
        }

        #endregion

        #region IModel Members


        public bool ShowAfterBurner
        {
            get
            {
                return showAfterBurnerEffects;
            }
            set
            {
                showAfterBurnerEffects = value;
            }
        }

        #endregion
    }
}
