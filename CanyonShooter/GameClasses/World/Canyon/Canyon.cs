using System.Collections.Generic;
using CanyonShooter.DataLayer.Level;
using CanyonShooter.Engine;
using Microsoft.Xna.Framework;
using System;

namespace CanyonShooter.GameClasses.World.Canyon
{
    /// <summary>
    /// Canyon Komponente, d.h. Generierung und Verwaltung der Meshes.
    /// Bekommt von der World-Komponente einen Bitstrom, in dem die Richtung,
    /// Breite, Höhe und andere charakterisierende Werte stehen. Daraus werden
    /// mehrere Teilstücke des Canyons erzeugt und beim draw()-call werden die
    /// sichtbaren Teile an den Renderer übergeben.
    /// In einer fortgeschrittenen Version müssen vereinfachte Meshs nach außen hin
    /// verfügbar sein, um die Kollision mit dem Player zu berechnen.
    /// </summary>
    public class Canyon : DrawableGameComponent, ICanyon
    {
        private ICanyonShooterGame game = null;

        public Canyon(ICanyonShooterGame game)
            : base(game as Game)
        {
            this.game = game;
            this.DrawOrder = (int)DrawOrderType.Default;
        }

        private readonly List<Segment> canyonSegments = new List<Segment>();
        private List<Segment> prevCanyonSegments = new List<Segment>();


        public override void Draw(GameTime gameTime)
        {
            // TODO: LoadContent wird nicht aufgerufen.
            // Das ist nur eine Dirty Lösung.
            // Bitte umbedingt fixen.
            foreach (Segment segment in canyonSegments)
            {
                segment.Draw(gameTime);
            }
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (Segment segment in canyonSegments)
            {
                segment.Update(gameTime);
            }
            base.Update(gameTime);
        }

        public void Dispose()
        {
            foreach (Segment segment in canyonSegments)
            {
                segment.Dispose();
            }
        }

        protected override void LoadContent()
        {
           // canyonSegments.Add(new CanyonSegment(game, null));
            
            base.LoadContent();
        }

        int scale;
        private void GenerateCanyon()
        {
            scale = 5;
            List<Vector2> form = new List<Vector2>();
            List<Vector2> form2 = new List<Vector2>();
            /*List<Vector3> form3 = new List<Vector3>();
            List<Vector3> form4 = new List<Vector3>();*/
            form.Add(new Vector2(-10, 2) * scale);
            form.Add(new Vector2(-3, 2) * scale);
            form.Add(new Vector2(-3, -2) * scale);
            form.Add(new Vector2(3, -2) * scale);
            form.Add(new Vector2(3, 2) * scale);
            form.Add(new Vector2(10, 2) * scale);

            form2.Add(new Vector2(-13, 2) * scale);
            form2.Add(new Vector2(-4, 3) * scale);
            form2.Add(new Vector2(-4, -3) * scale);
            form2.Add(new Vector2(4, -3) * scale);
            form2.Add(new Vector2(4, 3) * scale);
            form2.Add(new Vector2(13, 2) * scale);
            /*form.Add(new Vector3(-20, 8, 0) * scale);
            form.Add(new Vector3(1, 7.5f, 0) * scale);
            form.Add(new Vector3(-10, 0, 0) * scale);
            form.Add(new Vector3(-10, -6, 0) * scale);
            form.Add(new Vector3(-4, -6, 0) * scale);
            form.Add(new Vector3(-0.5f, -7, 0) * scale);
            form.Add(new Vector3(0.5f, -7, 0) * scale);
            form.Add(new Vector3(4, -6, 0) * scale);
            form.Add(new Vector3(10, -6, 0) * scale);
            form.Add(new Vector3(10, 0, 0) * scale);
            form.Add(new Vector3(-1, 7.5f, 0) * scale);
            form.Add(new Vector3(20, 8, 0) * scale);

            form2.Add(new Vector3(-20, 8, 0) * scale);
            form2.Add(new Vector3(-6, 8, 0) * scale);
            form2.Add(new Vector3(-10, 0, 0) * scale);
            form2.Add(new Vector3(-11, -6.5f, 0) * scale);
            form2.Add(new Vector3(-2, -6, 0) * scale);
            form2.Add(new Vector3(-1, -6.5f, 0) * scale);
            form2.Add(new Vector3(1, -6.5f, 0) * scale);
            form2.Add(new Vector3(2, -6, 0) * scale);
            form2.Add(new Vector3(11, -6.5f, 0) * scale);
            form2.Add(new Vector3(10, 0, 0) * scale);
            form2.Add(new Vector3(6, 8, 0) * scale);
            form2.Add(new Vector3(20, 8, 0) * scale);

            form3.Add(new Vector3(-20, 8, 0) * scale);
            form3.Add(new Vector3(-6, 8, 0) * scale);
            form3.Add(new Vector3(-11, -6, 0) * scale);
            form3.Add(new Vector3(-11, -5, 0) * scale);
            form3.Add(new Vector3(-2, -5, 0) * scale);
            form3.Add(new Vector3(-0.5f, 3, 0) * scale);
            form3.Add(new Vector3(0.5f, 3, 0) * scale);
            form3.Add(new Vector3(2, -5, 0) * scale);
            form3.Add(new Vector3(11, -5, 0) * scale);
            form3.Add(new Vector3(11, -6, 0) * scale);
            form3.Add(new Vector3(6, 8, 0) * scale);
            form3.Add(new Vector3(20, 8, 0) * scale);

            form4.Add(new Vector3(-20, 8, 0) * scale);
            form4.Add(new Vector3(1, 8, 0) * scale);
            form4.Add(new Vector3(-11, 0, 0) * scale);
            form4.Add(new Vector3(1, -4, 0) * scale);
            form4.Add(new Vector3(-10, -6, 0) * scale);
            form4.Add(new Vector3(-1, -6, 0) * scale);
            form4.Add(new Vector3(1, -6, 0) * scale);
            form4.Add(new Vector3(10, -6, 0) * scale);
            form4.Add(new Vector3(-1, -4, 0) * scale);
            form4.Add(new Vector3(11, 0, 0) * scale);
            form4.Add(new Vector3(-1, 8, 0) * scale);
            form4.Add(new Vector3(20, 8, 0) * scale);
            */
            /*List<Vector3> form2 = new List<Vector3>();
            Vector3 vec91 = new Vector3(-10, 2.5f, 0) * scale;
            Vector3 vec911 = new Vector3(-4.5f, 2.5f, 0) * scale;
            Vector3 vec92 = new Vector3(-4, 2, 0) * scale;
            Vector3 vec921 = new Vector3(-5, 0, 0) * scale;
            Vector3 vec922 = new Vector3(-5, -2, 0) * scale;
            Vector3 vec93 = new Vector3(-4, -4, 0) * scale;
            Vector3 vec931 = new Vector3(0, -4, 0) * scale;
            Vector3 vec94 = new Vector3(4, -4, 0) * scale;
            Vector3 vec941 = new Vector3(5, -2, 0) * scale;
            Vector3 vec942 = new Vector3(6, 0, 0) * scale;
            Vector3 vec95 = new Vector3(4, 2, 0) * scale;
            Vector3 vec96 = new Vector3(4.5f, 2.5f, 0) * scale;
            Vector3 vec961 = new Vector3(10, 2.5f, 0) * scale;

            List<Vector3> form3 = new List<Vector3>();
            Vector3 vec81 = new Vector3(-10, 2.5f, 0) * scale;
            Vector3 vec811 = new Vector3(-2.5f, 2.5f, 0) * scale;
            Vector3 vec82 = new Vector3(-2, 2, 0) * scale;
            Vector3 vec821 = new Vector3(-6, 0, 0) * scale;
            Vector3 vec822 = new Vector3(-3, -2, 0) * scale;
            Vector3 vec83 = new Vector3(-4, -4, 0) * scale;
            Vector3 vec831 = new Vector3(0, -6, 0) * scale;
            Vector3 vec84 = new Vector3(4, -4, 0) * scale;
            Vector3 vec841 = new Vector3(3, -2, 0) * scale;
            Vector3 vec842 = new Vector3(6, 0, 0) * scale;
            Vector3 vec85 = new Vector3(2, 2, 0) * scale;
            Vector3 vec86 = new Vector3(2.5f, 2.5f, 0) * scale;
            Vector3 vec861 = new Vector3(10, 2.5f, 0) * scale;

            form.Add(vec1);
            //form.Add(vec11);
            form.Add(vec2);
            //form.Add(vec21);
            //form.Add(vec22);
            form.Add(vec3);
            //form.Add(vec31);
            form.Add(vec4);
            //form.Add(vec41);
            //form.Add(vec42);
            form.Add(vec5);
            form.Add(vec6);
            //form.Add(vec61);

            form3.Add(vec81);
            form3.Add(vec811);
            form3.Add(vec82);
            form3.Add(vec821);
            form3.Add(vec822);
            form3.Add(vec83);
            form3.Add(vec831);
            form3.Add(vec84);
            form3.Add(vec841);
            form3.Add(vec842);
            form3.Add(vec85);
            form3.Add(vec86);
            form3.Add(vec861);

            form2.Add(vec91);
            form2.Add(vec911);
            form2.Add(vec92);
            form2.Add(vec921);
            form2.Add(vec922);
            form2.Add(vec93);
            form2.Add(vec931);
            form2.Add(vec94);
            form2.Add(vec941);
            form2.Add(vec942);
            form2.Add(vec95);
            form2.Add(vec96);
            form2.Add(vec961);
            */
            /*Segment segment = new Segment(game);
            
            SegmentDescription des1 = new SegmentDescription();
            des1.Direction = new Vector3(0, 0, -30) * scale;
            des1.CanyonForm = form;
            segment.CanyonValues = des1;


            Segment segment2 = new Segment(game);

            SegmentDescription des2 = new SegmentDescription();
            des2.Direction = new Vector3(-1, 0, -30) * scale;
            des2.CanyonForm = form2;
            segment2.CanyonValues = des2;

            Segment segment3 = new Segment(game);

            SegmentDescription des3 = new SegmentDescription();
            des3.Direction = new Vector3(0, 0, -30) * scale;
            des3.CanyonForm = form3;
            segment3.CanyonValues = des3;

            Segment segment4 = new Segment(game);

            SegmentDescription des4 = new SegmentDescription();
            des4.Direction = new Vector3(2, 0, -30) * scale;
            des4.CanyonForm = form2;
            segment4.CanyonValues = des4;

            canyonSegments.Add(segment);
            canyonSegments.Add(segment2);
            canyonSegments.Add(segment3);
            canyonSegments.Add(segment4);
            Random test = new Random((int)DateTime.Now.Ticks);
            Random test2 = new Random((int)DateTime.Now.Ticks);
            Random test3 = new Random((int)DateTime.Now.Ticks);*/
            /*segment.LocalPosition = new Vector3(0, -1, -1);
            segment2.LocalPosition = new Vector3(0, -1, -1);
            segment3.LocalPosition = new Vector3(0, -1, -1);
            segment4.LocalPosition = new Vector3(0, -1, -1);*/
            /*for (int i = 0; i < 100; i++)
            {
                SegmentDescription tmpdes = new SegmentDescription();
                int fr = test3.Next(1, 5);
                if (fr == 1)
                {
                    tmpdes.CanyonForm = form;
                }
                else if (fr == 2)
                {
                    tmpdes.CanyonForm = form2;
                }
                else if (fr == 3)
                {
                    tmpdes.CanyonForm = form3;
                }
                else
                {
                    tmpdes.CanyonForm = form2;
                }
                
                Segment tmp = new Segment(game);
                SegmentDescription des = new SegmentDescription(new Vector3(1,0,0),new Vector3(0,1,0), new Vector3(0,0,-10), form) ;
                tmp.CanyonValues = des;
                tmp.LocalPosition = new Vector3(0, -1, -1);
                canyonSegments.Add(tmp);
            }
            connectCanyons();
            
            foreach (Segment seg in canyonSegments)
            {
                if (seg.LastCanyonSegment != null)
                {
                    seg.LoadCanyon();
                }
            }*/
        }

        private void connectCanyons()
        {
            Segment last = null;

            foreach (Segment segment in canyonSegments)
            {
                if (last == null)
                {
                    last = segment;
                }
                else
                {
                    segment.LastCanyonSegment = last;
                    last = segment;
                }
            }
        }

        public Segment GetSegmentFromGlobalIndex(int index)
        {
            foreach (Segment seg in canyonSegments)
            {
                if (seg.GlobalIndex == index)
                {
                    return seg;
                }
            }
            return null;
        }

        #region ICanyon Member

        /*
        bool ICanyon.AddSegment(SegmentDescription data)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        */
        void ICanyon.StreamUnload(int index)
        {
            Segment tmp = canyonSegments[0];
            tmp.NextCanyonSegement.Fields.Last = null;
            tmp.NextCanyonSegement.LastCanyonSegment = null;
            canyonSegments.Remove(tmp);
            tmp.Dispose(); 
        }



        void ICanyon.StreamLoad(int index, Level level)
        {
            //if (index == 0) return; // Ersten Block überspringen, da hier noch keine Wand aufgespannt werden kann.
            List<Vector2> cForm = new List<Vector2>();
            foreach (Vec2 tmp in level.Blocks[index].Walls)
            {
                cForm.Add(tmp.v);
            }
            if (index == 0)
            {
                
                SegmentDescription des = new SegmentDescription(level.Cache[index].X, level.Cache[index].Y, Vector3.Zero, cForm);
                Segment tmpSegment = new Segment(game);
                tmpSegment.CanyonValues = des;
                tmpSegment.GlobalIndex = index;
                canyonSegments.Add(tmpSegment);

            }
            else if (index == 1)
            {
                SegmentDescription des = new SegmentDescription(level.Cache[index].X, level.Cache[index].Y, level.Cache[index - 1].ADir, cForm);
                Segment tmpSegment = new Segment(game);
                tmpSegment.CanyonValues = des;
                canyonSegments[canyonSegments.Count - 1].NextCanyonSegement = tmpSegment;
                tmpSegment.LastCanyonSegment = canyonSegments[canyonSegments.Count - 1];
                tmpSegment.GlobalIndex = index;
                canyonSegments.Add(tmpSegment);
                canyonSegments[canyonSegments.Count - 1].LoadSegmentPoints();

            }
            else
            {
                SegmentDescription des = new SegmentDescription(level.Cache[index].X, level.Cache[index].Y, level.Cache[index - 1].ADir, cForm);
                Segment tmpSegment = new Segment(game);
                tmpSegment.CanyonValues = des;
                canyonSegments[canyonSegments.Count - 1].NextCanyonSegement = tmpSegment;
                tmpSegment.LastCanyonSegment = canyonSegments[canyonSegments.Count - 1];
                tmpSegment.GlobalIndex = index;
                canyonSegments.Add(tmpSegment);
                canyonSegments[canyonSegments.Count - 1].LoadSegmentPoints();
                canyonSegments[canyonSegments.Count - 2].LoadCanyonModel();
                if (index > 5)
                {
                    canyonSegments[canyonSegments.Count - 5].LoadBoundingBoxes();
                }
                if (index > 7)
                {
                    canyonSegments[canyonSegments.Count - 7].LoadSkyBox();
                }
            }
            /*
            List<Vector2> cForm = new List<Vector2>();
            foreach (Vec2 tmp in level.Blocks[index-1].Walls)
            {      
               cForm.Add(tmp.v);
            }
            SegmentDescription des = new SegmentDescription(level.Cache[index-1].X, level.Cache[index-1].Y, level.Cache[index-1].ADir, cForm) ;
            Segment tmpSegment = new Segment(game);
            tmpSegment.CanyonValues = des;
            if (canyonSegments.Count != 0)
            {
                tmpSegment.LastCanyonSegment = canyonSegments[canyonSegments.Count - 1]; 
                
            }
            */
            //canyonSegments.Add(tmpSegment);
            /*if (tmpSegment.LastCanyonSegment != null)
            {
                tmpSegment.LoadSegements();
            }
            if (tmpSegment.LastCanyonSegment != null)
            {
                tmpSegment.LoadCanyon();
            }*/
            // ToDo: implementieren
            //
            // Alle Benötigten Vars:
            // - level.Blocks[index].Walls -- enthält die Liste der Wände
            // - level.Cache[index] -- enthält die "Zwischenebenen"

            // genauer:  (wenn du mehr brauchst, machst du was falsch) ;-)
            // - level.Blocks[index-1].Walls
            // - level.Blocks[index].Walls
            // - level.Cache[index-1].Pos
            // - level.Cache[index-1].X
            // - level.Cache[index-1].Y
            // - level.Cache[index].Pos
            // - level.Cache[index].X
            // - level.Cache[index].y
            //level.Cache[index]
            // Momentan werden bereits Beispielwerte übergeben. Diese sind denen von oben sehr ähnlich.
        }

        #endregion

        #region IGameComponent Member

        void IGameComponent.Initialize()
        {
            
        }

        #endregion
    }
}
