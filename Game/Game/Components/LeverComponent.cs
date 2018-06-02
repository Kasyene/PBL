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

        public LeverComponent(GameObject _parent)
        {
            parent = _parent;
            parent.UnSetModelQuat();
            parent.RotationY = MathHelper.PiOver2;
            startRotation = parent.RotationZ;
        }

        public override void Update(GameTime gameTime)
        {
            if (direction)
            {
                parent.RotationZ += 0.4f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (parent.RotationZ > MathHelper.PiOver2)
                {
                    direction = false;
                }
            }
            else
            {
                if(parent.RotationZ >= startRotation)
                {
                    parent.RotationZ -= 0.1f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }
        }
    }
}
