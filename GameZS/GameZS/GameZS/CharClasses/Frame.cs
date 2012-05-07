using System;
using System.Collections.Generic;
using System.Text;

namespace ZombieSmashers
{
    public class Frame
    {
        Part[] parts;
        public String Name;

        public Frame()
        {
            parts = new Part[16];
            for (int i = 0; i < parts.Length; i++)
                parts[i] = new Part();
            Name = "";
        }

        public Part GetPart(int idx)
        {
            return parts[idx];
        }

        public void SetPart(int idx, Part _part)
        {
            parts[idx] = _part;
        }

        public Part[] GetPartArray()
        {
            return parts;
        }
    }
}
