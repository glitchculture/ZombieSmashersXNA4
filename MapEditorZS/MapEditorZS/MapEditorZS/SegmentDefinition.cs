using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MapEditor.map
{
    class SegmentDefinition
    {
        private String name;
        private int srcIdx;
        private Rectangle srcRect;
        private int flags;

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
