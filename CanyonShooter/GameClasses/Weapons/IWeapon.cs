using CanyonShooter.GameClasses.World;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace CanyonShooter.GameClasses.Weapons
{
    public interface IWeapon : IGameObject
    {
        [Obsolete("Diese Funktion ist veraltet, und wird demnächst verschwinden. Benutze stattdessen BeginFire() und EndFire().")]
        void Fire(Vector3 startPosition, Vector3 direction);

        void BeginFire();
        void EndFire();
        bool IsFiring{ get;}
        void AimAt();
        void AimAt(Vector3 direction);
        void AimAt(IGameObject target);
        
        int AmmoInMagazine { get; set;}
        int Ammo { get; set;}
        float ProjectileSpeed { get;}
        bool CanFire{ get;}
        TimeSpan FirePause { get; set;}

        WeaponState State { get; set;}
        Texture2D Icon { get; }
        Texture2D GetCrossHair(WeaponState state);
        Texture2D GetCrossHair();

        WeaponType GetWeaponType();
        string GetMagazineInfo();

    }
}
