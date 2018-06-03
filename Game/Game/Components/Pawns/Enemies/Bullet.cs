using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Components.Collisions;
using Microsoft.Xna.Framework;
using PBLGame.MainGame;
using PBLGame.SceneGraph;

namespace Game.Components.Pawns.Enemies
{
    class Bullet : ConsumableTrigger
    {
        private int dmg;
        private Vector2 direction;
        private float bulletSpeed = 0.09f;
        private Vector3 basePosition;
        private GameObject rangedEnemyOwner;

        public Bullet(GameObject owner, GameObject rangedEnemy) : base(owner)
        {
            dmg = 2;
            rangedEnemyOwner = rangedEnemy;
            basePosition = owner.Position;
            direction = new Vector2((float)System.Math.Cos(owner.RotationZ), (float)System.Math.Sin(owner.RotationZ));
        }

        public override void Update(GameTime time)
        {
            owner.Translate(new Vector3(direction.Y * bulletSpeed * (float)time.ElapsedGameTime.TotalMilliseconds, 0f, direction.X * bulletSpeed * (float)time.ElapsedGameTime.TotalMilliseconds));
            if (Vector3.Distance(basePosition, this.owner.Position) >= rangedEnemyOwner.GetComponent<RangedEnemy>().range)
            {
                base.OnTrigger();
            }
        }

        public override void OnTrigger()
        {
            base.OnTrigger();
            GameServices.GetService<GameObject>().GetComponent<Player>().Hp -= dmg;
            
        }


    }
}
