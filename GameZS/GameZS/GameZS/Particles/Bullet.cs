using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZombieSmashers.net;
using Microsoft.Xna.Framework.Net;

namespace ZombieSmashers.Particles
{
    class Bullet : Particle
    {
        public Bullet(Vector2 loc,
            Vector2 traj,
            int owner)
        {
            
            this.Location = loc;
            this.Trajectory = traj;
            this.owner = owner;
            this.rotation = GlobalFunctions.GetAngle(new Vector2(),
                traj);
            this.Exists = true;
            this.frame = 0.5f;

            this.additive = true;
        }

        public Bullet(PacketReader reader)
        {
            this.Location =
                new Vector2(
                NetPacker.ShortToBigFloat(reader.ReadInt16()),
                NetPacker.ShortToBigFloat(reader.ReadInt16()));

            this.Trajectory =
                new Vector2(
                NetPacker.ShortToBigFloat(reader.ReadInt16()),
                NetPacker.ShortToBigFloat(reader.ReadInt16()));

            this.owner = NetPacker.ShortToInt(reader.ReadInt16());

            this.rotation = GlobalFunctions.GetAngle(new Vector2(),
                Trajectory);
            this.Exists = true;
            this.frame = 0.5f;

            this.additive = true;
        }

        public override void NetWrite(PacketWriter writer)
        {
            writer.Write(NetGame.MSG_PARTICLE);
            writer.Write(Particle.PARTICLE_BULLET);
            writer.Write(Background);
            writer.Write(NetPacker.BigFloatToShort(Location.X));
            writer.Write(NetPacker.BigFloatToShort(Location.Y));

            writer.Write(NetPacker.BigFloatToShort(Trajectory.X));
            writer.Write(NetPacker.BigFloatToShort(Trajectory.Y));

            writer.Write(NetPacker.IntToShort(owner));
            
        }

        public override void Update(float gameTime, 
            ZombieSmashers.map.Map map,
            ParticleManager pMan,
            Character[] c)
        {
            if (HitManager.CheckHit(this, c, pMan))
                frame = 0f;
 
            if (map.CheckParticleCol(Location))
            {
                this.frame = 0f;
                pMan.MakeBulletDust(Location, Trajectory);
            }
            base.Update(gameTime, map, pMan, c);
        }

        public override void Draw(SpriteBatch sprite, Texture2D spritesTex)
        {

            sprite.Draw(spritesTex, GameLocation,
                new Rectangle(0, 128, 64, 64), 
                new Color(
                new Vector4(1f, 0.8f, 0.6f, 0.2f)
                ),
                rotation, new Vector2(32.0f, 32.0f), 
                new Vector2(1f, 0.1f), 
                SpriteEffects.None, 1.0f);
            
        }
    }
}
