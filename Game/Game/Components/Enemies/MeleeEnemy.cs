using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PBLGame.SceneGraph;

namespace Game.Components.Enemies
{
    class MeleeEnemy : Enemy
    {
        public MeleeEnemy(GameObject parent) : base(parent)
        {
            parentGameObject = parent;
            enemySpeed = 0.05f;
        }

        protected override void EnemyBehaviour()
        {
            base.EnemyBehaviour();
            if (distance < wakeUpDistance && heightDifference < 3.0f)
            {
                if (distance < range)
                {
                    Attack();
                }
                else
                {
                    Movement();
                }
            }
        }

        protected override void Movement()
        {
            MoveForward(enemySpeed);
        }

        protected override void Attack()
        {
           Debug.WriteLine("ATAK MELEE");
        }
    }
}
