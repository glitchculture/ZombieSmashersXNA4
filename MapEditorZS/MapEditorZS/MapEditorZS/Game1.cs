using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using MapEditor.map;

namespace MapEditor
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        ContentManager content;

        Text text;
        Texture2D[] mapsTex;
        Texture2D nullTex;
        Texture2D textTex;

        SpriteBatch sprite;

        Texture2D iconsTex;
        int mosX, mosY;
        bool mouseDown;
        bool mouseClick;

        bool midMouseDown;
        Vector2 scroll;

        int scriptScroll;
        int selScript = -1;

        int selIdx;
        int selScroll;

        int mouseDragSeg = -1;
        int curLayer = 1;
        int pMosX, pMosY;

        int segScroll = 0;

        const int COLOR_NONE = 0;
        const int COLOR_YELLOW = 1;
        const int COLOR_GREEN = 2;

        const int DRAW_SELECT = 0;
        const int DRAW_COL = 1;
        const int DRAW_LEDGE = 2;
        const int DRAW_SCRIPT = 3;

        int drawType = DRAW_SELECT;

        int curLedge = 0;

        Map map;

        KeyboardState oldKeyState;
        int editingText = -1;

        const int EDITING_NONE = -1;
        const int EDITING_PATH = 0;
        const int EDITING_SCRIPT = 1;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            map = new Map();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            sprite = new SpriteBatch(GraphicsDevice);

            sprite = new SpriteBatch(graphics.GraphicsDevice);
            textTex = Content.Load<Texture2D>(@"gfx/arial");
            text = new Text(textTex, sprite);
            nullTex = Content.Load<Texture2D>(@"gfx/1x1");
            mapsTex = new Texture2D[1];
            for (int i = 0; i < mapsTex.Length; i++)
                mapsTex[i] = Content.Load<Texture2D>(@"gfx/maps" + (i + 1).ToString());

            iconsTex = Content.Load<Texture2D>(@"gfx/icons");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private void UpdateKeys()
        {
            KeyboardState keystate = new KeyboardState();

            keystate = Keyboard.GetState();

            Keys[] currentkeys = keystate.GetPressedKeys();
            Keys[] lastkeys = oldKeyState.GetPressedKeys();
            bool found = false;

            for (int i = 0; i < currentkeys.Length; i++)
            {
                found = false;

                for (int y = 0; y < lastkeys.Length; y++)
                {
                    if (currentkeys[i] == lastkeys[y]) found = true;
                }
                if (found == false)
                {
                    PressKey(currentkeys[i]);
                }
            }

            oldKeyState = keystate;
        }

        private bool ScriptEnter()
        {
            if (selScript >= map.script.Length - 1)
                return false;
            for (int i = map.script.Length - 1; i > selScript; i--)
                map.script[i] = map.script[i - 1];
            selScript++;
            return true;
        }

        private bool ScriptDelLine()
        {
            if (selScript <= 0)
                return false;
            for (int i = selScript; i < map.script.Length - 1; i++)
                map.script[i] = map.script[i + 1];
            return true;
        }

        private void PressKey(Keys key)
        {
            String t = "";
            switch (editingText)
            {
                case EDITING_PATH:
                    t = map.path;
                    break;
                case EDITING_SCRIPT:
                    if (selScript < 0)
                        return;
                    t = map.script[selScript];
                    break;
                default:
                    return;
            }

            bool delLine = false;

            if (key == Keys.Back)
            {
                if (t.Length > 0)
                    t = t.Substring(0, t.Length - 1);
                else if (editingText == EDITING_SCRIPT)
                {
                    delLine = ScriptDelLine();
                }
            }
            else if (key == Keys.Enter)
            {
                if (editingText == EDITING_SCRIPT)
                {
                    if (ScriptEnter())
                    {
                        t = "";
                    }
                }
                else
                    editingText = EDITING_NONE;
            }
            else
            {
                t = (t + (char)key).ToLower();
            }

            if (!delLine)
            {
                switch (editingText)
                {
                    case EDITING_PATH:
                        map.path = t;
                        break;
                    case EDITING_SCRIPT:
                        map.script[selScript] = t;
                        break;
                }
            }
            else
                selScript--;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            UpdateKeys();
            MouseState mState = Mouse.GetState();
            mosX = mState.X;
            mosY = mState.Y;
            bool pMouseDown = mouseDown;
            if (mState.LeftButton == ButtonState.Pressed)
            {
                if (!mouseDown)
                {
                    if (GetCanEdit())
                    {
                        if (drawType == DRAW_SELECT)
                        {
                            int f = map.GetHoveredSegment(mosX, mosY, curLayer, scroll);
                            if (f != -1)
                            {
                                mouseDragSeg = f;
                                pMosX = mosX;
                                pMosY = mosY;
                            }
                        }
                        if (drawType == DRAW_LEDGE)
                        {
                            if (map.GetLedgeTotalNodes(curLedge) < 15)
                            {
                                map.SetLedgeNode(curLedge, map.GetLedgeTotalNodes(curLedge),
                                    new Vector2((float)mosX, (float)mosY) + scroll / 2.0f);
                                map.SetLedgeTotalNodes(curLedge, map.GetLedgeTotalNodes(curLedge) + 1);
                            }
                        }
                        if (drawType == DRAW_SCRIPT)
                        {
                            if (selScript > -1)
                            {
                                if (mosX < 400)
                                {
                                    Vector2 v = new Vector2((float)mosX, (float)mosY) + scroll / 2.0f;
                                    v *= 2f;
                                    map.script[selScript] +=
                                        ((int)(v.X)).ToString() + " " +
                                        ((int)(v.Y)).ToString();
                                }
                            }
                        }
                    }
                }
                mouseDown = true;
            }
            else
                mouseDown = false;
            if (pMouseDown && !mouseDown) mouseClick = true;

            if (mouseClick) editingText = EDITING_NONE;


            if (mState.MiddleButton == ButtonState.Pressed)
            {
                if (!midMouseDown)
                {
                    pMosX = mosX;
                    pMosY = mosY;
                    midMouseDown = true;
                }
            }
            else
                midMouseDown = false;

            if (mouseDragSeg > -1)
            {
                if (!mouseDown)
                    mouseDragSeg = -1;
                else
                {
                    Vector2 loc = map.GetSegLoc(curLayer, mouseDragSeg);
                    loc.X += (float)(mosX - pMosX);
                    loc.Y += (float)(mosY - pMosY);
                    pMosX = mosX;
                    pMosY = mosY;
                    map.SetSegLoc(curLayer, mouseDragSeg, loc);
                }
            }
            if (midMouseDown)
            {
                scroll.X -= (float)(mosX - pMosX) * 2.0f;
                scroll.Y -= (float)(mosY - pMosY) * 2.0f;
                pMosX = mosX;
                pMosY = mosY;
            }

            if (drawType == DRAW_COL)
            {
                if (GetCanEdit())
                {
                    int x = (mosX + (int)scroll.X / 2) / 32;
                    int y = (mosY + (int)scroll.Y / 2) / 32;
                    if (x >= 0 && y >= 0 && x < map.xSize && y < map.ySize)
                    {
                        if (mState.LeftButton == ButtonState.Pressed)
                            map.SetCol(x, y, 1);

                        if (mState.RightButton == ButtonState.Pressed)
                            map.SetCol(x, y, 0);
                    }
                }
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        private void DrawGrid()
        {
            sprite.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            for (int y = 0; y <= map.ySize; y++)
            {
                for (int x = 0; x <= map.xSize; x++)
                {
                    Rectangle dRect = new Rectangle(
                        x * 32 - (int)scroll.X / 2,
                        y * 32 - (int)scroll.Y / 2,
                        32,
                        32
                        );

                    if (x < map.xSize)
                        sprite.Draw(nullTex, new Rectangle(
                            dRect.X, dRect.Y,
                            32,
                            1
                            ), new Color(new Vector4(1.0f, 0.0f, 0.0f, 0.3f)));

                    if (y < map.ySize)
                        sprite.Draw(nullTex, new Rectangle(
                            dRect.X, dRect.Y,
                            1,
                            32
                            ), new Color(new Vector4(1.0f, 0.0f, 0.0f, 0.3f)));

                    if (x < map.xSize && y < map.ySize)
                    {
                        if (map.GetCol(x, y) == 1)
                        {
                            sprite.Draw(nullTex, dRect, new Color(new Vector4(1.0f, 0.0f, 0.0f, 0.3f)));
                        }
                    }
                }
            }

            sprite.End();
        }

        private bool GetCanEdit()
        {
            if (mosX > 100 && mosX < 500 && mosY > 100 && mosY < 550)
                return true;
            return false;
        }

        private void DrawLedges()
        {
            Rectangle rect = new Rectangle();
            sprite.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            Color tColor = new Color();


            rect.X = 32;
            rect.Y = 0;
            rect.Width = 32;
            rect.Height = 32;

            for (int i = 0; i < 16; i++)
            {
                if (map.GetLedgeTotalNodes(i) > 0)
                {
                    for (int n = 0; n < map.GetLedgeTotalNodes(i); n++)
                    {
                        Vector2 tVec;
                        tVec = map.GetLedgeNode(i, n);
                        tVec -= scroll / 2.0f;
                        tVec.X -= 5.0f;
                        if (curLedge == i)
                            tColor = Color.Yellow;
                        else
                            tColor = Color.White;
                        sprite.Draw(iconsTex, tVec, rect,
                            tColor, 0.0f, new Vector2(0, 0), 0.35f, SpriteEffects.None, 0.0f);

                        if (n < map.GetLedgeTotalNodes(i) - 1)
                        {
                            Vector2 nVec;
                            nVec = map.GetLedgeNode(i, n + 1);
                            nVec -= scroll / 2.0f;
                            nVec.X -= 4.0f;
                            for (int x = 1; x < 20; x++)
                            {
                                Vector2 iVec = (nVec - tVec) * ((float)x / 20.0f) + tVec;

                                Color nColor = new Color(new Vector4(1.0f, 1.0f, 1.0f, 0.25f));
                                if (map.GetLedgeFlags(i) == 1) nColor =
                                    new Color(new Vector4(1.0f, 0.0f, 0.0f, 0.25f));
                                sprite.Draw(iconsTex, iVec, rect,
                                    nColor,
                                    0.0f, new Vector2(0, 0), 0.25f,
                                    SpriteEffects.None, 0.0f);
                            }
                        }
                    }
                }
            }

            sprite.End();
        }

        private void DrawCursor()
        {
            Rectangle rect = new Rectangle();
            sprite.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            Color tColor = new Color();

            tColor = Color.White;
            if (GetCanEdit()) tColor = Color.Yellow;

            rect.X = 0;
            rect.Y = 0;
            rect.Width = 32;
            rect.Height = 32;

            sprite.Draw(iconsTex, new Vector2((float)mosX, (float)mosY), rect,
                tColor, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.0f);

            sprite.End();
        }

        private bool drawButton(int x, int y, int idx, int mosX, int mosY, bool mouseClick)
        {
            bool r = false;

            Rectangle sRect = new Rectangle(32 * (idx % 8),
                32 * (idx / 8), 32, 32);
            Rectangle dRect = new Rectangle(x, y, 32, 32);

            if (dRect.Contains(mosX, mosY))
            {
                dRect.X -= 1;
                dRect.Y -= 1;
                dRect.Width += 2;
                dRect.Height += 2;
                if (mouseClick)
                    r = true;
            }
            sprite.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            sprite.Draw(iconsTex, dRect, sRect, Color.White);
            sprite.End();

            return r;
        }



        private int GetCommandColor(String s)
        {
            switch (s)
            {
                case "fog":
                case "monster":
                case "makebucket":
                case "addbucket":
                case "ifnotbucketgoto":

                case "wait":

                case "setflag":
                case "iftruegoto":
                case "iffalsegoto":

                case "setglobalflag":
                case "ifglobaltruegoto":
                case "ifglobalfalsegoto":

                case "stop":
                case "setleftexit":
                case "setleftentrance":
                case "setrightexit":
                case "setrightentrance":
                case "setintroentrance":
                case "water":
                    return COLOR_GREEN;
                case "tag":
                    return COLOR_YELLOW;
            }
            return COLOR_NONE;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            Rectangle sRect = new Rectangle();
            Rectangle dRect = new Rectangle();

            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            map.Draw(sprite, mapsTex, scroll);

            //if (drawType == DRAW_COL)
            DrawGrid();
            DrawLedges();

            sprite.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            Color oColor = new Color(new Vector4(1.0f, 1.0f, 1.0f, 0.3f));
            sprite.Draw(nullTex, new Rectangle(100, 50, 400, 1), oColor);
            sprite.Draw(nullTex, new Rectangle(100, 50, 1, 500), oColor);
            sprite.Draw(nullTex, new Rectangle(500, 50, 1, 500), oColor);
            sprite.Draw(nullTex, new Rectangle(100, 550, 400, 1), oColor);


            sprite.Draw(nullTex, new Rectangle(100, 300, 400, 1), oColor);

            sprite.End();


            String layerName = "map";
            switch (curLayer)
            {
                case 0:
                    layerName = "back";
                    break;
                case 1:
                    layerName = "mid";
                    break;
                case 2:
                    layerName = "fore";
                    break;
            }
            if (text.DrawClickText(5, 5, "layer: " + layerName, mosX, mosY, mouseClick))
                curLayer = (curLayer + 1) % 3;

            switch (drawType)
            {
                case DRAW_SELECT:
                    layerName = "select";
                    break;
                case DRAW_COL:
                    layerName = "col";
                    break;
                case DRAW_LEDGE:
                    layerName = "ledge";
                    break;
                case DRAW_SCRIPT:
                    layerName = "script";
                    break;
            }
            if (text.DrawClickText(5, 25, "draw: " + layerName, mosX, mosY, mouseClick))
                drawType = (drawType + 1) % 4;

            if (drawType == DRAW_SCRIPT)
            {
                sprite.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                sprite.Draw(nullTex, new Rectangle(400, 20, 400, 565), new Color(
                    new Vector4(0f, 0f, 0f, .62f)));
                sprite.End();

                for (int i = scriptScroll; i < scriptScroll + 28; i++)
                {
                    if (selScript == i)
                    {
                        text.SetColor(Color.White);
                        text.DrawText(405, 25 + (i - scriptScroll) * 20,
                            i.ToString() + ": " + map.script[i] + "*");
                    }
                    else
                    {
                        if (text.DrawClickText(405, 25 + (i - scriptScroll) * 20,
                            i.ToString() + ": " + map.script[i],
                            mosX, mosY, mouseClick))
                        {
                            selScript = i;
                            editingText = EDITING_SCRIPT;
                        }
                    }
                    if (map.script[i].Length > 0)
                    {
                        String[] split = map.script[i].Split(' ');
                        int c = GetCommandColor(split[0]);
                        if (c > COLOR_NONE)
                        {
                            switch (c)
                            {
                                case COLOR_GREEN:
                                    text.SetColor(Color.Lime);
                                    break;
                                case COLOR_YELLOW:
                                    text.SetColor(Color.Yellow);
                                    break;
                            }
                            text.DrawText(405, 25 + (i - scriptScroll) * 20,
                                i.ToString() + ": " + split[0]);
                        }
                    }
                    text.SetColor(Color.White);
                    text.DrawText(405, 25 + (i - scriptScroll) * 20,
                        i.ToString() + ": ");
                }

                if (drawButton(770, 20, 1, mosX, mosY, mouseDown) &&
                    scriptScroll > 0)
                    scriptScroll--;

                if (drawButton(770, 550, 2, mosX, mosY, mouseDown) &&
                    scriptScroll < map.script.Length - 28)
                    scriptScroll++;
            }

            if (drawType == DRAW_LEDGE)
            {
                for (int i = 0; i < 16; i++)
                {
                    int y = 50 + i * 20;
                    if (curLedge == i)
                    {
                        text.SetColor(Color.Lime);
                        text.DrawText(520, 50 + i * 20, "ledge " + i.ToString());
                    }
                    else
                    {
                        if (text.DrawClickText(520, 50 + i * 20, "ledge " + i.ToString(),
                            mosX, mosY, mouseClick))
                            curLedge = i;
                    }
                    text.SetColor(Color.White);
                    text.DrawText(620, 50 + i * 20, "n" + map.GetLedgeTotalNodes(i).ToString());

                    if (text.DrawClickText(680, 50 + i * 20, "f" +
                        map.GetLedgeFlags(i).ToString(), mosX, mosY, mouseClick))
                        map.SetLedgeFlags(i, (map.GetLedgeFlags(i) + 1) % 2);
                }
            }

            text.SetColor(Color.White);
            if (editingText == EDITING_PATH)
                text.DrawText(5, 45, map.path + "*");
            else
            {
                if (text.DrawClickText(5, 45, map.path, mosX, mosY, mouseClick))
                    editingText = EDITING_PATH;
            }

            if (drawButton(5, 65, 3, mosX, mosY, mouseClick))
            {
                map.Write();
                map.Write(true);
            }

            if (drawButton(40, 65, 4, mosX, mosY, mouseClick))
            {
                map.Read();
            }

            for (int i = selScroll; i < selScroll + 20; i++)
            {
                if (map.GetSegIdx(curLayer, i) > -1)
                {
                    SegmentDefinition segDef =
                        map.GetSegDef(map.GetSegIdx(curLayer, i));
                    if (selIdx == i)
                    {
                        text.SetColor(Color.Lime);
                        text.DrawText(5, 100 + (i - selScroll) * 16, segDef.GetName());
                    }
                    else
                    {
                        if (text.DrawClickText(
                            5, 100 + (i - selScroll) * 16, segDef.GetName(),
                            mosX, mosY, mouseClick))
                        {
                            selIdx = i;
                        }
                    }
                }
            }
            if (drawButton(100, 100, 1, mosX, mosY, mouseDown))
            {
                if (selScroll > 0) selScroll--;
            }
            if (drawButton(100, 500, 2, mosX, mosY, mouseDown))
            {
                if (selScroll < 43) selScroll++;
            }
            if (drawButton(5, 500, 1, mosX, mosY, mouseDown))
            {
                if (selIdx > 0)
                {
                    map.SwapSegs(curLayer, selIdx, selIdx - 1);
                    selIdx--;
                }
            }
            if (drawButton(25, 500, 2, mosX, mosY, mouseDown))
            {
                if (selIdx < 63)
                {
                    map.SwapSegs(curLayer, selIdx, selIdx + 1);
                    selIdx++;
                }
            }

            if (drawType == DRAW_SELECT)
            {
                sprite.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

                sprite.Draw(nullTex, new Rectangle(500, 20, 280, 550), new Color(
                    new Vector4(0.0f, 0.0f, 0.0f, 0.4f)));
                sprite.End();

                for (int i = segScroll; i < segScroll + 9; i++)
                {

                    SegmentDefinition segDef = map.GetSegDef(i);
                    if (segDef != null)
                    {
                        sprite.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

                        dRect.X = 500;
                        dRect.Y = 50 + (i - segScroll) * 60;

                        sRect = segDef.GetSrcRect();

                        if (sRect.Width > sRect.Height)
                        {
                            dRect.Width = 45;
                            dRect.Height = (int)(((float)sRect.Height / (float)sRect.Width) * 45.0f);
                        }
                        else
                        {
                            dRect.Height = 45;
                            dRect.Width = (int)(((float)sRect.Width / (float)sRect.Height) * 45.0f);
                        }

                        sprite.Draw(mapsTex[map.GetSegDef(i).GetSrcIdx()], dRect, sRect, Color.White);

                        sprite.End();

                        text.SetSize(0.5f);
                        text.SetColor(Color.White);
                        text.DrawText(dRect.X + 50, dRect.Y, segDef.GetName());

                        if (mouseDown)
                        {
                            if (mosX > dRect.X && mosX < 700 && mosY > dRect.Y && mosY < dRect.Y + 45)
                            {
                                if (mouseDragSeg == -1)
                                {
                                    int f = map.AddSeg(curLayer, i);
                                    if (f > -1)
                                    {
                                        float layerScalar = 0.5f;
                                        if (curLayer == 0)
                                            layerScalar = 0.375f;
                                        if (curLayer == 2)
                                            layerScalar = 0.675f;

                                        map.SetSegLoc(curLayer, f,
                                            new Vector2((float)(mosX - sRect.Width / 4 + scroll.X * layerScalar),
                                            (float)(mosY - sRect.Height / 4 + scroll.Y * layerScalar)));
                                        mouseDragSeg = f;
                                        pMosX = mosX;
                                        pMosY = mosY;
                                    }
                                }
                            }
                        }
                    }
                }

                if (drawButton(740, 20, 1, mosX, mosY, mouseDown))
                {
                    if (segScroll > 0) segScroll--;
                }
                if (drawButton(740, 550, 2, mosX, mosY, mouseDown))
                {
                    if (segScroll < 80) segScroll++;
                }
            }

            Vector2 v = new Vector2((float)mosX, (float)mosY) + scroll / 2.0f;
            v *= 2f;
            text.SetSize(.5f);
            text.SetColor(Color.White);
            text.DrawText(5, 580, ((int)v.X).ToString() + ", " +
                ((int)v.Y).ToString());

            DrawCursor();

            mouseClick = false;

            base.Draw(gameTime);


        }
    }
}
