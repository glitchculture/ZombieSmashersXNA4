using System;
using System.Collections.Generic;
using System.Text;
using ZombieSmashers.audio;
using ZombieSmashers.ai;

namespace ZombieSmashers
{
    public class Script
    {
        Character character;

        public Script(Character _character)
        {
            character = _character;
        }

        public void DoScript(int animIdx, int keyFrameIdx)
        {
            CharDef charDef = character.GetCharDef();
            Animation animation = charDef.GetAnimation(animIdx);
            KeyFrame keyFrame = animation.GetKeyFrame(keyFrameIdx);

            bool done = false;

            for (int i = 0; i < keyFrame.GetScriptArray().Length; i++)
            {
                if (done)
                {
                    break;
                }
                else
                {
                    ScriptLine line = keyFrame.GetScript(i);
                    if (line != null)
                    {
                        switch (line.GetCommand())
                        {

                            case Commands.SetAnim:
                                character.SetAnim(line.GetSParam());
                                break;
                            case Commands.Goto:
                                character.SetFrame(line.GetIParam());
                                done = true;
                                break;
                            case Commands.IfUpGoto:
                                if (character.KeyUp)
                                {
                                    character.SetFrame(line.GetIParam());
                                    done = true;
                                }
                                break;
                            case Commands.IfDownGoto:
                                if (character.KeyDown)
                                {
                                    character.SetFrame(line.GetIParam());
                                    done = true;
                                }
                                break;

                            case Commands.Float:
                                character.Floating = true;
                                break;
                            case Commands.UnFloat:
                                character.Floating = false;
                                break;
                            case Commands.Slide:
                                character.Slide((float)line.GetIParam());
                                break;
                            case Commands.Backup:
                                character.Slide((float)-line.GetIParam());
                                break;
                            case Commands.SetJump:
                                character.SetJump((float)line.GetIParam());
                                break;
                            case Commands.JoyMove:
                                if (character.KeyLeft)
                                    character.Trajectory.X = -character.Speed;
                                if (character.KeyRight)
                                    character.Trajectory.X = character.Speed;
                                break;
                            
                            
                            case Commands.ClearKeys:
                                character.PressedKey = PressedKeys.None;
                                break;
                            case Commands.SetUpperGoto:
                                character.GotoGoal[(int)PressedKeys.Upper] =
                                    line.GetIParam();

                                break;
                            case Commands.SetLowerGoto:
                                character.GotoGoal[(int)PressedKeys.Lower] =
                                    line.GetIParam();

                                break;
                            case Commands.SetAtkGoto:
                                character.GotoGoal[(int)PressedKeys.Attack] =
                                    line.GetIParam();
                                
                                break;
                            case Commands.SetAnyGoto:
                                character.GotoGoal[(int)PressedKeys.Upper] =
                                    line.GetIParam();
                                character.GotoGoal[(int)PressedKeys.Lower] =
                                    line.GetIParam();
                                character.GotoGoal[(int)PressedKeys.Attack] =
                                    line.GetIParam();
                                
                                break;

                            case Commands.SetSecondaryGoto:
                                character.GotoGoal[(int)PressedKeys.Secondary] =
                                    line.GetIParam();
                                character.GotoGoal[(int)PressedKeys.SecUp] =
                                    line.GetIParam();
                                character.GotoGoal[(int)PressedKeys.SecDown] =
                                    line.GetIParam();
                                
                                break;
                            case Commands.SetSecUpGoto:
                                character.GotoGoal[(int)PressedKeys.SecUp] =
                                    line.GetIParam();
                                
                                break;
                            case Commands.SetSecDownGoto:
                                character.GotoGoal[(int)PressedKeys.SecDown] =
                                    line.GetIParam();

                                break;
                            case Commands.CanCancel:
                                character.CanCancel = true;
                                break;
                            case Commands.PlaySound:
                                Sound.PlayCue(line.GetSParam());
                                break;
                            case Commands.Ethereal:
                                character.ethereal = true;
                                break;
                            case Commands.Solid:
                                character.ethereal = false;
                                break;
                            case Commands.Speed:
                                character.Speed = (float)line.GetIParam();
                                break;
                            case Commands.HP:
                                character.HP = character.MHP = line.GetIParam();
                                break;
                            case Commands.DeathCheck:
                                if (character.HP < 0)
                                {
                                    character.KillMe();
                                }
                                break;
                            case Commands.IfDyingGoto:
                                if (character.HP < 0)
                                {
                                    character.SetFrame(line.GetIParam());
                                    done = true;
                                }
                                break;
                            case Commands.KillMe:
                                character.KillMe();
                                break;
                            case Commands.AI:
                                switch (line.GetSParam())
                                {
                                    case "zombie":
                                        character.Ai = new Zombie();
                                        break;
                                    case "wraith":
                                        character.Ai = new Wraith();
                                        break;
                                    case "carlos":
                                        character.Ai = new Carlos();
                                        break;
                                    default:
                                        character.Ai = new Zombie();
                                        break;
                                }
                                break;
                            case Commands.Size:
                                character.Scale = (float)(line.GetIParam()) / 200f;
                                break;
                            case Commands.NoLifty:
                                character.NoLifty = true;
                                break;
                        }
                    }
                }
            }
        }
    }
}
