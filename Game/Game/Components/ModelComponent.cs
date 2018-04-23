using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using System.CodeDom;
using Game.Misc;
using Microsoft.Xna.Framework.Content;

namespace PBLGame.SceneGraph
{
    public class ModelComponent : Component
    {
        public Model model;
        public Effect modelEffect;
        Texture2D texture;

        public ModelComponent(Model _model, Effect _modelEffect, Texture2D _texture = null)
        {
            model = _model;
            modelEffect = _modelEffect;
            texture = _texture;
            Visible = true;
        }

        public override void Draw(GameObject parent, Camera camera, Matrix localTransformations, Matrix worldTransformations, bool createShadowMap = false)
        {
            string techniqueName = createShadowMap ? "CreateShadowMap" : "Draw";
            foreach (var mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = modelEffect;
                    modelEffect.CurrentTechnique = modelEffect.Techniques[techniqueName];
                    modelEffect.Parameters["World"].SetValue(worldTransformations);
                    modelEffect.Parameters["View"].SetValue(camera.ViewMatrix);
                    modelEffect.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
                    modelEffect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(worldTransformations)));
                    modelEffect.Parameters["ViewVector"].SetValue(camera.GetViewVector());
                    modelEffect.Parameters["DirectionalLightDirection"].SetValue(ShroomGame.directionalLight.direction);
                    modelEffect.Parameters["DirectionalAmbientColor"].SetValue(ShroomGame.directionalLight.ambient);
                    modelEffect.Parameters["DirectionalDiffuseColor"].SetValue(ShroomGame.directionalLight.diffuse);
                    modelEffect.Parameters["DirectionalSpecularColor"].SetValue(ShroomGame.directionalLight.specular);
                    modelEffect.Parameters["DirectionalLightViewProj"].SetValue(ShroomGame.directionalLight.CreateLightViewProjectionMatrix());
                    if (texture != null)
                    {
                        modelEffect.Parameters["ModelTexture"].SetValue(texture);
                    }
                    else
                    {
                        modelEffect.Parameters["ModelTexture"].SetValue(ShroomGame.missingTexture);
                    }
                }
                mesh.Draw();
            }
        }

        public override BoundingBox GetBoundingBox(GameObject parent, Matrix localTransformations, Matrix worldTransformations)
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

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void LoadContent(ContentManager contentManager)
        {
            base.LoadContent(contentManager);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
