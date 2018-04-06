using Microsoft.Xna.Framework;

namespace PBLGame.SceneGraph
{
    public class Component
    {
        protected bool visible;

        public virtual void Draw(GameObject parent, Camera camera, Matrix localTransformations, Matrix worldTransformations)
        {
        }

        public virtual BoundingBox GetBoundingBox(GameObject parent, Matrix localTransformations, Matrix worldTransformations)
        {
            return new BoundingBox();
        }

        public bool Visible
        {
            get
            {
                return visible;
            }

            set
            {
                visible = value;
            }
        }
    }
}
