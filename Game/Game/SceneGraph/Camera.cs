using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PBLGame.Input;

namespace PBLGame.SceneGraph
{
    public class Camera : GameObject
    {
        GameObject cameraTarget;
        public float minZoom = -500f;
        public float maxZoom = -250f;
        public float minYRotation = 0.1f;
        public float maxYRotation = 1f;
        private readonly InputManager inputManager;

        public Camera()
        {
            inputManager = InputManager.Instance;
            visible = false;
            Position = new Vector3(0f, 0f, -400f);
            //colliders.Add(new Collider(new BoundingBox(), null, this));
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
                    lookAtVector = cameraTarget.Position + new Vector3(0f, 6f, 0f);
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
                float farClipPlane = 1000;
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
            Vector3 viewVector = Vector3.Transform(cameraTarget.Position - Position, Matrix.CreateRotationY(0));
            viewVector.Normalize();
            return viewVector;
        }

        public override void Update()
        {
            base.Update();
            float rotY = RotationY + inputManager.Mouse.PositionsDelta.Y * 0.01f;
            float posZ = PositionZ + inputManager.Mouse.ScrollValue * 0.1f; ;
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
