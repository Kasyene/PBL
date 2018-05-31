using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PBLGame.Misc.Anim;
using PBLGame.SceneGraph;

namespace Game.Components.Pawns.Enemies
{
    class RangedEnemy : Enemy
    {
        private GameObject enemyHat;
        private GameObject enemyLeg;

        public RangedEnemy(GameObject parent) : base(parent)
        {
            parentGameObject = parent;
            enemySpeed = 0.07f;
            wakeUpDistance = 300f;
            range = 300f;
            enemyHat = parentGameObject.FindChildNodeByTag("Hat");
            enemyLeg = parentGameObject.FindChildNodeByTag("Leg");
        }

        protected override void EnemyBehaviour()
        {
            base.EnemyBehaviour();
            if (distance < wakeUpDistance && heightDifference < 5.0f)
            {
                if (distance < 100f )
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
            if (enemyHat.GetComponent<AnimationManager>().defaultKey != "walk")
            {
                enemyHat.GetComponent<AnimationManager>().SetDefaultAnimation("walk");
                enemyLeg.GetComponent<AnimationManager>().SetDefaultAnimation("walk");
            }
            MoveBack(enemySpeed);
        }

        protected override void Attack()
        {
            if (enemyHat.GetComponent<AnimationManager>().isReady)
            {
                enemyHat.GetComponent<AnimationManager>().PlayAnimation("gotHit");
                enemyLeg.GetComponent<AnimationManager>().PlayAnimation("gotHit");
            }
            Debug.WriteLine("ATAK RANGED");
        }
    }
}
