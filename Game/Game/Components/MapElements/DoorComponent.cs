using Microsoft.Xna.Framework;
using PBLGame.SceneGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Components.MapElements
{
    class DoorComponent : Component
    {
        GameObject parent;
        float startRotation;
        Vector3 startPosition;
        float openRotation;
        Vector3 openPosition;
        public bool closed = false;

        public DoorComponent(GameObject _parent, float targetRotation, Vector3 openPosition)
        {
            parent = _parent;
            parent.UnSetModelQuat();
            parent.RotationY = -MathHelper.PiOver2;
            startRotation = 0;
            startPosition = parent.Position;
            this.openRotation = targetRotation;
            this.openPosition = parent.Position + openPosition;
        }

        public override void Update(GameTime gameTime)
        {
            if (closed)
            {
                parent.RotationZ = startRotation;
                parent.Position = startPosition;
            }
            else
            {
                parent.RotationZ = openRotation;
                parent.Position = openPosition;
            }
        }
    }
}
