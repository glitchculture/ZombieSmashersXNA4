using System;
using System.Collections.Generic;
using System.Text;

namespace xCharEdit.Character
{
    class Frame
    {
        Part[] part;
        public String name;

        public Frame()
        {
            part = new Part[16];
            for (int i = 0; i < part.Length; i++)
                part[i] = new Part();
            name = "";
        }

        public Part GetPart(int idx)
        {
            return part[idx];
        }

        public void SetPart(int idx, Part _part)
        {
            part[idx] = _part;
        }

        public Part[] GetPartArray()
        {
            return part;
        }
    }
}
