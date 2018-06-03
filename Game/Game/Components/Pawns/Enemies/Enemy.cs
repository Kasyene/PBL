using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using PBLGame.MainGame;
using PBLGame.SceneGraph;

namespace Game.Components
{
    abstract class Enemy : Pawn
    {
        public float range;
        public float wakeUpDistance;
        public float distance;
        public float heightDifference;
        public float enemySpeed;

        protected Enemy(GameObject parent) : base()
        {
            parentGameObject = parent;
        }

        // Use this for initialization
        protected void Start()
        {
            MaxHp = this.Hp;
            wakeUpDistance = 20;
        }
        public override void Update(GameTime time)
        {
            base.Update(time);
            CheckIfDead();
            EnemyBehaviour();
        }

        protected virtual void EnemyBehaviour()
        {
            Vector3 playerPosition = GameServices.GetService<GameObject>().Position;
            LookAtTarget(playerPosition, parentGameObject.Position);
            distance = Vector3.Distance(playerPosition, this.parentGameObject.Position);
            heightDifference = System.Math.Abs(playerPosition.Y - this.parentGameObject.Position.Y);
        }

        protected void CheckIfDead()
        {
            if (this.Hp == 0)
            {
                RollForHpPickUp();
                //TODO destroy this enemy
            }
        }

        protected void RollForHpPickUp()
        {
            Random rnd = new Random();
            int a = rnd.Next(0, 6);
            if (a == 0 || a == 5)
            {
                Debug.WriteLine("HP pickup spawn");
               //TODO spawn hp pickup
            }
        }

        protected virtual void Movement()
        {
            // to override
        }

        protected virtual void Attack()
        {
            // to override
        }

    }
}
