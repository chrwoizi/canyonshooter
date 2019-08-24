// Zuständigkeit: Richard

#region Using Statements

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CanyonShooter.DataLayer.Descriptions;
using CanyonShooter.DataLayer.Level;
using CanyonShooter.Engine.Graphics.Cameras;
using CanyonShooter.Engine.Graphics.Effects;
using CanyonShooter.Engine.Graphics.Lights;
using CanyonShooter.Engine.Helper;
using CanyonShooter.Engine.Physics;
using CanyonShooter.GameClasses.Console;
using CanyonShooter.GameClasses.Huds;
using CanyonShooter.GameClasses.Items;
using CanyonShooter.GameClasses;
using CanyonShooter.GameClasses.World.Canyon;
using CanyonShooter.GameClasses.World.Debris;
using CanyonShooter.GameClasses.World.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaDevRu.Physics;
using System.IO;

#endregion

namespace CanyonShooter.GameClasses.World
{
    /// <summary>
    /// Diese Komponente verwaltet alle Objekte des Levels (Gegner, Statics, Canyon, ...)
    /// Dazu gehört nicht das Laden und Anzeigen, sondern nur das Erstellen und Sammeln in 
    /// einer Art Collection. Außerdem wird hier das Level geladen, d.h. Positionen und 
    /// Eigenschaften aus einer Datei laden und an die Konstruktoren bzw. Init-Methoden der
    /// Objekte weiterzugeben.
    /// </summary>
    public class World : GameComponent, IWorld
    {
        #region private fields

        private ICanyonShooterGame game = null;
        private GameComponentCollection components = null;
        private string levelName = null;
        private Level level = null;
        private int levelLoadStatus = -1;
        private int levelUnloadStatus = -1;

        private Color ambientLight = new Color(50, 40, 40);

        private ICanyon canyon = null;
        private Sky sky = null;
        private IPlayer player, ghost;
        private FreePerspectiveCamera freeCamera;

        private List<IPointLight> pointLights = new List<IPointLight>();

        private List<IEffect> effects = new List<IEffect>();

        // specific objects. all these objects are also stored in gameObjects
        private List<IItem> items = new List<IItem>();
        private List<IEnemy> enemies = new List<IEnemy>();
        private List<IStatic> statics = new List<IStatic>();

        // contains all GameObjects
        private List<IGameObject> gameObjects = new List<IGameObject>();

        private const int StreamUnloadBackward = 2;
        private const int StreamLoadForward = 12;

        #endregion

        public Level Level { get { return level; } }

        public int LevelUnloadStatus { get { return levelUnloadStatus; } }
        public int LevelLoadStatus { get { return levelLoadStatus; } }
        private Finish finish;

        public Finish Finish { 
            get { return finish; }
            set { finish = value; }
        }

        public World(ICanyonShooterGame Game, string LevelName, GameComponentCollection Components)
            : base(Game as Game)
        {
            game = Game;
            components = Components;
            levelName = LevelName;

            game.World = this; // Erste Komponenten benötigen dies bereits jetzt

            // wenn world sich selbst zerstört, soll kein objekt mehr in der update-warteschleife sein, weil dieses objekt world benötigen könnte.
            UpdateOrder = int.MaxValue-1;

            // Sky
            sky = new Sky(game, "Skybox");
            if (game.Graphics.ShadowMappingSupported)
            {
                sky.Sunlight.Shadows = true;
            }
            components.Add(sky);

            if (levelName != "")
            {
                LoadLevel();

                // Canyon
                canyon = new Canyon.Canyon(game);
                components.Add(canyon);

                freeCamera = new FreePerspectiveCamera(game);
                components.Add(freeCamera);

                // Player 1
                //player = new Player(game, 1);
                player = new Player2(game);
                //ghost = new Ghost(game);
                AddObject(player);

                if (levelName.Equals("The Hell of Tunnels") || levelName.Equals("The Corkscrew")
                    || levelName.Equals("The Way goes Up and Down"))
                {
                    ((Player2)player).RemainingTime = 90;
                }

                //AddObject(ghost);

                ObjectTests();

                StreamLoader();
            }
        }

        private void ObjectTests()
        {
            // Items
            AddObject(new Item(game, "Health_Medium"));

            // static test
            /*Static static1 = new Static(game, "Rock");
            static1.LocalPosition = new Vector3(50, -100, -1000);
            AddObject(static1);

            Vector3[] positions = new Vector3[2];
            Quaternion[] rotations = new Quaternion[2];
            float[] scales = new float[2];
            positions[0] = new Vector3(50, -100, -900);
            positions[1] = new Vector3(50, -100, -1100);
            rotations[0] = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), (float)Math.PI / 4);
            rotations[1] = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), -(float)Math.PI / 4);
            scales[0] = 0.5f;
            scales[1] = 0.5f;
            Statics statics1 = new Statics(game, 0, "Rocks", positions, rotations, scales, true);
            AddObject(statics1);*/

            // debris test
            /*debris2 = new DebrisEmitter(game, "debrisTest", 50, 0.5f, 1.0f);
            debris2.LocalPosition = new Vector3(50, 200, -1000);
            debris2.Type = new DebrisEmitterTypeVolumeOOBB(game, new Vector3(100, 100, 200));
            AddObject(debris2);
            if (!Helper.Lock("Debris Test 2", TimeSpan.FromSeconds(15)))
                debris2.Emit(50);

            debris3 = new DebrisEmitter(game, "debrisTest", 20, 0.5f, 1.0f);
            debris3.LocalPosition = new Vector3(-230, 0, -1000);
            debris3.Type = new DebrisEmitterTypeCone(game, new Vector3(1, 0, 0), 10, 10, 50);
            AddObject(debris3);
            if (!Helper.Lock("Debris Test 3", TimeSpan.FromSeconds(15)))
                debris3.Emit(20);*/

            MovingPointLight light1 = new MovingPointLight(game, Color.Green, new Vector3(-100, 50, -400), new Vector3(150, 0, 0), Vector3.UnitY, 0.02f);
            AddPointLight(light1);
            components.Add(light1);
            MovingPointLight light2 = new MovingPointLight(game, new Color(150,0,0), new Vector3(100, 50, -400), new Vector3(-150, 0, 0), Vector3.UnitY, 0.02f);
            AddPointLight(light2);
            components.Add(light2);

            // physics test

            //            EnablePlayerPhysics();
            GraphicalConsole.GetSingleton(game).RegisterObjectFunction(this, "World", "EnablePlayerPhysics");

            if (game.Graphics.ShadowMappingSupported)
            {
                sky.Sunlight.ShadowMapLow.Scene.AddDrawable(canyon);
                sky.Sunlight.ShadowMapHigh.Scene.AddDrawable(player);
                sky.Sunlight.ShadowMapHigh.Scene.AddDrawable(canyon);
            }
        }


        public new void Dispose()
        {
            clearObjects();
            if(canyon != null)
            canyon.Dispose();
            components.Remove(canyon);
            if(player != null)
            player.Dispose();
            components.Remove(player);
            if(freeCamera != null)
            freeCamera.Dispose();
            components.Remove(freeCamera);
            if(sky != null)
            sky.Dispose();
            components.Remove(sky);

            base.Dispose();
        }


        private void clearObjects()
        {
            // delete all objects (gameObjects includes enemies, items, statics and more)
            // otherwise they would remain active in the physics engine and they might throw an exception on collision
            foreach (IGameObject obj in gameObjects)
            {
                obj.Dispose();
                components.Remove(obj);
            }
            gameObjects.Clear();

            // clear specific object lists
            enemies.Clear();
            items.Clear();
            statics.Clear();

            // remove all effects
            foreach(IEffect ef in effects)
            {
                components.Remove(ef);
            }
            effects.Clear();
        }
        

        public void EnablePlayerPhysics()
        {
            //// Crate Player Shape:
            SphereShapeData sData = new SphereShapeData();
            sData.Radius = 5;
            player.AddShape(sData);
        }
        

        private void LoadLevel()
        {
            level = LevelManager.Load(string.Format("GameClasses\\World\\Levels\\{0}.csl", levelName));

            level.CalcCache();
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            freeCamera.Initialize();

            base.Initialize();
        }

        private void StreamUnloader()
        {
            if (player.CanyonPosition - StreamUnloadBackward > levelUnloadStatus &&
                player.CanyonPosition - StreamUnloadBackward >= 0/* &&
                player.CanyonPosition - StreamUnloadBackward < level.Blocks.Length - 1*/)
            {
                levelUnloadStatus++;
                canyon.StreamUnload(levelUnloadStatus);

                List<IGameObject> newList = new List<IGameObject>();
                List<IGameObject> oldList = gameObjects;
                gameObjects = new List<IGameObject>();

                foreach (IGameObject o in oldList)
                {
                    if (o.CanyonSegment <= levelUnloadStatus && o.CanyonSegment > -1)
                    {
                        //o.Dispose();
                        o.Destroy();
                    }
                    else
                    {
                        newList.Add(o);
                    }
                }

                gameObjects = newList;
                
                StreamUnloader();
            }
        }
        private void StreamLoader()
        {
            if (player.CanyonPosition + StreamLoadForward > levelLoadStatus &&
                player.CanyonPosition + StreamLoadForward < level.Blocks.Length - 1)
            {
                levelLoadStatus++;
                canyon.StreamLoad(levelLoadStatus, level);

                // Objekte laden
                if (level.Blocks[levelLoadStatus].Objects != null)
                {
                    foreach (EditorDescription d in level.Blocks[levelLoadStatus].Objects)
                    {
                        Type t = Type.GetType("CanyonShooter.GameClasses.World."+d.CreateType);
                        if(typeof(GameObject).IsAssignableFrom(t)) // kann GameObject dieser Type zugewiesen werden?
                        {
                            d.RelativeSpawnLocation  = level.Cache[levelLoadStatus].APos;
                            d.RelativeSpawnLocation += level.Cache[levelLoadStatus].X * d.RelativeSpawnPosition.X;
                            d.RelativeSpawnLocation += level.Cache[levelLoadStatus].Y * d.RelativeSpawnPosition.Y;
                            d.RelativeSpawnLocation += level.Cache[levelLoadStatus].Z * d.RelativeSpawnPosition.Z;
                            d.SegmentId = levelLoadStatus;
                            AddObject(Activator.CreateInstance(t, new object[] { game, d }) as IGameObject);
                        }
                    }
                }

                // TODO load from level data
                if(levelLoadStatus > 1)
                {
                    if (game.Graphics.ShaderModel3Supported)
                    {
                        StaticsFactory.createWithRandomRotation(game, canyon.GetSegmentFromGlobalIndex(levelLoadStatus - 1), "SmallRock", game.GameStates.Profil.CurrentProfil.Detail * 2.5f, 1.0f, 2.0f);
                        StaticsFactory.createWithRandomAxisRotation(game, canyon.GetSegmentFromGlobalIndex(levelLoadStatus - 1), "Tussocks", game.GameStates.Profil.CurrentProfil.Detail * 2.5f, 0.2f, 0.4f, Vector3.UnitY, 0, (float)Math.PI * 2);
                    }
                }

                StreamLoader();
            }
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (levelName != "")
            {
                if (levelName.Equals("The Race"))
                {
                    if (levelLoadStatus == 300)
                    {
                        levelLoadStatus++;     
                        ((Player2)player).Finish();
                        game.GameStates.Score.AddPoints(100 * ((Player2)player).RemainingTime);
                    }
                }
                if (game.Input.HasKeyJustBeenPressed("Game.Menu"))
                {
                    game.GameStates.SetStateMenu();
                    //((CanyonShooterGame)game).Exit();
                    return;
                }

                StreamUnloader();
                StreamLoader();

                if (game.Input.HasKeyJustBeenPressed("Debug.SwitchCameras"))
                {
                    if (game.Renderer.Camera == freeCamera)
                    {

                        game.Renderer.Camera = game.World.Players[0].Camera;
                        game.GameStates.InputFocus = game.World;
                    }
                    else
                    {
                        freeCamera.LocalPosition = game.Renderer.Camera.GlobalPosition;
                        freeCamera.LocalRotation = game.Renderer.Camera.GlobalRotation; // Quaternion.Identity;
                        //freeCamera.Fov = 90;
                        game.Renderer.Camera = freeCamera;
                        game.GameStates.InputFocus = freeCamera;
                    }
                }

            }
            SortLights();
        }

        private class PointLightComparer : IComparer<IPointLight>
        {
            private ICanyonShooterGame game;
            public PointLightComparer(ICanyonShooterGame game)
            {
                this.game = game;
            }

            #region IComparer<IPointLight> Members

            public int Compare(IPointLight x, IPointLight y)
            {
                Vector3 diffX = game.Renderer.Camera.GlobalPosition - x.GlobalPosition;
                Vector3 diffY = game.Renderer.Camera.GlobalPosition - y.GlobalPosition;

                return (int)Math.Ceiling(diffX.Length() - diffY.Length());
            }

            #endregion
        }

        private void SortLights()
        {
            pointLights.Sort(new PointLightComparer(game));   
        }


        #region IWorld Members

        public string Name
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public ReadOnlyCollection<IPlayer> Players
        {
            get
            {
                Collection<IPlayer> p = new Collection<IPlayer>();
                p.Add(player);
                return new ReadOnlyCollection<IPlayer>(p);
            }
        }

        public ICanyon Canyon
        {
            get { return canyon; }
        }

        public ISky Sky
        {
            get { return sky; }
        }


        public ReadOnlyCollection<IItem> Items
        {
            get { return new ReadOnlyCollection<IItem>(this.items); }
        }

        public ReadOnlyCollection<IStatic> Statics
        {
            get { return new ReadOnlyCollection<IStatic>(this.statics); }
        }

        public ReadOnlyCollection<IEffect> Effects
        {
            get { return new ReadOnlyCollection<IEffect>(this.effects); }
        }

        public ReadOnlyCollection<IEnemy> Enemies
        {
            get { return new ReadOnlyCollection<IEnemy>(this.enemies); }
        }

        public ReadOnlyCollection<IPointLight> PointLights
        {
            get { return new ReadOnlyCollection<IPointLight>(pointLights); }
        }


        public void AddPointLight(IPointLight light)
        {
            if(light == null)
                throw new Exception("Ein Licht darf nicht NULL sein! Ich bin dein Vater!");

            if (pointLights.Contains(light))
                throw new Exception("Element bereits im Index.");

            pointLights.Add(light);
        }

        public void RemovePointLight(IPointLight light)
        {
            components.Remove(light);
            pointLights.Remove(light);
        }

        public void AddObject(IGameObject obj)
        {
            if (gameObjects.Contains(obj))
                throw new Exception("Element bereits im Index.");

            if (obj is IEnemy)
            {
                enemies.Add((IEnemy)obj);
            }
            if (obj is IItem)
            {
                items.Add((IItem)obj);
            }
            if (obj is IStatic)
            {
                statics.Add((IStatic)obj);
            }

            gameObjects.Add(obj);
            components.Add(obj);
        }

        public void RemoveObject(IGameObject obj)
        {
            if (obj is IEnemy)
            {
                enemies.Remove((IEnemy)obj);
            }
            if (obj is IItem)
            {
                items.Remove((IItem)obj);
            }
            if (obj is IStatic)
            {
                statics.Remove((IStatic)obj);
            }

            gameObjects.Remove(obj);
            components.Remove(obj);          
        }

        public Color AmbientLight
        {
            get { return ambientLight; }
            set { ambientLight = value; }
        }

        public void AddEffect(IEffect fx)
        {
            effects.Add(fx);
            components.Add(fx);
        }

        public void RemoveEffect(IEffect fx)
        {
            effects.Remove(fx);
            components.Remove(fx);
        }

        #endregion



        #region IWorld Member

        #endregion
    }
}


