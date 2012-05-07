using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;
using ZombieSmashers.net;

namespace ZombieSmashers.Particles
{
    class Fire : Particle
    {
        public Fire(Vector2 loc,
            Vector2 traj,
            float size,
            int icon)
        {
            this.Location = loc;
            this.Trajectory = traj;
            this.size = size;
            this.flag = icon;
            this.Exists = true;
            this.frame = 0.5f;
            this.additive = true;
        }

        public Fire(Vector2 loc,
            Vector2 traj,
            float size,
            int icon,
            float frame)
        {
            this.Location = loc;
            this.Trajectory = traj;
            this.size = size;
            this.flag = icon;
            this.Exists = true;
            this.frame = frame;
            this.additive = true;
        }

        public Fire(PacketReader reader)
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
            this.flag = NetPacker.SbyteToInt(reader.ReadSByte());
            this.frame = NetPacker.ShortToSmallFloat(reader.ReadInt16());
            
            this.Exists = true;
            this.additive = true;
        }

        public override void NetWrite(PacketWriter writer)
        {
            writer.Write(NetGame.MSG_PARTICLE);
            writer.Write(Particle.PARTICLE_FIRE);
            writer.Write(Background);
            writer.Write(NetPacker.BigFloatToShort(Location.X));
            writer.Write(NetPacker.BigFloatToShort(Location.Y));

            writer.Write(NetPacker.BigFloatToShort(Trajectory.X));
            writer.Write(NetPacker.BigFloatToShort(Trajectory.Y));

            writer.Write(NetPacker.SmallFloatToShort(size));
            writer.Write(NetPacker.IntToSbyte(flag));
            writer.Write(NetPacker.SmallFloatToShort(frame));
        }
        
        public override void Draw(SpriteBatch sprite, Texture2D spritesTex)
        {
            if (frame > .5f)
                return;

            Rectangle sRect = new Rectangle(flag * 64, 64, 64, 64);
            float bright = frame * 5.0f;
            float tsize;

            if (frame > 0.4)
            {
                r = 1.0f;
                g = 1.0f;
                b = (frame - 0.4f) * 10.0f;
                if (frame > 0.45f)
                    tsize = (0.5f - frame) * size * 20.0f;
                else
                    tsize = size;
            }
            else if (frame > 0.3f)
            {
                r = 1.0f;
                g = (frame - 0.3f) * 10.0f;
                b = 0.0f;
                tsize = size;
            }
            else
            {
                r = frame * 3.3f;
                g = 0.0f;
                b = 0.0f;
                tsize = (frame / 0.3f) * size;
            }

            if (flag % 2 == 0)
                rotation = (frame * 7.0f + size * 20.0f);
            else
                rotation = (-frame * 11.0f + size * 20.0f);

            sprite.Draw(spritesTex, GameLocation, sRect, new Color(
                new Vector4(r, g, b, 1.0f)
                ),
                rotation, new Vector2(32.0f, 32.0f), tsize, 
                SpriteEffects.None, 1.0f);
            
        }
    }
}
