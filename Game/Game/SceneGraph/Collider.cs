using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBLGame.SceneGraph
{
    class Collider
    {
        protected BoundingBox boundingBox;
        public bool isTrigger;

        public Collider(BoundingBox bounding)
        {
            boundingBox = bounding;
        }

        public void SetBoundingBox(BoundingBox bounding)
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
