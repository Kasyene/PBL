using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace PBLGame.SceneGraph
{
    class ModelEntity : Entity
    {
        public Model model;

        public Matrix Projection;

        public Matrix View;


        public ModelEntity(Model loadedeModel)
        {
            model = loadedeModel;

            float aspectRatio = 1f;
            float fieldOfView = Microsoft.Xna.Framework.MathHelper.PiOver4;
            float nearClipPlane = 1;
            float farClipPlane = 200;
            Projection = Matrix.CreatePerspectiveFieldOfView(
                    fieldOfView, aspectRatio, nearClipPlane, farClipPlane);

            // Create default View matrix (camera)
            var cameraPosition = new Vector3(0, 0, 50);
            var cameraLookAtVector = Vector3.Zero;
            var cameraUpVector = Vector3.UnitY;
            View = Matrix.CreateLookAt(cameraPosition, cameraLookAtVector, cameraUpVector);
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

        public void Draw(SceneNode parent, Matrix localTransformations, Matrix worldTransformations)
        {
            foreach (var mesh in model.Meshes)
            {
                // iterate effect in mesh
                foreach (BasicEffect effect in mesh.Effects)
                {
                    // set world matrix
                    effect.World = worldTransformations;

                    // set view matrix
                    effect.View = View;

                    // set projection matrix
                    effect.Projection = Projection;
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
