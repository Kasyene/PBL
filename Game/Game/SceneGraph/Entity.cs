using Microsoft.Xna.Framework;

namespace PBLGame.SceneGraph
{
    public class Entity
    {
        public void Draw(SceneNode parent, Matrix localTransformations, Matrix worldTransformations)
        {

        }

        public BoundingBox GetBoundingBox(SceneNode parent, Matrix localTransformations, Matrix worldTransformations)
        {
            return new BoundingBox();
        }
        public bool Visible { get; set; }
    }
}
