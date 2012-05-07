using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;
using ZombieSmashers.net;

namespace ZombieSmashers.Particles
{
    class MuzzleFlash : Particle
    {
        public MuzzleFlash(Vector2 loc,
            Vector2 traj,
            float size)
        {
            this.Location = loc;
            this.Trajectory = traj;
            this.size = size;
            this.rotation = Rand.GetRandomFloat(0f, 6.28f);
            this.Exists = true;
            this.frame = 0.05f;
            this.additive = true;
        }

        public MuzzleFlash(PacketReader reader)
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

            this.rotation = Rand.GetRandomFloat(0f, 6.28f);
            this.Exists = true;
            this.frame = 0.05f;
            this.additive = true;
        }

        public override void NetWrite(PacketWriter writer)
        {
            writer.Write(NetGame.MSG_PARTICLE);
            writer.Write(Particle.PARTICLE_MUZZLEFLASH);
            writer.Write(Background);
            writer.Write(NetPacker.BigFloatToShort(Location.X));
            writer.Write(NetPacker.BigFloatToShort(Location.Y));

            writer.Write(NetPacker.BigFloatToShort(Trajectory.X));
            writer.Write(NetPacker.BigFloatToShort(Trajectory.Y));

            writer.Write(NetPacker.SmallFloatToShort(size));

        }
        
        public override void Draw(SpriteBatch sprite, Texture2D spritesTex)
        {

            sprite.Draw(spritesTex, GameLocation,
                new Rectangle(64, 128, 64, 64), 
                new Color(
                new Vector4(1f, 0.8f, 0.6f, frame * 8f)
                ),
                rotation, new Vector2(32.0f, 32.0f), 
                size - frame, 
                SpriteEffects.None, 1.0f);
            
        }
    }
}
