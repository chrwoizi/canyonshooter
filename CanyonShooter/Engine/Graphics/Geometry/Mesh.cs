using System;
using CanyonShooter.Engine.Physics;
using DescriptionLibs.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CanyonShooter.Engine.Graphics.Geometry
{
    public abstract class Mesh : Transformable, IMesh
    {
        private MeshAnimation animation;
        private bool playingTranslationAnimation = false;
        private bool playingRotationAnimation = false;
        private float currentTranslationAnimationDistance = 0.0f;
        private float currentRotationAnimationAngle = 0.0f;
        private Transformable translationAnimationTransformation;
        private Transformable rotationAnimationTransformationTF;
        private Transformable rotationAnimationTransformationR;
        private Transformable rotationAnimationTransformationTB;
        private bool setupParentChain = true;

        public Mesh(ICanyonShooterGame game) : base(game)
        {
            translationAnimationTransformation = new Transformable(game);
            rotationAnimationTransformationTF = new Transformable(game);
            rotationAnimationTransformationR = new Transformable(game);
            rotationAnimationTransformationTB = new Transformable(game);

            base.Parent = translationAnimationTransformation;
            translationAnimationTransformation.Parent = rotationAnimationTransformationTB;
            rotationAnimationTransformationTB.Parent = rotationAnimationTransformationR;
            rotationAnimationTransformationR.Parent = rotationAnimationTransformationTF;

            setupParentChain = false;
        }

        public override ITransformable Parent
        {
            get 
            {
                return base.Parent;
            }
            set
            {
                if (setupParentChain)
                {
                    base.Parent = value;
                }
                else
                {
                    rotationAnimationTransformationTF.Parent = value;
                }
            }
        }

        #region IMesh Members

        public MeshAnimation Animation
        {
            get
            {
                return animation;
            }
            set
            {
                animation = value;
                rotationAnimationTransformationTF.LocalPosition = animation.RotationPoint;
                rotationAnimationTransformationTB.LocalPosition = -animation.RotationPoint;
                rotationAnimationTransformationR.LocalRotation = Quaternion.CreateFromAxisAngle(animation.RotationAxis, 0);
            }
        }

        public abstract void Draw(GraphicsDevice device);

        public abstract Effect Effect { get; set; }

        public abstract string Name { get; }

        public void PlayTranslationAnimation()
        {
            if (animation.MaxTranslation.Length() > 0)
            {
                playingTranslationAnimation = true;
            }
            else throw new Exception("Mesh " + Name + " has no translation animation.");
        }

        public void StopTranslationAnimation()
        {
            playingTranslationAnimation = false;
        }

        public void PlayRotationAnimation()
        {
            if (Math.Abs(animation.RotationSpeed) > 0)
            {
                playingRotationAnimation = true;
            }
            else throw new Exception("Mesh " + Name + " has no rotation animation.");
        }

        public void StopRotationAnimation()
        {
            playingRotationAnimation = false;
        }

        public void Update(GameTime gameTime)
        {
            if(playingRotationAnimation)
            {
                currentRotationAnimationAngle += animation.RotationSpeed * (float)gameTime.ElapsedGameTime.Milliseconds/1000.0f;
                rotationAnimationTransformationR.LocalRotation = Quaternion.CreateFromAxisAngle(animation.RotationAxis, currentRotationAnimationAngle);
            }
        }

        #endregion

        #region IMesh Member


        public bool IsPlayingTranslationAnimation()
        {
            return playingTranslationAnimation;
        }

        public bool IsPlayingRotationAnimation()
        {
            return playingRotationAnimation;
        }

        #endregion
    }
}
