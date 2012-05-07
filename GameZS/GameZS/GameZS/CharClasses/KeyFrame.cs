using System;
using System.Collections.Generic;
using System.Text;

namespace ZombieSmashers
{
    public class KeyFrame
    {
        public int FrameRef;
        public int Duration;
       
        ScriptLine[] scripts;

        public KeyFrame()
        {
            FrameRef = -1;
            Duration = 0;
         

            scripts = new ScriptLine[4];
            for (int i = 0; i < scripts.Length; i++)
                scripts[i] = null;
        }

        public void SetScript(int idx, String val)
        {
            scripts[idx] = new ScriptLine(val);
        }

        public ScriptLine GetScript(int idx)
        {
            return scripts[idx];
        }

        public ScriptLine[] GetScriptArray()
        {
            return scripts;
        }
    }
}
