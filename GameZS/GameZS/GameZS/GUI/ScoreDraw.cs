using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ZombieSmashers.hud
{
    class ScoreDraw
    {
        SpriteBatch spriteBatch;
        Texture2D spritesTex;

        public enum Justify : int
        {
            Left = 0,
            Right = 1
        }

        public ScoreDraw(SpriteBatch _spriteBatch, Texture2D _spritesTex)
        {
            spriteBatch = _spriteBatch;
            spritesTex = _spritesTex;
        }

        public void Draw(long score, Vector2 loc, Color color, Justify justify)
        {
            int place = 0;

            if (justify == Justify.Left)
            {
                loc.X -= 17f;
                long s = score;
                if (s == 0)
                    loc.X += 17f;
                else
                    while (s > 0)
                    {
                        s /= 10;
                        loc.X += 17f;
                    }
            }

            while (true)
            {
                long digit = score % 10;
                score = score / 10;

                spriteBatch.Draw(spritesTex, loc + new Vector2((float)place * -17f, 0f),
                    new Rectangle((int)digit * 16, 224, 16, 32),
                    color);
                place++;
                if (score <= 0)
                    return;
            }
        }
    }
}
