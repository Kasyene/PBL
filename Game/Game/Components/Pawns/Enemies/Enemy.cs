using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Components.Audio;
using Game.Components.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PBLGame.MainGame;
using PBLGame.SceneGraph;

namespace Game.Components
{
    abstract class Enemy : Pawn
    {
        public float range;
        public float wakeUpDistance;
        public float distance;
        public float heightDifference;
        public float enemySpeed;
        public double attackDelay;
        public double lastAttack = 0.0d;
        public Model heartModel { get; set; }
        public Texture2D heartTex { get; set; }
        public Effect standardEffect { get; set; }
        protected AudioComponent audioComponent;

        protected Enemy(GameObject parent) : base()
        {
            parentGameObject = parent;
            parent.AddComponent(new AudioComponent(parent));
            audioComponent = parent.GetComponent<AudioComponent>();
        }

        // Use this for initialization
        protected void Start()
        {
            MaxHp = this.Hp;
            wakeUpDistance = 20;
        }
        public override void Update(GameTime time)
        {
            if (!isDead)
            {
                CheckIfDead();
                if (!GameServices.GetService<GameObject>().GetComponent<Player>().timeStop)
                {
                    audioComponent.Update(time);
                    base.Update(time);
                    lastAttack += (time.ElapsedGameTime.TotalMilliseconds / 1000.0d);
                    EnemyBehaviour();
                }
            }
        }

        protected virtual void EnemyBehaviour()
        {
            Vector3 playerPosition = GameServices.GetService<GameObject>().Position;
            LookAtTarget(playerPosition, parentGameObject.Position);
            distance = Vector3.Distance(playerPosition, this.parentGameObject.Position);
            heightDifference = System.Math.Abs(playerPosition.Y - this.parentGameObject.Position.Y);
        }

        protected void CheckIfDead()
        {
            if (this.Hp == 0)
            {
                RollForHpPickUp();
                Die();
            }
        }

        protected virtual void RollForHpPickUp()
        {
            Random rnd = new Random();
            int a = rnd.Next(0, 6);
            if (a == 0 || a == 3 || a == 5)
            {
                Debug.WriteLine("HP pickup spawn");
                SpawnHp();
            }
        }

        protected void SpawnHp()
        {
            GameObject heart = new GameObject("Serce");
            heart.AddComponent(new ModelComponent(heartModel, standardEffect, heartTex));
            heart.Rotation = this.parentGameObject.Rotation;
            heart.Position = new Vector3(this.parentGameObject.Position.X, this.parentGameObject.Position.Y + 6.0f, this.parentGameObject.Position.Z);
            heart.Scale = new Vector3(0.4f);
            this.parentGameObject?.Parent?.AddChildNode(heart);
            heart.CreateColliders();
            heart.SetAsTrigger(new HeartConsumableTrigger(heart));
        }

        protected virtual void Movement()
        {
            // to override
        }

        protected virtual void Attack()
        {
            // to override
        }

        protected virtual void Die()
        {
            isDead = true;
            audioComponent?.PlaySound("roblox");
            parentGameObject.Dispose();
        }

        public override void ReceiveHit()
        {
            base.ReceiveHit();
            audioComponent?.PlaySound("hit");
        }
    }
}
