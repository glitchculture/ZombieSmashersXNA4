using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Net;
using ZombieSmashers.Particles;

namespace ZombieSmashers.net
{
    /// <summary>
    /// Manages all things network-related.
    /// 
    /// We'll be using this in chapter 12.
    /// </summary>
    public class NetPlay
    {
        public NetConnect NetConnect;
        public NetGame NetGame;

        public NetworkSession NetSession;

        public bool Hosting = false;
        public bool Joined = false;

        public NetPlay()
        {
            NetConnect = new NetConnect(this);
            NetGame = new NetGame(this);
        }

        public void Update(Character[] c, ParticleManager pMan)
        {
            if (NetSession != null)
                if (!NetSession.IsDisposed)
                    NetSession.Update();

            NetConnect.Update();

            if (NetSession != null)
            {
                if (NetSession.SessionState == NetworkSessionState.Playing)
                {
                    NetGame.Update(c, pMan);
                }
            }
        }
    }
}
