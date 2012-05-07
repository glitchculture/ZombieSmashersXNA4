using System;
using System.Collections.Generic;
using System.Text;

namespace xCharEdit.Character
{
    class KeyFrame
    {
        public int frameRef;
        public int duration;
        String[] script;

        public KeyFrame()
        {
            frameRef = -1;
            duration = 0;
            script = new String[4];
            for (int i = 0; i < script.Length; i++)
                script[i] = "";
        }

        public void SetScript(int idx, String val)
        {
            script[idx] = val;
        }

        public String GetScript(int idx)
        {
            return script[idx];
        }

        public String[] getScriptArray()
        {
            return script;
        }
    }
}
