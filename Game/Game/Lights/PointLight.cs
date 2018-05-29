using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PBLGame.Lights
{
    public class PointLight
    {
        public Vector3 position;

        public Vector3 attenuation;

        public Vector4 ambient;
        public Vector4 diffuse;
        public Vector4 specular;

        public TextureCube shadowMap;

        public PointLight(Vector3 _position)
        {
            position = _position;
            attenuation = new Vector3(0.8f, 0.001f, 0.0004f);
            ambient = new Vector4(0.0f);
            diffuse = new Vector4(1f);
            specular = new Vector4(1f);
        }

        public PointLight(Vector3 _position, Vector3 _attenuation)
        {
            position = _position;
            attenuation = _attenuation;
            ambient = new Vector4(0.0f);
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

        static public Vector3[] GetPointLightsPositionArray()
        {
            int number = ShroomGame.pointLights.Count;
            Vector3[] array = new Vector3[number];
            for (int i = 0; i < number; i++)
            {
                array[i] = ShroomGame.pointLights[i].position;
            }
            return array;
        }

        static public Vector3[] GetPointLightsAttenuationArray()
        {
            int number = ShroomGame.pointLights.Count;
            Vector3[] array = new Vector3[number];
            for (int i = 0; i < number; i++)
            {
                array[i] = ShroomGame.pointLights[i].attenuation;
            }
            return array;
        }

        static public Vector4[] GetPointLightsAmbientArray()
        {
            int number = ShroomGame.pointLights.Count;
            Vector4[] array = new Vector4[number];
            for (int i = 0; i < number; i++)
            {
                array[i] = ShroomGame.pointLights[i].ambient;
            }
            return array;
        }

        static public Vector4[] GetPointLightsSpecularArray()
        {
            int number = ShroomGame.pointLights.Count;
            Vector4[] array = new Vector4[number];
            for (int i = 0; i < number; i++)
            {
                array[i] = ShroomGame.pointLights[i].specular;
            }
            return array;
        }
    }
}
