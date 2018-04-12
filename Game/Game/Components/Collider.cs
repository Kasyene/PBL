using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public SceneGraph.GameObject owner;

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

        public Collider(BoundingBox bounding, Component comp, GameObject owner)
        {
            if(collidersList == null)
            {
                collidersList = new List<Collider>();
            }
            component = comp;
            boundingBox = bounding;
            collidersList.Add(this);
            this.owner = owner;
        }

        public bool IsCollision(Collider other)
        {
            if (boundingBox.Intersects(other.boundingBox))
            {
                //Debug.WriteLine("pies");
                if (other.isTrigger)
                {
                    Debug.WriteLine("interakcja z sercem");
                    //other.owner.visible = false;
                    return false;
                }
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
                    if (IsCollision(col))
                    {
                        isColliding = true;
                    }
                }
            }
            return isColliding;
        }

        public override void Dispose()
        {
            owner?.colliders.Remove(this);
            owner = null;

            foreach (Collider collider in collidersList)
            {
                collider?.Dispose();
                collidersList.Remove(collider);
            }
            base.Dispose();
        }
    }
}
