using System;
using System.Collections.Generic;
using System.Text;

namespace ZombieSmashers
{
    public class Animation
    {
        public String name;
        KeyFrame[] keyFrame;

        public Animation()
        {
            name = "";
            keyFrame = new KeyFrame[64];
            for (int i = 0; i < keyFrame.Length; i++)
                keyFrame[i] = new KeyFrame();
        }

        public KeyFrame GetKeyFrame(int idx)
        {   
            return keyFrame[idx];
        }

        public void SetKeyFrame(int idx, KeyFrame _keyFrame)
        {
            keyFrame[idx] = _keyFrame;
        }

        public KeyFrame[] getKeyFrameArray()
        {
            return keyFrame;
        }
    }
}
