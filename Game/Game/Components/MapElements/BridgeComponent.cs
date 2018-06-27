using Game.Components.Audio;
using Microsoft.Xna.Framework;
using PBLGame.SceneGraph;

namespace Game.Components.MapElements
{
    class BridgeComponent : Component
    {
        GameObject parent;
        private bool playedSound = false;
        private AudioComponent audioComponent;
        public bool dropBridge = false;

        public BridgeComponent(GameObject _parent)
        {
            parent = _parent;
            parent.AddComponent(new AudioComponent(parent));
            audioComponent = parent.GetComponent<AudioComponent>();
        }

        public override void Update(GameTime gameTime)
        {
            if(dropBridge)
            {
                if (!playedSound)
                {
                    playedSound = true;
                    audioComponent?.PlaySound("bridge");
                }
                
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
