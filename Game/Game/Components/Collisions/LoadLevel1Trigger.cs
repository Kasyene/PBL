﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PBLGame.SceneGraph;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PBLGame;
using Game.MainGame;
using Game.Misc.Time;

namespace Game.Components.Collisions
{
    class LoadLevel1Trigger : ConsumableTrigger
    {
        short[] bBoxIndices = {
            0, 1, 1, 2, 2, 3, 3, 0, // Front edges
            4, 5, 5, 6, 6, 7, 7, 4, // Back edges
            0, 4, 1, 5, 2, 6, 3, 7 // Side edges connecting front and back
        };

        Vector3 minimumPosition;
        Vector3 maximumPosition;
        ShroomGame game;


        public LoadLevel1Trigger(GameObject owner, Vector3 minPos, Vector3 maxPos, ShroomGame game) : base(owner)
        {
            minimumPosition = minPos;
            maximumPosition = maxPos;
            this.game = game;
        }

        public override void OnTrigger(GameObject triggered)
        {
            if (triggered?.tag == "player" && game.tutorialCompleted)
            {
                ShroomGame.loadLevelTime = Timer.gameTime.TotalGameTime.TotalSeconds;
                new Cutscene(game.Content.Load<Texture2D>("Cutscene/1.4"),12f, "Narrator: After the weird fight with Borovikus our hero found the throne room door locked.",
                    "Narrator: While he was looking for a way to break in, his attention was drawn by something unusual.");
                new Cutscene(game.Content.Load<Texture2D>("Cutscene/1.4"), 12f, "Player: Is it the newest painting by Michelshroomgelo? I know this scene, but I remember it differently...",
                    "Narrator: Our brave hero were lost in memories of scene depicted on the painting.");
                game.areCollidersAndTriggersSet = false;
                game.cutsceneLoaded = true;
                ShroomGame.actualGameState = GameState.LevelOne;
                base.OnTrigger(null);
            }
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
