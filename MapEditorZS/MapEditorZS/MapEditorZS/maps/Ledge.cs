using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MapEditor.map
{
    class Ledge
    {
        Vector2[] node = new Vector2[16];
        public int totalNodes = 0;
        public int flags = 0;

        public Vector2 GetNode(int i)
        {
            return node[i];
        }

        public void SetNode(int i, Vector2 v)
        {
            node[i] = v;
        }

        
    }
}
