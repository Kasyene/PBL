using Game.Components.Collisions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PBLGame.SceneGraph;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PBLGame;
using PBLGame.MainGame;
using Game.Misc.Time;

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
        ShroomGame game;

        public TutorialTrigger(GameObject owner, Vector3 minPos, Vector3 maxPos, ShroomGame game) : base(owner)
        {
            minimumPosition = minPos;
            maximumPosition = maxPos;
            this.game = game;
        }

        public override void OnTrigger(GameObject triggered)
        {
            if (triggered?.tag == "player")
            {
                if(game.usedE == false && game.player.GetComponent<Player>().canUseE == false)
                {
                    new DialogueString("Press ‘space’ to jump.");
                    new DialogueString("Don't worry.");
                    new DialogueString("If you fall use 'E' to go back in time.");
                    game.player.GetComponent<Player>().canUseE = true;
                }

                if (game.usedE == true && game.usedR == false && game.player.GetComponent<Player>().canUseR == false)
                {
                    new DialogueString("Right-click to throw your hat.");
                    new DialogueString("Then press 'R' to teleport to the hat.");
                    game.player.GetComponent<Player>().canUseR = true;
                }

                if (game.usedR == true && game.usedQ == false && game.player.GetComponent<Player>().canUseQ == false)
                {
                    new DialogueString("Press ‘Q’ to stop the time.");
                    new DialogueString("It will affect enemies and your surrounding.");
                    game.player.GetComponent<Player>().canUseQ = true;
                }

                System.Diagnostics.Debug.WriteLine("Triggered at time: " + Timer.gameTime.TotalGameTime.Seconds);
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
