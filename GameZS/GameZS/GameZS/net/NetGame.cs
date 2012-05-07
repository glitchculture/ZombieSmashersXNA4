using System;
using System.Collections.Generic;
using System.Text;
using ZombieSmashers.Particles;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework;

namespace ZombieSmashers.net
{
    public class NetGame
    {
        NetPlay netPlay;

        public const byte MSG_SERVER_DATA = 0;
        public const byte MSG_CLIENT_DATA = 1;
        public const byte MSG_CHARACTER = 2;
        public const byte MSG_PARTICLE = 3;
        public const byte MSG_END = 4;

        PacketWriter writer;
        PacketReader reader;

        float frame;

        public float FrameTime;

        public NetGame(NetPlay _netPlay)
        {
            netPlay = _netPlay;

            writer = new PacketWriter();
            reader = new PacketReader();
        }

        public void Update(Character[] c, ParticleManager pMan)
        {
            LocalNetworkGamer gamer = GetGamer();
            if (gamer == null)
                return;

            frame -= FrameTime;
            if (frame < 0f)
            {
                frame = .05f;

                

                if (netPlay.Hosting)
                {
                    if (c[0] != null)
                    {
                        writer.Write(MSG_SERVER_DATA);

                        c[0].WriteToNet(writer);

                        for (int i = 2; i < c.Length; i++)
                            if (c[i] != null)
                                c[i].WriteToNet(writer);

                        pMan.NetWriteParticles(writer);

                        writer.Write(MSG_END);
                        gamer.SendData(writer, SendDataOptions.None);
                    }
                }
                if (netPlay.Joined)
                {
                    if (c[1] != null)
                    {
                        writer.Write(MSG_CLIENT_DATA);

                        c[1].WriteToNet(writer);

                        pMan.NetWriteParticles(writer);

                        writer.Write(MSG_END);
                        gamer.SendData(writer, SendDataOptions.None);
                    }
                    
                }
                
            }
            if (gamer.IsDataAvailable)
            {
                NetworkGamer sender;
                gamer.ReceiveData(reader, out sender);
                
                if (!sender.IsLocal)
                {
                    byte type = reader.ReadByte();

                    if (netPlay.Joined)
                    {
                        for (int i = 0; i < c.Length; i++)
                            if (i != 1)
                                if (c[i] != null)
                                    c[i].ReceivedNetUpdate = false;
                        
                    }

                    bool end = false;
                    while (!end)
                    {
                        byte msg = reader.ReadByte();
                        switch (msg)
                        {
                            case MSG_END:
                                end = true;
                                break;
                            case MSG_CHARACTER:
                                
                                int defID = NetPacker.SbyteToInt(reader.ReadSByte());
                                int team = NetPacker.SbyteToInt(reader.ReadSByte());
                                int ID = NetPacker.SbyteToInt(reader.ReadSByte());

                                if (c[ID] == null)
                                {   
                                    c[ID] = new Character(new Vector2(),
                                        Game1.charDef[defID],
                                        ID, team);
                                }

                                c[ID].ReadFromNet(reader);

                                c[ID].ReceivedNetUpdate = true;
                                break;
                            case MSG_PARTICLE:
                                byte pType = reader.ReadByte();
                                bool bg = reader.ReadBoolean();

                                switch (pType)
                                {
                                    case Particle.PARTICLE_NONE:
                                        //
                                        break;
                                    case Particle.PARTICLE_BLOOD:
                                        pMan.AddParticle(new Blood(reader), bg, true);
                                        break;
                                    case Particle.PARTICLE_BLOOD_DUST:
                                        pMan.AddParticle(new BloodDust(reader), bg, true);
                                        break;
                                    case Particle.PARTICLE_BULLET:
                                        pMan.AddParticle(new Bullet(reader), bg, true);
                                        break;
                                    case Particle.PARTICLE_FIRE:
                                        pMan.AddParticle(new Fire(reader), bg, true);
                                        break;
                                    case Particle.PARTICLE_FOG:
                                        pMan.AddParticle(new Fog(reader), bg, true);
                                        break;
                                    case Particle.PARTICLE_HEAT:
                                        pMan.AddParticle(new Heat(reader), bg, true);
                                        break;
                                    case Particle.PARTICLE_HIT:
                                        pMan.AddParticle(new Hit(reader), bg, true);
                                        break;
                                    case Particle.PARTICLE_MUZZLEFLASH:
                                        pMan.AddParticle(new MuzzleFlash(reader), bg, true);
                                        break;
                                    case Particle.PARTICLE_ROCKET:
                                        pMan.AddParticle(new Rocket(reader), bg, true);
                                        break;
                                    case Particle.PARTICLE_SHOCKWAVE:
                                        pMan.AddParticle(new Shockwave(reader), bg, true);
                                        break;
                                    case Particle.PARTICLE_SMOKE:
                                        pMan.AddParticle(new Smoke(reader), bg, true);
                                        break;
                                    default:
                                        //Error!
                                        break;
                                }
                                break;
                        }
                    }

                    if (netPlay.Joined)
                    {
                        for (int i = 0; i < c.Length; i++)
                            if (i != 1)
                                if (c[i] != null)
                                    if (c[i].ReceivedNetUpdate == false)
                                    {
                                        c[i] = null;
                                    }

                    }
                }
            }
            
        }

        private LocalNetworkGamer GetGamer()
        {
            foreach (LocalNetworkGamer gamer in netPlay.NetSession.LocalGamers)
                if (gamer.SignedInGamer.PlayerIndex == PlayerIndex.One)
                    return gamer;
            return null;
        }

        
    }
}
