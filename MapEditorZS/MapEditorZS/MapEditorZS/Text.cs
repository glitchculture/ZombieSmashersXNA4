using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MapEditor
{
    class Text
    {
        private float size;
        private Color color;
        Texture2D textTex;
        SpriteBatch sprite;

        public Text(Texture2D _textTex, SpriteBatch _sprite)
        {
            size = 0.8f;
            color = Color.White;

            textTex = _textTex;
            sprite = _sprite;
        }

        public void SetColor(Color newColor)
        {
            color = newColor;
        }

        public void SetSize(float newSize)
        {
            size = newSize;
        }

        public bool DrawClickText(int x, int y, String s,
            int mosX, int mosY, bool mouseClick)
        {
            if (s.Length == 0) return false;
            int eX = x + GetStringSpace(s.ToCharArray());
            bool r = false;

            if ((mosX > x) &&
                (mosX < eX) &&
                (mosY > y) &&
                (mosY < y + (int)(36.0f * size)))
            {

                color = Color.Yellow;
                if (mouseClick) r = true;
            }
            else
            {
                color = Color.White;
            }

            DrawText(x, y, s);

            return r;
        }

        public void DrawText(int x, int y, String s)
        {
            sprite.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            int tX = x;
            int iDx = -1;
            String tS;
            Rectangle rect = new Rectangle();
            Rectangle dRect = new Rectangle();

            char[] c = s.ToCharArray();

            for (int i = 0; i < s.Length; i++)
            {
                tS = s.Substring(i, 1);
                if (tS == " ")
                {
                    x += (int)(6.0f * size);
                }
                else
                {
                    iDx = GetTextIndex(c[i]);

                    if (iDx > -1)
                    {
                        rect.X = (iDx % 16) * 32;
                        rect.Y = (iDx / 16) * 36;
                        rect.Width = 32;
                        rect.Height = 36;

                        dRect.X = x;
                        dRect.Y = y;
                        dRect.Width = (int)(32.0f * size);
                        dRect.Height = (int)(36.0f * size);

                        sprite.Draw(textTex, dRect, rect, color);

                    }
                    x += (int)((float)GetTextSpace(c[i]) * size);
                }
            }
            sprite.End();
        }

        private int GetStringSpace(Char[] c)
        {
            int x = 0;
            
            for (int i = 0; i < c.Length; i++)
            {
                
                if (c[i] == ' ')
                    x += (int)(6.0f * size);
                else
                    x += (int)((float)GetTextSpace(c[i]) * size);

            }
            return x;
        }

        private int GetTextSpace(Char s)
        {
            switch (s)
            {
                case 'i':
                case 'I':
                case 'l':
                    return 6;
                case '!':
                case ',':
                case '.':
                case ':':
                case ';':
                case '\'':
                    return 8;
                case 'j':
                case '[':
                case ']':
                    return 10;
                case 'f':
                case 't':
                case '(':
                case ')':
                    return 12;
                
                case 'r':
                case '1':
                case '/':
                case '-':
                    return 13;
                case '"':
                    return 14;
                case '*':
                    return 15;
                case 'J':
                    return 16;
                case 'L':
                case '^':
                    return 17;
                case 'a':
                case 'b':
                case 'h':
                case 'o':
                case 'q':
                case 'v':
                case 'y':
                case 'z':
                case '$':
                case '?':
                    return 18;
                case 'c':
                case 'd':
                case 'e':
                case 'g':
                case 'k':
                case 'n':
                
                case 'p':
                case 's':
                case 'u':
                case 'x':
                case 'F':
                case '=':
                case '<':
                case '>':
                    return 19;
                
                case 'K':
                case '0':
                case '2':
                case '#':
                case '+':
                    return 20;
                case 'B':
                case 'E':
                case 'H':
                case 'N':
                case 'P':
                case '3':
                case '4':
                case '7':
                case '8':
                    return 21;
                case 'S':
                case 'T':
                case 'U':
                case 'Z':
                case '5':
                case '6':
                case '9':
                case '&':
                    return 22;
                case 'D':
                case 'R':
                    return 23;
                case 'C':
                case 'M':
                case 'X':
                    return 24;
                case 'G':
                    return 25;
                case 'A':
                case 'O':
                case 'Q':
                case 'V':
                case 'Y':
                    return 26;
                case 'w':
                    return 27;
                case 'm':
                    return 28;
                case '%':
                    return 29;
                case 'W':
                    return 31;
                case '@':
                    return 32;

            }

            Console.WriteLine("can't find " + s.ToString());
            return 32;
        }


        private int GetTextIndex(Char s)
        {
            int x;
            switch (s)
            {
                case 'a':
                    x = 0;
                    break;
                case 'b':
                    x = 1;
                    break;
                case 'c':
                    x = 2;
                    break;
                case 'd':
                    x = 3;
                    break;
                case 'e':
                    x = 4;
                    break;
                case 'f':
                    x = 5;
                    break;
                case 'g':
                    x = 6;
                    break;
                case 'h':
                    x = 7;
                    break;
                case 'i':
                    x = 8;
                    break;
                case 'j':
                    x = 9;
                    break;
                case 'k':
                    x = 10;
                    break;
                case 'l':
                    x = 11;
                    break;
                case 'm':
                    x = 12;
                    break;
                case 'n':
                    x = 13;
                    break;
                case 'o':
                    x = 14;
                    break;
                case 'p':
                    x = 15;
                    break;
                case 'q':
                    x = 16;
                    break;
                case 'r':
                    x = 17;
                    break;
                case 's':
                    x = 18;
                    break;
                case 't':
                    x = 19;
                    break;
                case 'u':
                    x = 20;
                    break;
                case 'v':
                    x = 21;
                    break;
                case 'w':
                    x = 22;
                    break;
                case 'x':
                    x = 23;
                    break;
                case 'y':
                    x = 24;
                    break;
                case 'z':
                    x = 25;
                    break;

                case 'A':
                    x = 26;
                    break;
                case 'B':
                    x = 27;
                    break;
                case 'C':
                    x = 28;
                    break;
                case 'D':
                    x = 29;
                    break;
                case 'E':
                    x = 30;
                    break;
                case 'F':
                    x = 31;
                    break;
                case 'G':
                    x = 32;
                    break;
                case 'H':
                    x = 33;
                    break;
                case 'I':
                    x = 34;
                    break;
                case 'J':
                    x = 35;
                    break;
                case 'K':
                    x = 36;
                    break;
                case 'L':
                    x = 37;
                    break;
                case 'M':
                    x = 38;
                    break;
                case 'N':
                    x = 39;
                    break;
                case 'O':
                    x = 40;
                    break;
                case 'P':
                    x = 41;
                    break;
                case 'Q':
                    x = 42;
                    break;
                case 'R':
                    x = 43;
                    break;
                case 'S':
                    x = 44;
                    break;
                case 'T':
                    x = 45;
                    break;
                case 'U':
                    x = 46;
                    break;
                case 'V':
                    x = 47;
                    break;
                case 'W':
                    x = 48;
                    break;
                case 'X':
                    x = 49;
                    break;
                case 'Y':
                    x = 50;
                    break;
                case 'Z':
                    x = 51;
                    break;

                case '0':
                    x = 52;
                    break;
                case '1':
                    x = 53;
                    break;
                case '2':
                    x = 54;
                    break;
                case '3':
                    x = 55;
                    break;
                case '4':
                    x = 56;
                    break;
                case '5':
                    x = 57;
                    break;
                case '6':
                    x = 58;
                    break;
                case '7':
                    x = 59;
                    break;
                case '8':
                    x = 60;
                    break;
                case '9':
                    x = 61;
                    break;

                case '!':
                    x = 62;
                    break;
                case '@':
                    x = 63;
                    break;
                case '#':
                    x = 64;
                    break;
                case '$':
                    x = 65;
                    break;
                case '%':
                    x = 66;
                    break;
                case '^':
                    x = 67;
                    break;
                case '&':
                    x = 68;
                    break;
                case '*':
                    x = 69;
                    break;
                case '(':
                    x = 70;
                    break;
                case ')':
                    x = 71;
                    break;
                case ',':
                    x = 72;
                    break;
                case '.':
                    x = 73;
                    break;
                case '/':
                    x = 74;
                    break;
                case '?':
                    x = 75;
                    break;
                case ':':
                    x = 76;
                    break;
                case ';':
                    x = 77;
                    break;
                case '\'':
                    x = 78;
                    break;
                case '"':
                    x = 79;
                    break;
                case '-':
                    x = 80;
                    break;
                case '=':
                    x = 81;
                    break;
                case '+':
                    x = 82;
                    break;
                case '<':
                    x = 83;
                    break;
                case '>':
                    x = 84;
                    break;
                case '[':
                    x = 85;
                    break;
                case ']':
                    x = 86;
                    break;
                default:
                    x = -1;
                    break;
            }
            return x;
        }
    }
}
