using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using ZombieSmashers.map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ZombieSmashers.hud
{
    /// <summary>
    /// Manage and draw the game HUD--hearts, score, etc.
    /// 
    /// This is covered in chapter 9.
    /// </summary>
    class HUD
    {
        SpriteBatch sprite;
        Texture2D spritesTex;
        Texture2D nullTex;

        Character[] character;

        Map map;

        ScoreDraw scoreDraw;

        float heartFrame;
        float[] fHP = { 0f, 0f };

        public HUD(SpriteBatch _sprite, Texture2D _spritesTex, 
            Texture2D _nullTex,
            Character[] _character,
            Map _map)
        {
            sprite = _sprite;
            spritesTex = _spritesTex;
            character = _character;
            map = _map;
            nullTex = _nullTex;
            scoreDraw = new ScoreDraw(sprite, spritesTex);
        }

        public void Update()
        {
            heartFrame += Game1.FrameTime;
            if (heartFrame > 6.28f)
                heartFrame -= 6.28f;

            for (int p = 0; p < Game1.Players; p++)
            {
                if ((float)character[p].HP > fHP[p])
                {
                    fHP[p] += Game1.FrameTime * 15f;
                    if (fHP[p] > (float)character[p].HP)
                        fHP[p] = (float)character[p].HP;
                }
                if ((float)character[p].HP < fHP[p])
                {
                    fHP[p] -= Game1.FrameTime * 15f;
                    if (fHP[p] < (float)character[p].HP)
                        fHP[p] = (float)character[p].HP;
                }
            }
        }

        public void Draw()
        {
            sprite.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            if (Game1.Players == 1)
                scoreDraw.Draw(Game1.Score, new Vector2(50f, 78f), 
                    Color.White, ScoreDraw.Justify.Left);
            
            for (int p = 0; p < Game1.Players; p++)
            {
                float fProg = fHP[p] / (float)character[p].MHP;
                float prog = (float)character[p].HP / (float)character[p].MHP;
                fProg *= 5f;
                prog *= 5f;
                for (int i = 0; i < 5; i++)
                {
                    float r = (float)Math.Cos((double)heartFrame * 2.0 + (double)i) * .1f;
                    float t = (p == 0 ?
                        66f + (float)i * 32f :
                        Game1.ScreenSize.X - 66f - (float)i * 32f);

                    sprite.Draw(spritesTex, new Vector2(t, 66f),
                            new Rectangle(i * 32, 192, 32, 32),
                            new Color(new Vector4(0.5f, 0f, 0f, .25f)),
                            r, new Vector2(16f, 16f), 1.25f,
                            SpriteEffects.None, 1f);

                    float ta = fProg - (float)i;


                    if (ta > 1f) ta = 1f;
                    if (ta > 0f)
                    {
                        
                        sprite.Draw(spritesTex, new Vector2(t, 66f),
                            (p == 0 ?
                            new
                            Rectangle(i * 32, 192, (int)(32f * ta), 32)
                            :
                            new
                            Rectangle(i * 32 + (int)(32f * (1f - ta)), 192, (int)(32f * ta), 32)
                            ),
                            new Color(new Vector4(1f, 0f, 0f, .75f)),
                            r, new Vector2(16f
                            - (p == 1 ? 32f * (1f - ta) : 0f), 16f), 1.25f,
                            SpriteEffects.None, 1f);
                    }

                    ta = prog - (float)i;
                    if (ta > 1f) ta = 1f;
                    if (ta > 0f)
                    {
                        sprite.Draw(spritesTex, new Vector2(t, 66f),
                            (p == 0 ?
                            new 
                            Rectangle(i * 32, 192, (int)(32f * ta), 32)
                            :
                            new 
                            Rectangle(i * 32 + (int)(32f * (1f - ta)), 192, (int)(32f * ta), 32)
                            ),
                            new Color(new Vector4(.9f, 0f, 0f, 1f)),
                            r, new Vector2(16f
                            - (p == 1 ? 32f * (1f - ta) : 0f), 16f), 1.25f,
                            SpriteEffects.None, 1f);
                    }
                }
            }

            float a = map.GetTransVal();
            if (a > 0f)
            {
                sprite.Draw(nullTex, new Rectangle(0, 0,
                    (int)Game1.ScreenSize.X,
                    (int)Game1.ScreenSize.Y), new Color(
                    new Vector4(0f, 0f, 0f, a)));
            }

            sprite.End();
        }
    }
}
