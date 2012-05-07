using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZombieSmashers.Particles;
using ZombieSmashers.map.bucket;


namespace ZombieSmashers.map
{
    public class Map
    {
        SegmentDefinition[] segDef;
        MapSegment[,] mapSeg;
        int[,] col;
        Ledge[] ledge;

        private int xSize = 20, ySize = 20;

        public String path = "map";

        public const int LAYER_BACK = 0;
        public const int LAYER_MAP = 1;
        public const int LAYER_FORE = 2;

        public MapScript mapScript;
        public MapFlags GlobalFlags;

        public int Fog;

        public Bucket Bucket;

        protected float pFrame;
        protected float frame;

        public float transInFrame = 0f;
        public float transOutFrame = 0f;

        public float water = 0f;

       

        public string[] TransitionDestination = { "", "", "" };

        public TransitionDirection TransDir;

        

        public Map()
        {
            GlobalFlags = new MapFlags(64);

            ledge = new Ledge[16];
            for (int l = 0; l < ledge.Length; l++)
                ledge[l] = new Ledge();
            segDef = new SegmentDefinition[512];
            mapSeg = new MapSegment[3, 64];
            col = new int[20, 20];
            ReadSegmentDefinitions();
        }

        public void Read()
        {
            BinaryReader file = new BinaryReader(File.Open(@"data/maps/" + path + ".zmx",
                FileMode.Open, FileAccess.Read));

            for (int i = 0; i < ledge.Length; i++)
            {
                ledge[i].TotalNodes = file.ReadInt32();
                for (int n = 0; n < ledge[i].TotalNodes; n++)
                {
                    ledge[i].Nodes[n] = new Vector2(
                        file.ReadSingle() * 2f, file.ReadSingle() * 2f);
                }
                ledge[i].Flags = file.ReadInt32();
            }
            for (int l = 0; l < 3; l++)
            {
                for (int i = 0; i < 64; i++)
                {
                    int t = file.ReadInt32();
                    if (t == -1)
                        mapSeg[l, i] = null;
                    else
                    {
                        mapSeg[l, i] = new MapSegment();
                        mapSeg[l, i].SetDefIdx(t);
                        mapSeg[l, i].SetLoc(new Vector2(
                            file.ReadSingle(),
                            file.ReadSingle()));
                    }
                }
            }
            for (int x = 0; x < 20; x++)
                for (int y = 0; y < 20; y++)
                    col[x, y] = file.ReadInt32();
                
            

            mapScript = new MapScript(this);

            for (int i = 0; i < mapScript.Lines.Length; i++)
            {
                String s = file.ReadString();
                if (s.Length > 0)
                    mapScript.Lines[i] = new MapScriptLine(s);
                else
                    mapScript.Lines[i] = null;
            }

           

            file.Close();

            Bucket = null;
            Fog = -1;
            water = 0f;

            for (int i = 0; i < TransitionDestination.Length; i++)
                TransitionDestination[i] = "";

            if (mapScript.GotoTag("init"))
                mapScript.IsReading = true;
        }

        public void Write()
        {
            BinaryWriter file = new BinaryWriter(File.Open(@"data/" + path + ".zmx",
                FileMode.Create));

            for (int i = 0; i < ledge.Length; i++)
            {
                file.Write(ledge[i].TotalNodes);
                for (int n = 0; n < ledge[i].TotalNodes; n++)
                {
                    file.Write(ledge[i].Nodes[n].X);
                    file.Write(ledge[i].Nodes[n].Y);
                }
                file.Write((int)ledge[i].Flags);
            }
            for (int l = 0; l < 3; l++)
            {
                for (int i = 0; i < 64; i++)
                {
                    if (mapSeg[l, i] == null)
                        file.Write(-1);
                    else
                    {
                        file.Write(mapSeg[l, i].GetDefIdx());
                        file.Write(mapSeg[l, i].GetLoc().X);
                        file.Write(mapSeg[l, i].GetLoc().Y);
                    }
                }
            }
            for (int x = 0; x < 20; x++)
            {
                for (int y = 0; y < 20; y++)
                {
                    file.Write(col[x, y]);
                }
            }
            file.Close();
        }

        public void SetLedgeNode(int l, int n, Vector2 v)
        {
            ledge[l].Nodes[n] = v;
        }

        public void SetLedgeTotalNodes(int l, int t)
        {
            ledge[l].TotalNodes = t;
        }

        public Vector2 GetLedgeNode(int l, int n)
        {
            return ledge[l].Nodes[n];
        }

        public int GetLedgeTotalNodes(int l)
        {
            return ledge[l].TotalNodes;
        }

        public void SetLedgeFlags(int l, int f)
        {
            ledge[l].Flags = f;
        }

        public int GetLedgeFlags(int l)
        {
            return ledge[l].Flags;
        }

        public int GetLedgeSec(int l, float x)
        {
            int r = -1;

            for (int i = 0; i < ledge[l].TotalNodes - 1; i++)
            {

                if (x >= ledge[l].Nodes[i].X &&
                    x <= ledge[l].Nodes[i + 1].X)
                {
                    r = i;
                    break;
                }
            }
            return r;
        }

        public float GetLedgeYLoc(int l, int i, float x)
        {
            return (ledge[l].Nodes[i + 1].Y - ledge[l].Nodes[i].Y) *
                ((x - ledge[l].Nodes[i].X) / (ledge[l].Nodes[i + 1].X 
                - ledge[l].Nodes[i].X))
                + ledge[l].Nodes[i].Y;
        }

        private void ReadSegmentDefinitions()
        {
            StreamReader s = new StreamReader(@"data/maps.zdx");
            String t = "";
            int n;
            int currentTex = 0;
            int curDef = -1;
            Rectangle tRect = new Rectangle();
            String[] split;

            t = s.ReadLine();

            while (!s.EndOfStream)
            {
                t = s.ReadLine();
                if (t.StartsWith("#"))
                {
                    if (t.StartsWith("#src"))
                    {
                        split = t.Split(' ');
                        if (split.Length > 1)
                        {
                            n = Convert.ToInt32(split[1]);
                            currentTex = n - 1;
                        }
                    }
                }
                else
                {
                    curDef++;
                    String name = t;
                    
                    t = s.ReadLine();
                    split = t.Split(' ');
                    if (split.Length > 3)
                    {
                        tRect.X = Convert.ToInt32(split[0]);
                        tRect.Y = Convert.ToInt32(split[1]);
                        tRect.Width = Convert.ToInt32(split[2]) - tRect.X;
                        tRect.Height = Convert.ToInt32(split[3]) - tRect.Y;
                    }
                    else
                    {
                        Console.WriteLine("read fail: " + name);
                    }
                    
                    int tex = currentTex;

                    t = s.ReadLine();
                    int flags = Convert.ToInt32(t);

                    segDef[curDef] = new SegmentDefinition(name, tex, tRect, flags);
                }
            }
        }

        public int GetCol(int x, int y)
        {
            return col[x, y];
        }

        public void SetCol(int x, int y, int val)
        {
            col[x, y] = val;
        }

        public SegmentDefinition GetSegDef(int idx)
        {
            return segDef[idx];
        }

        public Vector2 GetSegLoc(int l, int i)
        {
            return mapSeg[l, i].GetLoc();
        }

        public void SetSegLoc(int l, int i, Vector2 loc)
        {
            mapSeg[l, i].SetLoc(loc);
        }

        public int AddSeg(int l, int idx)
        {
            for (int i = 0; i < 64; i++)
            {
                if (mapSeg[l, i] == null)
                {
                    mapSeg[l, i] = new MapSegment();
                    mapSeg[l, i].SetDefIdx(idx);
                    return i;
                }
            }
            return -1;
        }

        public bool CheckParticleCol(Vector2 loc)
        {
            if (CheckCol(loc))
                return true;
            for (int i = 0; i < 16; i++)
            {
                if (GetLedgeTotalNodes(i) > 1)
                {
                    if (GetLedgeFlags(i) == (int)LedgeFlags.Solid)
                    {
                        int s = GetLedgeSec(i, loc.X);

                        if (s > -1)
                        {
                            if (GetLedgeYLoc(i, s, loc.X) < loc.Y)
                                return true;
                        }
                    }
                }
            }
            return false;
        }


        public bool CheckCol(Vector2 loc)
        {
            if (loc.X < 0f) return true;
            if (loc.Y < 0f) return true;

            int x = (int)(loc.X / 64f);
            int y = (int)(loc.Y / 64f);
            
            if (x >= 0 && y >= 0 && x < xSize && y < ySize)
            {
                if (col[x, y] == 0) return false;
            }
            return true;
        }

        

        public float GetXLim()
        {
            return (float)xSize * 64f - Game1.ScreenSize.X;
        }

        public float GetYLim()
        {
            return (float)ySize * 64f - Game1.ScreenSize.Y;
        }

        public float GetTransVal()
        {
            if (transInFrame > 0f)
            {
                return transInFrame;
            }
            if (transOutFrame > 0f)
            {
                return 1 - transOutFrame;
            }
            return 0f;
        }

        public void CheckTransitions(Character[] c)
        {   
            if (transOutFrame <= 0f && transInFrame <= 0f)
            {
                if (c[0].DyingFrame > 0f)
                    return;

                if (c[0].Loc.X > (float)xSize * 64f - 32f &&
                    c[0].Trajectory.X > 0f &&
                    c[0].KeyRight && c[0].AnimName == "run")
                {
                    if (TransitionDestination[(int)TransitionDirection.Right]
                        != "")
                    {
                        transOutFrame = 1f;
                        TransDir = TransitionDirection.Right;
                    }
                }
                if (c[0].Loc.X < 0f + 32f &&
                    c[0].Trajectory.X < 0f &&
                    c[0].KeyLeft && c[0].AnimName == "run")
                {
                    if (TransitionDestination[(int)TransitionDirection.Left]
                           != "")
                    {
                        transOutFrame = 1f;
                        TransDir = TransitionDirection.Left;
                    }
                }
            }
        }

        public void Update(ParticleManager pMan, Character[] c)
        {
            CheckTransitions(c);
            if (transOutFrame > 0f)
            {
                transOutFrame -= Game1.FrameTime * 3f;
                if (transOutFrame <= 0f)
                {
                    path = TransitionDestination[(int)TransDir];
                    Read();
                    transInFrame = 1.1f;
                    for (int i = 1; i < c.Length; i++)
                        c[i] = null;
                    pMan.Reset();
                }
            }
            if (transInFrame > 0f)
            {
                transInFrame -= Game1.FrameTime * 3f;
            }

            if (mapScript.IsReading)
                mapScript.DoScript(c);

            if (Bucket != null)
            {
                if (!Bucket.IsEmpty)
                    Bucket.Update(c);
            }

            frame += Game1.FrameTime;

            
            if (Fog > -1)
            {
                if ((int)(pFrame * 10f) != (int)(frame * 10f))
                {
                    pMan.AddParticle(new Fog(
                        Rand.GetRandomVector2(0f, (float)xSize * 64f,
                        (float)Fog + 150f, (float)Fog + 300f)));
                }
            }
            for (int i = 0; i < 64; i++)
            {
                if (mapSeg[LAYER_MAP, i] != null)
                {
                    if (segDef[mapSeg[LAYER_MAP, i].GetDefIdx()].GetFlags() ==
                        SegmentDefinition.FLAGS_TORCH)
                    {
                        pMan.AddParticle(new Smoke(
                            mapSeg[LAYER_MAP, i].GetLoc() * 2f
                            + new Vector2(20f, 13f),
                            Rand.GetRandomVector2(-50.0f, 50.0f, -300.0f, -200.0f),
                            1.0f, 0.8f, 0.6f, 1.0f, Rand.GetRandomFloat(0.25f, 0.5f),
                            Rand.GetRandomInt(0, 4)), true);
                        pMan.AddParticle(new Fire(
                            mapSeg[LAYER_MAP, i].GetLoc() * 2f
                            + new Vector2(20f, 37f),
                            Rand.GetRandomVector2(
                            -30.0f, 30.0f, -250.0f, -200.0f),
                                        Rand.GetRandomFloat(0.25f, 0.75f),
                                        Rand.GetRandomInt(0, 4)), true);
                        pMan.AddParticle(new Heat(mapSeg[LAYER_MAP, i].GetLoc() * 2f
                            + new Vector2(20f, -50f),
                            Rand.GetRandomVector2(-50f, 50f, -400f, -300f),
                            Rand.GetRandomFloat(1f, 2f)));
                    }
                }
            }
            
            pFrame = frame;
        }

        public void Draw(SpriteBatch sprite, Texture2D[] mapsTex, 
            Texture2D[] mapBackTex,
            int startLayer, int endLayer)
        {
            Rectangle sRect = new Rectangle();
            Rectangle dRect = new Rectangle();

            sprite.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            if (startLayer == LAYER_BACK)
            {
                float xLim = GetXLim();
                float yLim = GetYLim();
                Vector2 targ = new Vector2(
                    Game1.ScreenSize.X / 2f - ((Game1.Scroll.X / xLim) - 0.5f) * 100f,
                    Game1.ScreenSize.Y / 2f - ((Game1.Scroll.Y / yLim) - 0.5f) * 100f
                    );

                sprite.Draw(mapBackTex[0], targ, new Rectangle(0, 0, 1280, 720), Color.White, 0f,
                    new Vector2(640f, 360f), 1f, SpriteEffects.None, 1f);
            }

            for (int l = startLayer; l < endLayer; l++)
            {
                float scale = 1.0f;
                Color color = Color.White;
                if (l == 0)
                {
                    color = Color.Gray;
                    scale = 0.75f;
                }
                if (l == 2)
                {
                    color = Color.Black;
                    scale = 1.25f;
                }

                //scale *= 0.5f;

                for (int i = 0; i < 64; i++)
                {
                    if (mapSeg[l, i] != null)
                    {
                        sRect = segDef[mapSeg[l, i].GetDefIdx()].GetSrcRect();
                        dRect.X = (int)(mapSeg[l, i].GetLoc().X * 2f - 
                            Game1.Scroll.X * scale);
                        dRect.Y = (int)(mapSeg[l, i].GetLoc().Y * 2f -
                            Game1.Scroll.Y * scale);
                        dRect.Width = (int)((float)sRect.Width * scale);
                        dRect.Height = (int)((float)sRect.Height * scale);

                        sprite.Draw(mapsTex[segDef[mapSeg[l, i].GetDefIdx()].GetSrcIdx()],
                            dRect,
                            sRect,
                            color);
                    }
                }
            }

            sprite.End();
        }

        
    }
}
