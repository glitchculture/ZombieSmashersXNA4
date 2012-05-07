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
    public class Particle
    {
        public const byte PARTICLE_NONE = 0;
        public const byte PARTICLE_BLOOD = 1;
        public const byte PARTICLE_BLOOD_DUST = 2;
        public const byte PARTICLE_BULLET = 3;
        public const byte PARTICLE_FIRE = 4;
        public const byte PARTICLE_FOG = 5;
        public const byte PARTICLE_HEAT = 6;
        public const byte PARTICLE_HIT = 7;
        public const byte PARTICLE_MUZZLEFLASH = 8;
        public const byte PARTICLE_ROCKET = 9;
        public const byte PARTICLE_SHOCKWAVE = 10;
        public const byte PARTICLE_SMOKE = 11;


        protected Vector2 Location;
        protected Vector2 Trajectory;

        protected float frame;
        protected float r, g, b, a;
        protected float size;
        protected float rotation;

        protected int flag;

        protected int owner;

        protected bool additive;

        public bool Exists;

        public bool Background;

        public bool refract;

        public bool netSendAndKill;
        public bool netSend;

        public Vector2 GetLoc()
        {
            return Location;
        }

        public Vector2 GetTraj()
        {
            return Trajectory;
        }

        public int GetFlag()
        {
            return flag;
        }

        public Vector2 GameLocation
        {
            get { return Location - Game1.Scroll; }
        }

        public bool Additive
        {
            get { return additive; }
            protected set { additive = value; }
        }

        public int Owner
        {
            get { return owner; }
            set { owner = value; }
        }

        public int Flag
        {
            get { return flag; }
        }

        public Particle()
        {
            Exists = false;
        }

        

        public virtual void Update(float gameTime, 
            Map map, 
            ParticleManager pMan,
            Character[] c)
        {
            Location += Trajectory * gameTime;
            frame -= gameTime;
            if (frame < 0.0f) KillMe();
        }

        public virtual void KillMe()
        {
            Exists = false;
        }

        public virtual void NetWrite(PacketWriter writer)
        {
            writer.Write(NetGame.MSG_PARTICLE);
            writer.Write(PARTICLE_NONE);
            writer.Write(Background);
        }

        public virtual void Draw(SpriteBatch sprite, Texture2D spritesTex)
        {
        }
    }
}
