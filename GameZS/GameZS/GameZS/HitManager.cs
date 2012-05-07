using System;
using System.Collections.Generic;
using System.Text;
using ZombieSmashers.Particles;
using Microsoft.Xna.Framework;
using ZombieSmashers.audio;
using ZombieSmashers.quake;
using ZombieSmashers.menu;

namespace ZombieSmashers
{
    class HitManager
    {
        private static CharDir GetFaceFromTraj(Vector2 traj)
        {
            if (traj.X > 0f)
                return CharDir.Right;
            else
                return CharDir.Left;
        }

        public static bool CheckHit(Particle p, Character[] c, 
            ParticleManager pMan)
        {
            bool r = false;

            CharDir tFace = GetFaceFromTraj(p.GetTraj());

            for (int i = 0; i < c.Length; i++)
            {
                if (i != p.Owner)
                {
                    if (c[i] != null)
                    {
                        if (c[i].DyingFrame < 0f &&
                            !c[i].ethereal)
                        {
                            if (c[i].InHitBounds(p.GetLoc()))
                            {
                                float hVal = 1f;

                                c[i].LastHitBy = p.Owner;
                                CharState pState = c[i].State;
                                Vector2 pLoc = c[i].Loc;

                                bool noAnim = false;
                                if (c[i].StunFrame > 0f &&
                                    c[i].StunFrame < 3f)
                                    noAnim = true;
                                if (c[i].NoLifty)
                                {
                                    if (c[i].StunFrame <= 0f ||
                                        c[i].StunFrame > 5.2f)
                                        c[i].StunFrame = 5.5f;
                                    
                                }

                                if (typeof(Bullet).Equals(p.GetType()))
                                {
                                    if (!r)
                                    {
                                        hVal *= 4f;

                                        c[i].Face = 1 - tFace;

                                        c[i].SetAnim("idle");
                                        if (!noAnim)
                                        {
                                            c[i].SetAnim("hit");
                                            c[i].Slide(-100f);
                                        }
                                        Sound.PlayCue("bullethit");

                                        Vector2 v = new Vector2(c[i].Loc.X,
                                            p.GetLoc().Y);
                                        pMan.MakeBulletBlood(v, p.GetTraj() / 2f);
                                        pMan.MakeBulletBlood(v, -p.GetTraj());
                                        pMan.MakeBulletDust(v, p.GetTraj());
                                        Game1.SlowTime = 0.05f;

                                        

                                        r = true;
                                    }
                                }
                                else if (typeof(Rocket).Equals(p.GetType()))
                                {
                                    pMan.MakeExplosion(p.GetLoc(), 1f);
                                    hVal *= 5f;
                                    if (!noAnim)
                                    {
                                        c[i].Trajectory.X = (p.GetTraj().X > 0f ? 600f : -600f);
                                        c[i].SetAnim("jhit");
                                        c[i].SetJump(300f);
                                    }
                                    Game1.SlowTime = 0.25f;
                                    r = true;
                                }
                                else if (typeof(Hit).Equals(p.GetType()))
                                {
                                    c[i].Face = 1 - tFace;
                                    float tX = 1f;
                                    if (tFace == CharDir.Left)
                                        tX = -1f;
                                    if (!noAnim)
                                    {
                                        c[i].SetAnim("idle");
                                        c[i].SetAnim("hit");
                                    }
                                    Sound.PlayCue("zomhit");

                                    if (c[i].State == CharState.Grounded)
                                        c[i].Slide(-200f);
                                    else
                                        c[i].Slide(-50f);

                                    

                                    switch (p.GetFlag())
                                    {
                                            
                                        case Character.TRIG_ZOMBIE_HIT:
                                            hVal *= 5f;
                                            pMan.MakeBloodSplash(p.GetLoc(),
                                                new Vector2(50f * tX, 100f));
                                            
                                            break;
                                        case Character.TRIG_WRENCH_DIAG_DOWN:
                                            hVal *= 5f;
                                            pMan.MakeBloodSplash(p.GetLoc(),
                                                new Vector2(50f * tX, 100f));
                                            
                                            Game1.SlowTime = 0.1f;

                                            break;
                                        case Character.TRIG_WRENCH_DIAG_UP:
                                            hVal *= 5f;
                                            pMan.MakeBloodSplash(p.GetLoc(),
                                                new Vector2(-50f * tX, -100f));
                                            
                                            Game1.SlowTime = 0.1f;

                                            break;
                                        case Character.TRIG_WRENCH_UP:
                                            hVal *= 5f;
                                            pMan.MakeBloodSplash(p.GetLoc(),
                                                new Vector2(30f * tX, -100f));
                                            Game1.SlowTime = 0.1f;

                                            break;
                                        case Character.TRIG_WRENCH_DOWN:
                                            hVal *= 5f;
                                            pMan.MakeBloodSplash(p.GetLoc(),
                                                new Vector2(-50f * tX, 100f));
                                            
                                            Game1.SlowTime = 0.1f;
                                            Sound.PlayCue("zomhit");

                                            break;
                                        case Character.TRIG_WRENCH_UPPERCUT:
                                        case Character.TRIG_CHAINSAW_UPPER:
                                            hVal *= 15f;
                                            pMan.MakeBloodSplash(p.GetLoc(),
                                                new Vector2(-50f * tX, -150f));
                                            c[i].Trajectory.X = 100f * tX;

                                            c[i].SetAnim("jhit");
                                            c[i].SetJump(700f);
                                            Game1.SlowTime = 0.125f;
                                            QuakeManager.SetQuake(.5f);
                                            QuakeManager.SetBlast(.5f, p.GetLoc());
                                            break;
                                        case Character.TRIG_WRENCH_SMACKDOWN:
                                        case Character.TRIG_CHAINSAW_DOWN:
                                            hVal *= 15f;
                                            pMan.MakeBloodSplash(p.GetLoc(),
                                                new Vector2(-50f * tX, 150f));
                                            c[i].SetAnim("jfall");
                                            c[i].SetJump(-900f);

                                            
                                            Game1.SlowTime = 0.125f;
                                            QuakeManager.SetQuake(.5f);
                                            QuakeManager.SetBlast(.5f, p.GetLoc());
                                            break;
                                        case Character.TRIG_KICK:
                                            hVal *= 15f;
                                            pMan.MakeBloodSplash(p.GetLoc(),
                                                new Vector2(300f * tX, 0f));

                                            c[i].Trajectory.X = 1000f * tX;

                                            c[i].SetAnim("jhit");
                                            c[i].SetJump(300f);

                                            Game1.SlowTime = 0.25f;
                                            QuakeManager.SetQuake(.5f);
                                            QuakeManager.SetBlast(.75f, p.GetLoc());
                                            break;
                                    }
                                }
                                if (c[i].State == CharState.Air)
                                {
                                    
                                    if (c[i].AnimName == "hit")
                                    {
                                        c[i].SetAnim("jmid");
                                        c[i].SetJump(300f);
                                        if (typeof(Hit).Equals(p.GetType()))
                                        {
                                            if (c[p.Owner].Team ==
                                                Character.TEAM_GOOD_GUYS)
                                            {
                                                c[i].Loc.Y =
                                                    c[p.Owner].Loc.Y;
                                            }
                                        }
                                    }
                                    if (c[i].NoLifty)
                                    {
                                        if (pState == CharState.Grounded)
                                        {
                                            c[i].Loc = pLoc;
                                            c[i].State = pState;
                                            c[i].SetAnim("hit");
                                        }
                                        if (c[i].Trajectory.X > 300f)
                                            c[i].Trajectory.X = 300f;
                                        if (c[i].Trajectory.X < -300f)
                                            c[i].Trajectory.X = -300f;
                                    }
                                }

                                c[i].HP -= (int)hVal;

                                if (c[i].LastHitBy == 0)
                                    Game1.Score += (int)hVal * 50;


                                if (c[i].HP < 0)
                                {
                                    if (c[i].AnimName == "hit")
                                        c[i].SetAnim("diehit");
                                    if (i == 0)
                                    {
                                        if (c[i].AnimName == "hit")
                                        {
                                            c[i].SetAnim("jmid");
                                            c[i].SetJump(300f);
                                            
                                        }
                                        Game1.Menu.Die();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return r;
        } 
    }
}
