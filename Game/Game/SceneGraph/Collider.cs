using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBLGame.SceneGraph
{
    public class Collider : Component
    {
        private Component component;
        protected BoundingBox boundingBox;
        public bool isTrigger;

        public BoundingBox BoundingBox
        {
            get
            {
                return boundingBox;
            }

            set
            {
                boundingBox = value;
            }
        }

        public Component Component
        {
            get
            {
                return component;
            }
        }

        public Collider(BoundingBox bounding, Component comp)
        {
            component = comp;
            boundingBox = bounding;
        }

        public bool IsCollision(BoundingBox other)
        {
            if (boundingBox.Intersects(other)) return true;
            return false;
        }
    }
}
