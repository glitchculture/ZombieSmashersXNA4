using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ZombieSmashers
{
    public class CharDef
    {
        Animation[] animation;
        Frame[] frame;
        public String Path;

        public int HeadIndex;
        public int TorsoIndex;
        public int LegsIndex;
        public int WeaponIndex;        

        public int DefID;

        public CharDef(String loadPath, CharacterDefinitions _defID)
        {
            Reset();
            Path = loadPath;
            Read();
            DefID = (int)_defID;
        }

        public CharDef()
        {
            Reset();
        }

        private void Reset()
        {
            animation = new Animation[64];
            for (int i = 0; i < animation.Length; i++)
                animation[i] = new Animation();
            frame = new Frame[512];
            for (int i = 0; i < frame.Length; i++)
                frame[i] = new Frame();

            Path = "char";
        }

        public Animation GetAnimation(int idx)
        {   
            return animation[idx];
        }

        public void SetAnimation(int idx, Animation _animation)
        {
            animation[idx] = _animation;
        }

        public Animation[] GetAnimationArray()
        {
            return animation;
        }

        public Frame GetFrame(int idx)
        {
            return frame[idx];
        }

        public void SetFrame(int idx, Frame _frame)
        {
            frame[idx] = _frame;
        }

        public Frame[] GetFrameArray()
        {
            return frame;
        }

        public void DoScript(int animIdx, int animFrame)
        {
            KeyFrame keyFrame = animation[animIdx].GetKeyFrame(animFrame);
            
        }

        public void Read()
        {
            BinaryReader b = new BinaryReader(File.Open(@"data/" + Path + ".zmx",
                FileMode.Open, FileAccess.Read));


            Path = b.ReadString();
            HeadIndex = b.ReadInt32();
            TorsoIndex = b.ReadInt32();
            LegsIndex = b.ReadInt32();
            WeaponIndex = b.ReadInt32();
            

            for (int i = 0; i < animation.Length; i++)
            {   
                animation[i].name = b.ReadString();

                for (int j = 0; j < animation[i].getKeyFrameArray().Length; j++)
                {
                    KeyFrame keyframe = animation[i].GetKeyFrame(j);
                    keyframe.FrameRef = b.ReadInt32();
                    keyframe.Duration = b.ReadInt32();

                    ScriptLine[] script = keyframe.GetScriptArray();
                    for (int s = 0; s < script.Length; s++)
                        script[s] = new ScriptLine(b.ReadString());
                }
            }

            for (int i = 0; i < frame.Length; i++)
            {   
                frame[i].Name = b.ReadString();

                for (int j = 0; j < frame[i].GetPartArray().Length; j++)
                {
                    Part p = frame[i].GetPart(j);
                    p.Index = b.ReadInt32();
                    p.Location.X = b.ReadSingle();
                    p.Location.Y = b.ReadSingle();
                    p.Rotation = b.ReadSingle();
                    p.Scaling.X = b.ReadSingle();
                    p.Scaling.Y = b.ReadSingle();
                    p.Flip = b.ReadInt32();
                }
            }

            b.Close();

            Console.WriteLine("Loaded.");
        }
    }
}
