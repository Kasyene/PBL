using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace PBLGame.SceneGraph
{
    class ModelEntity : Entity
    {
        public Model model;

        public ModelEntity(Model loadedeModel)
        {
            model = loadedeModel;
        }

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

        public void Draw(SceneNode parent, Camera camera, Matrix localTransformations, Matrix worldTransformations)
        {
            foreach (var mesh in model.Meshes)
            {
                // iterate effect in mesh
                foreach (BasicEffect effect in mesh.Effects)
                {
                    // set world matrix
                    effect.World = worldTransformations;

                    // set view matrix
                    effect.View = camera.ViewMatrix;

                    // set projection matrix
                    effect.Projection = camera.ProjectionMatrix;
                }

                // draw current mesh
                mesh.Draw();
            }
        }

        public BoundingBox GetBoundingBox(SceneNode parent, Matrix localTransformations, Matrix worldTransformations)
        {
            throw new NotImplementedException();
        }
    }
}
