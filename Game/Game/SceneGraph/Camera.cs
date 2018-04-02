using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PBLGame.SceneGraph
{
    public class Camera : SceneNode
    {
        GraphicsDevice graphicsDevice;

        public Camera()
        {
            visible = false;
        }

        public Matrix ViewMatrix
        {
            get
            {
                Vector3 lookAtVector = Vector3.Zero;
                Vector3 cameraUpVector = Vector3.UnitY;

                return Matrix.CreateLookAt(transform.position, lookAtVector, cameraUpVector);
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
    }
}
