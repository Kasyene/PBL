using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace PBLGame.SceneGraph
{
    public class Component : IDisposable
    {
        protected bool visible;

        public virtual void Draw(GameObject parent, Camera camera, Matrix localTransformations, Matrix worldTransformations)
        {
        }

        public virtual BoundingBox GetBoundingBox(GameObject parent, Matrix localTransformations, Matrix worldTransformations)
        {
            return new BoundingBox();
        }

        public virtual void Update(GameTime gameTime) { }

        public virtual void LoadContent(ContentManager contentManager) { }

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

        public virtual void Dispose()
        {
            
        }
    }
}
