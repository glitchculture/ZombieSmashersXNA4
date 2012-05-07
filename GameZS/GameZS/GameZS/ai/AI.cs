using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using ZombieSmashers.map;

namespace ZombieSmashers.ai
{
    /// <summary>
    /// Here is our AI base class--we'll be extending this for our Zombie AI class in chapter 8
    /// and eventually a Wraith and Carlos AI class.  
    /// </summary>
    public class AI
    {
        public const int JOB_IDLE = 0;
        public const int JOB_MELEE_CHASE = 1;
        public const int JOB_SHOOT_CHASE = 2;
        public const int JOB_AVOID = 3;

        protected int job = JOB_IDLE;
        protected int targ = -1;
        protected float jobFrame = 0f;

        protected Character me;

        protected int FindTarg(Character[] c)
        {
            int closest = -1;
            float d = 0f;

            for (int i = 0; i < c.Length; i++)
            {
                if (i != me.ID)
                {
                    if (c[i] != null)
                    {
                        if (c[i].Team != me.Team)
                        {
                            float newD = (me.Loc - c[i].Loc).Length();
                            if (closest == -1 || newD < d)
                            {
                                d = newD;
                                closest = i;
                            }
                        }
                    }
                }
            }

            return closest;
        }

        public virtual void Update(Character[] c, int ID, Map map)
        {
            me = c[ID];

            me.KeyLeft = false;
            me.KeyRight = false;
            me.KeyUp = false;
            me.KeyDown = false;
            me.KeyAttack = false;
            me.KeySecondary = false;
            me.KeyJump = false;

            jobFrame -= Game1.FrameTime;

            DoJob(c, ID);
        }

        protected void DoJob(Character[] c, int ID)
        {
            switch (job)
            {
                case JOB_IDLE:
                    //do nothing!
                    break;
                case JOB_MELEE_CHASE:
                    if (targ > -1 && c[targ] != null)
                    {
                        if (!ChaseTarg(c, c[ID].Scale * 100f))
                        {
                            if (!FaceTarg(c))
                            {
                                me.KeyAttack = true;
                            }
                        }
                    }
                    else
                        targ = FindTarg(c);
                    break;

                case JOB_AVOID:
                    if (targ > -1 && c[targ] != null)
                    {
                        AvoidTarg(c, 500f);
                        
                    }
                    else
                        targ = FindTarg(c);
                    break;

                case JOB_SHOOT_CHASE:
                    if (targ > -1 && c[targ] != null)
                    {
                        if (!ChaseTarg(c, 150f))
                        {
                            if (!FaceTarg(c))
                            {
                                me.KeySecondary = true;
                            }
                        }
                    }
                    else
                        targ = FindTarg(c);
                    break;
            }
            if (!me.KeyAttack && !me.KeySecondary)
            {
                if (me.KeyLeft)
                {
                    if (FriendInWay(c, ID, CharDir.Left))
                        me.KeyLeft = false;
                }
                if (me.KeyRight)
                {
                    if (FriendInWay(c, ID, CharDir.Right))
                        me.KeyRight = false;
                }
            }
        }

        private bool FriendInWay(Character[] c, int ID, CharDir face)
        {
            for (int i = 0; i < c.Length; i++)
            {
                if (i != ID)
                {
                    if (c[i] != null)
                    {
                        if (me.Team == c[i].Team)
                        {
                            if (me.Loc.Y > c[i].Loc.Y - 100f &&
                                me.Loc.Y < c[i].Loc.Y + 10f)
                            {
                                if (face == CharDir.Right)
                                {
                                    if (c[i].Loc.X > me.Loc.X &&
                                        c[i].Loc.X < me.Loc.X + 70f)
                                        return true;
                                }
                                else
                                {
                                    if (c[i].Loc.X < me.Loc.X &&
                                        c[i].Loc.X > me.Loc.X - 70f)
                                        return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        protected bool ChaseTarg(Character[] c, float distance)
        {
            if (c[targ] == null)
                return false;
            if (me.Loc.X > c[targ].Loc.X + distance)
            {
                me.KeyLeft = true;
                return true;
            }
            else if (me.Loc.X < c[targ].Loc.X - distance)
            {
                me.KeyRight = true;
                return true;
            }
            return false;
        }

        protected bool AvoidTarg(Character[] c, float distance)
        {
            if (c[targ] == null)
                return false;
            if (me.Loc.X < c[targ].Loc.X + distance)
            {
                me.KeyRight = true;
                return true;
            }
            else if (me.Loc.X > c[targ].Loc.X - distance)
            {
                me.KeyLeft = true;
                return true;
            }
            return false;
        }

        protected bool FaceTarg(Character[] c)
        {
            if (c[targ] == null)
                return false;
            if (me.Loc.X > c[targ].Loc.X && me.Face == CharDir.Right)
            {
                me.KeyLeft = true;
                return true;
            }
            else if (me.Loc.X > c[targ].Loc.X && me.Face == CharDir.Right)
            {
                me.KeyRight = true;
                return true;
            }
            else
                return false;
        }
    }
}
