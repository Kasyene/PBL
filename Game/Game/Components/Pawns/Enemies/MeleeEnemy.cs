using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Components.Collisions;
using Microsoft.Xna.Framework;
using PBLGame.MainGame;
using PBLGame.Misc.Anim;
using PBLGame.SceneGraph;

namespace Game.Components.Enemies
{
    class MeleeEnemy : Enemy
    {
        private GameObject enemyModel;
        public MeleeEnemy(GameObject parent) : base(parent)
        {
            attackDelay = 1.0d;
            parentGameObject = parent;
            enemySpeed = 0.05f;
            wakeUpDistance = 350f;
            range = 35f;
            enemyModel = parentGameObject.FindChildNodeByTag("meleeEnemy");
        }

        protected override void EnemyBehaviour()
        {
            base.EnemyBehaviour();
            if (distance < wakeUpDistance && heightDifference < 70.0f)
            {
                if (distance < range)
                {
                    if (lastAttack >= attackDelay)
                    {
                        Attack();
                    }
                }
                else
                {
                    Movement();
                }
            }
        }

        protected override void Movement()
        {
            if (enemyModel.GetComponent<AnimationManager>().defaultKey != "walk")
            {
                enemyModel.GetComponent<AnimationManager>().SetDefaultAnimation("walk");
            }
            Move(new Vector3(0f, 0f, enemySpeed));
        }

        public override void Update(GameTime time)
        {
            base.Update(time);
            if (!isDead)
            {
                if (GameServices.GetService<GameObject>().GetComponent<Player>().timeStop)
                {
                    enemyModel.GetComponent<AnimationManager>().SetDefaultAnimation("idle");
                }

            }

        }

        public override void ReceiveHit()
        {
            base.ReceiveHit();
            enemyModel.GetComponent<AnimationManager>().PlayAnimation("gotHit", true);
        }

        protected override void Attack()
        {
            lastAttack = 0.0d;
            isAttacking = true;
            enemyModel.GetComponent<HitTrigger>().ClearBoxList();
            if (enemyModel.GetComponent<AnimationManager>().isReady)
            {
                enemyModel.GetComponent<AnimationManager>().PlayAnimation("attack");
                audioComponent.PlaySound("attack5");
            }
            Debug.WriteLine("MELEEATAK");
        }
    }
}
