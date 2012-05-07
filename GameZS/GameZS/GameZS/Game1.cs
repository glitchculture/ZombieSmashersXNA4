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
using ZombieSmashers.map;
using ZombieSmashers.Particles;
using ZombieSmashers.audio;
using ZombieSmashers.quake;
using ZombieSmashers.ai;
using ZombieSmashers.menu;
using ZombieSmashers.hud;
using ZombieSmashers.net;
using ZombieSmashers.store;

namespace ZombieSmashers
{

    

    /// <summary>
    /// This is the main type for Zombie Smashers.
    /// 
    /// We'll be doing all of our game updating in Update() and our drawing in Draw().
    /// 
    /// 
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public enum GameModes : int
        {
            Menu = 0,
            Playing = 1
        };

        public enum GameType : int
        {
            Solo = 0,
            Arena = 1
        };


        Map map;



        Texture2D[] mapsTex = new Texture2D[1];
        Character[] character = new Character[16];
        Texture2D[] mapBackTex = new Texture2D[1];
        public static CharDef[] charDef = new CharDef[16];

        Texture2D nullTex;
        Texture2D spritesTex;

        ParticleManager pManager;

        #region Statics
        private static float frameTime = 0f;
        private static Vector2 scroll = new Vector2();
        private static Vector2 screenSize = new Vector2();
        private static float gravity = 1400f;
        private static float friction = 1000f;
        private static float slowTime = 0f;
        private static long score = 0;
        private static GameModes gameMode;
        private static GameType gameType;
        private static int players;
        private static Menu menu;
        private static NetPlay netPlay;

        public static GameModes GameMode
        {
            get { return gameMode; }
            set { gameMode = value; }
        }

        public static Menu Menu
        {
            get { return menu; }
            set { menu = value; }
        }

        public static long Score
        {
            get { return score; }
            set { score = value; }
        }

        public static CharDef[] CharDefs
        {
            get { return charDef; }
        }

        public static float SlowTime
        {
            get { return slowTime; }
            set { slowTime = value; }
        }

        public static Vector2 ScreenSize
        {
            get { return screenSize; }
            set { screenSize = value; }
        }

        public static Vector2 Scroll
        {
            get { return scroll; }
            set { scroll = value; }
        }

        public static float Friction
        {
            get { return friction; }
        }

        public static float Gravity
        {
            get { return gravity; }
        }

        public static int Players
        {
            get { return players; }
        }

        public static float FrameTime
        {
            get { return frameTime; }
        }

        public static NetPlay NetPlay
        {
            get { return netPlay; }
        }
        #endregion


        

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        RenderTarget2D mainTarget;
        RenderTarget2D gameTarget;
        RenderTarget2D[] bloomTarget;
        RenderTarget2D waterTarget;
        RenderTarget2D refractTarget;

        float[] bloomPulse = { 0f, 0f };

        Effect filterEffect;
        Effect pauseEffect;
        Effect negEffect;
        Effect bloomEffect;
        Effect waterEffect;

        HUD hud;

        float waterDelta = 0f;
        float waterTheta = 0f;

        public static Store store;
        public static Settings settings;

        bool hasBloom = false;

        

        

        
        

        public void NewGame()
        {
            NewGame(false);
        }

        /// <summary>
        /// Starts a new game.
        /// </summary>
        /// <param name="arena">Specifies whether this is an arena game.  We'll use an arena game for multiplayer games, covered in Chapter 12.</param>
        public void NewGame(bool arena)
        {
            gameMode = GameModes.Playing;

            
            pManager.Reset();

            if (arena)
            {
                map.path = "arena";
                gameType = GameType.Arena;
                players = 2;
            }
            else
            {
                map.path = "start";
                gameType = GameType.Solo;
                players = 1;
            }

            for (int i = 0; i < players; i++)
            {
                character[i]
                    = new Character(new Vector2(300f
                    + (float)i * 200f, 100f),
                    charDef[(int)CharacterDefinitions.Guy],
                    i,
                    Character.TEAM_GOOD_GUYS);
                character[i].HP = character[i].MHP = 100;
            }
            for (int i = players; i < character.Length; i++)
                character[i] = null;

            map.GlobalFlags = new MapFlags(64);
            map.Read();
            map.TransDir = TransitionDirection.Intro;
            map.transInFrame = 1f;
        }

        public void Quit()
        {
            this.Exit();
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Components.Add(new GamerServicesComponent(this));
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// 
        /// We'll instantiate our stuff, load character definitions.
        /// 
        /// 
        /// </summary>
        protected override void Initialize()
        {
            Rand.random = new Random();

            map = new Map();


            netPlay = new NetPlay();

            screenSize.X = 800f;
            screenSize.Y = 600f;

            charDef[(int)CharacterDefinitions.Guy] =
                new CharDef("chars/guy", CharacterDefinitions.Guy);
            charDef[(int)CharacterDefinitions.Zombie] =
                new CharDef("chars/zombie", CharacterDefinitions.Zombie);
            charDef[(int)CharacterDefinitions.Wraith] =
                new CharDef("chars/wraith", CharacterDefinitions.Wraith);
            charDef[(int)CharacterDefinitions.Carlos] =
                new CharDef("chars/carlos", CharacterDefinitions.Carlos);
            

            
            Sound.Initialize();
            Music.Initialize();

            QuakeManager.Init();

            base.Initialize();

            store = new Store();
            settings = new Settings();
            store.GetDevice();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// 
        /// 
        /// </summary>
        protected override void LoadContent()
        {   
            spriteBatch = new SpriteBatch(GraphicsDevice);
            pManager = new ParticleManager(spriteBatch);
            spritesTex = Content.Load<Texture2D>
                (@"gfx/spritesTex");

            for (int i = 0; i < mapsTex.Length; i++)
                mapsTex[i] = Content.Load<Texture2D>(@"gfx/maps" + 
                    (i + 1).ToString());

            for (int i = 0; i < mapBackTex.Length; i++)
                mapBackTex[i] = Content.Load<Texture2D>(@"gfx/back" +
                    (i + 1).ToString());

            /*
             * Create our render targets--we'll start using these in chapter 8 when we
             * switch the render loop around. 
             */
            mainTarget = new RenderTarget2D(GraphicsDevice,
                graphics.PreferredBackBufferWidth,
                graphics.PreferredBackBufferHeight,
                true, SurfaceFormat.Color, DepthFormat.Depth24);
                //1, SurfaceFormat.Color);

            gameTarget = new RenderTarget2D(GraphicsDevice,
                graphics.PreferredBackBufferWidth,
                graphics.PreferredBackBufferHeight,
                true, SurfaceFormat.Color, DepthFormat.Depth24);
                //1, SurfaceFormat.Color);
            /*
             * We'll use refractTarget for our post process refraction effect in 
             * chapter 11.
             */
            refractTarget = new RenderTarget2D(GraphicsDevice,
                graphics.PreferredBackBufferWidth,
                graphics.PreferredBackBufferHeight,
                true, SurfaceFormat.Color, DepthFormat.Depth24);
                //1, SurfaceFormat.Color);

            /*
             * The next RenderTargets and Effects are used for post process effects
             * in chapter 11.
             */
            bloomTarget = new RenderTarget2D[2];
            bloomTarget[0] =
                new RenderTarget2D(GraphicsDevice,
                128, 128,
                true, SurfaceFormat.Color, DepthFormat.Depth24);
                //1, SurfaceFormat.Color);
            bloomTarget[1] =
                new RenderTarget2D(GraphicsDevice,
                256, 256,
                true, SurfaceFormat.Color, DepthFormat.Depth24);
                //1, SurfaceFormat.Color);
            waterTarget =
                new RenderTarget2D(GraphicsDevice,
                256, 256,
                true, SurfaceFormat.Color, DepthFormat.Depth24);
                //1, SurfaceFormat.Color);

            filterEffect = Content.Load<Effect>(@"fx/filter");
            pauseEffect = Content.Load<Effect>(@"fx/pause");
            negEffect = Content.Load<Effect>(@"fx/negative");
            bloomEffect = Content.Load<Effect>(@"fx/bloom");
            waterEffect = Content.Load<Effect>(@"fx/water");

            Character.LoadTextures(Content);

            nullTex = Content.Load<Texture2D>(@"gfx/1x1");

            /*
             * Create our menu and HUD objects, which we'll use in Chapter 9.
             */ 
            menu = new Menu(
                Content.Load<Texture2D>(@"gfx/pose"),
                Content.Load<Texture2D>(@"gfx/posefore"),
                Content.Load<Texture2D>(@"gfx/options"),
                mapBackTex[0],
                spritesTex,
                spriteBatch);

            hud = new HUD(spriteBatch, spritesTex, nullTex, character, map);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        private void UpdateGame()
        {
            if (GamePad.GetState(PlayerIndex.One).Triggers.Left > .2f)
                frameTime /= 20f;

            int idx = 0;
            if (NetPlay.Joined)
                idx = 1;

            if (character[idx] != null)
            {
                Scroll += ((character[idx].Loc -
                                    new Vector2(400f, 400f)) - Scroll) * FrameTime * 20f;
            }

            Scroll += QuakeManager.Quake.Vector;

            bloomPulse[0] += FrameTime * .5f;
            bloomPulse[1] += FrameTime * .9f;
            for (int i = 0; i < bloomPulse.Length; i++)
                if (bloomPulse[i] > 6.28f) bloomPulse[i] -= 6.28f;

            float xLim = map.GetXLim();
            float yLim = map.GetYLim();

            
            waterDelta += FrameTime * 8f;
            waterTheta += FrameTime * 10f;
            if (waterDelta > 6.28f)
                waterDelta -= 6.28f;
            if (waterTheta > 6.28f)
                waterTheta -= 6.28f;

            if (Scroll.X < 0f) scroll.X = 0f;
            if (Scroll.X > xLim) scroll.X = xLim;
            if (Scroll.Y < 0f) scroll.Y = 0f;
            if (Scroll.Y > yLim) scroll.Y = yLim;

            if (map.transOutFrame <= 0f)
            {
                pManager.UpdateParticles(FrameTime, map, character);


                if (gameType == GameType.Solo)
                {
                    if (character[0] != null)
                        character[0].DoInput(0);
                }
                else if (gameType == GameType.Arena)
                {
                    if (NetPlay.Hosting)
                        if (character[0] != null)
                            character[0].DoInput(0);
                    if (NetPlay.Joined)
                        if (character[1] != null)
                            character[1].DoInput(0);
                }

                for (int i = 0; i < character.Length; i++)
                {
                    if (character[i] != null)
                    {
                        character[i].Update(map, pManager, character);
                        if (character[i].DyingFrame > 1f)
                        {
                            if (character[i].Team == Character.TEAM_GOOD_GUYS)
                            {
                                character[i].DyingFrame = 1f;
                            }
                            else
                            {
                                if (character[i].Name != "")
                                    map.mapScript.Flags.SetFlag(character[i].Name);
                                character[i] = null;
                            }
                        }
                    }
                }
            }
            if (GamePad.GetState(PlayerIndex.One).Triggers.Left < .2f)
            map.Update(pManager, character);
            hud.Update();
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

            Sound.Update();
            Music.Play("music1");
            QuakeManager.Update();

            frameTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            NetPlay.NetGame.FrameTime = FrameTime;

            frameTime *= 1.5f;
            
            if (SlowTime > 0f)
            {
                SlowTime -= FrameTime;
                frameTime /= 10f;
            }

            NetPlay.Update(character, pManager);
            store.Update();

            switch (gameMode)
            {
                case GameModes.Playing:
                    UpdateGame();

                    break;
                case GameModes.Menu:
                    if (menu.menuMode == Menu.MenuMode.Dead)
                    {
                        float pTime = FrameTime;
                        frameTime /= 3f;
                        UpdateGame();

                        frameTime = pTime;
                    }
                    menu.Update(this);
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// We'll draw our standard stuff--map, particles, characters, 
        /// and more particles, then do a bunch of post effects.
        /// 
        /// The post effects are covered in chapter 11.
        /// </summary>
        private void DrawGame()
        {
            //graphics.GraphicsDevice.SetRenderTarget(0, mainTarget);
            graphics.GraphicsDevice.SetRenderTarget(mainTarget);
            graphics.GraphicsDevice.Clear(Color.Black);

            map.Draw(spriteBatch, mapsTex, mapBackTex, 0, 2);

            pManager.DrawParticles(spritesTex, true);

            for (int i = 0; i < character.Length; i++)
                if (character[i] != null)
                    character[i].Draw(spriteBatch);


            pManager.DrawParticles(spritesTex, false);

            /*
             * Draw our refract particles to the refractTarget.
             */
            //graphics.GraphicsDevice.SetRenderTarget(0, refractTarget);
            graphics.GraphicsDevice.SetRenderTarget(refractTarget);
            graphics.GraphicsDevice.Clear(Color.Black);
            pManager.DrawRefractParticles(spritesTex);


            /*
             * Set up our water texture. 
             */
            EffectPass pass;

            float waterLevel = map.water - (.2f * ScreenSize.Y);
            if (map.water > 0f)
            {
                //graphics.GraphicsDevice.SetRenderTarget(0, waterTarget);
                graphics.GraphicsDevice.SetRenderTarget(waterTarget);

                float wLev = (ScreenSize.Y / 2f + waterLevel - Scroll.Y) / ScreenSize.Y;

                waterEffect.Parameters["delta"].SetValue(waterDelta);
                waterEffect.Parameters["theta"].SetValue(waterTheta);
                waterEffect.Parameters["horizon"].SetValue(wLev);
                //waterEffect.Begin();
                waterEffect.CurrentTechnique.Passes[0].Apply();
                //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.None,
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);//,
                        //SaveStateMode.SaveState);
                pass = waterEffect.CurrentTechnique.Passes[0];
                
                //pass.Begin();

                //spriteBatch.Draw(mainTarget.GetTexture(), new Rectangle(0, 0, 256, 256), Color.White);
                spriteBatch.Draw(mainTarget, new Rectangle(0, 0, 256, 256), Color.White);

                //pass.End();
                spriteBatch.End();
                //waterEffect.End();
            }

            /*
             * Switch to gameTarget and draw our mainTarget with refraction and
             * color filter effects as well as water and bloom overlay.
             */

            //graphics.GraphicsDevice.SetRenderTarget(0, gameTarget);
            graphics.GraphicsDevice.SetRenderTarget(gameTarget);

            if (gameMode == GameModes.Menu)
            {
                //pauseEffect.Begin();
                pauseEffect.CurrentTechnique.Passes[0].Apply();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque); //.None,
                    //SaveStateMode.SaveState);
                pass = pauseEffect.CurrentTechnique.Passes[0];
                //pass.Begin();
                //spriteBatch.Draw(mainTarget.GetTexture(), new Vector2(), Color.White);
                spriteBatch.Draw(mainTarget, new Vector2(), Color.White);
                //pass.End();
                spriteBatch.End();
                //pauseEffect.End();
            }
            else
            {
                //graphics.GraphicsDevice.Textures[1] = refractTarget.GetTexture();
                graphics.GraphicsDevice.Textures[1] = refractTarget;
                filterEffect.Parameters["burn"].SetValue(.15f);

                filterEffect.Parameters["saturation"].SetValue(0.5f);
                filterEffect.Parameters["r"].SetValue(1.0f);
                filterEffect.Parameters["g"].SetValue(0.98f);
                filterEffect.Parameters["b"].SetValue(0.85f);
                filterEffect.Parameters["brite"].SetValue(0.05f);

                //filterEffect.Begin();
                filterEffect.CurrentTechnique.Passes[0].Apply();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque); //.None,
                    //SaveStateMode.SaveState);
                pass = filterEffect.CurrentTechnique.Passes[0];
                //pass.Begin();

                //spriteBatch.Draw(mainTarget.GetTexture(), new Vector2(), Color.White);
                spriteBatch.Draw(mainTarget, new Vector2(), Color.White);

                //pass.End();
                spriteBatch.End();
                //filterEffect.End();

                graphics.GraphicsDevice.Textures[1] = null;

                //if (map.water > 0f)
                if (false)
                {
                    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

                    //spriteBatch.Draw(waterTarget.GetTexture(), new Rectangle(0,
                    spriteBatch.Draw(waterTarget, new Rectangle(0,
                        (int)(waterLevel - Scroll.Y),
                        (int)ScreenSize.X, (int)ScreenSize.Y), Color.White);

                    spriteBatch.End();

                }

                map.Draw(spriteBatch, mapsTex, mapBackTex, 2, 3);

                //if (hasBloom)
                if (false)
                {
                    //spriteBatch.Begin(SpriteBlendMode.Additive);
                    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Additive);
                    
                    for (int i = 0; i < 2; i++)
                        //spriteBatch.Draw(bloomTarget[i].GetTexture(),
                        spriteBatch.Draw(bloomTarget[i],
                        new Rectangle(0, 0, (int)ScreenSize.X,
                        (int)ScreenSize.Y), Color.White);
                    
                    spriteBatch.End();

                }
            }

            /*
             * Set up the bloom surfaces with our current scene.  Since we're
             * using an already-bloomed scene to create bloom, we end up with a
             * sort of feedback that gives us a nice haze effect.
             */
            for (int i = 0; i < 2; i++)
            {
                hasBloom = true;
                //graphics.GraphicsDevice.SetRenderTarget(0, bloomTarget[i]);
                graphics.GraphicsDevice.SetRenderTarget(bloomTarget[i]);

                bloomEffect.Parameters["a"].SetValue(.25f);
                bloomEffect.Parameters["v"].SetValue((float)(i + 1) * 0.01f *
                    ((float)Math.Cos((double)bloomPulse[i]) * .25f + 0.7f));
                //bloomEffect.Begin();
                bloomEffect.CurrentTechnique.Passes[0].Apply();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque); //.None,
                    //SaveStateMode.SaveState);
                pass = bloomEffect.CurrentTechnique.Passes[0];
                //pass.Begin();

                //spriteBatch.Draw(gameTarget.GetTexture(),
                spriteBatch.Draw(gameTarget,
                    new Rectangle(0, 0, 128 * (i + 1), 128 * (i + 1)),
                    Color.White);

                //pass.End();
                spriteBatch.End();
                //bloomEffect.End();
            }

            /*
             * Render back the gameTarget to backbuffer.
             */

            //graphics.GraphicsDevice.SetRenderTarget(0, null);
            graphics.GraphicsDevice.SetRenderTarget(null);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Opaque); //.None);

            //spriteBatch.Draw(gameTarget.GetTexture(), new Vector2(), Color.White);
            spriteBatch.Draw(gameTarget, new Vector2(), Color.White);

            spriteBatch.End();

            /*
             * Draw our blast effect, which we set up in chapter 8.
             */

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            if (QuakeManager.Blast.Value > 0f)
            {
                for (int i = 7; i >= 0; i--)
                {
                    //spriteBatch.Draw(gameTarget.GetTexture(),
                    spriteBatch.Draw(gameTarget,
                        QuakeManager.Blast.center - Scroll,
                        new Rectangle(0, 0, (int)ScreenSize.X, (int)ScreenSize.Y),
                        new Color(new Vector4(1f, 1f, 1f,
                        .25f * (QuakeManager.Blast.Value / QuakeManager.Blast.Magnitude))),
                        0f, QuakeManager.Blast.center - Scroll,
                        (1.0f + (QuakeManager.Blast.Magnitude - QuakeManager.Blast.Value)
                        * .1f
                        + ((float)(i) / 50f)),
                        SpriteEffects.None, 1f);

                }
            }

            spriteBatch.End();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            switch (gameMode)
            {
                case GameModes.Playing:
                    DrawGame();
                    hud.Draw();
                    break;
                case GameModes.Menu:
                    if (menu.menuMode == Menu.MenuMode.Pause ||
                        menu.menuMode == Menu.MenuMode.Dead)
                        DrawGame();
                    menu.Draw();
                    break;
            }
            

            base.Draw(gameTime);
        }
    }
}
