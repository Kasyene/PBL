﻿using Game.Components.Collisions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PBLGame.SceneGraph;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Components.Coliisions
{
    class TutorialTrigger : Trigger
    {
        short[] bBoxIndices = {
            0, 1, 1, 2, 2, 3, 3, 0, // Front edges
            4, 5, 5, 6, 6, 7, 7, 4, // Back edges
            0, 4, 1, 5, 2, 6, 3, 7 // Side edges connecting front and back
        };

        Vector3 minimumPosition;
        Vector3 maximumPosition;

        public TutorialTrigger(GameObject owner, Vector3 minPos, Vector3 maxPos) : base(owner)
        {
            minimumPosition = minPos;
            maximumPosition = maxPos;
        }

        public override void OnTrigger(GameObject triggered)
        {
            if (triggered?.tag == "player")
            {
                System.Diagnostics.Debug.WriteLine("Triggered by player!!!");
            }
            System.Diagnostics.Debug.WriteLine("Triggered!!!");
            base.OnTrigger(triggered);
        }

        public override BoundingBox GetBoundingBox(GameObject parent, Matrix localTransformations, Matrix worldTransformations)
        {
            return new BoundingBox(minimumPosition, maximumPosition);
        }

        public void DrawBoundingBox(GameObject parent, Matrix localTransformations, Matrix worldTransformations, Camera camera)
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

        public override void Draw(GameObject parent, Camera camera, Matrix localTransformations, Matrix worldTransformations, bool createShadowMap = false)
        {
            DrawBoundingBox(parent, localTransformations, worldTransformations, camera);   
        }
    }
}