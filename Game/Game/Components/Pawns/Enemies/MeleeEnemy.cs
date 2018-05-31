using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PBLGame.Misc.Anim;
using PBLGame.SceneGraph;

namespace Game.Components.Enemies
{
    class MeleeEnemy : Enemy
    {
        private GameObject enemyModel;
        public MeleeEnemy(GameObject parent) : base(parent)
        {
            parentGameObject = parent;
            enemySpeed = 0.05f;
            wakeUpDistance = 150f;
            range = 30f;
            enemyModel = parentGameObject.FindChildNodeByTag("meleeEnemy");
        }

        protected override void EnemyBehaviour()
        {
            base.EnemyBehaviour();
            if (distance < wakeUpDistance && heightDifference < 5.0f)
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
            if (enemyModel.GetComponent<AnimationManager>().defaultKey != "walk")
            {
                enemyModel.GetComponent<AnimationManager>().SetDefaultAnimation("walk");
            }
            MoveForward(enemySpeed);
        }

        protected override void Attack()
        {
            if (enemyModel.GetComponent<AnimationManager>().isReady)
            {
                enemyModel.GetComponent<AnimationManager>().PlayAnimation("attack");
            }
            Debug.WriteLine("ATAK MELEE");
        }
    }
}
