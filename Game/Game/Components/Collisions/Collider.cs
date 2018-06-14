using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Components.Collisions;
using Microsoft.Xna.Framework.Graphics;
using PBLGame.MainGame;
using PBLGame.Misc.Anim;

namespace PBLGame.SceneGraph
{
    public class Collider : Component
    {
        private static List<Collider> collidersList;
        private Component component;
        public Vector3 penetrationDepth = new Vector3(0.0f);
        protected BoundingBox boundingBox;
        public bool isTrigger = false;
        public bool isCollider = true;
        public bool isReadyToBeDisposed = false;
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

        public bool IsCollision(Collider other, Vector3 vec = new Vector3())
        {
            if (!other.isCollider && !other.isTrigger)
            {
                return false;
            }
            if (this.owner.Parent == other.owner.Parent)
            {
                return false;
            }
            if (new BoundingBox(this.boundingBox.Min + vec, this.boundingBox.Max + vec).Intersects(other.boundingBox))
            {
                if (this.owner.tag == "meleeEnemy" && other.owner.tag == "meleeEnemy")
                {

                }
                if (other.isTrigger && !other.isCollider) //e.g. player hits trigger
                {
                    other.owner.GetComponent<Trigger>().OnTrigger(this.owner.Parent);
                    return false;
                }

                if (this.isTrigger && !this.isCollider) //e.g. trigger hits wall
                {
                    if (this.owner.tag == "cameraCollision")
                    {
                        this.owner.GetComponent<Trigger>().OnTrigger(other.owner);
                    }
                    else
                    {
                        this.owner.GetComponent<Trigger>().OnTrigger(other.owner.Parent);
                    }   
                    return false;
                }

                if (!other.isTrigger && this.isTrigger && this.isCollider && (other.owner.tag == "Wall" || other.owner.tag == "Ground") && this.owner.tag == "Hat" && this.owner.GetComponent<AnimationManager>().isCurrentAnimation("throw"))
                {
                    this.owner.hatAnimationCollision = true;
                    return false;
                }
                return true;
            }

            return false;
        }

        public List<GameObject> CollisionUpdate(Vector3 vec = new Vector3())
        {
            penetrationDepth = new Vector3(0.0f);
            List<GameObject> collided = new List<GameObject>();
            List<Collider> triggered = new List<Collider>();
            foreach (Collider col in collidersList)
            {

                if (col != this)
                {
                    if (IsCollision(col, vec))
                    {
                        if ((this.isCollider && this.isTrigger))
                        {
                           if (col.owner.tag != "Ground" && col.owner.tag != "Wall" && (this.owner.tag != "meleeEnemy" || col.owner.tag != "meleeEnemy"))
                            {
                                this.owner.GetComponent<HitTrigger>().OnTrigger(col.owner.Parent);
                            }
                            else
                            {
                                this.penetrationDepth += PenetrationDepth(new BoundingBox(this.boundingBox.Min + vec, this.boundingBox.Max + vec), col.boundingBox);
                                collided.Add(col.owner);
                            }
                        }
                        else
                        {
                            if (col.isCollider && col.isTrigger && col.owner.tag != "meleeEnemy")
                            {
                                // do nothing
                            }
                            else
                            {
                                if (col.owner.tag == "Ground")
                                {
                                    if (Math.Abs(this.boundingBox.Min.Y - col.boundingBox.Max.Y) > 5)
                                    {
                                        this.owner.Parent.Position = this.owner.Parent.Position - PenetrationDepth(new BoundingBox(this.boundingBox.Min + vec, this.boundingBox.Max + vec), col.boundingBox);
                                    }
                                }
                                else
                                {
                                    this.penetrationDepth += PenetrationDepth(new BoundingBox(this.boundingBox.Min + vec, this.boundingBox.Max + vec), col.boundingBox);
                                    //this.penetrationDepth.Y = 0.0f; NAPRAWIC 
                                    collided.Add(col.owner);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (col.isReadyToBeDisposed)
                        {
                            triggered.Add(col);
                        }
                    }
                }
            }

            foreach (var trig in triggered)
            {
                collidersList.Remove(trig);
            }

            return collided;
        }

        public void checkIfGrounded()
        {
            foreach (Collider col in collidersList)
            {
                if (col != this)
                {
                    if (IsCollision(col) && col.owner.tag == "Ground" && (col.boundingBox.Max.Y - this.boundingBox.Min.Y) < 5)
                    {
                        this.owner.Parent.isGrounded = true;
                        return;
                    }
                }
            }

            if (this.owner.tag == "Leg" || this.owner.tag == "meleeEnemy")
            {
                this.owner.Parent.isGrounded = false;
            }
            
        }

        public Vector3 PenetrationDepth(BoundingBox boxA, BoundingBox boxB)
        {
            Vector3 result = new Vector3(0.0f);
            result.X = SingleAxisPenetrationDepth(boxA.Max.X, boxA.Min.X, boxB.Max.X, boxB.Min.X);
            result.Y = SingleAxisPenetrationDepth(boxA.Max.Y, boxA.Min.Y, boxB.Max.Y, boxB.Min.Y);
            result.Z = SingleAxisPenetrationDepth(boxA.Max.Z, boxA.Min.Z, boxB.Max.Z, boxB.Min.Z);
            if (Math.Abs(result.X) > 0.0f && Math.Abs(result.Z) > 0.0f)
            {
                if (Math.Abs(result.X) > Math.Abs(result.Z))
                {
                    result.X = 0.0f;
                }
                else
                {
                    result.Z = 0.0f;
                }
            }

            if (result.Y < 0.0f)
            {
                if (Math.Abs(result.Y) > 3f)
                {
                    result.Y = 0.0f;
                }
            }
            else
            {
                result.Y = 0.0f;
            }
           

            return result;
        }

        private float SingleAxisPenetrationDepth(float maxA, float minA, float maxB, float minB)
        {
            float result = 0.0f;
            if (((maxA > maxB) && (minA < minB)) || ((maxB > maxA) && (minB < minA)))
            {
                return result;
            }
            result = ((maxA - minB) > (maxB - minA)) ? -(maxB - minA) : (maxA - minB);
            return result;
        }

        //TODO Styczeń to nie działa, robimy to inaczej, żal skasować, napraw xd
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
