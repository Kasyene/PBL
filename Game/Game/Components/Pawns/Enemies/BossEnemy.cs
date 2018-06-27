using Microsoft.Xna.Framework;
using PBLGame;
using PBLGame.MainGame;
using PBLGame.SceneGraph;

namespace Game.Components.Pawns.Enemies
{
    class BossEnemy : Enemy
    {
        private int tutorialHits = 0;
        private GameObject enemyHat;
        private GameObject enemyLeg;
        public BossEnemy(GameObject parent) : base(parent)
        {
            this.Hp = 99999;
            enemyHat = parentGameObject.FindChildNodeByTag("Hat");
            enemyLeg = parentGameObject.FindChildNodeByTag("Leg");
        }

        public override void ReceiveHit()
        {
            if (!GameServices.GetService<ShroomGame>().levelOneCompleted)
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
        }

        public override void Update(GameTime time)
        {
            base.Update(time);
        }

        protected override void Attack()
        {
            base.Attack();
        }

        protected override void EnemyBehaviour()
        {
            base.EnemyBehaviour();
        }

        protected override void Movement()
        {
            base.Movement();
        }
    }
}