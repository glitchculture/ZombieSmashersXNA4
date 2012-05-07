//#define SCREENSHOTMODE

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
using xCharEdit.Character;

namespace xCharEdit
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D arialText;
        Text text;
        CharDef charDef;

        Texture2D[] headTex;
        Texture2D[] torsoTex;
        Texture2D[] legsTex;
        Texture2D[] weaponTex;

        Texture2D nullTex;
        Texture2D iconsTex;

        const int FACE_LEFT = 0;
        const int FACE_RIGHT = 1;

        int selPart;
        int selFrame;
        int selAnim;
        int selKeyFrame;

        int frameScroll;
        int animScroll;
        int keyFrameScroll;

        bool mouseClick;

        MouseState mouseState;
        MouseState preState = Mouse.GetState();

        KeyboardState oldKeyState;

        bool playing;
        int curKey;
        float curFrame;

        const int AUX_SCRIPT = 0;
        const int AUX_TRIGS = 1;
        const int AUX_TEXTURES = 2;

        int auxMode = AUX_SCRIPT;
        int trigScroll = 0;

        const int EDITING_NONE = -1;
        const int EDITING_FRAME_NAME = 0;
        const int EDITING_PATH = 1;
        const int EDITING_ANIMATION_NAME = 2;
        const int EDITING_SCRIPT = 3;

        public const int TRIG_PISTOL_ACROSS = 0;
        public const int TRIG_PISTOL_UP = 1;
        public const int TRIG_PISTOL_DOWN = 2;
        public const int TRIG_WRENCH_UP = 3;
        public const int TRIG_WRENCH_DOWN = 4;
        public const int TRIG_WRENCH_DIAG_UP = 5;
        public const int TRIG_WRENCH_DIAG_DOWN = 6;
        public const int TRIG_WRENCH_UPPERCUT = 7;
        public const int TRIG_WRENCH_SMACKDOWN = 8;
        public const int TRIG_KICK = 9;
        public const int TRIG_ZOMBIE_HIT = 10;

        public const int TRIG_BLOOD_SQUIRT_UP = 11;
        public const int TRIG_BLOOD_SQUIRT_UP_FORWARD = 12;
        public const int TRIG_BLOOD_SQUIRT_FORWARD = 13;
        public const int TRIG_BLOOD_SQUIRT_DOWN_FORNWARD = 14;
        public const int TRIG_BLOOD_SQUIRT_DOWN = 15;
        public const int TRIG_BLOOD_SQUIRT_DOWN_BACK = 16;
        public const int TRIG_BLOOD_SQUIRT_BACK = 17;
        public const int TRIG_BLOOD_SQUIRT_UP_BACK = 18;

        public const int TRIG_BLOOD_CLOUD = 19;
        public const int TRIG_BLOOD_SPLAT = 20;

        public const int TRIG_CHAINSAW_DOWN = 21;
        public const int TRIG_CHAINSAW_UPPER = 22;
        public const int TRIG_ROCKET = 23;
        public const int TRIG_FIRE_DIE = 24;

        int selScriptLine = 0;

        int editingText = EDITING_NONE;

        const bool screenShotMode = true;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            charDef = new CharDef();

#if SCREENSHOTMODE
                graphics.PreferredBackBufferWidth = 1280;
                graphics.PreferredBackBufferHeight = 720;
                graphics.ApplyChanges();
#endif

            base.Initialize();
        }

        private bool drawButton(int x, int y, int idx, bool mouseClick)
        {
            return drawButton(x, y, idx, mouseClick, 1.0f);
        }

        private bool drawButton(int x, int y, int idx, bool mouseClick, float scale)
        {
            bool r = false;

            Rectangle sRect = new Rectangle(32 * (idx % 8),
                32 * (idx / 8), 32, 32);
            Rectangle dRect = new Rectangle(x, y, 
                (int)(32f * scale), 
                (int)(32f * scale));

            if (dRect.Contains(mouseState.X, mouseState.Y))
            {
                dRect.X -= 1;
                dRect.Y -= 1;
                dRect.Width += 2;
                dRect.Height += 2;
                if (mouseClick)
                    r = true;
            }
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            spriteBatch.Draw(iconsTex, dRect, sRect, Color.White);
            spriteBatch.End();

            return r;
        }

        private void DrawCursor()
        {
            Rectangle rect = new Rectangle();
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            Color tColor = new Color();

            tColor = Color.White;
            //if (GetCanEdit()) tColor = Color.Yellow;

            rect.X = 0;
            rect.Y = 0;
            rect.Width = 32;
            rect.Height = 32;

            spriteBatch.Draw(iconsTex, new Vector2((float)mouseState.X, 
                (float)mouseState.Y), rect,
                tColor, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.0f);

            spriteBatch.End();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            arialText = Content.Load<Texture2D>(@"gfx/arialText");
            iconsTex = Content.Load<Texture2D>(@"gfx/icons");
            nullTex = Content.Load<Texture2D>(@"gfx/1x1");
            text = new Text(arialText, spriteBatch);

            legsTex = new Texture2D[3];
            torsoTex = new Texture2D[5];
            headTex = new Texture2D[5];
            weaponTex = new Texture2D[3];

            for (int i = 0; i < legsTex.Length; i++)
                legsTex[i] = Content.Load<Texture2D>(@"gfx/legs" + (i + 1).ToString());

            for (int i = 0; i < torsoTex.Length; i++)
                torsoTex[i] = Content.Load<Texture2D>(@"gfx/torso" + (i + 1).ToString());

            for (int i = 0; i < headTex.Length; i++)
                headTex[i] = Content.Load<Texture2D>(@"gfx/head" + (i + 1).ToString());

            for (int i = 0; i < weaponTex.Length; i++)
                weaponTex[i] = Content.Load<Texture2D>(@"gfx/weapon" + (i + 1).ToString());

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            UpdateKeys();

            Animation animation = charDef.GetAnimation(selAnim);
            KeyFrame keyframe = animation.GetKeyFrame(curKey);

            if (playing)
            {
                curFrame += (float)gameTime.ElapsedGameTime.TotalSeconds * 30.0f;

                if (curFrame > (float)keyframe.duration)
                {
                    curFrame -= (float)keyframe.duration;
                    curKey++;
                    keyframe = animation.GetKeyFrame(curKey);

                    if (curKey >=
                        animation.getKeyFrameArray().Length)
                        curKey = 0;
                }
            }
            else
                curKey = selKeyFrame;
            
            if (keyframe.frameRef < 0)
                curKey = 0;
            

            mouseState = Mouse.GetState();

            
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (preState.LeftButton == ButtonState.Released)
                {

                }
                else
                {
                    if (CanEdit())
                    {
                        int xM = mouseState.X - preState.X;
                        int yM = mouseState.Y - preState.Y;

                        charDef.GetFrame(selFrame).GetPart(selPart).location +=
                            new Vector2((float)xM / 2.0f, (float)yM / 2.0f);
                    }
                }
            }
            else
            {
                if (preState.LeftButton == ButtonState.Pressed)
                {
                    mouseClick = true;
                }
            }

            if (mouseState.RightButton == ButtonState.Pressed)
            {
                if (preState.RightButton == ButtonState.Pressed)
                {
                    if (CanEdit())
                    {
                        int yM = mouseState.Y - preState.Y;

                        charDef.GetFrame(selFrame).GetPart(selPart).rotation +=
                            (float)yM / 100.0f;
                    }
                }
            }

            if (mouseState.MiddleButton == ButtonState.Pressed)
            {
                if (preState.MiddleButton == ButtonState.Pressed)
                {
                    if (CanEdit())
                    {
                        int xM = mouseState.X - preState.X;
                        int yM = mouseState.Y - preState.Y;

                        charDef.GetFrame(selFrame).GetPart(selPart).scaling +=
                            new Vector2((float)xM * 0.01f, (float)yM * 0.01f);
                    }
                }
            }
            
            preState = mouseState;
            
            base.Update(gameTime);
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

        private void PressKey(Keys key)
        {
#if SCREENSHOTMODE
            if (key == Keys.End)
                return;
#endif
            String t = "";
            switch (editingText)
            {
                case EDITING_FRAME_NAME:
                    t = charDef.GetFrame(selFrame).name;
                    break;
                case EDITING_ANIMATION_NAME:
                    t = charDef.GetAnimation(selAnim).name;
                    break;
                case EDITING_PATH:
                    t = charDef.path;
                    break;
                case EDITING_SCRIPT:
                    t = charDef.GetAnimation(selAnim).GetKeyFrame(selKeyFrame).GetScript(selScriptLine);
                    break;
                default:
                    return;
            }

            if (key == Keys.Back)
            {
                if (t.Length > 0) t = t.Substring(0, t.Length - 1);
            }
            else if (key == Keys.Enter)
            {
                editingText = EDITING_NONE;
            }
            else
            {
                t = (t + (char)key).ToLower();
            }

            switch (editingText)
            {
                case EDITING_FRAME_NAME:
                    charDef.GetFrame(selFrame).name = t;
                    break;
                case EDITING_ANIMATION_NAME:
                    charDef.GetAnimation(selAnim).name = t;
                    break;
                case EDITING_PATH:
                    charDef.path = t;
                    break;
                case EDITING_SCRIPT:
                    charDef.GetAnimation(selAnim).GetKeyFrame(selKeyFrame).SetScript(selScriptLine, t);
                    break;
            }
        }

        private void DrawCharacter(Vector2 loc, float scale, int face, int frameIdx,
            bool preview, float alpha)
        {
            Rectangle sRect = new Rectangle();

            Frame frame = charDef.GetFrame(frameIdx);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            for (int i = 0; i < frame.GetPartArray().Length; i++)
            {
                Part part = frame.GetPart(i);
                if (part.idx > -1)
                {
                    
                    sRect.X = ((part.idx % 64) % 5) * 64;
                    sRect.Y = ((part.idx % 64) / 5) * 64;
                    sRect.Width = 64;
                    sRect.Height = 64;
                    if (part.idx >= 192)
                    {
                        sRect.X = ((part.idx % 64) % 4) * 80;
                        sRect.Y = ((part.idx % 64) / 4) * 64;
                        sRect.Width = 80;
                    }

                    float rotation = part.rotation;
                    

                    Vector2 location = part.location * scale + loc;
                    Vector2 scaling = part.scaling * scale;
                    if (part.idx >= 128) scaling *= 1.35f;

                    if (face == FACE_LEFT)
                    {
                        rotation = -rotation;
                        location.X -= part.location.X * scale * 2.0f;
                    }

                    if (part.idx >= 1000 && alpha >= 1f)
                    {
                        spriteBatch.End();
                        
                        text.Color = Color.Lime;
                        if (preview)
                        {
                            text.Size = 0.45f;
                            text.DrawText((int)location.X,
                                (int)location.Y,
                                "*");
                        }
                        else
                        {
                            text.Size = 1f;
                            text.DrawText((int)location.X,
                                (int)location.Y,
                                "*" + GetTrigName(part.idx - 1000));
                        }
                        spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                    }
                    else
                    {
                        Texture2D texture;

                        int t = part.idx / 64;
                        switch (t)
                        {
                            case 0:
                                texture = headTex[charDef.headIdx];
                                break;
                            case 1:
                                texture = torsoTex[charDef.torsoIdx];
                                break;
                            case 2:
                                texture = legsTex[charDef.legsIdx];
                                break;
                            case 3:
                                texture = weaponTex[charDef.weaponIdx];
                                break;
                            default:
                                texture = null;
                                break;
                        }
                        Color color = new Color(new
                            Vector4(1.0f, 1.0f, 1.0f, alpha));
                        if (!preview && selPart == i) color = new
                            Color(new Vector4(1.0f, 0.0f, 0.0f, alpha));

                        bool flip = false;

                        if ((face == FACE_RIGHT && part.flip == 0) ||
                            (face == FACE_LEFT && part.flip == 1)) flip = true;

                        if (texture != null)
                        {
                            spriteBatch.Draw(texture, location, sRect,
                                color, rotation, new Vector2(
                                (float)sRect.Width / 2f, 32.0f),
                                scaling, (flip ?
                                SpriteEffects.None : SpriteEffects.FlipHorizontally),
                                1.0f);
                        }
                    }
                }
            }

            spriteBatch.End();
        }

        private void SwapParts(int idx1, int idx2)
        {
            if (idx1 < 0 || idx2 < 0 ||
                idx1 >= charDef.GetFrame(selFrame).GetPartArray().Length ||
                idx2 >= charDef.GetFrame(selFrame).GetPartArray().Length)
                return;

            Part t = new Part();
            Part i = charDef.GetFrame(selFrame).GetPart(idx1);
            Part j = charDef.GetFrame(selFrame).GetPart(idx2);

            t.idx = i.idx;
            t.location = i.location;
            t.rotation = i.rotation;
            t.scaling = i.scaling;
            t.flip = i.flip;

            i.idx = j.idx;
            i.location = j.location;
            i.rotation = j.rotation;
            i.scaling = j.scaling;
            i.flip = j.flip;

            j.idx = t.idx;
            j.location = t.location;
            j.rotation = t.rotation;
            j.scaling = t.scaling;
            j.flip = t.flip;
        }

        private void CopyFrame(int src, int dest)
        {

            Frame keySrc = charDef.GetFrame(src);
            Frame keyDest = charDef.GetFrame(dest);

            keyDest.name = keySrc.name;
            for (int i = 0; i < keyDest.GetPartArray().Length; i++)
            {
                Part srcPart = keySrc.GetPart(i);
                Part destPart = keyDest.GetPart(i);

                destPart.idx = srcPart.idx;
                destPart.location = new Vector2(srcPart.location.X,
                    srcPart.location.Y);
                destPart.rotation = srcPart.rotation;
                destPart.scaling = new Vector2(srcPart.scaling.X,
                    srcPart.scaling.Y);
                destPart.flip = srcPart.flip;
                
            }
        }

        private bool CanEdit()
        {
            if (mouseState.X > 200 && mouseState.Y > 110 && mouseState.X <
                590 && mouseState.Y < 450)
                return true;
            else
                return false;
        }

        private string GetTrigName(int idx)
        {
            switch (idx)
            {
                case TRIG_PISTOL_ACROSS:
                    return "pistol across";
                case TRIG_PISTOL_DOWN:
                    return "pistol down";
                case TRIG_PISTOL_UP:
                    return "pistol up";
                case TRIG_WRENCH_DOWN:
                    return "wrench down";
                case TRIG_WRENCH_SMACKDOWN:
                    return "wrench smackdown";
                case TRIG_WRENCH_DIAG_UP:
                    return "wrench diag up";
                case TRIG_WRENCH_DIAG_DOWN:
                    return "wrench diag down";
                case TRIG_WRENCH_UP:
                    return "wrench up";
                case TRIG_WRENCH_UPPERCUT:
                    return "wrench uppercut";
                case TRIG_KICK:
                    return "kick";
                case TRIG_ZOMBIE_HIT:
                    return "zombie hit";
                    
                case TRIG_BLOOD_SQUIRT_UP:
                    return "squirt up";
                case TRIG_BLOOD_SQUIRT_UP_FORWARD:
                    return "squirt upforward";
                case TRIG_BLOOD_SQUIRT_FORWARD:
                    return "squirt forward";
                case TRIG_BLOOD_SQUIRT_DOWN_FORNWARD:
                    return "squirt downforward";
                case TRIG_BLOOD_SQUIRT_DOWN:
                    return "squirt down";
                case TRIG_BLOOD_SQUIRT_DOWN_BACK:
                    return "squirt downback";
                case TRIG_BLOOD_SQUIRT_BACK:
                    return "squirt back";
                case TRIG_BLOOD_SQUIRT_UP_BACK:
                    return "squirt upback";

                case TRIG_BLOOD_CLOUD:
                    return "blood cloud";
                case TRIG_BLOOD_SPLAT:
                    return "blood splat";

                case TRIG_CHAINSAW_DOWN:
                    return "chain down";
                case TRIG_CHAINSAW_UPPER:
                    return "chain up";
                case TRIG_ROCKET:
                    return "rocket";
                case TRIG_FIRE_DIE:
                    return "firedie";
            }
            return "";
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            if (mouseClick)
                editingText = EDITING_NONE;

            bool drawRow = false;
#if SCREENSHOTMODE
            Keys[] keys = Keyboard.GetState().GetPressedKeys();
            foreach (Keys key in keys)
            {
                if (key == Keys.End)
                    drawRow = true;
            }
#endif
            if (drawRow)
            {
                graphics.GraphicsDevice.Clear(Color.White);

                KeyFrame[] keyFrame = charDef.GetAnimation(selAnim).getKeyFrameArray();
                for (int i = 0; i < keyFrame.Length; i++)
                {
                    int fref = charDef.GetAnimation(selAnim).GetKeyFrame(i).frameRef;
                    if (fref >= 0)
                    {
                        DrawCharacter(new Vector2(
                            100f + (float)(i % 12) * 100f, 
                            130f + (float)(i / 12) * 130f
                            ), 
                            0.5f, 
                            FACE_LEFT,
                            fref,
                            true, 1.0f);
                    }
                }
                
            }
            else
            {
                graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

                #region Black BG
                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                spriteBatch.Draw(nullTex, new Rectangle(300, 450, 200, 5), new Color(new
                    Vector4(1.0f, 0.0f, 0.0f, 0.5f)));
                spriteBatch.Draw(nullTex, new Rectangle(0, 0, 200, 450), new Color(new
                    Vector4(0.0f, 0.0f, 0.0f, 0.5f)));
                spriteBatch.Draw(nullTex, new Rectangle(590, 0, 300, 600), new Color(new
                    Vector4(0.0f, 0.0f, 0.0f, 0.5f)));
                spriteBatch.Draw(nullTex, new Rectangle(200, 0, 150, 130), new Color(new
                    Vector4(0.0f, 0.0f, 0.0f, 0.5f)));
                spriteBatch.End();
                #endregion

                if (selFrame > 0)
                    DrawCharacter(new Vector2(400f, 450f), 2f, FACE_RIGHT,
                        selFrame - 1, false, 0.2f);
                if (selFrame < charDef.GetFrameArray().Length - 1)
                    DrawCharacter(new Vector2(400f, 450f), 2f, FACE_RIGHT,
                        selFrame + 1, false, 0.2f);

                DrawCharacter(new Vector2(400f, 450f), 2f, FACE_RIGHT, selFrame,
                    false, 1.0f);

                int fref = charDef.GetAnimation(selAnim).GetKeyFrame(curKey).frameRef;
                if (fref < 0)
                    fref = 0;

                DrawCharacter(new Vector2(500f, 100f), 0.5f, FACE_LEFT,
                    fref,
                    true, 1.0f);

                text.Size = 0.45f;

                #region Play Stop
                if (playing)
                {
                    if (text.DrawClickText(480, 100, "stop",
                        mouseState.X, mouseState.Y, mouseClick))
                        playing = false;
                }
                else
                {
                    if (text.DrawClickText(480, 100, "play",
                        mouseState.X, mouseState.Y, mouseClick))
                        playing = true;
                }
                #endregion

                #region Load/Save

                if (drawButton(200, 5, 3, mouseClick))
                    charDef.WriteBackup();
                if (drawButton(230, 5, 4, mouseClick))
                    charDef.Read();
                if (editingText == EDITING_PATH)
                {
                    text.Color = Color.Lime;
                    text.DrawText(270, 15, charDef.path + "*");
                }
                else
                {
                    if (text.DrawClickText(270, 15, charDef.path, mouseState.X,
                        mouseState.Y, mouseClick))
                    {
                        editingText = EDITING_PATH;
                    }
                }
                #endregion

                #region Texture Switching
                if (auxMode == AUX_TEXTURES)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (drawButton(210 + i * 21, 40, 1, mouseClick, 0.45f))
                        {
                            switch (i)
                            {
                                case 0:
                                    if (charDef.headIdx > 0) charDef.headIdx--;
                                    break;
                                case 1:
                                    if (charDef.torsoIdx > 0) charDef.torsoIdx--;
                                    break;
                                case 2:
                                    if (charDef.legsIdx > 0) charDef.legsIdx--;
                                    break;
                                case 3:
                                    if (charDef.weaponIdx > 0) charDef.weaponIdx--;
                                    break;
                            }
                        }
                        string t = charDef.headIdx.ToString();
                        switch (i)
                        {
                            case 1:
                                t = charDef.torsoIdx.ToString();
                                break;
                            case 2:
                                t = charDef.legsIdx.ToString();
                                break;
                            case 3:
                                t = charDef.weaponIdx.ToString();
                                break;
                        }
                        text.Color = (Color.White);
                        text.DrawText(212 + i * 21, 60, t);
                        if (drawButton(210 + i * 21, 85, 2, mouseClick, 0.45f))
                        {
                            switch (i)
                            {
                                case 0:
                                    if (charDef.headIdx < headTex.Length - 1)
                                        charDef.headIdx++;
                                    break;
                                case 1:
                                    if (charDef.torsoIdx < torsoTex.Length - 1)
                                        charDef.torsoIdx++;
                                    break;
                                case 2:
                                    if (charDef.legsIdx < legsTex.Length - 1)
                                        charDef.legsIdx++;
                                    break;
                                case 3:
                                    if (charDef.weaponIdx < weaponTex.Length - 1)
                                        charDef.weaponIdx++;
                                    break;
                            }
                        }
                    }
                }
                #endregion

                #region Script/Trigs Selector
                if (auxMode == AUX_SCRIPT)
                {
                    text.Color = Color.Lime;
                    text.DrawText(210, 110, "script");
                }
                else
                {
                    if (text.DrawClickText(210, 110, "script", mouseState.X,
                        mouseState.Y, mouseClick))
                        auxMode = AUX_SCRIPT;
                }
                if (auxMode == AUX_TRIGS)
                {
                    text.Color = Color.Lime;
                    text.DrawText(260, 110, "trigs");
                }
                else
                {
                    if (text.DrawClickText(260, 110, "trigs", mouseState.X,
                        mouseState.Y, mouseClick))
                        auxMode = AUX_TRIGS;
                }
                if (auxMode == AUX_TEXTURES)
                {
                    text.Color = Color.Lime;
                    text.DrawText(300, 110, "tex");
                }
                else
                {
                    if (text.DrawClickText(300, 110, "tex", mouseState.X,
                        mouseState.Y, mouseClick))
                        auxMode = AUX_TEXTURES;
                }
                #endregion

                #region Trigs
                if (auxMode == AUX_TRIGS)
                {
                    if (drawButton(330, 42, 1,
                        (mouseState.LeftButton == ButtonState.Pressed), 0.5f))
                    {
                        if (trigScroll > 0) trigScroll--;
                    }
                    if (drawButton(330, 92, 2,
                        (mouseState.LeftButton == ButtonState.Pressed), 0.5f))
                    {
                        if (trigScroll < 100) trigScroll++;
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        int t = i + trigScroll;
                        if (text.DrawClickText(210, 42 + i * 16,
                            GetTrigName(t), mouseState.X,
                            mouseState.Y, mouseClick))
                        {
                            charDef.GetFrame(selFrame).GetPart(selPart).idx
                                        = t + 1000;
                        }
                    }
                }
                #endregion

                #region Script
                if (auxMode == AUX_SCRIPT)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (editingText == EDITING_SCRIPT && selScriptLine == i)
                        {
                            text.Color = Color.Lime;
                            text.DrawText(210, 42 + i * 16, i.ToString() + ": " +
                                charDef.GetAnimation(selAnim).GetKeyFrame(selKeyFrame).GetScript(i)
                                + "*");
                        }
                        else
                        {
                            if (text.DrawClickText(210, 42 + i * 16, i.ToString() + ": " +
                                charDef.GetAnimation(selAnim).GetKeyFrame(selKeyFrame).GetScript(i),
                                mouseState.X, mouseState.Y, mouseClick))
                            {
                                selScriptLine = i;
                                editingText = EDITING_SCRIPT;
                            }
                        }


                    }
                }
                #endregion

                #region Parts List
                for (int i = 0; i < charDef.GetFrame(selFrame).GetPartArray().Length; i++)
                {

                    String line = "";
                    int idx = charDef.GetFrame(selFrame).GetPart(i).idx;
                    if (idx < 0)
                        line = "";
                    else if (idx < 64)
                        line = "head" + idx.ToString();
                    else if (idx < 74)
                        line = "torso" + idx.ToString();
                    else if (idx < 128)
                        line = "arms" + idx.ToString();
                    else if (idx < 192)
                        line = "legs" + idx.ToString();
                    else if (idx < 1000)
                        line = "weapon" + idx.ToString();
                    else
                        line = GetTrigName(idx - 1000);

                    if (selPart == i)
                    {
                        text.Color = Color.Lime;
                        text.DrawText(600, 5 + i * 15, i.ToString() + ": " +
                            line);

                        if (drawButton(700, 5 + i * 15, 1, mouseClick, 0.5f))
                        {
                            SwapParts(selPart, selPart - 1);
                            if (selPart > 0) selPart--;
                        }
                        if (drawButton(720, 5 + i * 15, 2, mouseClick, 0.5f))
                        {
                            SwapParts(selPart, selPart + 1);
                            if (selPart <
                                charDef.GetFrame(selFrame).GetPartArray().Length - 1)
                                selPart++;
                        }
                        Part part = charDef.GetFrame(selFrame).GetPart(selPart);
                        if (text.DrawClickText(740, 5 + i * 15,
                            (part.flip == 0 ? "(n)" : "(m)"),
                            mouseState.X, mouseState.Y, mouseClick))
                        {
                            part.flip = 1 - part.flip;
                        }
                        if (text.DrawClickText(762, 5 + i * 15, "(r)",
                            mouseState.X, mouseState.Y, mouseClick))
                        {
                            part.scaling =
                                new Vector2(1.0f, 1.0f);
                        }
                        if (text.DrawClickText(780, 5 + i * 15, "(x)",
                            mouseState.X, mouseState.Y, mouseClick))
                        {
                            part.idx = -1;
                        }
                    }
                    else
                    {
                        if (text.DrawClickText(600, 5 + i * 15, i.ToString() + ": " +
                            line, mouseState.X, mouseState.Y, mouseClick))
                        {
                            selPart = i;
                        }
                    }
                }
                #endregion

                #region Frame List
                for (int i = frameScroll; i < frameScroll + 20; i++)
                {
                    if (i < charDef.GetFrameArray().Length)
                    {
                        int y = (i - frameScroll) * 15 + 280;
                        if (i == selFrame)
                        {
                            text.Color = Color.Lime;
                            text.DrawText(600, y, i.ToString() + ": " +
                                charDef.GetFrame(i).name +
                                (editingText == EDITING_FRAME_NAME
                                ? "*" : ""));

                            if (text.DrawClickText(720, y, "(a)",
                                mouseState.X, mouseState.Y, mouseClick))
                            {
                                Animation animation = charDef.GetAnimation(selAnim);

                                for (int j = 0; j <
                                    animation.getKeyFrameArray().Length
                                    ; j++)
                                {

                                    KeyFrame keyframe = animation.GetKeyFrame(j);

                                    if (keyframe.frameRef == -1)
                                    {
                                        keyframe.frameRef = i;
                                        keyframe.duration = 1;

                                        break;
                                    }
                                }

                            }
                        }
                        else
                        {
                            if (text.DrawClickText(600, y, i.ToString() + ": " +
                                charDef.GetFrame(i).name, mouseState.X,
                                mouseState.Y, mouseClick))
                            {
                                if (selFrame != i)
                                {
                                    if (charDef.GetFrame(i).name == "")
                                        CopyFrame(selFrame, i);
                                    selFrame = i;
                                    editingText = EDITING_FRAME_NAME;
                                }
                            }
                        }
                    }

                }
                if (drawButton(770, 280, 1, (mouseState.LeftButton == ButtonState.Pressed)) && frameScroll > 0) frameScroll--;
                if (drawButton(770, 570, 2, (mouseState.LeftButton == ButtonState.Pressed)) && frameScroll <
                    charDef.GetFrameArray().Length - 20) frameScroll++;
                #endregion

                #region Animation List
                for (int i = animScroll; i < animScroll + 15; i++)
                {
                    if (i < charDef.GetAnimationArray().Length)
                    {
                        int y = (i - animScroll) * 15 + 5;
                        if (i == selAnim)
                        {
                            text.Color = Color.Lime;
                            text.DrawText(5, y, i.ToString() + ": " +
                                charDef.GetAnimation(i).name +
                                (editingText == EDITING_ANIMATION_NAME
                                ? "*" : ""));
                        }
                        else
                        {
                            if (text.DrawClickText(5, y, i.ToString() + ": " +
                                charDef.GetAnimation(i).name, mouseState.X,
                                mouseState.Y, mouseClick))
                            {
                                selAnim = i;
                                editingText = EDITING_ANIMATION_NAME;
                            }
                        }
                    }

                }
                if (drawButton(170, 5, 1, (mouseState.LeftButton == ButtonState.Pressed)) && animScroll > 0)
                    animScroll--;
                if (drawButton(170, 200, 2, (mouseState.LeftButton == ButtonState.Pressed)) && animScroll <
                    charDef.GetAnimationArray().Length - 15) animScroll++;
                #endregion

                #region Keyframe List
                for (int i = keyFrameScroll; i < keyFrameScroll + 13; i++)
                {

                    Animation animation = charDef.GetAnimation(selAnim);

                    if (i < animation.getKeyFrameArray().Length)
                    {
                        int y = (i - keyFrameScroll) * 15 + 250;
                        int frameRef = animation.GetKeyFrame(i).frameRef;
                        String name = "";
                        if (frameRef > -1)
                        {
                            name = charDef.GetFrame(frameRef).name;
                        }
                        if (i == selKeyFrame)
                        {
                            text.Color = Color.Lime;
                            text.DrawText(5, y, i.ToString() + ": " +
                                name);
                        }
                        else
                        {
                            if (text.DrawClickText(5, y, i.ToString() + ": " +
                                name, mouseState.X,
                                mouseState.Y, mouseClick))
                            {
                                selKeyFrame = i;
                            }
                        }
                        if (animation.GetKeyFrame(i).frameRef > -1)
                        {
                            if (text.DrawClickText(110, y, "-", mouseState.X,
                                mouseState.Y, mouseClick))
                            {
                                animation.GetKeyFrame(i).duration--;
                                if (animation.GetKeyFrame(i).duration <= 0)
                                {
                                    for (int j = i; j <
                                        animation.getKeyFrameArray().Length
                                        - 1; j++)
                                    {
                                        KeyFrame keyframe = animation.GetKeyFrame(j);
                                        keyframe.frameRef =
                                            animation.GetKeyFrame(j + 1).frameRef;
                                        keyframe.duration =
                                            animation.GetKeyFrame(j + 1).duration;
                                    }
                                    animation.GetKeyFrame(
                                        animation.getKeyFrameArray().Length - 1).frameRef
                                        = -1;
                                }

                            }
                            text.DrawText(125, y,
                                animation.GetKeyFrame(i).duration.ToString());

                            if (text.DrawClickText(140, y, "+", mouseState.X,
                                mouseState.Y, mouseClick))
                            {
                                animation.GetKeyFrame(i).duration++;
                            }
                        }
                    }

                }
                if (drawButton(170, 250, 1, (mouseState.LeftButton == ButtonState.Pressed)) && keyFrameScroll > 0)
                    keyFrameScroll--;
                if (drawButton(170, 410, 2, (mouseState.LeftButton == ButtonState.Pressed)) && keyFrameScroll <
                    charDef.GetAnimation(selAnim).getKeyFrameArray().Length - 13) keyFrameScroll++;
                #endregion

                #region Icon Palette
                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                for (int l = 0; l < 4; l++)
                {
                    Texture2D texture = null;
                    switch (l)
                    {
                        case 0:
                            texture = headTex[charDef.headIdx];
                            break;
                        case 1:
                            texture = torsoTex[charDef.torsoIdx];
                            break;
                        case 2:
                            texture = legsTex[charDef.legsIdx];
                            break;
                        case 3:
                            texture = weaponTex[charDef.weaponIdx];
                            break;
                    }
                    if (texture != null)
                    {
                        for (int i = 0; i < 25; i++)
                        {
                            Rectangle sRect = new Rectangle((i % 5) * 64,
                                (i / 5) * 64, 64, 64);
                            Rectangle dRect = new Rectangle(i * 23, 467
                                + l * 32, 23, 32);
                            spriteBatch.Draw(nullTex, dRect, new Color(new Vector4(0f, 0f, 0f, 0.1f)));
                            if (l == 3)
                            {
                                sRect.X = (i % 4) * 80;
                                sRect.Y = (i / 4) * 64;
                                sRect.Width = 80;

                                if (i < 15)
                                {
                                    dRect.X = i * 30;
                                    dRect.Width = 30;
                                }

                            }
                            spriteBatch.Draw(texture, dRect,
                                sRect, Color.White);

                            if (dRect.Contains(mouseState.X, mouseState.Y))
                            {
                                if (mouseClick)
                                {
                                    if (l < 3 || i < 15)
                                        charDef.GetFrame(selFrame).GetPart(selPart).idx
                                            = i + 64 * l;
                                }
                            }
                        }
                    }
                }
                spriteBatch.End();
                #endregion


                DrawCursor();
            }
            
            mouseClick = false;

            base.Draw(gameTime);
        }
    }
}
