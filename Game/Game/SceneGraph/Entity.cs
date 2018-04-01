using Microsoft.Xna.Framework;

namespace PBLGame.SceneGraph
{
    public interface Entity
    {
        void Draw(SceneNode parent, Matrix localTransformations, Matrix worldTransformations);

        BoundingBox GetBoundingBox(SceneNode parent, Matrix localTransformations, Matrix worldTransformations);

        bool Visible { get; set; }
    }
}
