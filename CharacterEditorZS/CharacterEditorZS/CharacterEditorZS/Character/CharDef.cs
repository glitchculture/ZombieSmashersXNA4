using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace xCharEdit.Character
{
    class CharDef
    {
        Animation[] animation;
        Frame[] frame;
        public String path;

        public int headIdx;
        public int torsoIdx;
        public int legsIdx;
        public int weaponIdx;

        public CharDef()
        {
            animation = new Animation[64];
            for (int i = 0; i < animation.Length; i++)
                animation[i] = new Animation();
            frame = new Frame[512];
            for (int i = 0; i < frame.Length; i++)
                frame[i] = new Frame();

            path = "char";
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

        public void WriteBackup()
        {
            Write(@"data/" + path + ".zmx");
            //Write(@"../../../../ZombieSmashersXNA/data/chars/" + path + ".zmx");
        }

        public void Write()
        {
            Write(@"data/" + path + ".zmx");
        }

        public void Write(String writePath)
        {
            BinaryWriter b = new BinaryWriter(File.Open(writePath, FileMode.Create));

            b.Write(path);
            b.Write(headIdx);
            b.Write(torsoIdx);
            b.Write(legsIdx);
            b.Write(weaponIdx);

            for (int i = 0; i < animation.Length; i++)
            {
                b.Write(animation[i].name);

                for (int j = 0; j < animation[i].getKeyFrameArray().Length; j++)
                {
                    KeyFrame keyframe = animation[i].GetKeyFrame(j);
                    b.Write(keyframe.frameRef);
                    b.Write(keyframe.duration);
                    String[] script = keyframe.getScriptArray();
                    for (int s = 0; s < script.Length; s++)
                        b.Write(script[s]);
                }
            }

            for (int i = 0; i < frame.Length; i++)
            {
                b.Write(frame[i].name);

                for (int j = 0; j < frame[i].GetPartArray().Length; j++)
                {
                    Part p = frame[i].GetPart(j);
                    b.Write(p.idx);
                    b.Write(p.location.X);
                    b.Write(p.location.Y);
                    b.Write(p.rotation);
                    b.Write(p.scaling.X);
                    b.Write(p.scaling.Y);
                    b.Write(p.flip);
                }
            }

            b.Close();

            Console.WriteLine("Saved.");
        }

        public void Read()
        {
            BinaryReader b;
            try
            {
                b = new BinaryReader(File.Open(@"data/" + path + ".zmx", FileMode.Open, FileAccess.Read));
            }
            catch (Exception e)
            {
                return;
            }

            path = b.ReadString();
            headIdx = b.ReadInt32();
            torsoIdx = b.ReadInt32();
            legsIdx = b.ReadInt32();
            weaponIdx = b.ReadInt32();
            

            for (int i = 0; i < animation.Length; i++)
            {   
                animation[i].name = b.ReadString();

                for (int j = 0; j < animation[i].getKeyFrameArray().Length; j++)
                {
                    KeyFrame keyframe = animation[i].GetKeyFrame(j);
                    keyframe.frameRef = b.ReadInt32();
                    keyframe.duration = b.ReadInt32();

                    String[] script = keyframe.getScriptArray();
                    for (int s = 0; s < script.Length; s++)
                        script[s] = b.ReadString();
                }
            }

            for (int i = 0; i < frame.Length; i++)
            {   
                frame[i].name = b.ReadString();

                for (int j = 0; j < frame[i].GetPartArray().Length; j++)
                {
                    Part p = frame[i].GetPart(j);
                    p.idx = b.ReadInt32();
                    p.location.X = b.ReadSingle();
                    p.location.Y = b.ReadSingle();
                    p.rotation = b.ReadSingle();
                    p.scaling.X = b.ReadSingle();
                    p.scaling.Y = b.ReadSingle();
                    p.flip = b.ReadInt32();
                }
            }

            b.Close();

            Console.WriteLine("Loaded.");
        }
    }
}
