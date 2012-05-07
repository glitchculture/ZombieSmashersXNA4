using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace xCharEdit.Character
{
    class Part
    {
        public Vector2 location;
        public float rotation;
        public Vector2 scaling;
        public int idx;
        public int flip;

        public Part()
        {
            idx = -1;
            scaling = new Vector2(1.0f, 1.0f);
        }
    }
}
