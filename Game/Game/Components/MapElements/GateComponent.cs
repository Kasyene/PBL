using Microsoft.Xna.Framework;
using PBLGame.SceneGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Components.MapElements
{
    class GateComponent : Component
    {
        GameObject parent;
        LeverComponent component;
        float startPosition;
        float targetPosition;
        float stepSize;

        public GateComponent(GameObject _parent, LeverComponent _component)
        {
            parent = _parent;
            component = _component;
            startPosition = parent.Position.Y;
            targetPosition = startPosition + 100f;
            stepSize = Math.Abs((startPosition - targetPosition) / component.numberOfSteps);
        }

        public override void Update(GameTime gameTime)
        {
            if (component.direction)
            {
                parent.PositionY += 6 * stepSize * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                if (parent.PositionY >= startPosition)
                {
                    parent.PositionY -= 6 * stepSize * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }
        }
    }
}
