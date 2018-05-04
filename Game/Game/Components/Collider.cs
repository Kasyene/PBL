﻿using Microsoft.Xna.Framework;
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

        public bool IsCollision(Collider other)
        {
            if (!other.isCollider && !other.isTrigger)
            {
                return false;
            }
            if (this.owner.Parent == other.owner.Parent)
            {
                return false;
            }
            if (boundingBox.Intersects(other.boundingBox))
            {
                if (other.isTrigger)
                {
                    Debug.WriteLine("interakcja z sercem");
                    other.isTrigger = false;
                    other.isReadyToBeDisposed = true;
                    other.owner.Dispose();
                    return false;
                }
                System.Diagnostics.Debug.WriteLine("Collision " + DateTime.Now);
                return true;
            }

            return false;
        }

        public GameObject CollisionUpdate()
        {
            List<Collider> triggered = new List<Collider>();
            foreach (Collider col in collidersList)
            {
                if (col != this)
                {
                    if (IsCollision(col))
                    {
                        this.penetrationDepth = PenetrationDepth(this.boundingBox, col.boundingBox);
                        return col.owner;
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
            return null;
        }

        public Vector3 PenetrationDepth(BoundingBox boxA, BoundingBox boxB)
        {
            Vector3 result = new Vector3(0.0f);
            result.X = SingleAxisPenetrationDepth(boxA.Max.X, boxA.Min.X, boxB.Max.X, boxB.Min.X);
           // result.Y = SingleAxisPenetrationDepth(boxA.Max.Y, boxA.Min.Y, boxB.Max.Y, boxB.Min.Y);
            result.Z = SingleAxisPenetrationDepth(boxA.Max.Z, boxA.Min.Z, boxB.Max.Z, boxB.Min.Z);
            if (Math.Abs(result.X) > Math.Abs(result.Z))
            {
                if (result.Z != 0.0)
                {
                    result.X = result.Z;
                }
            }
            if (Math.Abs(result.Z) > Math.Abs(result.X))
            {
                if (result.X != 0.0)
                {
                    result.Z = result.X;
                }
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
