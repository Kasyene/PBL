using Microsoft.Xna.Framework;
using PBLGame.SceneGraph;

namespace PBLGame.Lights
{
    public class DirectionalLight
    {
        public Vector3 direction;

        public Vector4 ambient;
        public Vector4 diffuse;
        public Vector4 specular;

        public DirectionalLight()
        {
            direction = new Vector3(0.1f, 1f, 0.8f);

            ambient = new Vector4(0.6f);
            diffuse = new Vector4(1f);
            specular = new Vector4(1f);
        }

        public Matrix CreateLightViewProjectionMatrix()
        {
            Matrix lightView = Matrix.CreateLookAt(-direction, Vector3.Zero, Vector3.Up);

            Matrix lightProjection = Matrix.CreateOrthographic(1000, 1000, -1, 1000);

            return lightView * lightProjection;
        }
    }
}
