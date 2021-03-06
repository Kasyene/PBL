﻿using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Game.Components.Coliisions;
using Game.Components.Collisions;
using Microsoft.Xna.Framework.Graphics;
using PBLGame.MainGame;

namespace PBLGame.SceneGraph
{
    public delegate void NodeEventCallback(GameObject node);

    public class GameObject : IDisposable
    {
        public bool isGrounded = false;
        protected GameObject parent = null;
        public GameObject Parent { get { return parent; } }
        public static NodeEventCallback onTransformUpdate;
        public static NodeEventCallback onDraw;
        public string name;
        public string tag;
        protected Transformation transform = new Transformation();
        public virtual bool visible { get; set; }
        protected static readonly BoundingBox EmptyBoundingBox = new BoundingBox();
        protected Matrix localTransform = Matrix.Identity;
        protected Matrix worldTransform = Matrix.Identity;

        public List<GameObject> childs = new List<GameObject>();
        protected List<Component> components = new List<Component>();
        public List<Collider> colliders = new List<Collider>();
        protected bool isDirty = true;

        protected uint transformVersion = 0;
        public uint TransformVersion { get { return transformVersion; } }
        protected uint parentTransformVersion = 0;
        public bool hatAnimationCollision = false;

        public GameObject()
        {
            visible = true;
            tag = "";
        }

        public GameObject(string tag)
        {
            visible = true;
            this.tag = tag;
        }

        public virtual void Draw(Camera camera, bool createShadowMap = false, string newName = "Node")
        {
            if (!visible)
            {
                return;
            }

            UpdateTransformations();

            foreach (GameObject node in childs)
            {
                node.Draw(camera, createShadowMap);
            }

            onDraw?.Invoke(this);

            foreach (Component entity in components)
            {
                entity.Draw(this, camera, localTransform, worldTransform, createShadowMap);
            }
        }

        public virtual void Update()
        {
            foreach(GameObject node in childs)
            {
                node.Update();
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach(Component comp in components)
            {
                comp.Update(gameTime);
            }

            foreach (GameObject node in childs)
            {
                node.Update(gameTime);
            }
        }

        public void AddComponent(Component entity)
        {
            components.Add(entity);
        }

        public void RemoveComponent(Component entity)
        {
            components.Remove(entity);
        }

        public void AddChildNode(GameObject node)
        {
            if (node.parent != null)
            {
                //throw new System.Exception("Can't add");
                node.SetParent(null);
            }
            childs.Add(node);
            node.SetParent(this);
        }


        public GameObject FindChildNodeByTag(String searchedTag)
        {
            foreach (var node in childs)
            {
                if (node.tag == searchedTag)
                {
                    return node;
                }
            }

            return null;
        }

        public void RemoveChildNode(GameObject node)
        {
            if (node.parent != this)
            {
                throw new System.Exception("Can't remove");
            }
            childs.Remove(node);
            node.SetParent(null);
        }

        public void RemoveFromParent()
        {
            if (parent == null)
            {
                throw new System.Exception("Can't remove");
            }
            parent.RemoveChildNode(this);
        }

        protected virtual void OnWorldMatrixChange()
        {
            transformVersion++;
            onTransformUpdate?.Invoke(this);
            if (parent != null)
            {
                parent.OnChildWorldMatrixChange(this);
            }
        }

        public virtual void OnTransformationsSet()
        {
            isDirty = true;
        }

        public void SetParent(GameObject newParent)
        {
            parent = newParent;
            parentTransformVersion = newParent != null ? newParent.transformVersion - 1 : 1;
        }

        protected virtual void UpdateTransformations()
        {
            if (isDirty)
            {
                localTransform = transform.CreateMatrix();
            }
            if (isDirty || (parent != null && parentTransformVersion != parent.transformVersion) || (parent == null && parentTransformVersion != 0))
            {
                if (parent != null)
                {
                    worldTransform = localTransform * parent.worldTransform;
                    parentTransformVersion = parent.transformVersion;
                }
                else
                {
                    worldTransform = localTransform;
                    parentTransformVersion = 0;
                }
                OnWorldMatrixChange();
                UpdateColliders();
            }
            isDirty = false;
        }

        public Matrix LocalTransformations
        {
            get { UpdateTransformations(); return localTransform; }
        }

        public Matrix WorldTransformations
        {
            get { UpdateTransformations(); return worldTransform; }
        }

        public void ForceUpdate(bool recursive = true)
        {
            if (!visible)
            {
                return;
            }

            UpdateTransformations();

            if (recursive)
            {
                foreach (GameObject node in childs)
                {
                    node.ForceUpdate(recursive);
                }
            }
        }

        public void ResetTransformations()
        {
            transform = new Transformation();
            OnTransformationsSet();
        }

        public TransformationOrder TransformationsOrder
        {
            get { return transform.transformOrder; }
            set { transform.transformOrder = value; OnTransformationsSet(); }
        }

        public RotationOrder RotationOrder
        {
            get { return transform.rotationOrder; }
            set { transform.rotationOrder = value; OnTransformationsSet(); }
        }

        public Vector3 Position
        {
            get { return transform.position; }
            set { if (transform.position != value) OnTransformationsSet(); transform.position = value; }
        }

        public Vector3 GetWorldPosition()
        {
            Vector3 pos;
            Quaternion rot;
            Vector3 scl;
            WorldTransformations.Decompose(out scl, out rot, out pos);
            return pos;
        }

        public Vector3 Scale
        {
            get { return transform.scale; }
            set { if (transform.scale != value) OnTransformationsSet(); transform.scale = value; }
        }

        public Vector3 Rotation
        {
            get { return transform.rotation; }
            set { if (transform.rotation != value) OnTransformationsSet(); transform.rotation = value; }
        }

        public float RotationX
        {
            get { return transform.rotation.X; }
            set { if (transform.rotation.X != value) OnTransformationsSet(); transform.rotation.X = value; }
        }

        public float RotationY
        {
            get { return transform.rotation.Y; }
            set { if (transform.rotation.Y != value) OnTransformationsSet(); transform.rotation.Y = value; }
        }

        public float RotationZ
        {
            get { return transform.rotation.Z; }
            set { if (transform.rotation.Z != value) OnTransformationsSet(); transform.rotation.Z = value; }
        }

        public void SetModelQuat(Quaternion quat)
        {
            transform.useModelQuat = true;
            transform.modelQuat = quat;
        }

        public void UnSetModelQuat()
        {
            transform.useModelQuat = false;
        }

        public float ScaleX
        {
            get { return transform.scale.X; }
            set { if (transform.scale.X != value) OnTransformationsSet(); transform.scale.X = value; }
        }

        public float ScaleY
        {
            get { return transform.scale.Y; }
            set { if (transform.scale.Y != value) OnTransformationsSet(); transform.scale.Y = value; }
        }

        public float ScaleZ
        {
            get { return transform.scale.Z; }
            set { if (transform.scale.Z != value) OnTransformationsSet(); transform.scale.Z = value; }
        }

        public float PositionX
        {
            get { return transform.position.X; }
            set { if (transform.position.X != value) OnTransformationsSet(); transform.position.X = value; }
        }

        public float PositionY
        {
            get { return transform.position.Y; }
            set { if (transform.position.Y != value) OnTransformationsSet(); transform.position.Y = value; }
        }

        public float PositionZ
        {
            get { return transform.position.Z; }
            set { if (transform.position.Z != value) OnTransformationsSet(); transform.position.Z = value; }
        }

        public void Translate(Vector3 moveBy)
        {
            transform.position += moveBy;
            OnTransformationsSet();
        }

        public virtual void OnChildWorldMatrixChange(GameObject node)
        {
        }

        public bool Empty
        {
            get { return components.Count == 0 && childs.Count == 0; }
        }

        public bool HaveComponents
        {
            get { return components.Count != 0; }
        }

        public virtual BoundingBox GetBoundingBox(bool includeChildNodes = true)
        {
            if (Empty)
            {
                return EmptyBoundingBox;
            }
            UpdateTransformations();
            List<Vector3> corners = new List<Vector3>();

            if (includeChildNodes)
            {
                foreach (GameObject child in childs)
                {
                    if (!child.visible)
                    {
                        continue;
                    }

                    BoundingBox currBox = child.GetBoundingBox();
                    if (currBox.Min != currBox.Max)
                    {
                        corners.Add(currBox.Min);
                        corners.Add(currBox.Max);
                    }
                }
            }

            foreach (Component component in components)
            {
                if (!component.Visible)
                {
                    continue;
                }

                BoundingBox currBox = component.GetBoundingBox(this, localTransform, worldTransform);
                if (currBox.Min != currBox.Max)
                {
                    corners.Add(currBox.Min);
                    corners.Add(currBox.Max);
                }
            }
            if (corners.Count == 0)
            {
                return EmptyBoundingBox;
            }
            return BoundingBox.CreateFromPoints(corners);
        }


        public void CreateColliders()
        {
            foreach (Component component in components)
            {
                if(component.GetType() == typeof(ModelComponent) || component.GetType() == typeof(LoadLevel1Trigger) || component.GetType() == typeof(EndLevelTrigger) || component.GetType() == typeof(ModelAnimatedComponent) || component.GetType() == typeof(CameraCollisions) || component.GetType() == typeof(TutorialTrigger))
                {
                    bool duplikat = false;
                    BoundingBox currBox = component.GetBoundingBox(this, localTransform, worldTransform);
                    var a = new Collider(currBox, component, this);
                    foreach (var collider in colliders)
                    {
                        if (collider.Component.Equals(a.Component))
                        {
                            duplikat = true;
                            a.isReadyToBeDisposed = true;
                        }
                    }

                    if (!duplikat)
                    {
                        colliders.Add(a);
                        if (this.parent != null)
                        {
                            if (this.parent.CheckIfPawn())
                            {
                                this.parent.colliders.Add(a);
                            }
                        }
                    }
                    else
                    {
                        duplikat = false;
                    }

                    break;
                }
            }

            foreach (GameObject child in childs)
            {
                child.CreateColliders();
            }
        }

        public void CreateTriggers()
        {

        }

        public void UpdateColliders()
        {
            foreach (Collider col in colliders)
            {
                    BoundingBox currBox = col.Component.GetBoundingBox(this, localTransform, worldTransform);
                    col.BoundingBox = currBox;
                    //Debug.WriteLine("Collider");
                    //Debug.WriteLine(currBox);
            }
        }

        public void CollisionUpdate()
        {
            foreach (Collider col in colliders)
            {
                col.CollisionUpdate();
            }

            foreach (GameObject child in childs)
            {
                foreach (Collider col in child.colliders)
                {
                    col.CollisionUpdate();
                }
            }
        }

        public T GetComponent<T>() where T : Component
        {
            if (this.HaveComponents)
            {
                foreach (T component in components.OfType<T>())
                {
                    return component;
                }
            }
                return null;
        }

        public bool CheckIfPawn()
        {
            foreach (Component component in components)
            {
                if (component is Pawn)
                {
                    return true;
                }
            }

            return false;
        }

        [Obsolete("GetModelComponent is deprecated, please use GetComponent<T>() instead.")]
        public ModelComponent GetModelComponent()
        {
            if (this.HaveComponents)
            {
                foreach (var component in components)
                {
                    if (component.GetType() == typeof(ModelComponent))
                    {
                        return component as ModelComponent;
                    }
                }
                throw new System.Exception("GameObj doesn't have ModelComponent");
            }
            else
            {
                throw new System.Exception("GameObj doesn't have any Components");
            }
        }

        [Obsolete("GetModelAnimatedComponent is deprecated, please use GetComponent<T>() instead.")]
        public ModelAnimatedComponent GetModelAnimatedComponent()
        {
            if (this.HaveComponents)
            {
                foreach (var component in components)
                {
                    if (component.GetType() == typeof(ModelAnimatedComponent))
                    {
                        return component as ModelAnimatedComponent;
                    }
                }
                throw new System.Exception("GameObj doesn't have ModelComponent");
            }
            else
            {
                throw new System.Exception("GameObj doesn't have any Components");
            }
        }

        public void SetAsTrigger(Trigger triggerType)
        {
            AddComponent(triggerType);
            foreach (var col in colliders)
            {
                col.isTrigger = true;
                col.isCollider = false;
            }
        }

        public void SetAsTrigger()
        {
            foreach (var col in colliders)
            {
                col.isTrigger = true;
                col.isCollider = false;
            }
        }

        public void SetAsColliderAndTrigger(Trigger triggerType)
        {
            if (this.CheckIfPawn())
            {
                var hat = this.FindChildNodeByTag("Hat");
                hat.AddComponent(triggerType);
                foreach (var col in hat.colliders)
                {
                    col.isTrigger = true;
                    col.isCollider = true;
                }
            }
            else
            {
                AddComponent(triggerType);
                foreach (var col in colliders)
                {
                    col.isTrigger = true;
                    col.isCollider = true;
                }
            }
            
        }

        public void Dispose()
        {
            parent?.childs.Remove(this);
            parent = null;
            foreach (var col in colliders)
            {
                col.isTrigger = false;
                col.isCollider = false;
                col.isReadyToBeDisposed = true;
            }
            colliders.Clear();
            for (int i = components.Count - 1; i >= 0; i--)
            {
                components[i].Dispose();
            }
            for (int i = childs.Count - 1; i >= 0; i--)
            {
                childs[i].Dispose();
            }
        }
    }
}
