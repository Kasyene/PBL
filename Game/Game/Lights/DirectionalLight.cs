using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Lights
{
    public class DirectionalLight
    {
        public Vector3 direction;

        public Vector4 ambient;
        public Vector4 diffuse;
        public Vector4 specular;

        public DirectionalLight()
        {
            direction = new Vector3(0f, 1f, 0.8f);

            ambient = new Vector4(0.6f);
            diffuse = new Vector4(1f);
            specular = new Vector4(1f);
        }
    }
}
