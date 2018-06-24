using Microsoft.Xna.Framework;
using PBLGame.SceneGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Components.MapElements
{
    class PlatformComponent : Component
    {
        GameObject parent;
        Vector3 startPosition;
        Vector3 targetPosition;
        float moveStep;
        float time;
        float stayTime;
        float hideTime;
        bool direction = false;
        bool stay = true;

        public PlatformComponent(GameObject _parent, Vector3 _target, float _stayTime, float _hideTime)
        {
            parent = _parent;
            startPosition = parent.Position;
            targetPosition = _target;
            stayTime = _stayTime;
            hideTime = _hideTime;
            time = stayTime;
            moveStep = startPosition.X - targetPosition.X / 3000;
        }

        public override void Update(GameTime gameTime)
        {
            if (!stay)
            {
                if (direction)
                {
                    parent.PositionX -= Math.Abs(moveStep * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    if (parent.PositionX < targetPosition.X)
                    {
                        stay = true;
                        time = hideTime;
                    }                
                }
                else
                {
                    parent.PositionX += Math.Abs(moveStep * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    if (parent.PositionX > startPosition.X)
                    {
                        stay = true;
                        time = stayTime;
                    }
                }
            }
            else
            {
                time -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if(time <0)
                {
                    direction = !direction;
                    stay = !stay;
                }
            }
        }
    }
}
