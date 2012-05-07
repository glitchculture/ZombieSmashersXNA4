using System;
using System.Collections.Generic;
using System.Text;
using ZombieSmashers.map;

namespace ZombieSmashers.ai
{
    class Wraith : AI
    {
        public override void Update(Character[] c, int ID, Map map)
        {
            me = c[ID];

            if (jobFrame < 0f)
            {
                float r = Rand.GetRandomFloat(0f, 1f);
                if (r < 0.6f)
                {
                    job = JOB_MELEE_CHASE;
                    jobFrame = Rand.GetRandomFloat(2f, 4f);
                    FindTarg(c);
                }
                else
                {
                    job = JOB_SHOOT_CHASE;
                    jobFrame = Rand.GetRandomFloat(1f, 2f);
                    FindTarg(c);
                }
                
            }

            base.Update(c, ID, map);
        }
    }
}
