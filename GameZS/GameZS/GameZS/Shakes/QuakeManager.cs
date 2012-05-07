using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace ZombieSmashers.quake
{
    class QuakeManager
    {
        public static Rumble[] Rumbles = new Rumble[4];

        public static Quake Quake;

        public static Blast Blast;

        public static void Init()
        {
            Quake = new Quake();
            for (int i = 0; i < Rumbles.Length; i++)
                Rumbles[i] = new Rumble(i);
            Blast = new Blast();
        }

        public static void Update()
        {
            Quake.Update();
            for (int i = 0; i < Rumbles.Length; i++)
                Rumbles[i].Update();
            Blast.Update();
        }

        public static void SetBlast(float mag, Vector2 center)
        {
            Blast.Value = mag;
            Blast.Magnitude = mag;
            Blast.center = center;
        }

        public static void SetQuake(float val)
        {
            if (Quake.val < val) Quake.val = val;

            for (int i = 0; i < Rumbles.Length; i++)
            {
                Rumbles[i].Left = val;
                Rumbles[i].Right = val;
            }
        }

        public static void SetRumble(int i, int motor, float val)
        {
            if (Game1.settings.Rumble)
            {
                if (motor == 0) Rumbles[i].Left = val;
                else Rumbles[i].Right = val;
            }
        }
    }
}
