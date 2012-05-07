using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;
using ZombieSmashers.net;

namespace ZombieSmashers.Particles
{
    class Hit : Particle
    {
        public Hit(Vector2 loc,
            Vector2 traj,
            int owner,
            int flag)
        {
            
            this.Location = loc;
            this.Trajectory = traj;
            this.owner = owner;
            this.flag = flag;
            
            this.Exists = true;
            this.frame = 0.5f;
            
        }

        public Hit(PacketReader reader)
        {
            this.Location =
                new Vector2(
                NetPacker.ShortToBigFloat(reader.ReadInt16()),
                NetPacker.ShortToBigFloat(reader.ReadInt16()));

            this.Trajectory =
                new Vector2(
                NetPacker.ShortToBigFloat(reader.ReadInt16()),
                NetPacker.ShortToBigFloat(reader.ReadInt16()));

            this.owner = NetPacker.SbyteToInt(reader.ReadSByte());
            this.flag = NetPacker.SbyteToInt(reader.ReadSByte());

            this.Exists = true;
            this.frame = 0.5f;
        }

        public override void NetWrite(PacketWriter writer)
        {
            writer.Write(NetGame.MSG_PARTICLE);
            writer.Write(Particle.PARTICLE_HIT);
            writer.Write(Background);
            writer.Write(NetPacker.BigFloatToShort(Location.X));
            writer.Write(NetPacker.BigFloatToShort(Location.Y));

            writer.Write(NetPacker.BigFloatToShort(Trajectory.X));
            writer.Write(NetPacker.BigFloatToShort(Trajectory.Y));

            writer.Write(NetPacker.IntToSbyte(owner));
            writer.Write(NetPacker.IntToSbyte(flag));
        }

        public override void Update(float gameTime, 
            ZombieSmashers.map.Map map,
            ParticleManager pMan,
            Character[] c)
        {
            if (!netSend)
            {
                HitManager.CheckHit(this, c, pMan);

                KillMe();
            }
        }

        public override void Draw(SpriteBatch sprite, Texture2D spritesTex)
        {   
            //
        }
    }
}
