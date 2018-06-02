using Microsoft.Xna.Framework;
using PBLGame.SceneGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Components
{
    class LeverComponent : Component
    {
        GameObject parent;
        public bool direction = false;
        float startRotation;
        float endRotation = MathHelper.PiOver2;
        public int numberOfSteps;
        float stepSize = 0.1f;

        public LeverComponent(GameObject _parent)
        {
            parent = _parent;
            parent.UnSetModelQuat();
            parent.RotationY = MathHelper.PiOver2;
            startRotation = parent.RotationZ;
            numberOfSteps = (int) Math.Abs((startRotation - endRotation) / stepSize);
            System.Diagnostics.Debug.WriteLine(numberOfSteps);
        }

        public override void Update(GameTime gameTime)
        {
            if (direction)
            {
                parent.RotationZ += 4* stepSize * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (parent.RotationZ > endRotation)
                {
                    direction = false;
                }
            }
            else
            {
                if(parent.RotationZ >= startRotation)
                {
                    parent.RotationZ -= stepSize * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }
        }
    }
}
