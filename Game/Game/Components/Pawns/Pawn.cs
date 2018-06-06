using System;
using System.Diagnostics;
using Game.Misc.Time;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PBLGame.SceneGraph;
using Game = Microsoft.Xna.Framework.Game;

namespace PBLGame.MainGame
{
    public enum Side
    {
        Enemy,
        Player,
        Other
    };

    public class Pawn : Component
    {
        public bool isDead = false;
        public bool gravityWorking = false;
        private bool isMoving = false;
        protected bool isJumping = false;
        public bool isAttacking = false;
        private float positionYBeforeJump = 0.0f;
        public float AccelerationDueToGravity = 0.0f;
        protected GameObject parentGameObject;
        protected Vector3 lastPosition;
        public int Hp { get; internal set; } = 10;
        public int MaxHp { get; internal set; } = 10;
        public Side ObjectSide;
        public int dmg = 2;


        public Pawn() : base()
        {
            lastPosition = new Vector3(0f, 0f, 0f);
        }

        public int getDamage()
        {
            return dmg;
        }

        public override void Update(GameTime time)
        {
            CheckCollider();
            if (!parentGameObject.isGrounded && gravityWorking)
            {
                AccelerationDueToGravity = -0.12f;
                parentGameObject.Translate(new Vector3(0.0f,
                    AccelerationDueToGravity * (float) Timer.gameTime.ElapsedGameTime.TotalMilliseconds, 0.0f));
            }

            if (!gravityWorking && Timer.gameTime.TotalGameTime.Seconds > 2 && Timer.gameTime.TotalGameTime.Seconds < 4)
            {
                gravityWorking = true;
            }

            if (!isJumping)
            {
                positionYBeforeJump = parentGameObject.PositionY;
            }

            if (isJumping)
            {
                parentGameObject.Translate(new Vector3(0.0f,
                    0.20f * (float) Timer.gameTime.ElapsedGameTime.TotalMilliseconds, 0.0f));
                if (parentGameObject.PositionY > positionYBeforeJump + 75.0f)
                {
                    isJumping = false;
                }
            }

            parentGameObject.OnTransformationsSet();

            base.Update(time);
        }


        protected void Move(Vector3 move)
        {
            if (!isMoving)
            {
                Vector2 direction = new Vector2((float) System.Math.Cos(parentGameObject.RotationZ),
                    (float) System.Math.Sin(parentGameObject.RotationZ));
                float speed = Math.Max(Math.Abs(move.X), Math.Abs(move.Z));

                // LEFT
                if (move.X > 0 && Math.Abs(move.Z) < 0.1f)
                {
                    MoveLeft(speed);
                }

                // RIGHT
                if (move.X < 0 && Math.Abs(move.Z) < 0.1f)
                {
                    MoveRight(speed);
                }

                // FORWARD
                if (Math.Abs(move.X) < 0.1f && move.Z > 0)
                {
                    MoveForward(speed);
                }

                // BACK
                if (Math.Abs(move.X) < 0.1f && move.Z < 0)
                {
                    MoveBack(speed);
                }

                // FORWARD RIGHT
                if (move.X > 0 && move.Z > 0)
                {
                    MoveWD(speed);
                }

                // FORWARD LEFT
                if (move.X < 0 && move.Z > 0)
                {
                    MoveWA(speed);
                }

                // BACK RIGHT
                if (move.X > 0 && move.Z < 0)
                {
                    MoveSD(speed);
                }

                // BACK LEFT
                if (move.X < 0 && move.Z < 0)
                {
                    MoveSA(speed);
                }
            }
        }

        protected void MoveForward(float speed)
        {
            if (!isMoving)
            {
                isMoving = true;
                Vector2 direction = new Vector2((float) System.Math.Cos(parentGameObject.RotationZ),
                    (float) System.Math.Sin(parentGameObject.RotationZ));
                parentGameObject.Translate(new Vector3(
                    direction.Y * speed * (float) Timer.gameTime.ElapsedGameTime.TotalMilliseconds, 0f,
                    direction.X * speed * (float) Timer.gameTime.ElapsedGameTime.TotalMilliseconds));
                CheckCollider();
                isMoving = false;
            }
        }

        protected void MoveBack(float speed)
        {
            if (!isMoving)
            {
                isMoving = true;
                Vector2 direction = new Vector2((float) System.Math.Cos(parentGameObject.RotationZ),
                    (float) System.Math.Sin(parentGameObject.RotationZ));
                parentGameObject.Translate(new Vector3(
                    -direction.Y * speed * (float) Timer.gameTime.ElapsedGameTime.TotalMilliseconds, 0f,
                    -direction.X * speed * (float) Timer.gameTime.ElapsedGameTime.TotalMilliseconds));
                CheckCollider();
                isMoving = false;
            }
        }

        protected void MoveLeft(float speed)
        {
            if (!isMoving)
            {
                isMoving = true;
                Vector2 direction = new Vector2((float) System.Math.Cos(parentGameObject.RotationZ),
                    (float) System.Math.Sin(parentGameObject.RotationZ));
                parentGameObject.Translate(new Vector3(
                    -direction.X * speed * (float) Timer.gameTime.ElapsedGameTime.TotalMilliseconds, 0f,
                    direction.Y * speed * (float) Timer.gameTime.ElapsedGameTime.TotalMilliseconds));
                CheckCollider();
                isMoving = false;
            }
        }

        protected void MoveRight(float speed)
        {
            if (!isMoving)
            {
                isMoving = true;
                Vector2 direction = new Vector2((float) System.Math.Cos(parentGameObject.RotationZ),
                    (float) System.Math.Sin(parentGameObject.RotationZ));
                parentGameObject.Translate(new Vector3(
                    direction.X * speed * (float) Timer.gameTime.ElapsedGameTime.TotalMilliseconds, 0f,
                    -direction.Y * speed * (float) Timer.gameTime.ElapsedGameTime.TotalMilliseconds));
                CheckCollider();
                isMoving = false;
            }
        }

        protected void MoveWA(float speed)
        {
            if (!isMoving)
            {
                isMoving = true;
                Vector2 direction = new Vector2((float) System.Math.Cos(parentGameObject.RotationZ),
                    (float) System.Math.Sin(parentGameObject.RotationZ));
                parentGameObject.Translate(new Vector3(
                                               ((direction.Y + direction.X) * speed *
                                                (float) Timer.gameTime.ElapsedGameTime.TotalMilliseconds) / 1.5f, 0f,
                                               ((direction.X - direction.Y) * speed *
                                                (float) Timer.gameTime.ElapsedGameTime.TotalMilliseconds)) / 1.5f);
                CheckCollider();
                isMoving = false;
            }
        }

        protected void MoveWD(float speed)
        {
            if (!isMoving)
            {
                isMoving = true;
                Vector2 direction = new Vector2((float) System.Math.Cos(parentGameObject.RotationZ),
                    (float) System.Math.Sin(parentGameObject.RotationZ));
                parentGameObject.Translate(new Vector3(
                                               ((direction.Y - direction.X) * speed *
                                                (float) Timer.gameTime.ElapsedGameTime.TotalMilliseconds) / 1.5f, 0f,
                                               ((direction.X + direction.Y) * speed *
                                                (float) Timer.gameTime.ElapsedGameTime.TotalMilliseconds)) / 1.5f);
                CheckCollider();
                isMoving = false;
            }
        }

        protected void MoveSA(float speed)
        {
            if (!isMoving)
            {
                isMoving = true;
                Vector2 direction = new Vector2((float) System.Math.Cos(parentGameObject.RotationZ),
                    (float) System.Math.Sin(parentGameObject.RotationZ));
                parentGameObject.Translate(new Vector3(
                                               ((-direction.Y + direction.X) * speed *
                                                (float) Timer.gameTime.ElapsedGameTime.TotalMilliseconds) / 1.5f, 0f,
                                               ((-direction.X - direction.Y) * speed *
                                                (float) Timer.gameTime.ElapsedGameTime.TotalMilliseconds)) / 1.5f);
                isMoving = false;
            }
        }

        protected void MoveSD(float speed)
        {
            if (!isMoving)
            {
                isMoving = true;
                Vector2 direction = new Vector2((float) System.Math.Cos(parentGameObject.RotationZ),
                    (float) System.Math.Sin(parentGameObject.RotationZ));
                parentGameObject.Translate(new Vector3(
                                               ((-direction.Y - direction.X) * speed *
                                                (float) Timer.gameTime.ElapsedGameTime.TotalMilliseconds) / 1.5f, 0f,
                                               ((-direction.X + direction.Y) * speed *
                                                (float) Timer.gameTime.ElapsedGameTime.TotalMilliseconds)) / 1.5f);
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
                if (temp != null && (temp.tag != "Ground" || temp.Parent.tag != "Ground"))
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

            float desiredAngle = (float) System.Math.Atan2(z, x);

            parentGameObject.RotationZ = -desiredAngle + MathHelper.Pi + MathHelper.PiOver2;
        }
    }
}