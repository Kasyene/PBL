using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PBLGame.SceneGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBLGame.Components
{
    public struct ParticleData
    {
        public float BirthTime;
        public float MaxAge;
        public Vector3 OrginalPosition;
        public Vector3 Accelaration;
        public Vector3 Direction;
        public Vector3 Position;
        public float Scaling;
        public Color ModColor;
    }

    class ParticleSystem : Component
    {
        protected GameObject parentGameObject;
        Texture2D particleTexture;
        List<ParticleData> particleList = new List<ParticleData>();

        public ParticleSystem(GameObject parent, Texture2D texture, int numberOfParticles, float size, float maxAge, GameTime gameTime)
        {
            parentGameObject = parent;
            particleTexture = texture;
            for (int i = 0; i < numberOfParticles; i++)
            {
                AddExplosionParticle(parent.Position, size, maxAge, gameTime);
            }
        }

        private void AddExplosionParticle(Vector3 explosionPos, float explosionRadius, float maxAge, GameTime gameTime)
        {
            ParticleData particle = new ParticleData();

            particle.OrginalPosition = explosionPos;
            particle.Position = particle.OrginalPosition;

            particle.BirthTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
            particle.MaxAge = maxAge;
            particle.Scaling = 0.25f;
            particle.ModColor = Color.White;
        }

    }
}
