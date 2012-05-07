using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace ZombieSmashers.map
{
    public class MapScriptLine
    {
        public MapCommands Command;
        public int IParam;
        public Vector2 VParam;
        public String[] SParam;

        public MapScriptLine(String line)
        {
            if (line.Length < 1)
                return;

            SParam = line.Split(' ');
            switch (SParam[0])
            {
                case "fog":
                    Command = MapCommands.Fog;
                    IParam = Convert.ToInt32(SParam[1]);
                    break;
                case "water":
                    Command = MapCommands.Water;
                    IParam = Convert.ToInt32(SParam[1]);
                    break;

                case "monster":
                    Command = MapCommands.Monster;
                    VParam = new Vector2(
                        Convert.ToSingle(SParam[2]),
                        Convert.ToSingle(SParam[3])
                        );
                    break;
                case "makebucket":
                    Command = MapCommands.MakeBucket;
                    IParam = Convert.ToInt32(SParam[1]);
                    break;
                case "addbucket":
                    Command = MapCommands.AddBucket;
                    VParam = new Vector2(Convert.ToSingle(SParam[2]),
                        Convert.ToSingle(SParam[3]));
                    break;
                case "ifnotbucketgoto":
                    Command = MapCommands.IfNotBucketGoto;
                    break;

                case "wait":
                    Command = MapCommands.Wait;
                    IParam = Convert.ToInt32(SParam[1]);
                    break;

                case "setflag":
                    Command = MapCommands.SetFlag;
                    break;
                case "iftruegoto":
                    Command = MapCommands.IfTrueGoto;
                    break;
                case "iffalsegoto":
                    Command = MapCommands.IfFalseGoto;
                    break;

                case "setglobalflag":
                    Command = MapCommands.SetGlobalFlag;
                    break;
                case "ifglobaltruegoto":
                    Command = MapCommands.IfGlobalTrueGoto;
                    break;
                case "ifglobalfalsegoto":
                    Command = MapCommands.IfGlobalFalseGoto;
                    break;

                case "stop":
                    Command = MapCommands.Stop;
                    break;
                case "tag":
                    Command = MapCommands.Tag;
                    break;

                case "setleftexit":
                    Command = MapCommands.SetLeftExit;
                    break;
                case "setleftentrance":
                    Command = MapCommands.SetLeftEntrance;
                    VParam = new Vector2(Convert.ToSingle(SParam[1]),
                        Convert.ToSingle(SParam[2]));
                    break;
                case "setrightexit":
                    Command = MapCommands.SetRightExit;
                    break;
                case "setrightentrance":
                    Command = MapCommands.SetRightEntrance;
                    VParam = new Vector2(Convert.ToSingle(SParam[1]),
                        Convert.ToSingle(SParam[2]));
                    break;
                case "setintroentrance":
                    Command = MapCommands.SetIntroEntrance;
                    VParam = new Vector2(Convert.ToSingle(SParam[1]),
                        Convert.ToSingle(SParam[2]));
                    break;
            }
        }
    }
}
