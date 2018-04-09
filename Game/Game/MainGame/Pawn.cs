using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PBLGame.SceneGraph;

namespace PBLGame.MainGame
{
    class Pawn : GameObject
    {
        public int Hp { get; internal set; }
        public Pawn() : base()
        {
           
        }
        public virtual void Damage()
        {
            this.Hp -= 1;
        }

        public override void Update()
        {
            base.Update();
            CollisionUpdate();
        }

        protected void MoveForward(float speed)
        {
            Vector2 direction = new Vector2((float)System.Math.Cos(RotationZ), (float)System.Math.Sin(RotationZ));
            this.Translate(new Vector3(direction.Y * speed, 0f, direction.X * speed));
            if (CheckCollider())
            {
                this.Translate(new Vector3(-2 * direction.Y * speed, 0f, -2 * direction.X * speed));
            }
        }

        protected void MoveBack(float speed)
        {
            Vector2 direction = new Vector2((float)System.Math.Cos(RotationZ), (float)System.Math.Sin(RotationZ));
            this.Translate(new Vector3(-direction.Y * speed, 0f, -direction.X * speed));
            if (CheckCollider())
            {
                this.Translate(new Vector3(2 * direction.Y * speed, 0f, 2 * direction.X * speed));
            }
        }
        protected void MoveLeft(float speed)
        {
                Vector2 direction = new Vector2((float)System.Math.Cos(RotationZ), (float)System.Math.Sin(RotationZ));
                this.Translate(new Vector3(-direction.X * speed, 0f, direction.Y * speed));
                if (CheckCollider())
                {
                    this.Translate(new Vector3(2 * direction.X * speed, 0f, -2 * direction.Y * speed));
                }
        }

        protected void MoveRight(float speed)
        {
                Vector2 direction = new Vector2((float)System.Math.Cos(RotationZ), (float)System.Math.Sin(RotationZ));
                this.Translate(new Vector3(direction.X * speed, 0f, -direction.Y * speed));
                if (CheckCollider())
                {
                    this.Translate(new Vector3(-2 * direction.X * speed, 0f, 2 * direction.Y * speed));
                }
            
        }

        protected bool CheckCollider()
        {
            foreach (Collider col in this.colliders)
            {
                if (col.CollisionUpdate())
                {
                    return true;
                }
            }
            return false;
        }

        protected void Rotate(float angle)
        {
                RotationZ += angle;
        }
    }
}
