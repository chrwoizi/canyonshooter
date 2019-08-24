using System;
using System.Collections.Generic;
using System.Text;
using CanyonShooter.GameClasses.World;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using CanyonShooter.Engine.Helper;

namespace CanyonShooter.GameClasses
{
    class TrafficLight : Static
    {
        enum State
        {
            IDLE = 0,
            THREE = 1,
            TWO = 2,
            ONE = 3,
            GO = 4
        }

        private State state = State.IDLE;
        private DateTime start;

        private Dictionary<State, float> waitTime = new Dictionary<State, float>();

        private float moveRange = 3.0f;
        private float moveSpeed = 1.0f;
        private double moveCurrent = 0.0f;

        private bool running = false;

        private ICanyonShooterGame game;

        public delegate void OnGreenDelegate();

        private OnGreenDelegate onGreenDelegate;

        public TrafficLight(ICanyonShooterGame game, OnGreenDelegate onGreenDelegate)
            : base(game, "TrafficLight")
        {
            this.game = game;
            SetModel("TrafficLight");
            waitTime.Add(State.IDLE, 6.657f);
            waitTime.Add(State.THREE, 1.714f);
            waitTime.Add(State.TWO, 1.714f);
            waitTime.Add(State.ONE, 1.714f);
            waitTime.Add(State.GO, 0.5f);
            Enabled = false;
            this.onGreenDelegate = onGreenDelegate;

            LocalScale = new Vector3(7, 7, 7);
        }

        public override void SetShaderConstant(EffectParameter constant)
        {
            if (constant.Semantic == "TEX_ID")
            {
                constant.SetValue((int)state);
            }
            else base.SetShaderConstant(constant);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);

            if (game.Input.HasKeyJustBeenPressed("QuickStart")) state = State.GO;

            if (running)
            {
                float w;
                TimeSpan dt = DateTime.Now - start;

                switch (state)
                {
                    case State.IDLE:
                        waitTime.TryGetValue(State.IDLE, out w);
                        if (dt.TotalSeconds >= w)
                        {
                            state = State.THREE;
                            start = DateTime.Now;
                        }
                        break;
                    case State.THREE:
                        waitTime.TryGetValue(State.THREE, out w);
                        if (dt.TotalSeconds >= w)
                        {
                            state = State.TWO;
                            start = DateTime.Now;
                        }
                        break;
                    case State.TWO:
                        waitTime.TryGetValue(State.TWO, out w);
                        if (dt.TotalSeconds >= w)
                        {
                            state = State.ONE;
                            start = DateTime.Now;
                        }
                        break;
                    case State.ONE:
                        waitTime.TryGetValue(State.ONE, out w);
                        if (dt.TotalSeconds >= w)
                        {
                            state = State.GO;
                            start = DateTime.Now;
                        }
                        break;
                    case State.GO:
                        waitTime.TryGetValue(State.GO, out w);
                        if (dt.TotalSeconds >= w)
                        {
                            onGreenDelegate();
                            running = false;
                        }
                        break;
                }
            }

            // move up and down
            Model.LocalPosition = Vector3.UnitY * (float)Math.Sin(moveCurrent) * moveRange;
            moveCurrent += gameTime.ElapsedGameTime.TotalSeconds * moveSpeed;
        }

        public void Start()
        {
            state = State.IDLE;
            start = DateTime.Now;
            Enabled = true;
            running = true;
        }
    }
}
