using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PBLGame.SceneGraph;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game.MainGame;
using PBLGame;

namespace Game.Components.Collisions
{
    class EndLevelTrigger : ConsumableTrigger
    {
        short[] bBoxIndices = {
            0, 1, 1, 2, 2, 3, 3, 0, // Front edges
            4, 5, 5, 6, 6, 7, 7, 4, // Back edges
            0, 4, 1, 5, 2, 6, 3, 7 // Side edges connecting front and back
        };

        Vector3 minimumPosition;
        Vector3 maximumPosition;
        ShroomGame game;

        public EndLevelTrigger(GameObject owner, Vector3 minPos, Vector3 maxPos, ShroomGame game) : base(owner)
        {
            minimumPosition = minPos;
            maximumPosition = maxPos;
            this.game = game;
        }

        public override void OnTrigger(GameObject triggered)
        {
            if (triggered?.tag == "player")
            {
                new Cutscene(game.Content.Load<Texture2D>("Cutscene/2.1"), 6f, "Narrator: At the end of the secret mission, while collecting enemy data our hero felt into an ambush.",
                    "Narrator: He was surrounded and outnumbered by enemies.",
                    "Player: I will not go down easily!");
                new Cutscene(game.Content.Load<Texture2D>("Cutscene/2.2"), 5f, "Narrator: Opponents started to push our hero against the wall. Then a miracle happened.",
                    "Narrator: Borovikus was following our hero without a permission of the crown.",
                    "Narrator: When he saw how bad the situation was, he came to the rescue without much hesitation.");
                new Cutscene(game.Content.Load<Texture2D>("Cutscene/2.2"), 5f, "Borovikus: Hang on lad!",
                    "Player: Borovikus, you were not supposed to be here!",
                    "Borovikus: Screw the orders, I won't let you die here!");
                new Cutscene(game.Content.Load<Texture2D>("Cutscene/2.3"), 5f, "Narrator: With the help of Borovikus this hopeless situation turned into a fair fight.",
                    "Narrator: A few moments later the result of the fight was clear.");
                new Cutscene(game.Content.Load<Texture2D>("Cutscene/2.3"), 5f, "Narrator: The brave knights stood victorious on fallen enemies. The mission came to an end.",
                    "Borovikus: You're welcome.");
                game.levelOneCompleted = true;
                game.areCollidersAndTriggersSet = false;
                ShroomGame.actualGameState = GameState.LevelTutorial;
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
