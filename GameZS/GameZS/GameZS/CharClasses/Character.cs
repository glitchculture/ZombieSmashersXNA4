using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//using ZombieSmashers.map;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using ZombieSmashers.Particles;
using ZombieSmashers.audio;
using ZombieSmashers.quake;
using ZombieSmashers.ai;
using ZombieSmashers.menu;
using Microsoft.Xna.Framework.Net;
using ZombieSmashers.net;
using ZombieSmashers.map;

namespace ZombieSmashers
{
    public enum CharState
    {
        Grounded = 0,
        Air = 1
    }

    public enum CharDir
    {
        Left = 0,
        Right = 1
    }

    public class Character
    {
        Script script;

        public AI Ai;

        public static Texture2D[] headTex = new Texture2D[5];
        public static Texture2D[] torsoTex = new Texture2D[5];
        public static Texture2D[] legsTex = new Texture2D[3];
        public static Texture2D[] weaponTex = new Texture2D[3];

        public float Speed = 200f;

        public float ColMove = 0f;

        public Vector2 Loc;
        public Vector2 Trajectory;
        public CharDir Face;
        public float Scale;
        public int AnimFrame;
        public CharState State;
        public int Anim;
        public String AnimName;

        public bool Floating;
        public bool ethereal;

        public bool KeyLeft;
        public bool KeyRight;
        public bool KeyUp;
        public bool KeyDown;

        public bool KeyJump;
        public bool KeyAttack;
        public bool KeySecondary;

        public Vector2 RightAnalog;

        public int HP;
        public int MHP;

        public int LastHitBy;
        public bool NoLifty;
        public float StunFrame = 0f;

        #region Constants
         
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
        
        #endregion
        public const int TEAM_GOOD_GUYS = 0;
        public const int TEAM_BAD_GUYS = 1;

        public int Team;
        public int ID;

        public bool CanCancel;

        public PressedKeys PressedKey;
        
        public int[] GotoGoal = { -1, -1, -1, -1, -1, -1, -1, -1 };

        CharDef charDef;

        float frame = 0f;

        int ledgeAttach = -1;

        GamePadState curState = new GamePadState();
        GamePadState prevState = new GamePadState();

        public float DyingFrame = -1f;
        public String Name = "";

        public bool ReceivedNetUpdate;

        public void KillMe()
        {
            if (DyingFrame < 0f)
            {
                DyingFrame = 0f;
                if (LastHitBy == 0)
                    Game1.Score += MHP * 50;
            }
        }

        public void SetJump(float jump)
        {
            Trajectory.Y = -jump;
            State = CharState.Air;
            ledgeAttach = -1;
        }

        public void Slide(float x)
        {
            Trajectory.X = (float)Face * 2f * x - x;
            
        }

        public bool InHitBounds(Vector2 hitLoc)
        {
            if (hitLoc.X > Loc.X - 110f * Scale &&
                hitLoc.X < Loc.X + 110f * Scale &&
                hitLoc.Y > Loc.Y - 190f * Scale &&
                hitLoc.Y < Loc.Y + 10f * Scale)
                return true;
            return false;
        }

        public void WriteToNet(PacketWriter writer)
        {
            writer.Write(NetGame.MSG_CHARACTER);

            writer.Write(NetPacker.IntToSbyte(charDef.DefID));
            writer.Write(NetPacker.IntToSbyte(Team));
            writer.Write(NetPacker.IntToSbyte(ID));

            writer.Write(NetPacker.BigFloatToShort(Loc.X));
            writer.Write(NetPacker.BigFloatToShort(Loc.Y));

            writer.Write(NetPacker.IntToShort(Anim));
            writer.Write(NetPacker.IntToShort(AnimFrame));
            writer.Write(NetPacker.MidFloatToShort(frame));


            if (State == CharState.Air)
                writer.Write(true);
            else
                writer.Write(false);

            if (Face == CharDir.Right)
                writer.Write(true);
            else
                writer.Write(false);

            writer.Write(NetPacker.BigFloatToShort(Trajectory.X));
            writer.Write(NetPacker.BigFloatToShort(Trajectory.Y));

            writer.Write(KeyRight);
            writer.Write(KeyLeft);

            writer.Write(NetPacker.IntToShort(HP));
        }

        public void ReadFromNet(PacketReader reader)
        {
            Loc.X = NetPacker.ShortToBigFloat(reader.ReadInt16());
            Loc.Y = NetPacker.ShortToBigFloat(reader.ReadInt16());

            Anim = NetPacker.ShortToInt(reader.ReadInt16());
            AnimFrame = NetPacker.ShortToInt(reader.ReadInt16());
            AnimName = charDef.GetAnimation(Anim).name;
            frame = NetPacker.ShortToMidFloat(reader.ReadInt16());

            if (reader.ReadBoolean())
                State = CharState.Air;
            else
                State = CharState.Grounded;

            if (reader.ReadBoolean())
                Face = CharDir.Right;
            else
                Face = CharDir.Left;

            Trajectory.X = NetPacker.ShortToBigFloat(reader.ReadInt16());
            Trajectory.Y = NetPacker.ShortToBigFloat(reader.ReadInt16());

            KeyRight = reader.ReadBoolean();
            KeyLeft = reader.ReadBoolean();

            HP = NetPacker.ShortToInt(reader.ReadInt16());

            ReceivedNetUpdate = true;
        }

        public void DoInput(int idx)
        {
            curState = GamePad.GetState((PlayerIndex)idx);
            KeyLeft = false;
            KeyRight = false;
            KeyJump = false;
            KeyAttack = false;
            KeySecondary = false;
            KeyUp = false;
            KeyDown = false;

            if (curState.ThumbSticks.Left.X < -0.1f)
                KeyLeft = true;
            
            if (curState.ThumbSticks.Left.X > 0.1f)
                KeyRight = true;

            if (curState.ThumbSticks.Left.Y < -0.1f)
                KeyDown = true;

            if (curState.ThumbSticks.Left.Y > 0.1f)
                KeyUp = true;

            RightAnalog = curState.ThumbSticks.Right;
            
            if (curState.Buttons.A == ButtonState.Pressed &&
                prevState.Buttons.A == ButtonState.Released)
            {
                KeyJump = true;
            }

            if (curState.Buttons.Y == ButtonState.Pressed &&
                prevState.Buttons.Y == ButtonState.Released)
            {
                KeyAttack = true;
            }

            if (curState.Buttons.X == ButtonState.Pressed &&
                prevState.Buttons.X == ButtonState.Released)
            {
                KeySecondary = true;
            }

            if (curState.Buttons.Start == ButtonState.Pressed &&
                prevState.Buttons.Start == ButtonState.Released)
            {
                Game1.Menu.Pause();
            }

            prevState = curState;
            

        }

        private void CheckTrig(ParticleManager pMan)
        {
            int frameIdx = charDef.GetAnimation(Anim).GetKeyFrame(AnimFrame).FrameRef;

            Frame frame = charDef.GetFrame(frameIdx);

            for (int i = 0; i < frame.GetPartArray().Length; i++)
            {
                Part part = frame.GetPart(i);
                if (part.Index >= 1000)
                {

                    float rotation = part.Rotation;

                    Vector2 location = part.Location * Scale + Loc;
                    
                    if (Face == CharDir.Left)
                    {
                        location.X -= part.Location.X * Scale * 2.0f;
                    }

                    FireTrig(part.Index - 1000, location, pMan);
                    
                }
            }
        }

        private void FireTrig(int trig, Vector2 loc, ParticleManager pMan)
        {
            switch (trig)
            {
                case TRIG_PISTOL_ACROSS:
                case TRIG_PISTOL_UP:
                case TRIG_PISTOL_DOWN:
                    if (Team == TEAM_GOOD_GUYS && ID < 4)
                    {
                        QuakeManager.SetRumble(ID, 1, .5f);
                        QuakeManager.SetRumble(ID, 0, .3f);
                    }
                    break;
            }
            switch (trig)
            {
                case TRIG_FIRE_DIE:
                    for (int i = 0; i < 5; i++)
                    {
                        pMan.AddParticle(new Fire(loc + 
                            Rand.GetRandomVector2(-30f, 30f, -30f, 30f),
                            Rand.GetRandomVector2(-5f, 60f, -150f, -20f),
                            Rand.GetRandomFloat(.3f, .8f), Rand.GetRandomInt(0, 4),
                            Rand.GetRandomFloat(.5f, .8f)));
                    }
                    pMan.AddParticle(new Smoke(loc,
                        Rand.GetRandomVector2(-10f, 10f, -60f, 10f),
                        1f, .8f, .6f, 1f, Rand.GetRandomFloat(.5f, 1.2f), 
                        Rand.GetRandomInt(0, 4)));
                    pMan.AddParticle(new Heat(loc,
                        Rand.GetRandomVector2(-50f, 50f, -100f, 0f),
                        Rand.GetRandomFloat(1f, 2f)));
                    break;
                case TRIG_ROCKET:
                    pMan.AddParticle(new Rocket(loc, new Vector2((Face == CharDir.Right ? 350f : -350f),
                        100f), ID));
                    break;
                case TRIG_PISTOL_ACROSS:
                    pMan.MakeBullet(loc, new Vector2(2000f, 0f), Face, ID);
                    Sound.PlayCue("revol");
                    //QuakeManager.SetQuake(0.3f);
                    break;
                case TRIG_PISTOL_DOWN:
                    pMan.MakeBullet(loc, new Vector2(1400f, 1400f), Face, ID);
                    Sound.PlayCue("revol");
                    //QuakeManager.SetQuake(0.3f);
                    break;
                case TRIG_PISTOL_UP:
                    pMan.MakeBullet(loc, new Vector2(1400f, -1400f), Face, ID);
                    Sound.PlayCue("revol");
                    //QuakeManager.SetQuake(0.3f);
                    break;
                case TRIG_BLOOD_SQUIRT_BACK:
                case TRIG_BLOOD_SQUIRT_DOWN:
                case TRIG_BLOOD_SQUIRT_DOWN_BACK:
                case TRIG_BLOOD_SQUIRT_DOWN_FORNWARD:
                case TRIG_BLOOD_SQUIRT_FORWARD:
                case TRIG_BLOOD_SQUIRT_UP:
                case TRIG_BLOOD_SQUIRT_UP_BACK:
                case TRIG_BLOOD_SQUIRT_UP_FORWARD:
                    double r = 0.0;
                    switch (trig)
                    {
                        case TRIG_BLOOD_SQUIRT_FORWARD:
                            r = 0.0;
                            break;
                        case TRIG_BLOOD_SQUIRT_DOWN_FORNWARD:
                            r = Math.PI * .25;
                            break;
                        case TRIG_BLOOD_SQUIRT_DOWN:
                            r = Math.PI * .5;
                            break;
                        case TRIG_BLOOD_SQUIRT_DOWN_BACK:
                            r = Math.PI * .75;
                            break;
                        case TRIG_BLOOD_SQUIRT_BACK:
                            r = Math.PI;
                            break;
                        case TRIG_BLOOD_SQUIRT_UP_BACK:
                            r = Math.PI * 1.25;
                            break;
                        case TRIG_BLOOD_SQUIRT_UP:
                            r = Math.PI * 1.5;
                            break;
                        case TRIG_BLOOD_SQUIRT_UP_FORWARD:
                            r = Math.PI * 1.75;
                            break;
                    }
                    for (int i = 0; i < 7; i++)
                    {
                        pMan.AddParticle(new Blood(loc, new Vector2(
                            (float)Math.Cos(r) * (Face == CharDir.Right ? 1f : -1f),
                            (float)Math.Sin(r)
                            ) * Rand.GetRandomFloat(10f, 500f) +
                            Rand.GetRandomVector2(-90f, 90f, -90f, 90f),
                            1f, 0f, 0f, 1f, Rand.GetRandomFloat(0.1f, 0.5f),
                            Rand.GetRandomInt(0, 4)));
                    }
                    pMan.AddParticle(new BloodDust(loc,
                        Rand.GetRandomVector2(-30f, 30f, -30f, 30f),
                        1f, 0f, 0f, .2f,
                        Rand.GetRandomFloat(.25f, .5f),
                        Rand.GetRandomInt(0, 4)));
                    break;
                case TRIG_BLOOD_CLOUD:
                    pMan.AddParticle(new BloodDust(loc,
                        Rand.GetRandomVector2(-30f, 30f, -30f, 30f),
                        1f, 0f, 0f, .4f,
                        Rand.GetRandomFloat(.25f, .75f),
                        Rand.GetRandomInt(0, 4)));
                    break;
                case TRIG_BLOOD_SPLAT:
                    for (int i = 0; i < 6; i++)
                    {
                        pMan.AddParticle(new BloodDust(loc,
                        Rand.GetRandomVector2(-30f, 30f, -30f, 30f),
                        1f, 0f, 0f, .4f,
                        Rand.GetRandomFloat(.025f, .125f),
                        Rand.GetRandomInt(0, 4)));
                    }
                    break;
                default:
                    pMan.AddParticle(new Hit(loc, new Vector2(
                        200f * (float)Face - 100f, 0f),
                        ID, trig));
                    break;
            }
        }

        public void Update(Map map, ParticleManager pMan, Character[] c)
        {
            if (Ai != null)
                Ai.Update(c, ID, map);

            PressedKey = PressedKeys.None;
            if (KeyAttack)
            {   
                PressedKey = PressedKeys.Attack;
                if (KeyUp) PressedKey = PressedKeys.Lower;
                if (KeyDown) PressedKey = PressedKeys.Upper;
            }
            if (KeySecondary)
            {
                PressedKey = PressedKeys.Secondary;
                if (KeyUp) PressedKey = PressedKeys.SecUp;
                if (KeyDown) PressedKey = PressedKeys.SecDown;
            }
            if (PressedKey > PressedKeys.None) 
            {
                if (GotoGoal[(int)PressedKey] > -1) 
                {
                    SetFrame(GotoGoal[(int)PressedKey]);

                    if (KeyLeft)
                        Face = CharDir.Left;
                    if (KeyRight)
                        Face = CharDir.Right;

                    PressedKey = PressedKeys.None;

                    for (int i = 0; i < GotoGoal.Length; i++)
                        GotoGoal[i] = -1;

                    frame = 0f;
                }
            }

            if (StunFrame > 0f)
                StunFrame -= Game1.FrameTime;

            #region Update Dying
            if (DyingFrame > -1f)
            {
                DyingFrame += Game1.FrameTime;
                
            }
            #endregion

            #region Update Animation
            if (DyingFrame < 0f)
            {
                Animation animation = charDef.GetAnimation(Anim);
                KeyFrame keyframe = animation.GetKeyFrame(AnimFrame);



                frame += Game1.FrameTime * 30.0f;

                if (frame > (float)keyframe.Duration)
                {
                    int pframe = AnimFrame;

                    script.DoScript(Anim, AnimFrame);
                    CheckTrig(pMan);

                    frame -= (float)keyframe.Duration;
                    if (AnimFrame == pframe)
                        AnimFrame++;

                    keyframe = animation.GetKeyFrame(AnimFrame);

                    if (AnimFrame >=
                        animation.getKeyFrameArray().Length)
                        AnimFrame = 0;


                }


                if (keyframe.FrameRef < 0)
                    AnimFrame = 0;

                if (AnimName == "jhit")
                {
                    if (Trajectory.Y > -100f)
                        SetAnim("jmid");
                }
            }
            #endregion

            
            #region Collison w/ other characters
            for (int i = 0; i < c.Length; i++)
            {
                if (i != ID)
                {
                    if (c[i] != null)
                    {
                        if (!ethereal && !c[i].ethereal)
                        {
                            if (Loc.X > c[i].Loc.X - 90f * c[i].Scale &&
                                Loc.X < c[i].Loc.X + 90f * c[i].Scale &&
                                Loc.Y > c[i].Loc.Y - 120f * c[i].Scale &&
                                Loc.Y < c[i].Loc.Y + 10f * c[i].Scale)
                            {
                                float dif = (float)Math.Abs(Loc.X - c[i].Loc.X);
                                dif = 180f * c[i].Scale - dif;
                                dif *= 2f;
                                if (Loc.X < c[i].Loc.X)
                                {
                                    ColMove = -dif;
                                    c[i].ColMove = dif;
                                }
                                else
                                {
                                    ColMove = dif;
                                    c[i].ColMove = -dif;
                                }
                            }
                        }
                    }
                }
            }
            if (ColMove > 0f)
            {
                ColMove -= 400f * Game1.FrameTime;
                if (ColMove < 0f) ColMove = 0f;
            }
            if (ColMove < 0f)
            {
                ColMove += 400f * Game1.FrameTime;
                if (ColMove > 0f) ColMove = 0f;
            }
            #endregion

            #region Update loc by trajectory

            Vector2 pLoc = new Vector2(Loc.X, Loc.Y);

            if (State == CharState.Grounded || (State == CharState.Air && Floating))
            {
                if (Trajectory.X > 0f)
                {
                    Trajectory.X -= Game1.Friction * Game1.FrameTime;
                    if (Trajectory.X < 0f) Trajectory.X = 0f;
                }
                if (Trajectory.X < 0f)
                {
                    Trajectory.X += Game1.Friction * Game1.FrameTime;
                    if (Trajectory.X > 0f) Trajectory.X = 0f;
                }
            }
            Loc.X += Trajectory.X * Game1.FrameTime;
            Loc.X += ColMove * Game1.FrameTime;
            if (State == CharState.Air)
            {   
                Loc.Y += Trajectory.Y * Game1.FrameTime;
            }
            #endregion

            

            #region Collision detection
            if (State == CharState.Air)
            {
                #region Air State
                if (Floating)
                {
                    Trajectory.Y += Game1.FrameTime * Game1.Gravity * 0.5f;
                    if (Trajectory.Y > 100f) Trajectory.Y = 100f;
                    if (Trajectory.Y < -100f) Trajectory.Y = -100f;

                }
                else
                    Trajectory.Y += Game1.FrameTime * Game1.Gravity;

                CheckXCol(map, pLoc);

                #region Land on ledge
                if (Trajectory.Y > 0.0f)
                {
                    for (int i = 0; i < 16; i++)
                    {
                        if (map.GetLedgeTotalNodes(i) > 1)
                        {

                            int ts = map.GetLedgeSec(i, pLoc.X);
                            int s = map.GetLedgeSec(i, Loc.X);
                            float fY;
                            float tfY;
                            if (s > -1 && ts > -1)
                            {
                                
                                tfY = map.GetLedgeYLoc(i, s, pLoc.X);
                                fY = map.GetLedgeYLoc(i, s, Loc.X);
                                if (pLoc.Y <= tfY && Loc.Y >= fY)
                                {
                                    if (Trajectory.Y > 0.0f)
                                    {

                                        Loc.Y = fY;
                                        ledgeAttach = i;
                                        Land();
                                    }
                                }
                                else
                                    
                                    if (map.GetLedgeFlags(i) == (int)LedgeFlags.Solid
                                        &&
                                        Loc.Y >= fY)
                                    {
                                        Loc.Y = fY;
                                        ledgeAttach = i;
                                        Land();
                                    }
                            }
                            
                        }
                    }
                }
                #endregion

                #region Land on col
                if (State == CharState.Air)
                {
                    if (Trajectory.Y > 0f)
                    {
                        if (map.CheckCol(new Vector2(Loc.X, Loc.Y + 15f)))
                        {
                            Loc.Y = (float)((int)((Loc.Y + 15f) / 64f) * 64);
                            Land();
                        }
                    }
                }
                #endregion

                #endregion
            }
            else if (State == CharState.Grounded)
            {
                #region Grounded State

                if (ledgeAttach > -1)
                {
                    if (map.GetLedgeSec(ledgeAttach, Loc.X) == -1)
                    {
                        FallOff();
                    }
                    else
                    {
                        Loc.Y = map.GetLedgeYLoc(ledgeAttach,
                            map.GetLedgeSec(ledgeAttach, Loc.X), Loc.X);
                    }
                }
                else
                {
                    if (!map.CheckCol(new Vector2(Loc.X, Loc.Y + 15f)))
                        FallOff();
                }

                CheckXCol(map, pLoc);


                #endregion
            }
            #endregion

            #region Key input
            if (AnimName == "idle" || AnimName == "run" ||
                (State == CharState.Grounded && CanCancel))
            {
                if (AnimName == "idle" || AnimName == "run")
                {
                    if (KeyLeft)
                    {
                        SetAnim("run");
                        Trajectory.X = -Speed;
                        Face = CharDir.Left;
                    }
                    else if (KeyRight)
                    {
                        SetAnim("run");
                        Trajectory.X = Speed;
                        Face = CharDir.Right;
                    }
                    else
                    {
                        SetAnim("idle");
                    }
                }
                if (KeyAttack)
                {
                    SetAnim("attack");
                }
                if (KeySecondary)
                {
                    SetAnim("second");
                }
                if (KeyJump)
                {
                    SetAnim("jump");
                }
                if (RightAnalog.X > 0.2f || RightAnalog.X < -0.2f)
                {
                    SetAnim("roll");
                    if (AnimName == "roll")
                    {
                        if (RightAnalog.X > 0f)
                            Face = CharDir.Right;
                        else
                            Face = CharDir.Left;
                    }
                }
            }

            if (AnimName == "fly" ||
                (State == CharState.Air && CanCancel))
            {
                if (KeyLeft)
                {
                    Face = CharDir.Left;
                    if (Trajectory.X > -Speed)
                        Trajectory.X -= 500f * Game1.FrameTime;
                }
                if (KeyRight)
                {
                    Face = CharDir.Right;
                    if (Trajectory.X < Speed)
                        Trajectory.X += 500f * Game1.FrameTime;
                }
                if (KeySecondary)
                {
                    SetAnim("fsecond");
                }
                if (KeyAttack)
                {
                    SetAnim("fattack");
                }
            }

            #endregion
        }

        private void CheckXCol(Map map, Vector2 pLoc)
        {
            if (Trajectory.X + ColMove > 0f)
                if (map.CheckCol(new Vector2(Loc.X + 25f, Loc.Y - 15f)))
                    Loc.X = pLoc.X;

            if (Trajectory.X + ColMove < 0f)
                if (map.CheckCol(new Vector2(Loc.X - 25f, Loc.Y - 15f)))
                    Loc.X = pLoc.X;
        }

        private void FallOff()
        {
            State = CharState.Air;
            SetAnim("fly");
            Trajectory.Y = 0f;
            if (Trajectory.X > 300f) Trajectory.X = 300f;
            if (Trajectory.X < -300f) Trajectory.X = -300f;
        }

        private void Land()
        {
            State = CharState.Grounded;
            switch (AnimName)
            {
                case "jhit":
                case "jmid":
                case "jfall":
                    SetAnim("hitland");
                    if (HP < 0)
                        SetAnim("dieland");
                    break;
                default:
                    SetAnim("land");
                    break;
            }
            
        }

        public CharDef GetCharDef()
        {
            return charDef;
        }

        public void SetFrame(int newFrame)
        {
            AnimFrame = newFrame;
            frame = 0f;
            for (int i = 0; i < GotoGoal.Length; i++)
                GotoGoal[i] = -1;
            CanCancel = false;
        }



        public Character(Vector2 newLoc, CharDef newCharDef, int newID, int newTeam)
        {
            script = new Script(this);

            Ai = null;

            Loc = newLoc;
            ID = newID;
            Trajectory = new Vector2();

            Team = newTeam;

            Face = CharDir.Right;
            Scale = 0.5f;
            charDef = newCharDef;

            NoLifty = false;

            InitScript();

            AnimName = "";
            SetAnim("fly");
            ethereal = false;

            State = CharState.Air;

            
        }

        private void InitScript()
        {
            SetAnim("init");
            if (AnimName == "init")
            {
                for (int i = 0; i < charDef.GetAnimation(Anim).getKeyFrameArray().Length; i++)
                {
                    if (charDef.GetAnimation(Anim).GetKeyFrame(i).FrameRef > -1)
                        script.DoScript(Anim, i);
                }
            }
        }

        public void SetAnim(String newAnim)
        {
            
            if (AnimName != newAnim)
            {
                for (int i = 0; i < charDef.GetAnimationArray().Length; i++)
                {
                    if (charDef.GetAnimation(i).name == newAnim)
                    {
                        for (int t = 0; t < GotoGoal.Length; t++)
                            GotoGoal[t] = -1;
                        
                        
                        Floating = false;
                        Anim = i;
                        AnimFrame = 0;
                        frame = 0f;
                        AnimName = newAnim;
                        CanCancel = false;
                        ethereal = false;

                        if (KeyLeft)
                            Face = CharDir.Left;
                        if (KeyRight)
                            Face = CharDir.Right;

                    }
                    
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sRect = new Rectangle();

            int frameIdx = charDef.GetAnimation(Anim).GetKeyFrame(AnimFrame).FrameRef;

            Frame frame = charDef.GetFrame(frameIdx);

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            for (int i = 0; i < frame.GetPartArray().Length; i++)
            {
                Part part = frame.GetPart(i);
                if (part.Index > -1 && part.Index < 1000)
                {

                    sRect.X = ((part.Index % 64) % 5) * 64;
                    sRect.Y = ((part.Index % 64) / 5) * 64;
                    sRect.Width = 64;
                    sRect.Height = 64;
                    if (part.Index >= 192)
                    {
                        sRect.X = ((part.Index % 64) % 4) * 80;
                        sRect.Y = ((part.Index % 64) / 4) * 64;
                        sRect.Width = 80;
                    }

                    float rotation = part.Rotation;


                    Vector2 location = part.Location * Scale + Loc -
                        Game1.Scroll;
                    Vector2 scaling = part.Scaling * Scale;
                    if (part.Index >= 128) scaling *= 1.35f;

                    if (Face == CharDir.Left)
                    {
                        rotation = -rotation;
                        location.X -= part.Location.X * Scale * 2.0f;
                    }

                    Texture2D texture;
                    
                    int t = part.Index / 64;
                    switch (t)
                    {
                        case 0:
                            texture = headTex[charDef.HeadIndex];
                            break;
                        case 1:
                            texture = torsoTex[charDef.TorsoIndex];
                            break;
                        case 2:
                            texture = legsTex[charDef.LegsIndex];
                            break;
                        case 3:
                            texture = weaponTex[charDef.WeaponIndex];
                            break;
                        default:
                            texture = null;
                            break;
                    }
                    if (ID == 1 && Game1.Players == 2)
                    {
                        switch (t)
                        {
                            case 0:
                                texture = headTex[3];
                                break;
                            case 1:
                                texture = torsoTex[3];
                                break;
                            case 2:
                                texture = legsTex[2];
                                break;
                        }
                    }
                    Color color = new Color(new
                        Vector4(1.0f, 1.0f, 1.0f, 1f));

                    if (DyingFrame > 0f)
                        color = new Color(new Vector4(
                            1f - DyingFrame, 
                            1f - DyingFrame, 
                            1f - DyingFrame, 
                            1f - DyingFrame));

                    bool flip = false;

                    if ((Face == CharDir.Right && part.Flip == 0) ||
                        (Face == CharDir.Left && part.Flip == 1)) flip = true;


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

            spriteBatch.End();
        }

        internal static void LoadTextures(ContentManager Content)
        {
            for (int i = 0; i < headTex.Length; i++)
                headTex[i] = Content.Load<Texture2D>(@"gfx/head" +
                    (i + 1).ToString());

            for (int i = 0; i < torsoTex.Length; i++)
                torsoTex[i] = Content.Load<Texture2D>(@"gfx/torso" +
                    (i + 1).ToString());

            for (int i = 0; i < legsTex.Length; i++)
                legsTex[i] = Content.Load<Texture2D>(@"gfx/legs" +
                    (i + 1).ToString());

            for (int i = 0; i < weaponTex.Length; i++)
                weaponTex[i] = Content.Load<Texture2D>(@"gfx/weapon" +
                    (i + 1).ToString());

        }

        
    }
}
