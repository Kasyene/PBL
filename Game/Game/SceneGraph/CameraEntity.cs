
using System;
using Microsoft.Xna.Framework;

namespace PBLGame.SceneGraph
{
    class CameraEntity : Entity
    {
        public bool Visible
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public void Draw(SceneNode parent, Matrix localTransformations, Matrix worldTransformations)
        {
            throw new NotImplementedException();
        }

        public BoundingBox GetBoundingBox(SceneNode parent, Matrix localTransformations, Matrix worldTransformations)
        {
            throw new NotImplementedException();
        }
    }
}
