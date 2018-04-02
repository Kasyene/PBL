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
                foreach (BasicEffect effect in mesh.Effects)
                {
                    // set world matrix
                    effect.World = worldTransformations;

                    // set view matrix
                    effect.View = camera.ViewMatrix;

                    // set projection matrix
                    effect.Projection = camera.ProjectionMatrix;
                }
                mesh.Draw();
            }
        }

        public BoundingBox GetBoundingBox(SceneNode parent, Matrix localTransformations, Matrix worldTransformations)
        {
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                    int vertexBufferSize = meshPart.NumVertices * vertexStride;

                    float[] vertexData = new float[vertexBufferSize / sizeof(float)];
                    meshPart.VertexBuffer.GetData<float>(vertexData);
                
                    for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
                    {
                        Vector3 currPosition = new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]);
                        currPosition = Vector3.Transform(currPosition, worldTransformations);
                        min = Vector3.Min(min, currPosition);
                        max = Vector3.Max(max, currPosition);
                    }
                }
            }
            return new BoundingBox(min, max);
        }
    }
}
