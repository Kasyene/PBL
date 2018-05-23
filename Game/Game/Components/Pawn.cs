using System.Diagnostics;
using Game.Misc.Time;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PBLGame.SceneGraph;
using Game = Microsoft.Xna.Framework.Game;

namespace PBLGame.MainGame
{
    class Pawn : Component
    {
        private bool isMoving = false;
        protected bool isJumping = false;
        private float positionYBeforeJump = 0.0f;
        public float AccelerationDueToGravity;
        protected GameObject parentGameObject;
        protected Vector3 lastPosition;
        public int Hp { get; internal set; } = 10;
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
            CheckCollider();
            if (!parentGameObject.isGrounded)
            {
                AccelerationDueToGravity = -0.03f;
                parentGameObject.Translate(new Vector3(0.0f, AccelerationDueToGravity * (float) Timer.gameTime.ElapsedGameTime.TotalMilliseconds, 0.0f));
            }

            if (!isJumping)
            {
                positionYBeforeJump = parentGameObject.PositionY;
            }

            if (isJumping)
            {
                parentGameObject.Translate(new Vector3(0.0f, 0.13f * (float)Timer.gameTime.ElapsedGameTime.TotalMilliseconds, 0.0f));
                if (parentGameObject.PositionY > positionYBeforeJump + 55.0f)
                {
                    isJumping = false;
                }
            }



            base.Update(time);
        }


        protected void MoveForward(float speed)
        {
            if (!isMoving)
            {
                isMoving = true;
                Vector2 direction = new Vector2((float)System.Math.Cos(parentGameObject.RotationZ), (float)System.Math.Sin(parentGameObject.RotationZ));
                parentGameObject.Translate(new Vector3(direction.Y * speed * (float)Timer.gameTime.ElapsedGameTime.TotalMilliseconds, 0f, direction.X * speed * (float)Timer.gameTime.ElapsedGameTime.TotalMilliseconds));
                CheckCollider();
                isMoving = false;
            }
           
        }

        protected void MoveBack(float speed)
        {
            if (!isMoving)
            {
                isMoving = true;
                Vector2 direction = new Vector2((float)System.Math.Cos(parentGameObject.RotationZ), (float)System.Math.Sin(parentGameObject.RotationZ));
                parentGameObject.Translate(new Vector3(-direction.Y * speed * (float)Timer.gameTime.ElapsedGameTime.TotalMilliseconds, 0f, 
                    -direction.X * speed * (float)Timer.gameTime.ElapsedGameTime.TotalMilliseconds));
                CheckCollider();
                isMoving = false;
            }
        }
        protected void MoveLeft(float speed)
        {
            if (!isMoving)
            {
                isMoving = true;
                Vector2 direction = new Vector2((float)System.Math.Cos(parentGameObject.RotationZ), (float)System.Math.Sin(parentGameObject.RotationZ));
                parentGameObject.Translate(new Vector3(-direction.X * speed * (float)Timer.gameTime.ElapsedGameTime.TotalMilliseconds, 0f, 
                    direction.Y * speed * (float)Timer.gameTime.ElapsedGameTime.TotalMilliseconds));
                CheckCollider();
                isMoving = false;
            }
        }

        protected void MoveRight(float speed)
        {
            if (!isMoving)
            {
                isMoving = true;
                Vector2 direction = new Vector2((float)System.Math.Cos(parentGameObject.RotationZ), (float)System.Math.Sin(parentGameObject.RotationZ));
                parentGameObject.Translate(new Vector3(direction.X * speed * (float)Timer.gameTime.ElapsedGameTime.TotalMilliseconds, 0f,
                    -direction.Y * speed * (float)Timer.gameTime.ElapsedGameTime.TotalMilliseconds));
                CheckCollider();
                isMoving = false;
            }
        }

        protected void MoveWA(float speed)
        {
            if (!isMoving)
            {
                isMoving = true;
                Vector2 direction = new Vector2((float)System.Math.Cos(parentGameObject.RotationZ), (float)System.Math.Sin(parentGameObject.RotationZ));
                parentGameObject.Translate(new Vector3(((direction.Y + direction.X) * speed * (float)Timer.gameTime.ElapsedGameTime.TotalMilliseconds)/1.5f, 0f, ((direction.X - direction.Y) * speed * (float)Timer.gameTime.ElapsedGameTime.TotalMilliseconds))/1.5f);
                CheckCollider();
                isMoving = false;
            }
        }

        protected void MoveWD(float speed)
        {
            if (!isMoving)
            {
                isMoving = true;
                Vector2 direction = new Vector2((float)System.Math.Cos(parentGameObject.RotationZ), (float)System.Math.Sin(parentGameObject.RotationZ));
                parentGameObject.Translate(new Vector3(((direction.Y - direction.X) * speed * (float)Timer.gameTime.ElapsedGameTime.TotalMilliseconds) / 1.5f, 0f, ((direction.X + direction.Y) * speed * (float)Timer.gameTime.ElapsedGameTime.TotalMilliseconds)) / 1.5f);
                CheckCollider();
                isMoving = false;
            }
        }

        protected void MoveSA(float speed)
        {
            if (!isMoving)
            {
                isMoving = true;
                Vector2 direction = new Vector2((float)System.Math.Cos(parentGameObject.RotationZ), (float)System.Math.Sin(parentGameObject.RotationZ));
                parentGameObject.Translate(new Vector3(((-direction.Y + direction.X) * speed * (float)Timer.gameTime.ElapsedGameTime.TotalMilliseconds) / 1.5f, 0f, ((-direction.X - direction.Y) * speed * (float)Timer.gameTime.ElapsedGameTime.TotalMilliseconds)) / 1.5f);
                isMoving = false;
            }
        }

        protected void MoveSD(float speed)
        {
            if (!isMoving)
            {
                isMoving = true;
                Vector2 direction = new Vector2((float)System.Math.Cos(parentGameObject.RotationZ), (float)System.Math.Sin(parentGameObject.RotationZ));
                parentGameObject.Translate(new Vector3(((- direction.Y - direction.X) * speed * (float)Timer.gameTime.ElapsedGameTime.TotalMilliseconds) / 1.5f, 0f, ((- direction.X + direction.Y) * speed * (float)Timer.gameTime.ElapsedGameTime.TotalMilliseconds)) / 1.5f);
                CheckCollider();
                isMoving = false;
            }
        }

        protected void CheckCollider()
        {
            foreach (Collider col in parentGameObject.colliders)
            {
                col.checkIfGrounded();
                GameObject temp = col.CollisionUpdate();
                if ( temp != null && temp.tag != "Ground")
                {
                    parentGameObject.Position = parentGameObject.Position - col.penetrationDepth;
                }
            }
            lastPosition = parentGameObject.Position;
        }


        protected void Rotate(float angle)
        {
            parentGameObject.RotationZ += angle;
        }

        protected void LookAtTarget(Vector3 targetPosition, Vector3 position)
        {
            float x = position.X - targetPosition.X;
            float z = position.Z - targetPosition.Z;

            float desiredAngle = (float)System.Math.Atan2(z, x);

            parentGameObject.RotationZ = -desiredAngle + MathHelper.Pi + MathHelper.PiOver2;
        }
    }
}
