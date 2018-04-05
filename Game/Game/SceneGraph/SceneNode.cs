using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace PBLGame.SceneGraph
{
    public delegate void NodeEventCallback(SceneNode node);

    public class SceneNode
    {
        protected SceneNode parent = null;
        public SceneNode Parent { get { return parent; } }
        public static NodeEventCallback onTransformUpdate;
        public static NodeEventCallback onDraw;
        public string name;
        protected Transformation transform = new Transformation();
        public virtual bool visible { get; set; }
        protected static readonly BoundingBox EmptyBoundingBox = new BoundingBox();
        protected Matrix localTransform = Matrix.Identity;
        protected Matrix worldTransform = Matrix.Identity;

        protected List<SceneNode> childs = new List<SceneNode>();
        protected List<Entity> childEntities = new List<Entity>();
        protected bool isDirty = true;

        protected uint transformVersion = 0;
        public uint TransformVersion { get { return transformVersion; } }
        protected uint parentTransformVersion = 0;

        public SceneNode()
        {
            visible = true;
        }

        public virtual void Draw(Camera camera, string newName = "Node")
        {
            if (!visible)
            {
                return;
            }

            UpdateTransformations();

            foreach (SceneNode node in childs)
            {
                node.Draw(camera);
            }

            onDraw?.Invoke(this);

            foreach (Entity entity in childEntities)
            {
                entity.Draw(this, camera, localTransform, worldTransform);
            }
        }

        public void AddEntity(Entity entity)
        {
            childEntities.Add(entity);
        }

        public void RemoveEntity(Entity entity)
        {
            childEntities.Remove(entity);
        }

        public void AddChildNode(SceneNode node)
        {
            if (node.parent != null)
            {
                throw new System.Exception("Can't add");
            }
            childs.Add(node);
            node.SetParent(this);
        }

        public void RemoveChildNode(SceneNode node)
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

        protected virtual void OnTransformationsSet()
        {
            isDirty = true;
        }

        protected virtual void SetParent(SceneNode newParent)
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
                foreach (SceneNode node in childs)
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

        public virtual void OnChildWorldMatrixChange(SceneNode node)
        {
        }

        public bool Empty
        {
            get { return childEntities.Count == 0 && childs.Count == 0; }
        }

        public bool HaveEntities
        {
            get { return childEntities.Count != 0; }
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
                foreach (SceneNode child in childs)
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

            foreach (Entity entity in childEntities)
            {
                if (!entity.Visible)
                {
                    continue;
                }

                BoundingBox currBox = entity.GetBoundingBox(this, localTransform, worldTransform);
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
    }
}
