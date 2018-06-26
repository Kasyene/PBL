using Microsoft.Xna.Framework;
using PBLGame.SceneGraph;

namespace Game.Components.Pawns.Enemies
{
    class BossEnemy : Enemy
    {
        public BossEnemy(GameObject parent) : base(parent)
        {
            this.Hp = 99999;
        }

        public override void ReceiveHit()
        {
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