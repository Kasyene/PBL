﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PBLGame.Input;

namespace PBLGame.SceneGraph
{
    public class Camera : GameObject
    {
        GameObject cameraTarget;
        public float minZoom = -60f;
        public float maxZoom = -25f;
        public float minYRotation = 0f;
        public float maxYRotation = 1f;
        private readonly InputManager inputManager;

        public Camera()
        {
            inputManager = InputManager.Instance;
            visible = false;
            Position = new Vector3(0f, 0f, -50f);
            TransformationsOrder = TransformationOrder.ScalePositionRotation;
            
        }

        public Matrix ViewMatrix
        {
            get
            {
                isDirty = true;
                Vector3 lookAtVector;
                if (cameraTarget != null)
                {
                    lookAtVector = cameraTarget.Position;
                }
                else
                {
                    lookAtVector = Vector3.Zero;
                }
                Vector3 cameraUpVector = Vector3.UnitY;

                return Matrix.CreateLookAt(WorldTransformations.Translation, lookAtVector, cameraUpVector);
            }
        }

        public Matrix ProjectionMatrix
        {
            get
            {
                float aspectRatio = 1f;
                float fieldOfView = Microsoft.Xna.Framework.MathHelper.PiOver4;
                float nearClipPlane = 1;
                float farClipPlane = 400;
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

        public override void Update()
        {
            base.Update();
            //RotationZ += inputManager.Mouse.PositionsDelta.X * 0.01f;
            float rotY = RotationY + inputManager.Mouse.PositionsDelta.Y * 0.01f;
            float posZ = PositionZ + inputManager.Mouse.ScrollValue * 0.01f;
            if(posZ > minZoom && posZ <  maxZoom)
            {
                PositionZ = posZ;
            }
            if(rotY < maxYRotation && rotY > minYRotation)
            {
                RotationY = rotY;
            }
        }
    }
}
