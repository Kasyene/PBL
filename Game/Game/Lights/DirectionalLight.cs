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
            direction = new Vector3(-0.4f, 1f, 0.3f);

            ambient = new Vector4(1f);
            diffuse = new Vector4(1f);
            specular = new Vector4(1f);
        }

        public Matrix CreateLightViewProjectionMatrix()
        {
            Matrix lightView = Matrix.CreateLookAt(Vector3.Zero, -direction, Vector3.Up);

            Matrix lightProjection = Matrix.CreateOrthographic(2000, 2000, -1000, 1000);

            return lightView * lightProjection;
        }
    }
}
