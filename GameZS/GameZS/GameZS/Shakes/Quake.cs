using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace ZombieSmashers.quake
{
    class Quake
    {
        public float val;

        public float Value
        {
            get { return val; }
            set { val = value; }
        }

        public Vector2 Vector
        {
            get
            {
                if (val <= 0f)
                    return Vector2.Zero;

                return Rand.GetRandomVector2(-val, val, -val, val) * 10f;
            }
        }

        public void Update()
        {
            if (val > 0f)
                val -= Game1.FrameTime;
        }
    }
}
