using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace ZombieSmashers.map
{
    public class SegmentDefinition
    {
        private String name;
        private int srcIdx;
        private Rectangle srcRect;
        private int flags;

        public const int FLAGS_NONE = 0;
        public const int FLAGS_TORCH = 1;

        public SegmentDefinition(String _name,
            int _srcIdx,
            Rectangle _srcRect,
            int _flags)
        {
            name = _name;
            srcIdx = _srcIdx;
            srcRect = _srcRect;
            flags = _flags;
        }

        public String GetName()
        {
            return name;
        }

        public int GetSrcIdx()
        {
            return srcIdx;
        }

        public Rectangle GetSrcRect()
        {
            return srcRect;
        }

        public int GetFlags()
        {
            return flags;
        }
    }
}
