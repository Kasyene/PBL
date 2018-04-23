using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PBLGame.SceneGraph;

namespace PBLGame.MainGame
{
    class Pawn : Component
    {
        protected GameObject parentGameObject;
        protected Vector3 lastPosition;
        public int Hp { get; internal set; }
        public Pawn() : base()
        {
            lastPosition = new Vector3(0f, 0f, 0f);
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
            CheckCollider();
        }

        protected void MoveBack(float speed)
        {
            Vector2 direction = new Vector2((float)System.Math.Cos(parentGameObject.RotationZ), (float)System.Math.Sin(parentGameObject.RotationZ));
            parentGameObject.Translate(new Vector3(-direction.Y * speed, 0f, -direction.X * speed));
            CheckCollider();
        }
        protected void MoveLeft(float speed)
        {
            Vector2 direction = new Vector2((float)System.Math.Cos(parentGameObject.RotationZ), (float)System.Math.Sin(parentGameObject.RotationZ));
            parentGameObject.Translate(new Vector3(-direction.X * speed, 0f, direction.Y * speed));
            CheckCollider();
        }

        protected void MoveRight(float speed)
        {
            Vector2 direction = new Vector2((float)System.Math.Cos(parentGameObject.RotationZ), (float)System.Math.Sin(parentGameObject.RotationZ));
            parentGameObject.Translate(new Vector3(direction.X * speed, 0f, -direction.Y * speed));
            CheckCollider();
        }

        protected void CheckCollider()
        {
            foreach (Collider col in parentGameObject.colliders)
            {
                if (col.CollisionUpdate() != null)
                {
                    parentGameObject.Position = lastPosition;
                }
            }
            lastPosition = parentGameObject.Position;
        }

        protected void Rotate(float angle)
        {
            parentGameObject.RotationZ += angle;
        }
    }
}
