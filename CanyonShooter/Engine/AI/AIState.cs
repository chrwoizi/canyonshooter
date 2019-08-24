using CanyonShooter.GameClasses.World;
using Microsoft.Xna.Framework;

namespace CanyonShooter.Engine.AI
{
    /// <summary>
    /// An AI-State base for the State Machine.
    /// </summary>
    public abstract class AIState : IAIState
    {

        #region IAIState Member

        private AIStateMachine manager;

        public abstract string Name{ get;}

        public AIStateMachine Manager
        {
            get
            {
                return manager;
            }
            set
            {
                manager = value;
            }
        }

        public IGameObject Owner
        {
            get
            {
                return Manager.Owner;
            }
        }

        public GameTime GameTime
        {
            get { return manager.GameTime; }
        }

        public ICanyonShooterGame Game
        {
            get { return manager.Game; }
        }

        public abstract void OnInit();


        public abstract void OnEnter();

        public abstract void OnExit();

        public abstract void Update();

        public void ChangeState(string name)
        {
            manager.ChangeState(name);
        }

        #endregion

        #region IAIState Member


        public bool HasState(string stateName)
        {
            if (manager.States.ContainsKey(stateName))
                return true;
            else
                return false;
        }

        #endregion
    }
}
