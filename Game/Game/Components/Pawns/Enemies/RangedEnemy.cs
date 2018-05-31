using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PBLGame.SceneGraph;

namespace Game.Components.Pawns.Enemies
{
    class MeleeEnemy : Enemy
    {
        public MeleeEnemy(GameObject parent) : base(parent)
        {
            parentGameObject = parent;
            enemySpeed = 0.05f;
            wakeUpDistance = 300f;
            range = 300f;
        }

        protected override void EnemyBehaviour()
        {
            base.EnemyBehaviour();
            if (distance < wakeUpDistance && heightDifference < 5.0f)
            {
                if (distance < 30f )
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
