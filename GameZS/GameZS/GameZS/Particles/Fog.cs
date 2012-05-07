using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZombieSmashers.map;
using Microsoft.Xna.Framework.Net;
using ZombieSmashers.net;

namespace ZombieSmashers.Particles
{
    class Fog : Particle
    {
        public Fog(Vector2 loc)
        {
            this.Location = loc;
            this.Trajectory = new Vector2(80f, -30f);
            this.size = Rand.GetRandomFloat(6f, 8f);
            this.flag = Rand.GetRandomInt(0, 4);
            this.owner = -1;
            this.Exists = true;
            this.frame = (float)Math.PI * 2f;
            this.additive = true;
            this.rotation = Rand.GetRandomFloat(0f, 6.28f);
        }

        public Fog(PacketReader reader)
        {
            this.Location =
                new Vector2(
                NetPacker.ShortToBigFloat(reader.ReadInt16()),
                NetPacker.ShortToBigFloat(reader.ReadInt16()));

            this.Trajectory = new Vector2(80f, -30f);
            this.size = Rand.GetRandomFloat(6f, 8f);
            this.flag = Rand.GetRandomInt(0, 4);
            this.owner = -1;
            this.Exists = true;
            this.frame = (float)Math.PI * 2f;
            this.additive = true;
            this.rotation = Rand.GetRandomFloat(0f, 6.28f);
        }

        public override void NetWrite(PacketWriter writer)
        {
            writer.Write(NetGame.MSG_PARTICLE);
            writer.Write(Particle.PARTICLE_FOG);
            writer.Write(Background);
            writer.Write(NetPacker.BigFloatToShort(Location.X));
            writer.Write(NetPacker.BigFloatToShort(Location.Y));
        }

        public override void Draw(SpriteBatch sprite, Texture2D spritesTex)
        {
            sprite.Draw(spritesTex, GameLocation,
                new Rectangle(flag * 64, 0, 64, 64), 
                new Color(
                new Vector4(1f, 1f, 1f, (float)Math.Sin(frame / 2f) * .1f)
                ),
                rotation + frame / 4f, new Vector2(32.0f, 32.0f), size, SpriteEffects.None, 1.0f);
        }
    }
}
