using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PBLGame.SceneGraph;

namespace PBLGame.MainGame
{
    class Pawn : Component
    {
        protected GameObject parentGameObject;
        public int Hp { get; internal set; }
        public Pawn() : base()
        {

        }

        public virtual void Damage()
        {
            this.Hp -= 1;
        }

        public override void Update(GameTime time)
        {
            base.Update(time);
            parentGameObject.CollisionUpdate();
        }

        protected void MoveForward(float speed)
        {
            Vector2 direction = new Vector2((float)System.Math.Cos(parentGameObject.RotationZ), (float)System.Math.Sin(parentGameObject.RotationZ));
            parentGameObject.Translate(new Vector3(direction.Y * speed, 0f, direction.X * speed));
            if (CheckCollider())
            {
                parentGameObject.Translate(new Vector3(-2 * direction.Y * speed, 0f, -2 * direction.X * speed));
            }
        }

        protected void MoveBack(float speed)
        {
            Vector2 direction = new Vector2((float)System.Math.Cos(parentGameObject.RotationZ), (float)System.Math.Sin(parentGameObject.RotationZ));
            parentGameObject.Translate(new Vector3(-direction.Y * speed, 0f, -direction.X * speed));
            if (CheckCollider())
            {
                parentGameObject.Translate(new Vector3(2 * direction.Y * speed, 0f, 2 * direction.X * speed));
            }
        }
        protected void MoveLeft(float speed)
        {
                Vector2 direction = new Vector2((float)System.Math.Cos(parentGameObject.RotationZ), (float)System.Math.Sin(parentGameObject.RotationZ));
                parentGameObject.Translate(new Vector3(-direction.X * speed, 0f, direction.Y * speed));
                if (CheckCollider())
                {
                    parentGameObject.Translate(new Vector3(2 * direction.X * speed, 0f, -2 * direction.Y * speed));
                }
        }

        protected void MoveRight(float speed)
        {
                Vector2 direction = new Vector2((float)System.Math.Cos(parentGameObject.RotationZ), (float)System.Math.Sin(parentGameObject.RotationZ));
                parentGameObject.Translate(new Vector3(direction.X * speed, 0f, -direction.Y * speed));
                if (CheckCollider())
                {
                    parentGameObject.Translate(new Vector3(-2 * direction.X * speed, 0f, 2 * direction.Y * speed));
                }
            
        }

        protected bool CheckCollider()
        {
            foreach (Collider col in parentGameObject.colliders)
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
            parentGameObject.RotationZ += angle;
        }
    }
}
