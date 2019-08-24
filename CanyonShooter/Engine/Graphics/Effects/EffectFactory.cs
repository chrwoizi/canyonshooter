using System;
using CanyonShooter.ParticleEngine;
using DescriptionLibs.ParticleEffect;

namespace CanyonShooter.Engine.Graphics.Effects
{
    public class EffectFactory : IEffectFactory
    {
        #region IEffectFactory Member

        private ICanyonShooterGame game;
        public EffectFactory(ICanyonShooterGame game)
        {
            this.game = game;
        }

        public IEffect CreateEffect(string name)
        {

            ParticleEffectDescription fx = game.Content.Load<ParticleEffectDescription>(".\\Content\\ParticleEffects\\" + name);
            
            if(fx == null)
                throw new Exception("ParticleEffect: " + name + " not found!");

            EffectType type = (EffectType)Enum.Parse(typeof(EffectType), fx.EffectType, false);
            switch(type)
            {
                case EffectType.EXPLOSION:
                    return new ExplosionEmitter(game, type, fx.Settings);

                case EffectType.ROCKET_SMOKE:
                    return new RocketSmokeEmitter(game, type, fx.Settings);


                case EffectType.BLASTPIPE:
                    return new BlastPipeEmitter(game, type, fx.Settings);
                
                case EffectType.SMOKE_PLUME:
                    return new SmokePlumeEmitter(game, type, fx.Settings);

                case EffectType.LASER_WALL:
                        return new LaserWallEmitter(game, type, fx.Settings);
                    
                default:
                    throw new Exception("EffectType has no assigned Effect-Class!");
            }
        }
        #endregion
    }
}
