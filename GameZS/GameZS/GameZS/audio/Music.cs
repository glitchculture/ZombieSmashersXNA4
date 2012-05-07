using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace ZombieSmashers.audio
{
    class Music
    {   
        private static WaveBank wave;
        private static SoundBank sound;

        private static Cue musicCue;

        private static String musicStr;

        public static void Initialize()
        {   
            wave = new WaveBank(Sound.GetEngine(), @"Content/sfx/musicwavs.xwb", 0, 16);
            sound = new SoundBank(Sound.GetEngine(), @"Content/sfx/musicsnds.xsb");

            
        }

        public static void Play(String _musicStr)
        {
            
            if (musicStr !=  _musicStr) 
            {
                musicStr = _musicStr;

                if (musicCue != null)
                    musicCue.Dispose();

                musicCue = sound.GetCue(musicStr);
                musicCue.Play();
            }
            
        }
    }
}
