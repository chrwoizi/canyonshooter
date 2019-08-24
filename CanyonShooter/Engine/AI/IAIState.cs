using CanyonShooter.GameClasses.World;
using Microsoft.Xna.Framework;

namespace CanyonShooter.Engine.AI
{
    public interface IAIState
    {
        string Name { get;}
        IGameObject Owner { get;}
        GameTime GameTime {get;}
        ICanyonShooterGame Game { get;}

        AIStateMachine Manager { get; set;}
        void OnInit();
        void OnEnter();
        void OnExit();
        void Update();
        void ChangeState(string stateName);
        bool HasState(string stateName);
    }
}