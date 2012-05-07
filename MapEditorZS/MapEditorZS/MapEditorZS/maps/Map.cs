using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapEditor.map
{
    class Map
    {
        SegmentDefinition[] segDef;
        MapSegment[,] mapSeg;
        int[,] col;
        Ledge[] ledge;

        public int xSize = 20;
        public int ySize = 20;
        public String path = "map";

        public String[] script = new String[128];

        public Map()
        {
            for (int i = 0; i < script.Length; i++)
                script[i] = "";
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

            try
            {
                BinaryReader file = new BinaryReader(File.Open(@"data/" + path + ".zmx",
                    FileMode.Open));


                for (int i = 0; i < ledge.Length; i++)
                {
                    ledge[i].totalNodes = file.ReadInt32();
                    for (int n = 0; n < ledge[i].totalNodes; n++)
                    {
                        ledge[i].SetNode(n, new Vector2(
                            file.ReadSingle(), file.ReadSingle()));
                    }
                    ledge[i].flags = file.ReadInt32();
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
                {
                    for (int y = 0; y < 20; y++)
                    {
                        col[x, y] = file.ReadInt32();
                    }
                }

                for (int i = 0; i < script.Length; i++)
                    script[i] = file.ReadString();

                file.Close();
            }
            catch { return; }
        }

        public void Write()
        {
            Write(false);
        }

        public void Write(bool backUp)
        {
            BinaryWriter file;
            if (backUp)
            {
                //file = new BinaryWriter(File.Open(@"../../../../ZombieSmashersXNA/data/maps/" + path + ".zmx", FileMode.Create));
                file = new BinaryWriter(File.Open(@"data/" + path + ".zmx", FileMode.Create));
            }
            else
            {
                file = new BinaryWriter(File.Open(@"data/" + path + ".zmx",
                    FileMode.Create));
            }

            for (int i = 0; i < ledge.Length; i++)
            {
                file.Write(ledge[i].totalNodes);
                for (int n = 0; n < ledge[i].totalNodes; n++)
                {
                    file.Write(ledge[i].GetNode(n).X);
                    file.Write(ledge[i].GetNode(n).Y);
                }
                file.Write(ledge[i].flags);
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
            for (int i = 0; i < script.Length; i++)
                file.Write(script[i]);
            

            file.Close();
        }

        public void SetLedgeNode(int l, int n, Vector2 v)
        {
            ledge[l].SetNode(n, v);
        }

        public void SetLedgeTotalNodes(int l, int t)
        {
            ledge[l].totalNodes = t;
        }

        public Vector2 GetLedgeNode(int l, int n)
        {
            return ledge[l].GetNode(n);
        }

        public int GetLedgeTotalNodes(int l)
        {
            return ledge[l].totalNodes;
        }

        public void SetLedgeFlags(int l, int f)
        {
            ledge[l].flags = f;
        }

        public int GetLedgeFlags(int l)
        {
            return ledge[l].flags;
        }

        private void ReadSegmentDefinitions()
        {
            StreamReader s = new StreamReader(@"gfx/maps.zdx");
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

        public int GetSegIdx(int l, int i)
        {
            if (mapSeg[l, i] == null)
                return -1;
            return mapSeg[l, i].GetDefIdx();
        }

        public void SetSegLoc(int l, int i, Vector2 loc)
        {
            mapSeg[l, i].SetLoc(loc);
        }

        public void SwapSegs(int l, int i, int n)
        {
            MapSegment tSeg = new MapSegment();

            if (mapSeg[l, i] != null &&
                mapSeg[l, n] != null)
            {
                tSeg.SetDefIdx(mapSeg[l, i].GetDefIdx());
                tSeg.SetLoc(mapSeg[l, i].GetLoc());

                tSeg.SetDefIdx(mapSeg[l, i].GetDefIdx());
                tSeg.SetLoc(mapSeg[l, i].GetLoc());

                mapSeg[l, i].SetDefIdx(mapSeg[l, n].GetDefIdx());
                mapSeg[l, i].SetLoc(mapSeg[l, n].GetLoc());

                mapSeg[l, n].SetDefIdx(tSeg.GetDefIdx());
                mapSeg[l, n].SetLoc(tSeg.GetLoc());
            }
            
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

        public int GetHoveredSegment(int x, int y, int l, Vector2 scroll)
        {
            int r = -1;
            float scale = 1.0f;
            if (l == 0)
                scale = 0.75f;
            if (l == 2)
                scale = 1.25f;
            scale *= 0.5f;
            for (int i = 0; i < 64; i++)
            {
                if (mapSeg[l, i] != null)
                {
                    Rectangle sRect = segDef[mapSeg[l, i].GetDefIdx()].GetSrcRect();
                    Rectangle dRect = new Rectangle(
                        (int)(mapSeg[l, i].GetLoc().X - scroll.X * scale),
                        (int)(mapSeg[l, i].GetLoc().Y - scroll.Y * scale),
                        dRect.Width = (int)((float)sRect.Width * scale),
                        dRect.Height = (int)((float)sRect.Height * scale));
                    if (dRect.Contains(x, y))
                        r = i;
                }
            }
            return r;
        }

        public void Draw(SpriteBatch sprite, Texture2D[] mapsTex, Vector2 scroll)
        {
            Rectangle sRect = new Rectangle();
            Rectangle dRect = new Rectangle();

            sprite.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            for (int l = 0; l < 3; l++)
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
                    color = Color.DarkGray;
                    scale = 1.25f;
                }

                scale *= 0.5f;

                for (int i = 0; i < 64; i++)
                {
                    if (mapSeg[l, i] != null)
                    {
                        sRect = segDef[mapSeg[l, i].GetDefIdx()].GetSrcRect();
                        dRect.X = (int)(mapSeg[l, i].GetLoc().X - scroll.X * scale);
                        dRect.Y = (int)(mapSeg[l, i].GetLoc().Y - scroll.Y * scale);
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
