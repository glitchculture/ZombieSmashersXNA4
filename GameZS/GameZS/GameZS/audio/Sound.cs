using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace ZombieSmashers.audio
{
    class Sound
    {
        private static AudioEngine engine;
        private static SoundBank sound;
        private static WaveBank wave;

        public static void Initialize()
        {
            engine = new AudioEngine(@"Content/sfx/sfxproj.xgs");
            wave = new WaveBank(engine, @"Content/sfx/sfxwavs.xwb");
            sound = new SoundBank(engine, @"Content/sfx/sfxsnds.xsb");
        }

        public static AudioEngine GetEngine()
        {
            return engine;
        }

        public static void PlayCue(String cue)
        {
            sound.PlayCue(cue);
        }

        public static Cue GetCue(String cue)
        {
            return sound.GetCue(cue);
        }

        public static void Update()
        {
            engine.Update();
        }
    }
}
