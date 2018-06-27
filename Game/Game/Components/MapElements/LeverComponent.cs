using Microsoft.Xna.Framework;
using PBLGame.SceneGraph;
using System;
using System.Collections.Generic;
using Game.Components.Audio;

namespace Game.Components.MapElements
{
    class LeverComponent : Component
    {
        GameObject parent;
        public bool direction = false;
        float startRotation;
        float endRotation = MathHelper.PiOver2;
        public int numberOfSteps;
        float stepSize = 0.1f;
        private AudioComponent audioComponent;
        private bool playedSound = false;

        public LeverComponent(GameObject _parent)
        {
            parent = _parent;
            parent.UnSetModelQuat();
            parent.RotationY = MathHelper.PiOver2;
            startRotation = parent.RotationZ;
            numberOfSteps = (int) Math.Abs((startRotation - endRotation) / stepSize);
            System.Diagnostics.Debug.WriteLine(numberOfSteps);
            parent.AddComponent(new AudioComponent(parent));
            audioComponent = parent.GetComponent<AudioComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            if (direction)
            {
                if (!playedSound)
                {
                    audioComponent?.PlaySound("lever");
                    playedSound = true;
                }

                parent.RotationZ += 6 * stepSize * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (parent.RotationZ > endRotation)
                {
                    direction = false;
                    playedSound = false;
                }
            }
            else
            {
                if(parent.RotationZ >= startRotation)
                {
                    parent.RotationZ -= 6 * stepSize * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (!playedSound)
                    {
                        audioComponent?.PlaySound("lever");
                        playedSound = true;
                    }
                }
                else
                {
                    playedSound = false;
                }
            }
        }
    }
}
