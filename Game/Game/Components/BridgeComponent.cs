using Microsoft.Xna.Framework;
using PBLGame.SceneGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Components
{
    class BridgeComponent : Component
    {
        GameObject parent;
        public bool dropBridge = true;

        public BridgeComponent(GameObject _parent)
        {
            parent = _parent;
        }

        public override void Update(GameTime gameTime)
        {
            if(dropBridge)
            {
                parent.UnSetModelQuat();
                parent.RotationZ += 0.2f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (parent.RotationZ > MathHelper.PiOver2)
                {
                    dropBridge = false;
                }
            }
        }
    }
}
