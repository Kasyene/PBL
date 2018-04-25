using Microsoft.Xna.Framework;

namespace PBLGame.Lights
{
    public class PointLight
    {
        public Vector3 position;

        public Vector3 attenuation;

        public Vector4 ambient;
        public Vector4 diffuse;
        public Vector4 specular;

        public PointLight(Vector3 _position)
        {
            position = _position;
            attenuation = new Vector3(1f, 0.5f, 0.2f);
            ambient = new Vector4(0.6f);
            diffuse = new Vector4(1f);
            specular = new Vector4(1f);
        }

        public PointLight(Vector3 _position, Vector3 _attenuation, Vector4 _ambient, Vector4 _diffuse, Vector4 _specular)
        {
            position = _position;
            attenuation = _attenuation;
            ambient = _ambient;
            diffuse = _diffuse;
            specular = _specular;
        }
    }
}
