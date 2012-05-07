using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZombieSmashers.map;
using ZombieSmashers.quake;
using ZombieSmashers.audio;
using Microsoft.Xna.Framework.Net;

namespace ZombieSmashers.Particles
{
    /// <summary>
    /// ParticleManager manages all of our game particles, such as smoke, fire,
    /// missiles, melee attacks, etc.
    /// 
    /// ParticleManager is covered in chapter 6.
    /// </summary>
    public class ParticleManager
    {
        Particle[] particle = new Particle[1024];
        SpriteBatch sprite;

        

        public void Reset()
        {
            for (int i = 0; i < particle.Length; i++)
                particle[i] = null;
        }

        /// <summary>
        /// Make a bullet with muzzle flash.
        /// </summary>
        /// <param name="loc">Location to create the bullet from.</param>
        /// <param name="traj">Bullet trajectory</param>
        /// <param name="face">Facing of owner</param>
        /// <param name="owner">Owner of bullet</param>
        public void MakeBullet(Vector2 loc, Vector2 traj, CharDir face, int owner)
        {

            if (face == CharDir.Left)
            {
                AddParticle(new Bullet(loc, new Vector2(-traj.X, traj.Y)
                    + Rand.GetRandomVector2(-90f, 90f, -90f, 90f), 
                    owner));
                MakeMuzzleFlash(loc, new Vector2(-traj.X, traj.Y));
            }
            else
            {
                AddParticle(new Bullet(loc, traj
                    + Rand.GetRandomVector2(-90f, 90f, -90f, 90f), 
                    owner));
                MakeMuzzleFlash(loc, traj);
            }
        }

        /// <summary>
        /// Make muzzle flash.  A muzzle flash is comprised of heat haze, muzzleflash sprites,
        /// and smoke.
        /// </summary>
        /// <param name="loc">Location to make muzzle flash from</param>
        /// <param name="traj">Trajectory of bullet making muzzle flash</param>
        public void MakeMuzzleFlash(Vector2 loc, Vector2 traj)
        {
            for (int i = 0; i < 16; i++)
            {
                AddParticle(new MuzzleFlash(
                    loc + (traj * (float)i) * 0.001f +
                    Rand.GetRandomVector2(-5f, 5f, -5f, 5f), 
                    traj / 5f,
                    (20f - (float)i) * 0.06f));
            }
            for (int i = 0; i < 4; i++)
                AddParticle(new Smoke(
                    loc, Rand.GetRandomVector2(-30f, 30f, -100f, 0f),
                    0f, 0f, 0f, 0.25f,
                    Rand.GetRandomFloat(0.25f, 1.0f),
                    Rand.GetRandomInt(0, 4)));
            for (int i = 4; i < 12; i++)
                AddParticle(new Heat(
                    loc + (traj * (float)i) * 0.001f 
                    + Rand.GetRandomVector2(-30f, 30f, -30f, 30f), 
                    Rand.GetRandomVector2(-30f, 30f, -100f, 0f),
                    Rand.GetRandomFloat(.5f, 1.1f)));
        }

        /// <summary>
        /// Makes a good looking exit wound.
        /// </summary>
        /// <param name="loc">Location of bullet impact.</param>
        /// <param name="traj">Bullet trajectory.</param>
        public void MakeBulletBlood(Vector2 loc, Vector2 traj)
        {
            
            for (int t = 0; t < 32; t++)
                AddParticle(
                    new Blood(loc,
                    traj *
                    -1f * Rand.GetRandomFloat(0.01f, 0.2f) +
                    Rand.GetRandomVector2(-50f, 50f, -50f, 50f),
                    1f, 0f, 0f, 1f,
                    Rand.GetRandomFloat(0.1f, 0.3f),
                    Rand.GetRandomInt(0, 4)));
        
        }

        /// <summary>
        /// Make wraith missle explosion, using smoke, fire, and a shockwave refraction effect.
        /// </summary>
        /// <param name="loc">Explosion location</param>
        /// <param name="mag">Explosion magnitude--affects particle size</param>
        public void MakeExplosion(Vector2 loc, float mag)
        {
            for (int i = 0; i < 8; i++)
                AddParticle(new Smoke(loc,
                    Rand.GetRandomVector2(-100f, 100f,
                    -100f, 100f),
                    1f, .8f, .6f, 1f, 
                    Rand.GetRandomFloat(1f, 1.5f),
                    Rand.GetRandomInt(0, 4)));
            for (int i = 0; i < 8; i++)
                AddParticle(new Fire(loc,
                    Rand.GetRandomVector2(-80f, 80f, -80f, 80f),
                    1f, Rand.GetRandomInt(0, 4)));

            AddParticle(new Shockwave(loc, true, 25f));
            AddParticle(new Shockwave(loc, false, 10f));
            Sound.PlayCue("explode");
            QuakeManager.SetQuake(.5f);
            QuakeManager.SetBlast(1f, loc);
        }

        /// <summary>
        /// Make dust to match bullet exit wound.
        /// </summary>
        /// <param name="loc">Location of impact</param>
        /// <param name="traj">Bullet trajectory</param>
        public void MakeBulletDust(Vector2 loc, Vector2 traj)
        {
            for (int i = 0; i < 16; i++)
            {
                AddParticle(new Smoke(loc,
                    Rand.GetRandomVector2(-50f, 50f, -50f, 10f)
                    - traj * Rand.GetRandomFloat(0.001f, 0.1f),
                    1f, 1f, 1f, 0.25f,
                    Rand.GetRandomFloat(0.05f, 0.25f),
                    Rand.GetRandomInt(0, 4)));

                AddParticle(new Smoke(loc,
                    Rand.GetRandomVector2(-50f, 50f, -50f, 10f),
                    0.5f, 0.5f, 0.5f, 0.25f,
                    Rand.GetRandomFloat(0.1f, 0.5f),
                    Rand.GetRandomInt(0, 4)));
            }
        }

        /// <summary>
        /// Make blood splash that splatters in a direction.
        /// </summary>
        /// <param name="loc">Location of blood impact</param>
        /// <param name="traj">Blood splatter direction</param>
        public void MakeBloodSplash(Vector2 loc, Vector2 traj)
        {
            traj += Rand.GetRandomVector2(-100f, 100f, -100f, 100f);

            for (int i = 0; i < 64; i++)
            {
                AddParticle(new Blood(loc, traj *
                    Rand.GetRandomFloat(0.1f, 3.5f)
                    +
                    Rand.GetRandomVector2(-70f, 70f, -70f, 70f),
                    1f, 0f, 0f, 1f,
                    Rand.GetRandomFloat(0.01f, 0.25f),
                    Rand.GetRandomInt(0, 4)));

                AddParticle(new Blood(loc, traj *
                    Rand.GetRandomFloat(-0.2f, 0f)
                    +
                    Rand.GetRandomVector2(-120f, 120f, -120f, 120f),
                    1f, 0f, 0f, 1f,
                    Rand.GetRandomFloat(0.01f, 0.25f),
                    Rand.GetRandomInt(0, 4)));
            }
            MakeBulletDust(loc, traj * -20f);
            MakeBulletDust(loc, traj * 10f);
        }


        public ParticleManager(SpriteBatch sprite)
        {
            this.sprite = sprite;
        }

        /// <summary>
        /// Create new particle.
        /// </summary>
        /// <param name="newParticle">Particle to create</param>
        public void AddParticle(Particle newParticle)
        {
            AddParticle(newParticle, false);
        }

        /// <summary>
        /// Create new particle.
        /// </summary>
        /// <param name="newParticle">Particle to create</param>
        /// <param name="background">Specifies whether the particle is drawn behind characters or in front of them</param>
        public void AddParticle(Particle newParticle, bool background)
        {
            AddParticle(newParticle, background, false);
        }

        /// <summary>
        /// Creates new particle.
        /// </summary>
        /// <param name="newParticle">Particle to create</param>
        /// <param name="background">Specifies whether the particle is drawn behind characters or in front of them</param>
        /// <param name="netSent">Tells if particle has been sent over the network.  If it hasn't, it will be flagged to be sent over the network at the next send.</param>
        public void AddParticle(Particle newParticle, bool background, bool netSent)
        {
            for (int i = 0; i < particle.Length; i++)
            {
                if (particle[i] == null)
                {
                    particle[i] = newParticle;
                    particle[i].Background = background;

                    if (!netSent)
                    {
                        if (Game1.NetPlay.Joined)
                        {
                            if (particle[i].Owner == 1)
                                particle[i].netSend = true;
                            else
                                particle[i] = null;
                        }
                        else if (Game1.NetPlay.Hosting)
                        {
                            if (particle[i].Owner != 1)
                                particle[i].netSend = true;
                            else
                                particle[i] = null;
                        }
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Update all active particles.
        /// </summary>
        /// <param name="frameTime">Time delta that has elpased since the last udpate</param>
        /// <param name="map">The game map</param>
        /// <param name="c">The characters array</param>
        public void UpdateParticles(float frameTime, Map map, Character[] c)
        {
            
            for (int i = 0; i < particle.Length; i++)
            {
                if (particle[i] != null)
                {
                    particle[i].Update(frameTime, map, this, c);
                    if (!particle[i].Exists)
                    {
                        particle[i] = null;
                    }
                }
            }
        }

        /// <summary>
        /// Write all sendable particles to the network packet writer.
        /// </summary>
        /// <param name="writer">PacketWriter to write to</param>
        public void NetWriteParticles(PacketWriter writer)
        {
            for (int i = 0; i < particle.Length; i++)
                if (particle[i] != null)
                {
                    if (particle[i].netSend)
                    {
                        particle[i].NetWrite(writer);
                        particle[i].netSend = false;
                    }
                }
        }


        /// <summary>
        /// Draw all active particles.
        /// </summary>
        /// <param name="spritesTex">Texture to use for particles</param>
        /// <param name="background">Specifies whether this is a background draw pass or foreground draw pass</param>
        public void DrawParticles(Texture2D spritesTex, bool background)
        {
            sprite.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            foreach (Particle p in particle)
            {
                if (p != null)
                {   
                    if (!p.Additive && p.Background == background
                        && !p.refract)
                        p.Draw(sprite, spritesTex);
                }
            }
            sprite.End();

            sprite.Begin(SpriteSortMode.BackToFront, BlendState.Additive);
            foreach (Particle p in particle)
            {
                if (p != null)
                {
                    if (p.Additive && p.Background == background &&
                        !p.refract)
                        p.Draw(sprite, spritesTex);
                }
            }
            sprite.End();
        }

        /// <summary>
        /// Draw all active refract particles.  Refractive particles are covered in chapter 11.
        /// </summary>
        /// <param name="spritesTex">Texture to use for particles</param>
        public void DrawRefractParticles(Texture2D spritesTex)
        {
            sprite.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            foreach (Particle p in particle)
            {
                if (p != null)
                {
                    if (p.refract)
                        p.Draw(sprite, spritesTex);
                }
            }
            sprite.End();
        }
    }
}
