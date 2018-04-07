using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBLGame.SceneGraph
{
    class Collider : Component
    {
        protected BoundingBox boundingBox;
        public bool isTrigger;

        protected BoundingBox BoundingBox
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

        public Collider(BoundingBox bounding)
        {
            boundingBox = bounding;
        }

        public bool IsCollision(BoundingBox other)
        {
            if (boundingBox.Intersects(other)) return true;
            return false;
        }
    }
}
