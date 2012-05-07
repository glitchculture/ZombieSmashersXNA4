using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using ZombieSmashers.map;
using Microsoft.Xna.Framework.Net;
using ZombieSmashers.net;

namespace ZombieSmashers.Particles
{
    class Rocket : Particle
    {
        public Rocket(Vector2 loc, Vector2 traj, int owner)
        {
            this.Location = loc;
            this.Trajectory = traj;
            this.owner = owner;
            this.frame = 4f;
            this.Exists = true;
        }

        public Rocket(PacketReader reader)
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

            this.frame = 4f;
            this.Exists = true;
        }

        public override void NetWrite(PacketWriter writer)
        {
            writer.Write(NetGame.MSG_PARTICLE);
            writer.Write(Particle.PARTICLE_ROCKET);
            writer.Write(Background);
            writer.Write(NetPacker.BigFloatToShort(Location.X));
            writer.Write(NetPacker.BigFloatToShort(Location.Y));

            writer.Write(NetPacker.BigFloatToShort(Trajectory.X));
            writer.Write(NetPacker.BigFloatToShort(Trajectory.Y));

            writer.Write(NetPacker.IntToSbyte(owner));

        }

        public override void Update(float gameTime, Map map, ParticleManager pMan, Character[] c)
        {
            if (HitManager.CheckHit(this, c, pMan))
                frame = 0f;

            Trajectory.Y = (float)Math.Sin((double)frame * 13.0) * 150f;

            if (map.CheckParticleCol(Location))
            {
                this.frame = 0f;
                pMan.MakeExplosion(Location, 1f);
            }

            pMan.AddParticle(new Fire(Location, -Trajectory / 8f,
                .5f, Rand.GetRandomInt(0, 4)));
            pMan.AddParticle(new Smoke(Location, 
                Rand.GetRandomVector2(-20f, 20f, -50f, 10f)
                - Trajectory / 10f,
                1f, .8f, .6f, 1f, .5f,
                Rand.GetRandomInt(0, 4)));
            pMan.AddParticle(new Heat(Location,
                Rand.GetRandomVector2(-20f, 20f, -50f, -10f),
                Rand.GetRandomFloat(.5f, 2f)));

            base.Update(gameTime, map, pMan, c);

        }
    }
}
