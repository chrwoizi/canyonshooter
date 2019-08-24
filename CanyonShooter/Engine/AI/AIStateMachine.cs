using System.Collections.Generic;
using CanyonShooter.GameClasses.Console;
using CanyonShooter.GameClasses.World;
using Microsoft.Xna.Framework;

namespace CanyonShooter.Engine.AI
{
    /// <summary>
    /// An AI-System based on Finite State Machine (FSM)
    /// by Florian Maetschke
    /// </summary>
    public class AIStateMachine
    {
        #region Vars and Properties

        private ICanyonShooterGame game;

        private GameTime gameTime;

        private IGameObject owner;

        private string lastState = "LAST";

        private string currentState = "START";

        /// <summary>
        /// A dictionary containing all avaible gamestates.
        /// </summary>
        public readonly Dictionary<string,IAIState> States = new Dictionary<string, IAIState>();

        public bool DebugAI = false;

        public ICanyonShooterGame Game
        {
            get
            {
                return game;
            }
        }

        public GameTime GameTime
        {
            get
            {
                return gameTime;
            }
        }


        /// <summary>
        /// Gets the owner of this AI.
        /// </summary>
        /// <value>The owner.</value>
        public IGameObject Owner
        {
            get { return owner; }
        }

        /// <summary>
        /// Gets the name of the current state.
        /// </summary>
        /// <value>The name of the current state.</value>
        public string CurrentStateName
        {
            get { return currentState; }
        }


        /// <summary>
        /// Gets the name of the last state.
        /// </summary>
        /// <value>The name of the last state.</value>
        public string LastStateName
        {
            get { return lastState; }
        }

        #endregion

        public AIStateMachine(ICanyonShooterGame game, IGameObject owner)
        {
            this.game = game;
            this.owner = owner;
        }

        /// <summary>
        /// Adds the state to the AI Machine
        /// </summary>
        /// <param name="state">The state.</param>
        public void AddState(IAIState state)
        {
            States.Add(state.Name,state);
            if (States.Count == 1)
            {
                currentState = state.Name;
                lastState = state.Name;
            }

            state.Manager = this;

            if (DebugAI)
                GraphicalConsole.GetSingleton(game).WriteLine(
                    string.Format("AI-DEBUG:    Begin {0}.OnInit()", state.Name), 0);

            States[state.Name].OnInit(); // Call OnExit()

            if (DebugAI)
                GraphicalConsole.GetSingleton(game).WriteLine(
                    string.Format("AI-DEBUG:    End {0}.OnInit()", state.Name), 0);

        }

        public void ChangeState(string name)
        {
            if(!States.ContainsKey(name))
            {
                GraphicalConsole.GetSingleton(game).WriteLine("AI-ERROR:    Can't find state: " + name,0 );
                return;
            }
            if (DebugAI)
                GraphicalConsole.GetSingleton(game).WriteLine(
                    string.Format("AI-DEBUG:    Begin {0}.OnExit()", currentState), 0);

            States[currentState].OnExit(); // Call OnExit()

            if (DebugAI)
                GraphicalConsole.GetSingleton(game).WriteLine(
                    string.Format("AI-DEBUG:    End {0}.OnExit()", currentState), 0);


            if(DebugAI)
                GraphicalConsole.GetSingleton(game).WriteLine(
                    string.Format("AI-DEBUG:    {0} ---> {1}", currentState, name), 0);
            

            // Change States
            lastState = currentState;
            currentState = name;


            if (DebugAI)
                GraphicalConsole.GetSingleton(game).WriteLine(
                    string.Format("AI-DEBUG:    Begin {0}.OnEnter()", currentState), 0);

            States[currentState].OnEnter(); // Call OnEnter()

            if (DebugAI)
                GraphicalConsole.GetSingleton(game).WriteLine(
                    string.Format("AI-DEBUG:    End {0}.OnEnter()", currentState), 0);

        }

        public void UpdateAI(GameTime gameTime)
        {
            this.gameTime = gameTime;

            if (DebugAI)
                GraphicalConsole.GetSingleton(game).WriteLine(
                    string.Format("AI-DEBUG:    Begin {0}.Update()", currentState), 0);

            States[currentState].Update();

            if (DebugAI)
                GraphicalConsole.GetSingleton(game).WriteLine(
                    string.Format("AI-DEBUG:    End {0}.Update()", currentState), 0);

        }

 }
}
