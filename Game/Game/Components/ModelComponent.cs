using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using System.CodeDom;
using System.Linq;
using Game.Misc;
using Microsoft.Xna.Framework.Content;

namespace PBLGame.SceneGraph
{
    public class ModelComponent : Component
    {
        public Model model;
        public Effect modelEffect;
        public BasicEffect lineEffect;
        Texture2D texture;
        short[] bBoxIndices = {
            0, 1, 1, 2, 2, 3, 3, 0, // Front edges
            4, 5, 5, 6, 6, 7, 7, 4, // Back edges
            0, 4, 1, 5, 2, 6, 3, 7 // Side edges connecting front and back
        };

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
                    modelEffect.Parameters["DirectionalSpecularColor"].SetValue(ShroomGame.directionalLight.specular);
                    modelEffect.Parameters["DirectionalLightViewProj"].SetValue(ShroomGame.directionalLight.CreateLightViewProjectionMatrix());
                    modelEffect.Parameters["PointLightNumber"].SetValue(ShroomGame.pointLights.Count);
                    modelEffect.Parameters["PointLightPosition"].SetValue(Lights.PointLight.GetPointLightsPositionArray());
                    modelEffect.Parameters["PointLightAttenuation"].SetValue(Lights.PointLight.GetPointLightsAttenuationArray());
                    //modelEffect.Parameters["PointAmbientColor[i]"].SetValue(ShroomGame.pointLights[i].ambient);
                    //modelEffect.Parameters["PointSpecularColor[i]"].SetValue(ShroomGame.pointLights[i].specular);
                    
                    if(!createShadowMap)
                    {
                        modelEffect.Parameters["ShadowMap"].SetValue(ShroomGame.shadowRenderTarget);
                    }

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
                DrawBoundingBox(parent, localTransformations, worldTransformations, camera);
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

        public void DrawBoundingBox(GameObject parent, Matrix localTransformations, Matrix worldTransformations, Camera camera)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                if (lineEffect == null)
                {
                    lineEffect = new BasicEffect(GameServices.GetService<GraphicsDevice>());
                }
               BoundingBox box = GetBoundingBox(parent, localTransformations, worldTransformations);
                Vector3[] corners = box.GetCorners();
                VertexPositionColor[] primitiveList = new VertexPositionColor[corners.Length];

                // Assign the 8 box vertices
                for (int i = 0; i < corners.Length; i++)
                {
                    primitiveList[i] = new VertexPositionColor(corners[i], Color.White);
                }

                lineEffect.World = Matrix.Identity;
                lineEffect.View = camera.ViewMatrix;
                lineEffect.Projection = camera.ProjectionMatrix;

                foreach (EffectPass pass in lineEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    GameServices.GetService<GraphicsDevice>().DrawUserIndexedPrimitives(
                        PrimitiveType.LineList, primitiveList, 0, 8,
                        bBoxIndices, 0, 12);
                }
            } 
        }

        private static void AddVertex(List<VertexPositionColor> vertices, Vector3 position)
        {
            vertices.Add(new VertexPositionColor(position, Color.White));
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
