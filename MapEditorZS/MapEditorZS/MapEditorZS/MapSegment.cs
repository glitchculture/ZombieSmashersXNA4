using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MapEditor.map
{
    class MapSegment
    {
        Vector2 loc;
        int segDefIdx;

        public Vector2 GetLoc()
        {
            return loc;
        }

        public int GetDefIdx()
        {
            return segDefIdx;
        }

        public void SetLoc(Vector2 _loc)
        {
            loc = _loc;
        }

        public void SetDefIdx(int _defIdx)
        {
            segDefIdx = _defIdx;
        }
    }
}
