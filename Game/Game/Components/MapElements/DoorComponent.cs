using Microsoft.Xna.Framework;
using PBLGame.SceneGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Components.Audio;
using PBLGame;

namespace Game.Components.MapElements
{
    public class DoorComponent : Component
    {
        GameObject parent;
        float startRotation;
        Vector3 startPosition;
        float openRotation;
        Vector3 openPosition;
        public bool closed = false;
        public bool playedsound = false;
        protected AudioComponent audioComponent;

        public DoorComponent(GameObject _parent, float targetRotation, Vector3 openPosition)
        {
            parent = _parent;
            parent.UnSetModelQuat();
            parent.RotationY = -MathHelper.PiOver2;
            startRotation = 0;
            startPosition = parent.Position;
            this.openRotation = targetRotation;
            this.openPosition = parent.Position + openPosition;
            parent.AddComponent(new AudioComponent(parent));
            audioComponent = parent.GetComponent<AudioComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            if (closed)
            {
                parent.RotationZ = startRotation;
                parent.Position = startPosition;
                if (!playedsound && !GameServices.GetService<ShroomGame>().levelOneCompleted)
                {
                    playedsound = true;
                    audioComponent?.PlaySound("doors");
                }
            }
            else
            {
                parent.RotationZ = openRotation;
                parent.Position = openPosition;
            }
        }
    }
}
