using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace ZombieSmashers.map.bucket
{
    public class Bucket
    {
        BucketItem[] bucketItem = new BucketItem[64];
        public int Size;
        float updateFrame = 0f;
        public bool IsEmpty = false;

        public Bucket(int size)
        {
            for (int i = 0; i < bucketItem.Length; i++)
                bucketItem[i] = null;

            Size = size;
        }

        public void AddItem(Vector2 loc, CharacterDefinitions charDef)
        {
            for (int i = 0; i < bucketItem.Length; i++)
            {
                if (bucketItem[i] == null)
                {
                    bucketItem[i] = new BucketItem(loc, charDef);
                    return;
                }
            }
        }

        public void Update(Character[] c)
        {
            if (Game1.NetPlay.Joined)
                return;

            updateFrame -= Game1.FrameTime;

            if (updateFrame > 0f)
                return;
            updateFrame = 1f;

            int monsters = 0;

            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] != null)
                {
                    if (c[i].Team == Character.TEAM_BAD_GUYS)
                        monsters++;
                }
            }

            if (monsters < Size)
            {
                for (int i = 0; i < bucketItem.Length; i++)
                {
                    if (bucketItem[i] != null)
                    {
                        for (int n = Game1.Players; n < c.Length; n++)
                        {
                            if (c[n] == null)
                            {
                                c[n] = new Character(bucketItem[i].Location,
                                    Game1.charDef[(int)bucketItem[i].CharDef],
                                    n, Character.TEAM_BAD_GUYS);
                                bucketItem[i] = null;
                                return;
                            }
                        }
                    }
                }
                if (monsters == 0)
                    IsEmpty = true;
            }
            
        }
    }
}
