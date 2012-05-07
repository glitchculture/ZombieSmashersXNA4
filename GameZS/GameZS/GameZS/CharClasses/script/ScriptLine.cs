using System;
using System.Collections.Generic;
using System.Text;

namespace ZombieSmashers
{
    public class ScriptLine
    {
        Commands command;
        String sParam;
        int iParam;

        public ScriptLine(String line)
        {
            String[] split = line.Split(' ');
            try
            {
                switch (split[0])
                {
                    case "setanim":
                        command = Commands.SetAnim;
                        sParam = split[1];
                        break;
                    case "goto":
                        command = Commands.Goto;
                        iParam = Convert.ToInt32(split[1]);
                        break;
                    case "ifupgoto":
                        command = Commands.IfUpGoto;
                        iParam = Convert.ToInt32(split[1]);
                        break;
                    case "ifdowngoto":
                        command = Commands.IfDownGoto;
                        iParam = Convert.ToInt32(split[1]);
                        break;

                    case "float":
                        command = Commands.Float;
                        break;
                    case "unfloat":
                        command = Commands.UnFloat;
                        break;
                    case "slide":
                        command = Commands.Slide;
                        iParam = Convert.ToInt32(split[1]);
                        break;
                    case "backup":
                        command = Commands.Backup;
                        iParam = Convert.ToInt32(split[1]);
                        break;
                    case "setjump":
                        command = Commands.SetJump;
                        iParam = Convert.ToInt32(split[1]);
                        break;
                    case "joymove":
                        command = Commands.JoyMove;
                        break;
                    
                    case "clearkeys":
                        command = Commands.ClearKeys;
                        break;
                    case "setuppergoto":
                        command = Commands.SetUpperGoto;
                        iParam = Convert.ToInt32(split[1]);
                        break;
                    case "setlowergoto":
                        command = Commands.SetLowerGoto;
                        iParam = Convert.ToInt32(split[1]);
                        break;
                    case "setatkgoto":
                        command = Commands.SetAtkGoto;
                        iParam = Convert.ToInt32(split[1]);
                        break;
                    case "setanygoto":
                        command = Commands.SetAnyGoto;
                        iParam = Convert.ToInt32(split[1]);
                        break;
                    case "setsecgoto":
                        command = Commands.SetSecondaryGoto;
                        iParam = Convert.ToInt32(split[1]);
                        break;
                    case "setsecupgoto":
                        command = Commands.SetSecUpGoto;
                        iParam = Convert.ToInt32(split[1]);
                        break;
                    case "setsecdowngoto":
                        command = Commands.SetSecDownGoto;
                        iParam = Convert.ToInt32(split[1]);
                        break;
                    
                    case "cancancel":
                        command = Commands.CanCancel;
                        break;
                    case "play":
                        command = Commands.PlaySound;
                        sParam = split[1];
                        break;
                    case "ethereal":
                        command = Commands.Ethereal;
                        break;
                    case "solid":
                        command = Commands.Solid;
                        break;
                    case "speed":
                        command = Commands.Speed;
                        iParam = Convert.ToInt32(split[1]);
                        break;
                    case "hp":
                        command = Commands.HP;
                        iParam = Convert.ToInt32(split[1]);
                        break;
                    case "deathcheck":
                        command = Commands.DeathCheck;
                        break;
                    case "ifdyinggoto":
                        command = Commands.IfDyingGoto;
                        iParam = Convert.ToInt32(split[1]);
                        break;
                    case "killme":
                        command = Commands.KillMe;
                        break;
                    case "ai":
                        command = Commands.AI;
                        sParam = split[1];
                        break;
                    case "size":
                        command = Commands.Size;
                        iParam = Convert.ToInt32(split[1]);
                        break;
                    case "nolifty":
                        command = Commands.NoLifty;
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            
        }

        public Commands GetCommand()
        {
            return command;
        }

        public int GetIParam()
        {
            return iParam;
        }

        public String GetSParam()
        {
            return sParam;
        }
    }
}
