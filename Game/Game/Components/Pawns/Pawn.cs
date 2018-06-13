using System;
using System.Collections.Generic;
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
            if (Timer.gameTime.TotalGameTime.TotalSeconds > 2)
            {
                CheckCollider();
                if (!parentGameObject.isGrounded)
                {
                    AccelerationDueToGravity = -0.15f;
                    parentGameObject.Translate(new Vector3(0.0f,
                        AccelerationDueToGravity * (float)Timer.gameTime.ElapsedGameTime.TotalMilliseconds, 0.0f));
                }

                if (!isJumping)
                {
                    positionYBeforeJump = parentGameObject.PositionY;
                }

                if (isJumping)
                {
                    parentGameObject.Translate(new Vector3(0.0f,
                        0.25f * (float)Timer.gameTime.ElapsedGameTime.TotalMilliseconds, 0.0f));
                    if (parentGameObject.PositionY > positionYBeforeJump + 75.0f)
                    {
                        isJumping = false;
                    }
                }

                parentGameObject.OnTransformationsSet();

                base.Update(time);
            }
        }

        protected void Move(Vector3 move)
        {
            if (!isMoving)
            {
                isMoving = true;
                Vector2 direction = new Vector2((float)System.Math.Cos(parentGameObject.RotationZ),
                    (float)System.Math.Sin(parentGameObject.RotationZ));
                if (move.X != 0f && move.Z != 0.0f)
                {
                    move.X = move.X / 2f;
                    move.Z = move.Z / 2f;
                }
                Vector3 moveBy = new Vector3((((-move.X) * direction.X) + (move.Z * direction.Y)) * (float)Timer.gameTime.ElapsedGameTime.TotalMilliseconds, 0f,
                    ((move.Z * direction.X) + (move.X * direction.Y)) * (float)Timer.gameTime.ElapsedGameTime.TotalMilliseconds);
                if (!CheckCollider(moveBy))
                {
                    parentGameObject.Translate(moveBy);
                }
                isMoving = false;
            }
        }


        protected bool CheckCollider(Vector3 vec = new Vector3())
        {
            foreach (Collider col in parentGameObject.colliders)
            {
                col.checkIfGrounded();
                List<GameObject> temp = col.CollisionUpdate(vec);
                if (temp.Count > 0)
                {
                    parentGameObject.Position = parentGameObject.Position + (vec - col.penetrationDepth);
                    return true;
                }
            }
            lastPosition = parentGameObject.Position;
            return false;
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