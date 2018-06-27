using System.Diagnostics;
using Game.Components.Collisions;
using Microsoft.Xna.Framework;
using PBLGame;
using PBLGame.MainGame;
using PBLGame.Misc.Anim;
using PBLGame.SceneGraph;

namespace Game.Components.Pawns.Enemies
{
    class BossEnemy : Enemy
    {
        private int tutorialHits = 0;
        private GameObject enemyHat;
        private GameObject enemyLeg;
        private bool finalFight = GameServices.GetService<ShroomGame>().levelOneCompleted;
        public BossEnemy(GameObject parent) : base(parent)
        {
            if (finalFight)
            {
                Hp = 100;
            }
            else
            {
                Hp = 99999;
            }
            attackDelay = 3.0d;
            parentGameObject = parent;
            enemySpeed = 0.07f;
            wakeUpDistance = 500f;
            range = 80f;
            enemyHat = parentGameObject.FindChildNodeByTag("Hat");
            enemyLeg = parentGameObject.FindChildNodeByTag("Leg");
        }

        public override void ReceiveHit()
        {
            if (!finalFight)
            {
                tutorialHits++;
                if (tutorialHits == 10)
                {
                    new DialogueString("King: Stop wasting your time on playing games with me");
                }
                if (tutorialHits == 20)
                {
                    new DialogueString("King: There is no more time to waste");
                    new DialogueString("King: You have to find Borowikus quickly!");
                }
                if (tutorialHits == 100)
                {
                    new DialogueString("King: You're unreal Beta-tester but you wont kill me");
                }
                if (tutorialHits == 200)
                {
                    new DialogueString("King: Game would have been already over by this time");
                }
            }

            base.ReceiveHit();
            enemyHat.GetComponent<AnimationManager>().PlayAnimation("gotHit", true);
            enemyLeg.GetComponent<AnimationManager>().PlayAnimation("gotHit", true);
            Debug.WriteLine("hp = " + Hp);
        }

        public override void Update(GameTime time)
        {
            base.Update(time);
            if (!isDead)
            {
                if (!GameServices.GetService<GameObject>().GetComponent<Player>().timeStop)
                {
                    enemyHat.GetComponent<AnimationManager>().SetDefaultAnimation("idle");
                    enemyLeg.GetComponent<AnimationManager>().SetDefaultAnimation("idle");
                }
            }
        }

        protected override void Attack()
        {
            base.Attack();
        }

        private void MeleeAttack()
        {
            lastAttack = 0.0d;
            isAttacking = true;
            enemyHat.GetComponent<HitTrigger>().ClearBoxList();
            if (enemyHat.GetComponent<AnimationManager>().isReady)
            {
                enemyHat.GetComponent<AnimationManager>().PlayAnimation("attack");
                enemyLeg.GetComponent<AnimationManager>().PlayAnimation("attack");
            }
            Debug.WriteLine("MELEEATAK");
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
                        MeleeAttack();
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
            if (enemyHat.GetComponent<AnimationManager>().defaultKey != "walk")
            {
                enemyHat.GetComponent<AnimationManager>().SetDefaultAnimation("walk");
                enemyLeg.GetComponent<AnimationManager>().SetDefaultAnimation("walk");
            }
            Move(new Vector3(0f, 0f, enemySpeed));
        }
    }
}