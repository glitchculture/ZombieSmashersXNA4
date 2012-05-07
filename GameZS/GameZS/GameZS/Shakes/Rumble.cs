using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace ZombieSmashers.quake
{
    class Rumble
    {
        private Vector2 rumbleValue = Vector2.Zero;
        private PlayerIndex playerIndex;

        public Rumble(int idx)
        {
            playerIndex = (PlayerIndex)idx;
        }

        public void Update()
        {
            if (rumbleValue.X > 0f)
            {
                rumbleValue.X -= Game1.FrameTime;
                if (rumbleValue.X < 0f) rumbleValue.X = 0f;
            }

            if (rumbleValue.Y > 0f)
            {
                rumbleValue.Y -= Game1.FrameTime;
                if (rumbleValue.Y < 0f) rumbleValue.Y = 0f;
            }

            GamePad.SetVibration(playerIndex, rumbleValue.X, rumbleValue.Y);
        }

        public float Left
        {
            get { return rumbleValue.X; }
            set { rumbleValue.X = value; }
        }

        public float Right
        {
            get { return rumbleValue.Y; }
            set { rumbleValue.Y = value; }
        }
    }
}
