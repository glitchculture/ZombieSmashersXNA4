using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZombieSmashers.map;
using ZombieSmashers.net;
using Microsoft.Xna.Framework.Net;

namespace ZombieSmashers.Particles
{
    class Smoke : Particle
    {
        public Smoke(Vector2 loc,
            Vector2 traj,
            float r,
            float g,
            float b,
            float a,
            float size,
            int icon)
        {
            this.Location = loc;
            this.Trajectory = traj;
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
            this.size = size;
            this.flag = icon;
            this.owner = -1;
            this.Exists = true;
            this.frame = 1.0f;
        }

        public override void Update(float gameTime,
            Map map, ParticleManager pMan,
            Character[] c)
        {
            if (frame < 0.5f)
            {
                if (Trajectory.Y < -10.0f) Trajectory.Y += gameTime * 500.0f;
                if (Trajectory.X < -10.0f) Trajectory.X += gameTime * 150.0f;
                if (Trajectory.X > 10.0f) Trajectory.X -= gameTime * 150.0f;
            }
            base.Update(gameTime, map, pMan, c);
        }

        public Smoke(PacketReader reader)
        {
            this.Location =
                new Vector2(
                NetPacker.ShortToBigFloat(reader.ReadInt16()),
                NetPacker.ShortToBigFloat(reader.ReadInt16()));

            this.Trajectory =
                new Vector2(
                NetPacker.ShortToBigFloat(reader.ReadInt16()),
                NetPacker.ShortToBigFloat(reader.ReadInt16()));

            this.r = NetPacker.ByteToTinyFloat(reader.ReadByte());
            this.g = NetPacker.ByteToTinyFloat(reader.ReadByte());
            this.b = NetPacker.ByteToTinyFloat(reader.ReadByte());
            this.a = NetPacker.ByteToTinyFloat(reader.ReadByte());

            this.size = NetPacker.ShortToSmallFloat(reader.ReadInt16());
            this.flag = NetPacker.SbyteToInt(reader.ReadSByte());

            this.owner = -1;
            this.Exists = true;
            this.frame = 1.0f;
        }

        public override void NetWrite(PacketWriter writer)
        {
            writer.Write(NetGame.MSG_PARTICLE);
            writer.Write(Particle.PARTICLE_SMOKE);
            writer.Write(Background);
            writer.Write(NetPacker.BigFloatToShort(Location.X));
            writer.Write(NetPacker.BigFloatToShort(Location.Y));

            writer.Write(NetPacker.BigFloatToShort(Trajectory.X));
            writer.Write(NetPacker.BigFloatToShort(Trajectory.Y));

            writer.Write(NetPacker.TinyFloatToByte(r));
            writer.Write(NetPacker.TinyFloatToByte(g));
            writer.Write(NetPacker.TinyFloatToByte(b));
            writer.Write(NetPacker.TinyFloatToByte(a));

            writer.Write(NetPacker.SmallFloatToShort(size));
            writer.Write(NetPacker.IntToSbyte(flag));
        }

        public override void Draw(SpriteBatch sprite, Texture2D spritesTex)
        {
            
            Rectangle sRect = new Rectangle(flag * 64, 0, 64, 64);

            float frameAlpha;
            if (frame > 0.9f) frameAlpha = (1.0f - frame) * 10.0f;
            else
                frameAlpha = (frame / 0.9f);


            sprite.Draw(spritesTex, GameLocation, sRect, new Color(
                new Vector4(frame * r, frame * g, frame * b, a * frameAlpha)
                ),
                rotation, new Vector2(32.0f, 32.0f), size + (1.0f - frame), SpriteEffects.None, 1.0f);

            
        }
    }
}
