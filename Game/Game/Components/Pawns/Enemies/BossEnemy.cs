using System.Diagnostics;
using Game.Components.Collisions;
using Microsoft.Xna.Framework;
using PBLGame;
using PBLGame.MainGame;
using PBLGame.Misc.Anim;
using PBLGame.SceneGraph;
using Game.MainGame;
using Microsoft.Xna.Framework.Graphics;

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
                MaxHp = 100;
                Hp = 2;
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
                if (enemyHat.GetComponent<AnimationManager>().isCurrentAnimation("slash"))
                {
                    isAttacking = true;
                }
                else
                {
                    isAttacking = false;
                }

                if (isAttacking && previousAttackAnimationId != enemyHat.GetComponent<AnimationManager>().AnimID)
                {
                    previousAttackAnimationId = enemyHat.GetComponent<AnimationManager>().AnimID;
                    enemyHat.GetComponent<HitTrigger>().ClearBoxList();
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
            enemyHat.GetComponent<HitTrigger>().ClearBoxList();
            if (enemyHat.GetComponent<AnimationManager>().isReady)
            {
                enemyHat.GetComponent<AnimationManager>().PlayAnimation("slash");
                enemyLeg.GetComponent<AnimationManager>().PlayAnimation("slash");
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

        protected override void Die()
        {
            GameServices.GetService<ShroomGame>().bossFight = false;
            new Cutscene(GameServices.GetService<ShroomGame>().Content.Load<Texture2D>("Cutscene/3.1"), 5f, "Narrator: The hero rushed into a battle against the King.",
            "Narrator: A fierce fight broke out.",
            "Narrator: After many swings of their hats there was only one shroom standing.");
            new Cutscene(GameServices.GetService<ShroomGame>().Content.Load<Texture2D>("Cutscene/3.2"), 5f, "Borovikus: Thank you my friend. Praise the sun that you understood I am not the enemy of the Kingdom.",
            "Narrator: What next? The crown was laying between the two exhausted shroom knights.",
            "Narrator: It was glowing with a bright light. It was a sign that the Kingdom needed a new King.");
            new Cutscene(GameServices.GetService<ShroomGame>().Content.Load<Texture2D>("Cutscene/3.3"), 5f, "Crowd: The King is dead, long live the King!",
            "Narrator: But wait! Who was the new Monarch? Was it Borovikus or our hero?",
            "Narrator: This question will be left without an answer. It is a matter for another story.");
            GameServices.GetService<ShroomGame>().gameComplete = true;
            isDead = true;
        }
    }
}