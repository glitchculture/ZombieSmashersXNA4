using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace ZombieSmashers.map.bucket
{
    class BucketItem
    {
        public Vector2 Location;
        public CharacterDefinitions CharDef;

        public BucketItem(Vector2 loc, CharacterDefinitions charDef)
        {
            Location = loc;
            CharDef = charDef;
        }
    }
}
