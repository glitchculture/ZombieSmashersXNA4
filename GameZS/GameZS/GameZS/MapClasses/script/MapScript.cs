using System;
using System.Collections.Generic;
using System.Text;
using ZombieSmashers.map.bucket;
using Microsoft.Xna.Framework;

namespace ZombieSmashers.map
{
    public enum MapCommands
    {
        Fog = 0,
        Monster,
        MakeBucket,
        AddBucket,
        IfNotBucketGoto,
        Wait,
        SetFlag,
        IfTrueGoto,
        IfFalseGoto,
        SetGlobalFlag,
        IfGlobalTrueGoto,
        IfGlobalFalseGoto,
        Stop,
        Tag,
        SetLeftExit,
        SetLeftEntrance,
        SetRightExit,
        SetRightEntrance,
        SetIntroEntrance,
        Water
    }

    public class MapScript
    {
        Map map;

        public MapScriptLine[] Lines;

        int curLine;
        float waiting;
        public bool IsReading;

        public MapFlags Flags;

        public MapScript(Map map)
        {
            this.map = map;
            Flags = new MapFlags(32);
            Lines = new MapScriptLine[128];
        }
        
        public void DoScript(Character[] c)
        {
            if (waiting > 0f)
            {
                waiting -= Game1.FrameTime;
            }
            else
            {
                bool done = false;
                while (!done)
                {
                    curLine++;
                    if (Lines[curLine] != null)
                    {
                        switch (Lines[curLine].Command)
                        {
                            case MapCommands.Fog:
                                map.Fog = Lines[curLine].IParam;
                                break;
                            case MapCommands.Water:
                                map.water = (float)Lines[curLine].IParam;
                                break;
                            case MapCommands.Monster:
                                for (int i = 0; i < c.Length; i++)
                                {
                                    if (c[i] == null)
                                    {
                                        c[i] = new Character(
                                            Lines[curLine].VParam,
                                            Game1.charDef[(int)GetMonsterFromString(Lines[curLine].SParam[1])],
                                            i, Character.TEAM_BAD_GUYS);
                                        if (Lines[curLine].SParam.Length > 4)
                                            c[i].Name = Lines[curLine].SParam[4];
                                        break;
                                    }
                                }
                                break;

                            case MapCommands.MakeBucket:
                                map.Bucket = new Bucket(Lines[curLine].IParam);
                                break;
                            case MapCommands.AddBucket:
                                map.Bucket.AddItem(Lines[curLine].VParam,
                                    GetMonsterFromString(Lines[curLine].SParam[1]));
                                break;
                            case MapCommands.IfNotBucketGoto:
                                if (map.Bucket.IsEmpty)
                                {
                                    GotoTag(Lines[curLine].SParam[1]);
                                }
                                break;

                            case MapCommands.Wait:
                                waiting = (float)Lines[curLine].IParam / 100f;
                                done = true;
                                break;

                            case MapCommands.SetFlag:
                                Flags.SetFlag(Lines[curLine].SParam[1]);
                                break;
                            case MapCommands.IfTrueGoto:
                                if (Flags.GetFlag(Lines[curLine].SParam[1]))
                                    GotoTag(Lines[curLine].SParam[2]);
                                break;
                            case MapCommands.IfFalseGoto:
                                if (!Flags.GetFlag(Lines[curLine].SParam[1]))
                                    GotoTag(Lines[curLine].SParam[2]);
                                break;

                            case MapCommands.SetGlobalFlag:
                                map.GlobalFlags.SetFlag(Lines[curLine].SParam[1]);
                                break;
                            case MapCommands.IfGlobalTrueGoto:
                                if (map.GlobalFlags.GetFlag(Lines[curLine].SParam[1]))
                                    GotoTag(Lines[curLine].SParam[2]);
                                break;
                            case MapCommands.IfGlobalFalseGoto:
                                if (!map.GlobalFlags.GetFlag(Lines[curLine].SParam[1]))
                                    GotoTag(Lines[curLine].SParam[2]);
                                break;

                            case MapCommands.Stop:
                                IsReading = false;
                                done = true;
                                break;
                            case MapCommands.Tag:
                                //
                                break;
                            case MapCommands.SetLeftExit:
                                map.TransitionDestination[(int)TransitionDirection.Left] =
                                    Lines[curLine].SParam[1];
                                break;
                            case MapCommands.SetRightExit:
                                map.TransitionDestination[(int)TransitionDirection.Right] =
                                    Lines[curLine].SParam[1];
                                break;
                            case MapCommands.SetLeftEntrance:
                                if (map.TransDir ==
                                    TransitionDirection.Right)
                                {
                                    c[0].Loc = Lines[curLine].VParam;
                                    c[0].Face = CharDir.Right;
                                    c[0].SetAnim("fly");
                                    c[0].State = CharState.Air;
                                    c[0].Trajectory = new Vector2(200f, 0f);
                                    map.TransDir = TransitionDirection.None;
                                }
                                break;
                            case MapCommands.SetRightEntrance:
                                if (map.TransDir ==
                                    TransitionDirection.Left)
                                {
                                    c[0].Loc = Lines[curLine].VParam;
                                    c[0].Face = CharDir.Left;
                                    c[0].SetAnim("fly");
                                    c[0].State = CharState.Air;
                                    c[0].Trajectory = new Vector2(-200f, 0f);
                                    map.TransDir = TransitionDirection.None;
                                }
                                break;
                            case MapCommands.SetIntroEntrance:
                                if (map.TransDir ==
                                    TransitionDirection.Intro)
                                {
                                    c[0].Loc = Lines[curLine].VParam;
                                    c[0].Face = CharDir.Right;
                                    c[0].SetAnim("fly");
                                    c[0].State = CharState.Air;
                                    c[0].Trajectory = new Vector2(0f, 0f);
                                    map.TransDir = TransitionDirection.None;
                                }
                                break;
                                
                        }
                    }
                }
            }
        }

        public bool GotoTag(String tag)
        {
            for (int i = 0; i < Lines.Length; i++)
            {
                if (Lines[i] != null)
                {
                    if (Lines[i].Command == MapCommands.Tag)
                    {
                        if (Lines[i].SParam[1] == tag)
                        {
                            curLine = i;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static CharacterDefinitions GetMonsterFromString(String m)
        {
            switch (m)
            {
                case "wraith":
                    return CharacterDefinitions.Wraith;
                case "zombie":
                    return CharacterDefinitions.Zombie;
                case "carlos":
                    return CharacterDefinitions.Carlos;
                
            }
            return CharacterDefinitions.Zombie;
        }
    }
}
