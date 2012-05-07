using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ZombieSmashers.store;

namespace ZombieSmashers.menu
{
    public class Menu
    {
        public enum Trans : int 
        {
            Buttons = 0,
            All = 1
        };

        public enum Level : int
        {
            Main = 0,
            Options = 1,
            Quit = 2,
            NewGame = 3,
            ResumeGame = 4,
            EndGame = 5,
            Dead = 6,
            Multiplayer = 7,
            HostGame = 8,
            JoinGame = 9,
            NewArena = 10
        };

        public enum Option : int
        {
            None = -1,
            NewGame = 0,
            Continue = 1,
            Options = 2,
            Quit = 3,
            Back = 4,
            ResumeGame = 5,
            EndGame = 6,
            Multiplayer = 7,
            HostGame = 8,
            JoinGame = 9,
            RumbleOn = 10,
            RumbleOff = 11,
            Cancel = 12,
            AwaitingConnection = 13

        };

        public enum MenuMode : int
        {
            Main = 0,
            Pause = 1,
            Dead = 2
        }

        public float transFrame = 1f;
        public Trans transType = Trans.All;
        public Level transGoal;
        public Level level = Level.Main;
        public int selItem;

        Texture2D poseTex;
        Texture2D poseForeTex;
        Texture2D optionsTex;
        Texture2D backTex;
        Texture2D spritesTex;
        SpriteBatch sprite;

        int[] levelSel = new int[32];
        Option[] option = new Option[10];
        float[] optionFrame = new float[10];

        int totalOptions = 0;

        Vector2[] fog = new Vector2[128];

        float frame;

        GamePadState[] oldState = new GamePadState[4];

        public MenuMode menuMode = MenuMode.Main;
        
        public Menu(Texture2D _poseTex,
            Texture2D _poseForeTex,
            Texture2D _optionsTex,
            Texture2D _backTex,
            Texture2D _spritesTex,
            SpriteBatch _sprite)
        {
            poseTex = _poseTex;
            poseForeTex = _poseForeTex;
            optionsTex = _optionsTex;
            sprite = _sprite;
            backTex = _backTex;
            spritesTex = _spritesTex;

            for (int i = 0; i < fog.Length; i++)
            {
                fog[i] = Rand.GetRandomVector2(-50f,
                    Game1.ScreenSize.X + 50f,
                    Game1.ScreenSize.Y - 100f,
                    Game1.ScreenSize.Y);
            }
        }

        public void Pause()
        {
            
            menuMode = MenuMode.Pause;
            Game1.GameMode = Game1.GameModes.Menu;
            
            transFrame = 1f;
            level = Level.Main;
            transType = Trans.All;
        }

        public void Die()
        {

            menuMode = MenuMode.Dead;
            Game1.GameMode = Game1.GameModes.Menu;

            transFrame = 1f;
            level = Level.Dead;
            transType = Trans.All;
        }

        public void EndGame()
        {
            transFrame = 1f;
            transType = Trans.All;
            level = Level.Main;
            Game1.GameMode = Game1.GameModes.Menu;
        }

        public void Update(Game1 game)
        {
            frame += Game1.FrameTime / 2f;
            if (frame > 6.28f) frame -= 6.28f;

            if (transFrame < 2f)
            {
                float pFrame = transFrame;
                transFrame += Game1.FrameTime;
                if (transType == Trans.Buttons)
                    transFrame += Game1.FrameTime;
                if (pFrame < 1f && transFrame >= 1f)
                {
                    levelSel[(int)level] = selItem;
                    level = transGoal;
                    selItem = levelSel[(int)level];
                    switch (level)
                    {
                        case Level.NewGame:
                            game.NewGame();
                            break;
                        case Level.ResumeGame:
                            Game1.GameMode = Game1.GameModes.Playing;
                            break;
                        case Level.EndGame:
                            menuMode = MenuMode.Main;
                            level = Level.Main;
                            break;
                        case Level.Quit:
                            game.Quit();
                            break;
                        case Level.HostGame:
                            Game1.NetPlay.NetConnect.Host();
                            break;
                        case Level.JoinGame:
                            Game1.NetPlay.NetConnect.Find();
                            break;
                        case Level.NewArena:
                            game.NewGame(true);
                            Game1.NetPlay.NetConnect.NewGame();
                            break;
                    }
                }
            }

            for (int i = 0; i < fog.Length; i++)
            {
                fog[i].X -= Game1.FrameTime * (50f + (float)(i % 20 + 2));
                fog[i].Y += Game1.FrameTime * (float)(i % 14 + 5);
                if (fog[i].X < -150f)
                {
                    fog[i].X = Game1.ScreenSize.X +
                        Rand.GetRandomFloat(150f, 200f);
                    fog[i].Y = Game1.ScreenSize.Y -
                        Rand.GetRandomFloat(0f, 300f);
                }
            }

            for (int i = 0; i < optionFrame.Length; i++)
            {
                if (selItem == i)
                {
                    if (optionFrame[i] < 1f)
                    {
                        optionFrame[i] += Game1.FrameTime * 7f;
                        if (optionFrame[i] > 1f) optionFrame[i] = 1f;
                    }
                }
                else
                {
                    if (optionFrame[i] > 0f)
                    {
                        optionFrame[i] -= Game1.FrameTime * 4f;
                        if (optionFrame[i] < 0f) optionFrame[i] = 0f;
                    }
                }
            }

            PopulateOptions();

            for (int i = 0; i < 4; i++)
            {
                GamePadState gs = GamePad.GetState((PlayerIndex)i);

                if (totalOptions > 0)
                {
                    if ((gs.ThumbSticks.Left.Y > 0.3f &&
                        oldState[i].ThumbSticks.Left.Y <= 0.3f) ||
                        (gs.DPad.Up == ButtonState.Pressed &&
                        oldState[i].DPad.Up == ButtonState.Released))
                    {
                        selItem = (selItem + (totalOptions - 1)) % totalOptions;
                    }

                    if ((gs.ThumbSticks.Left.Y < -0.3f &&
                        oldState[i].ThumbSticks.Left.Y >= -0.3f) ||
                        (gs.DPad.Down == ButtonState.Pressed &&
                        oldState[i].DPad.Down == ButtonState.Released))
                    {
                        selItem = (selItem + 1) % totalOptions;
                    }
                }

                if (option[0] == Option.AwaitingConnection)
                    selItem = 1;

                bool ok = false;
                if (transFrame > 1.9f)
                {
                    if (gs.Buttons.A == ButtonState.Pressed &&
                        oldState[i].Buttons.A == ButtonState.Released)
                        ok = true;
                    if (gs.Buttons.Start == ButtonState.Pressed &&
                        oldState[i].Buttons.Start == ButtonState.Released)
                    {
                        if (menuMode == MenuMode.Main ||
                            menuMode == MenuMode.Dead)
                            ok = true;
                        else
                        {
                            Transition(Level.ResumeGame, true);
                        }
                    }

                    if (ok)
                    {
                        switch (level)
                        {
                            case Level.Main:
                                switch (option[selItem])
                                {
                                    case Option.NewGame:
                                        Transition(Level.NewGame, true);
                                        break;
                                    case Option.ResumeGame:
                                        Transition(Level.ResumeGame, true);
                                        break;
                                    case Option.EndGame:
                                        Transition(Level.EndGame, true);
                                        break;
                                    case Option.Continue:

                                        break;
                                    case Option.Multiplayer:
                                        Transition(Level.Multiplayer);
                                        break;
                                    case Option.Options:
                                        Transition(Level.Options);
                                        break;
                                    case Option.Quit:
                                        Transition(Level.Quit, true);
                                        break;
                                }
                                break;
                            case Level.Dead:
                                switch (option[selItem])
                                {
                                    case Option.EndGame:
                                        Transition(Level.EndGame, true);
                                        break;
                                    case Option.Quit:
                                        Transition(Level.Quit, true);
                                        break;
                                }
                                break;
                            case Level.Options:
                                switch (option[selItem])
                                {
                                    case Option.Back:
                                        Transition(Level.Main);
                                        Game1.store.Write(Store.STORE_SETTINGS);
                                        break;
                                    case Option.RumbleOn:
                                        Game1.settings.Rumble = false;
                                        break;
                                    case Option.RumbleOff:
                                        Game1.settings.Rumble = true;
                                        break;
                                }
                                break;
                            case Level.Multiplayer:
                                switch (option[selItem])
                                {
                                    case Option.Back:
                                        Transition(Level.Main);
                                        break;
                                    case Option.HostGame:
                                        Transition(Level.HostGame);
                                        break;
                                    case Option.JoinGame:
                                        Transition(Level.JoinGame);
                                        break;
                                }
                                break;
                            case Level.HostGame:
                                switch (option[selItem])
                                {
                                    case Option.Cancel:
                                        Transition(Level.Main);
                                        Game1.NetPlay.NetConnect.Disconnect();
                                        break;
                                }
                                break;
                            case Level.JoinGame:
                                switch (option[selItem])
                                {
                                    case Option.Cancel:
                                        Transition(Level.Main);
                                        Game1.NetPlay.NetConnect.Disconnect();
                                        break;
                                }
                                break;
                        }
                    }
                    else
                    {
                        switch (level)
                        {
                            case Level.JoinGame:
                                if (Game1.NetPlay.Joined)
                                    Transition(Level.NewArena);
                                break;
                            case Level.HostGame:
                                if (Game1.NetPlay.NetSession != null)
                                {
                                    if (Game1.NetPlay.NetSession.AllGamers.Count == 2)
                                        Transition(Level.NewArena);
                                }
                                break;
                        }
                    }
                }
                oldState[i] = gs;
            }
        }

        private void Transition(Level goal)
        {
            Transition(goal, false);
        }

        private void Transition(Level goal, bool all)
        {
            transGoal = goal;
            transFrame = 0f;
            if (all)
                transType = Trans.All;
            else
                transType = Trans.Buttons;
        }

        private float GetAlpha(bool buttons)
        {
            if (!buttons && transType == Trans.Buttons)
                return 1f;
            if (transFrame < 2f)
            {
                if (transFrame < 1f)
                    return 1f - transFrame;
                else
                    return transFrame - 1f;
            }
            return 1f;
        }

        public void Draw()
        {
            sprite.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            if (menuMode == MenuMode.Main)
            {
                sprite.Draw(backTex, new Rectangle(0, 0,
                    1280, 720), new Color(new Vector4(GetAlpha(false),
                    GetAlpha(false), GetAlpha(false), 1f)));
            }
            else if (menuMode == MenuMode.Pause)
            {
                sprite.Draw(backTex, new Rectangle(0, 0,
                    1280, 720), 
                    new Rectangle(600, 400, 200, 200),
                    new Color(new Vector4(1f, 1f, 1f, .5f)));
            }

            sprite.End();
            sprite.Begin(SpriteSortMode.BackToFront, BlendState.Additive);

            float pan = (float)Math.Cos((double)frame) * 10f + 10f;
            for (int i = fog.Length / 2; i < fog.Length; i++)
            {
                sprite.Draw(spritesTex, fog[i] + new Vector2(pan, 0f),
                    new Rectangle((i % 4) * 64, 0, 64, 64),
                    new Color(new Vector4(1f, 1f, 1f, .1f * GetAlpha(false))),
                    (fog[i].X + fog[i].Y) / 100f,
                    new Vector2(32f, 32f),
                    (float)(i % 10) * .5f + 2f, SpriteEffects.None, 1f);
            }

            sprite.End();
            sprite.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);


            float poseA = GetAlpha(false);
            if (menuMode != MenuMode.Dead)
            {
                if (menuMode != MenuMode.Main)
                    poseA = 0f;

                sprite.Draw(poseTex,
                    new Vector2(Game1.ScreenSize.X -
                    (Game1.ScreenSize.Y / 480f) * 380f * (.5f * GetAlpha(false) + .5f)
                    + (float)Math.Cos((double)frame) * 10f + 10f,
                    0f),
                    new Rectangle(0, 0, 421, 480),
                    new Color(new Vector4(poseA, poseA, poseA, 1f)), 0f,
                    new Vector2(), (Game1.ScreenSize.Y / 480f), SpriteEffects.None, 1f);
            }
            PopulateOptions();

            for (int i = 0; i < totalOptions; i++)
            {
                sprite.Draw(optionsTex,
                    new Vector2(190f + (float)i * 5f + pan + optionFrame[i] * 10f
                    + GetAlpha(true) * 50f,
                    300f + (float)i * 64f - (float)totalOptions * 32f),
                    new Rectangle(0, (int)option[i] * 64, 320, 64), new Color(
                    new Vector4(1f, 1f - optionFrame[i], 1f - optionFrame[i], GetAlpha(true))),
                    (1f - optionFrame[i]) * -.1f, new Vector2(160f, 32f), 1f, SpriteEffects.None, 1f);
            }


            sprite.End();

            if (menuMode != MenuMode.Dead)
            {
                sprite.Begin(SpriteSortMode.BackToFront, BlendState.Additive);


                pan *= 2f;
                for (int i = 0; i < fog.Length / 2; i++)
                {
                    sprite.Draw(spritesTex, fog[i] + new Vector2(pan, 0f),
                        new Rectangle((i % 4) * 64, 0, 64, 64),
                        new Color(new Vector4(1f, 1f, 1f, .1f * GetAlpha(false))),
                        (fog[i].X + fog[i].Y) / 100f,
                        new Vector2(32f, 32f),
                        (float)(i % 10) * .5f + 2f, SpriteEffects.None, 1f);
                }

                sprite.End();
                sprite.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

                sprite.Draw(poseForeTex,
                    new Vector2(Game1.ScreenSize.X - (Game1.ScreenSize.Y / 480f) * 616f * GetAlpha(false)
                    + (float)Math.Cos((double)frame) * 20f + 20f,
                    Game1.ScreenSize.Y - (Game1.ScreenSize.Y / 480f) * 286f),
                    new Rectangle(0, 0, 616, 286),
                    new Color(new Vector4(GetAlpha(false),
                    GetAlpha(false), GetAlpha(false), 1f)), 0f,
                    new Vector2(), (Game1.ScreenSize.Y / 480f), SpriteEffects.None, 1f);









                sprite.End();
            }
        }

        private void PopulateOptions()
        {
            for (int i = 0; i < option.Length; i++)
                option[i] = Option.None;

            switch (level)
            {
                case Level.Main:
                    if (menuMode == MenuMode.Pause)
                    {
                        option[0] = Option.ResumeGame;
                        option[1] = Option.EndGame;
                        option[2] = Option.Options;
                        option[3] = Option.Quit;
                        totalOptions = 4;
                    }
                    else
                    {
                        option[0] = Option.NewGame;
                        option[1] = Option.Continue;
                        option[2] = Option.Multiplayer;
                        option[3] = Option.Options;
                        option[4] = Option.Quit;
                        totalOptions = 5;
                    }
                    break;
                case Level.Options:
                    if (Game1.settings.Rumble)
                        option[0] = Option.RumbleOn;
                    else
                        option[0] = Option.RumbleOff;
                    option[1] = Option.Back;
                    totalOptions = 2;
                    break;
                case Level.Dead:
                    option[0] = Option.EndGame;
                    option[1] = Option.Quit;
                    totalOptions = 2;
                    break;
                case Level.Multiplayer:
                    option[0] = Option.HostGame;
                    option[1] = Option.JoinGame;
                    option[2] = Option.Back;
                    totalOptions = 3;
                    break;
                case Level.HostGame:
                    option[0] = Option.AwaitingConnection;
                    option[1] = Option.Cancel;
                    totalOptions = 2;
                    break;
                case Level.JoinGame:
                    option[0] = Option.AwaitingConnection;
                    option[1] = Option.Cancel;
                    totalOptions = 2;
                    break;
                default:
                    totalOptions = 0;
                    break;
            }

        }
    }

   
}
