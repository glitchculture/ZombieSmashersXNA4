using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;
using ZombieSmashers.net;

namespace ZombieSmashers.Particles
{
    class Heat : Particle
    {
        public Heat(Vector2 loc,
            Vector2 traj,
            float size)
        {
            this.Location = loc;
            this.Trajectory = traj;
            this.size = size;
            this.flag = Rand.GetRandomInt(0, 4);
            this.owner = -1;
            this.Exists = true;
            this.rotation = Rand.GetRandomFloat(0f, 6.28f);
            this.frame = Rand.GetRandomFloat(.5f, .785f);
            this.refract = true;
        }

        public Heat(PacketReader reader)
        {
            this.Location =
                new Vector2(
                NetPacker.ShortToBigFloat(reader.ReadInt16()),
                NetPacker.ShortToBigFloat(reader.ReadInt16()));

            this.Trajectory =
                new Vector2(
                NetPacker.ShortToBigFloat(reader.ReadInt16()),
                NetPacker.ShortToBigFloat(reader.ReadInt16()));

            this.size = NetPacker.ShortToSmallFloat(reader.ReadInt16());

            this.flag = Rand.GetRandomInt(0, 4);
            this.owner = -1;
            this.Exists = true;
            this.rotation = Rand.GetRandomFloat(0f, 6.28f);
            this.frame = Rand.GetRandomFloat(.5f, .785f);
            this.refract = true;
        }

        public override void NetWrite(PacketWriter writer)
        {
            writer.Write(NetGame.MSG_PARTICLE);
            writer.Write(Particle.PARTICLE_HEAT);
            writer.Write(Background);
            writer.Write(NetPacker.BigFloatToShort(Location.X));
            writer.Write(NetPacker.BigFloatToShort(Location.Y));

            writer.Write(NetPacker.BigFloatToShort(Trajectory.X));
            writer.Write(NetPacker.BigFloatToShort(Trajectory.Y));

            writer.Write(NetPacker.SmallFloatToShort(size));

        }

        public override void Draw(SpriteBatch sprite, Texture2D spritesTex)
        {
            
            Rectangle sRect = new Rectangle(flag * 64, 64, 64, 64);

            a = (float)Math.Sin((double)frame * 4.0) * .1f;


            sprite.Draw(spritesTex, GameLocation, sRect, new Color(
                new Vector4(1f, 0f, 0f, a)),
                rotation + frame * 16f, new Vector2(32.0f, 32.0f), 
                size, 
                SpriteEffects.None, 1.0f);

            
        }
    }
}
