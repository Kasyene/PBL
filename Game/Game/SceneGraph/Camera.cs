using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PBLGame.SceneGraph
{
    public class Camera : SceneNode
    {
        GraphicsDevice graphicsDevice;
        SceneNode cameraTarget;

        public Camera()
        {
            visible = false;
            Position = new Vector3(0f, 20f, 50f);
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
                float farClipPlane = 200;
                return Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearClipPlane, farClipPlane);
            }
        }

        public void SetCameraTarget(SceneNode target)
        {
            cameraTarget = target;
        }

        public SceneNode GetCameraTarget()
        {
            return cameraTarget;
        }
    }
}
