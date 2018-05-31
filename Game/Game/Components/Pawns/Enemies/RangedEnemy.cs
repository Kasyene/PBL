using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PBLGame.SceneGraph;

namespace Game.Components.Pawns.Enemies
{
    class RangedEnemy : Enemy
    {
        public RangedEnemy(GameObject parent) : base(parent)
        {
            parentGameObject = parent;
            enemySpeed = 0.07f;
            wakeUpDistance = 300f;
            range = 300f;
        }

        protected override void EnemyBehaviour()
        {
            base.EnemyBehaviour();
            if (distance < wakeUpDistance && heightDifference < 5.0f)
            {
                if (distance < 70f )
                {
                    Movement();
                    
                }
                else
                {
                    Attack();
                }
            }
        }

        protected override void Movement()
        {
            MoveBack(enemySpeed);
        }

        protected override void Attack()
        {
            Debug.WriteLine("ATAK RANGED");
        }
    }
}
