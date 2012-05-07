using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace ZombieSmashers.map
{
    public class Ledge
    {
        Vector2[] nodes = new Vector2[16];
        private int totalNodes = 0;
        private int flags = 0;

        public int TotalNodes
        {
            get { return totalNodes; }
            set { totalNodes = value; }
        }

        public int Flags
        {
            get { return flags; }
            set { flags = value; }
        }

        public Vector2[] Nodes
        {
            get { return nodes; }
        }

        
    }
}
