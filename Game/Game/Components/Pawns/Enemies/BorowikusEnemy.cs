using Microsoft.Xna.Framework;
using PBLGame.MainGame;
using PBLGame.SceneGraph;

namespace Game.Components.Pawns.Enemies
{
    class BorowikusEnemy : Enemy
    {
        public BorowikusEnemy(GameObject parent) : base(parent)
        {
            ObjectSide = Side.Enemy;
        }

        public override void Dispose()
        {
            base.Dispose();
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