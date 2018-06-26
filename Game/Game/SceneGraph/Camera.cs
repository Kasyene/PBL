using System;
using System.Diagnostics;
using Game.Misc.Time;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PBLGame.Input;

namespace PBLGame.SceneGraph
{
    public class Camera : GameObject
    {
        GameObject cameraTarget;
        public float minZoom = -250f;
        public float maxZoom = -50f;
        public float playerZoom = -150.0f;
        public bool zoomChangedByCollision = false;
        public bool zoomCanReturnToPlayerZoom = false;
        public bool playerUsedScroll = false;
        public float minYRotation = 0.1f;
        public float maxYRotation = 1f;
        private readonly InputManager inputManager;
        private Matrix viewMatrix;
        public float fieldOfView = MathHelper.PiOver4;

        public Camera()
        {
            inputManager = InputManager.Instance;
            visible = false;
            Position = new Vector3(0f, 30f, maxZoom);
            RotationY = minYRotation;
            //colliders.Add(new Collider(new BoundingBox(), null, this));
            TransformationsOrder = TransformationOrder.ScalePositionRotation;
            
        }

        public Matrix ViewMatrix
        {
            set
            {
                viewMatrix = value;
            }
            get
            {
                return viewMatrix;
            }
        }

        public Matrix CalcViewMatrix()
        {
            isDirty = true;
            Vector3 lookAtVector;
            if (cameraTarget != null)
            {
                lookAtVector = cameraTarget.Position + new Vector3(0f, 30f, 0f);
            }
            else
            {
                lookAtVector = Vector3.Zero;
            }
            Vector3 cameraUpVector = Vector3.UnitY;

            return Matrix.CreateLookAt(WorldTransformations.Translation, lookAtVector, cameraUpVector);
        }

        public Matrix ProjectionMatrix
        {
            get
            {
                float aspectRatio = 1f;
                float nearClipPlane = 1;
                float farClipPlane = 5000;
                return Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearClipPlane, farClipPlane);
            }
        }

        public void SetCameraTarget(GameObject target)
        {
            cameraTarget = target;
        }

        public GameObject GetCameraTarget()
        {
            return cameraTarget;
        }

        public Vector3 GetViewVector()
        {
            Vector3 viewVector = Vector3.Transform(cameraTarget.GetWorldPosition() - GetWorldPosition(), Matrix.CreateRotationY(0));
            viewVector.Normalize();
            return viewVector;
        }

        public override void Update()
        {
            base.Update();
            float rotY = RotationY - ShroomGame.mouseYAxis * inputManager.Mouse.PositionsDelta.Y * 0.00166f; //6x slower than 0.01
            float posZ = PositionZ + inputManager.Mouse.ScrollValue * 0.1f; ;
            if (posZ > minZoom && posZ <  maxZoom)
            {
                PositionZ = posZ;
            }
            if (Math.Abs(inputManager.Mouse.ScrollValue) > 0.0f)
            {
                playerZoom = PositionZ;
            }
            if (rotY < maxYRotation && rotY > minYRotation)
            {
                RotationY = rotY;
            }

            if (zoomCanReturnToPlayerZoom)
            {
                Debug.WriteLine("woofWoof " + Timer.gameTime.TotalGameTime.TotalSeconds);
                if (PositionZ > playerZoom)
                {
                    PositionZ -= 0.35f;
                }
            }
        }

        public void Scroll(float direction)
        {
            if (PositionZ > minZoom && PositionZ < maxZoom)
            {
                PositionZ += direction * 2.0f;
            }

            if (PositionZ >= maxZoom)
            {
                if (RotationY < maxYRotation && RotationY > minYRotation)
                {
                    RotationY += 0.5f;
                }
            }


        }
    }
}
