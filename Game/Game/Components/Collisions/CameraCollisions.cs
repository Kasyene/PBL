﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Misc.Time;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PBLGame.SceneGraph;

namespace Game.Components.Collisions
{
    class CameraCollisions : Trigger
    {
        public double timePassedFromLastOnTrigger = 0.0d;
        public double timeHelper = 0.0d;
        short[] bBoxIndices =
        {
            0, 1, 1, 2, 2, 3, 3, 0, // Front edges
            4, 5, 5, 6, 6, 7, 7, 4, // Back edges
            0, 4, 1, 5, 2, 6, 3, 7 // Side edges connecting front and back
        };

        public CameraCollisions(GameObject owner) : base(owner)
        {
            Visible = true;
        }

        private float setMinValue(float camera, float player)
        {
            return camera <= player ? camera : player;
        }

        private float setMaxValue(float camera, float player)
        {
            return camera <= player ? player : camera;
        }

        public override BoundingBox GetBoundingBox(GameObject parent, Matrix localTransformations,
            Matrix worldTransformations)
        {
            Vector3 cameraPosition = GameServices.GetService<Camera>().GetWorldPosition();
            Vector3 playerPos = GameServices.GetService<GameObject>().Position;
            Vector3 min = new Vector3();
            Vector3 max = new Vector3();
            min.X = setMinValue(cameraPosition.X, playerPos.X);
            max.X = setMaxValue(cameraPosition.X, playerPos.X);
            min.Y = setMinValue(cameraPosition.Y, playerPos.Y);
            max.Y = setMaxValue(cameraPosition.Y, playerPos.Y);
            min.Z = setMinValue(cameraPosition.Z, playerPos.Z);
            max.Z = setMaxValue(cameraPosition.Z, playerPos.Z);
            return new BoundingBox(min + new Vector3(0f, 50f, 0f), max);
        }

        public override void OnTrigger(GameObject triggered)
        {
            GameServices.GetService<Camera>().zoomChangedByCollision = true;
            GameServices.GetService<Camera>().zoomCanReturnToPlayerZoom = false;
            timePassedFromLastOnTrigger = 0.0d;
            if (triggered.tag == "Wall" || triggered.tag == "Ground")
            {
                GameServices.GetService<Camera>().Scroll(1.0f);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (GameServices.GetService<Camera>().zoomChangedByCollision)
            {
                timePassedFromLastOnTrigger += gameTime.TotalGameTime.TotalMilliseconds - timeHelper; 
            }
            timeHelper = gameTime.TotalGameTime.TotalMilliseconds;
            if (timePassedFromLastOnTrigger > 1500.0d)
            {
                GameServices.GetService<Camera>().zoomChangedByCollision = false;
                GameServices.GetService<Camera>().zoomCanReturnToPlayerZoom = true;
            }
            base.Update(gameTime);
        }

        public void DrawBoundingBox(GameObject parent, Matrix localTransformations, Matrix worldTransformations,
            Camera camera)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                BasicEffect lineEffect = new BasicEffect(GameServices.GetService<GraphicsDevice>());

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

        public override void Draw(GameObject parent, Camera camera, Matrix localTransformations,
            Matrix worldTransformations, bool createShadowMap = false)
        {
            //DrawBoundingBox(parent, localTransformations, worldTransformations, camera);   
        }
    }
}