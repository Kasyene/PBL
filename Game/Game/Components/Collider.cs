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
        private static List<Collider> collidersList;
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
            if(collidersList == null)
            {
                collidersList = new List<Collider>();
            }
            component = comp;
            boundingBox = bounding;
            collidersList.Add(this);
        }

        public bool IsCollision(BoundingBox other)
        {
            if (boundingBox.Intersects(other))
            {
                System.Diagnostics.Debug.WriteLine("Collision " + DateTime.Now);
                return true;
            }

            return false;
        }

        public bool CollisionUpdate()
        {
            bool isColliding = false;
            foreach (Collider col in collidersList)
            {
                if (col != this)
                {
                    if (IsCollision(col.BoundingBox))
                    {
                        isColliding = true;
                    }
                }
            }
            return isColliding;
        }
    }
}
