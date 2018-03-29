using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Game.SceneGraph
{
    class SceneNode
    {
        private SceneNode parent = null;
        private Transformation transform = new Transformation();
        private Matrix localTransform = Matrix.Identity;
        private Matrix worldTransform = Matrix.Identity;
        private List<SceneNode> childs = new List<SceneNode>();

        private bool isDirty = true; //true if Transformation change

        internal SceneNode Parent
        {
            get
            {
                return parent;
            }
        }

        public SceneNode()
        {

        }

        public virtual void Draw()
        {
            //UpdateTransformation();

            foreach (SceneNode node in childs)
            {
                node.Draw();
            }
        }

        public void AddChildNode(SceneNode node)
        {
            if (node.parent != null)
            {
                throw new System.Exception("Can't add a node that already have a parent.");
            }
            childs.Add(node);

            node.SetParent(this);
        }

        public void RemoveChildNode(SceneNode node)
        {
            if (node.parent != this)
            {
                throw new System.Exception("Can't remove a node that don't belong to this parent.");
            }
            childs.Remove(node);

            node.SetParent(null);
        }

        public void RemoveFromParent()
        {
            if (parent == null)
            {
                throw new System.Exception("Can't remove an orphan node from parent.");
            }
            parent.RemoveChildNode(this);
        }

        protected virtual void SetParent(SceneNode newParent)
        {
            parent = newParent;
        }

        protected virtual void UpdateTransformations()
        {
            if (isDirty)
            {
                localTransform = transform.CreateMatrix();
                if (parent != null)
                {
                    worldTransform = localTransform * parent.worldTransform;
                }
                else
                {
                    worldTransform = localTransform;
                }
            }
            isDirty = false;
        }
    }
}
