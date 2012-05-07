using System;
using System.Collections.Generic;
using System.Text;

namespace ZombieSmashers.map
{
    public class MapFlags
    {
        String[] flags;

        public MapFlags(int size)
        {
            flags = new String[size];
            for (int i = 0; i < flags.Length; i++)
                flags[i] = "";
        }

        public bool GetFlag(String flag)
        {
            for (int i = 0; i < flags.Length; i++)
            {   
                if (flags[i] == flag)
                    return true;
            }
            return false;
        }

        public void SetFlag(String flag)
        {
            if (GetFlag(flag))
                return;

            for (int i = 0; i < flags.Length; i++)
            {
                if (flags[i] == "")
                {
                    flags[i] = flag;
                    return;
                }   
            }
        }
    }
}
