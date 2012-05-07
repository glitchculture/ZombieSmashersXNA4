using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Net;

namespace ZombieSmashers.net
{
    public class NetConnect
    {
        public bool PendingHost = false;
        public bool PendingJoin = false;
        public bool PendingFind = false;

        IAsyncResult createResult;
        IAsyncResult findResult;
        IAsyncResult joinResult;

        NetPlay netPlay;

        public NetConnect(NetPlay _netPlay)
        {
            netPlay = _netPlay;
        }

        public void Disconnect()
        {
            if (netPlay.Hosting || netPlay.Joined)
            {
                netPlay.NetSession.Dispose();
                netPlay.Hosting = false;
                netPlay.Joined = false;
            }
        }

        public void Host()
        {
            if (netPlay.NetSession != null)
                netPlay.NetSession.Dispose();

            NetworkSessionProperties props = new NetworkSessionProperties();

            createResult = NetworkSession.BeginCreate(NetworkSessionType.SystemLink,
                1, 2, 0, props, new AsyncCallback(GotResult), null);
            PendingHost = true;
        }

        public void Find()
        {
            NetworkSessionProperties props = new NetworkSessionProperties();

            findResult = NetworkSession.BeginFind(NetworkSessionType.SystemLink,
                1, props, new AsyncCallback(GotResult), null);
            PendingFind = true;
        }

        public void Update()
        {
            

            if (PendingHost)
            {
                if (createResult.IsCompleted)
                {
                    netPlay.NetSession = NetworkSession.EndCreate(createResult);
                    netPlay.Hosting = true;
                    PendingHost = false;
                }
            }
            if (PendingFind)
            {
                if (findResult.IsCompleted)
                {
                    AvailableNetworkSessionCollection availableSessions =
                        NetworkSession.EndFind(findResult);
                    if (availableSessions.Count > 0)
                    {
                        joinResult = NetworkSession.BeginJoin(
                            availableSessions[0], new AsyncCallback(GotResult), null);
                        PendingJoin = true;
                     
                    }
                    PendingFind = false;
                }
                
            }
            if (PendingJoin)
            {
                if (joinResult.IsCompleted)
                {
                    netPlay.NetSession = NetworkSession.EndJoin(joinResult);
                    netPlay.Joined = true;
                    PendingJoin = false;
                }
            }
            
            if (netPlay.Hosting)
            {
                //
            }
            if (netPlay.Joined)
            {
                //
            }
        if (netPlay.NetSession != null)
        {
            if (!netPlay.NetSession.IsDisposed)
            {
                bool ended = false;
                if (netPlay.NetSession.SessionState == NetworkSessionState.Playing)
                {
                    if (netPlay.NetSession.AllGamers.Count < 2)
                        ended = true;
                    
                }
                else if (netPlay.NetSession.SessionState == NetworkSessionState.Ended)
                    ended = true;

                if (ended)
                {
                    Game1.Menu.EndGame();
                    netPlay.NetSession.Dispose();
                    netPlay.Hosting = false;
                    netPlay.Joined = false;
                }
            }
        }
    }

        public void NewGame()
        {
            if (netPlay.Hosting)
                netPlay.NetSession.StartGame();
        }

        private void GotResult(IAsyncResult result)
        {
            //
        }
    }
}
