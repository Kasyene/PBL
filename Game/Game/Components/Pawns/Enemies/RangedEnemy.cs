using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PBLGame.MainGame;
using PBLGame.Misc.Anim;
using PBLGame.SceneGraph;

namespace Game.Components.Pawns.Enemies
{
    class RangedEnemy : Enemy
    {
        private GameObject enemyHat;
        private GameObject enemyLeg;
        public List<GameObject> bullets;
        public Model bulletModel { get; set; }
        public Texture2D bulletEnemyTex { get; set; }
        public Texture2D bulletEnemyNormal { get; set; }


        public RangedEnemy(GameObject parent) : base(parent)
        {
            Hp = 8;
            attackDelay = 2.0d;
            parentGameObject = parent;
            enemySpeed = 0.07f;
            wakeUpDistance = 500f;
            range = 250f;
            enemyHat = parentGameObject.FindChildNodeByTag("Hat");
            enemyLeg = parentGameObject.FindChildNodeByTag("Leg");
            bullets = new List<GameObject>();
        }

        protected override void EnemyBehaviour()
        {
            base.EnemyBehaviour();
            if (distance < wakeUpDistance && heightDifference < 70.0f)
            {
                if (distance < 100f )
                {
                    Movement();
                    Move(new Vector3(0f, 0f, -enemySpeed));

                }
                else if (distance > range)
                {
                    Movement();
                    Move(new Vector3(0f, 0f, enemySpeed));
                }
                else
                {
                    if (lastAttack >= attackDelay)
                    {
                        Attack();
                    }
                }
            }
        }

        protected override void Movement()
        {
            if (enemyHat.GetComponent<AnimationManager>().defaultKey != "walk")
            {
                enemyHat.GetComponent<AnimationManager>().SetDefaultAnimation("walk");
                enemyLeg.GetComponent<AnimationManager>().SetDefaultAnimation("walk");
            }
        }

        public override void Update(GameTime time)
        {
            base.Update(time);
            if (!isDead)
            {
                if (!GameServices.GetService<GameObject>().GetComponent<Player>().timeStop)
                {
                    for (int i = bullets.Count - 1; i >= 0; i--)
                    {
                        if (bullets[i].Parent == null)
                        {
                            bullets.RemoveAt(i);
                        }
                        else
                        {
                            bullets[i].Update(time);
                        }
                    }
                }
                else
                {
                    enemyHat.GetComponent<AnimationManager>().SetDefaultAnimation("idle");
                    enemyLeg.GetComponent<AnimationManager>().SetDefaultAnimation("idle");
                }
                    
            }

        }

        protected override void Attack()
        {
            lastAttack = 0.0d;
            if (enemyHat.GetComponent<AnimationManager>().isReady)
            {
                enemyHat.GetComponent<AnimationManager>().PlayAnimation("gotHit");
                enemyLeg.GetComponent<AnimationManager>().PlayAnimation("gotHit");
            }
            SpawnBullet();
            Debug.WriteLine("ATAK RANGED" + Hp);
        }

        protected void SpawnBullet()
        {
            GameObject bullet = new GameObject("bullet");
            bullet.AddComponent(new ModelComponent(bulletModel, standardEffect, bulletEnemyTex, bulletEnemyNormal));
            bullet.Rotation = this.parentGameObject.Rotation;
            bullet.Position = new Vector3(this.parentGameObject.Position.X, this.parentGameObject.Position.Y + 55f, this.parentGameObject.Position.Z);
            this.parentGameObject?.Parent?.AddChildNode(bullet);
            bullet.CreateColliders();
            bullet.SetAsTrigger(new Bullet(bullet, this.parentGameObject));
            bullets.Add(bullet);
        }

        public override void Dispose()
        {
            bulletModel = null;
            standardEffect = null;
            bulletEnemyTex = null;
            bulletEnemyNormal = null;
            enemyHat.Dispose();
            enemyLeg.Dispose();
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                bullets[i].Dispose();
            }
            base.Dispose();
        }

        public override void ReceiveHit()
        {
            base.ReceiveHit();
            enemyHat.GetComponent<AnimationManager>().PlayAnimation("gotHit", true);
            enemyLeg.GetComponent<AnimationManager>().PlayAnimation("gotHit", true);
        }
    }
}
