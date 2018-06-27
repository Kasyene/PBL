using Game.Components.MapElements;
using PBLGame.SceneGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Components.Audio;

namespace Game.Components.Collisions
{
    class RopeCutConsumableTrigger : ConsumableTrigger
    {
        BridgeComponent component;
        protected AudioComponent audioComponent;

        public RopeCutConsumableTrigger(GameObject owner, BridgeComponent _component) : base(owner)
        {
            component = _component;
            owner.AddComponent(new AudioComponent(owner));
            audioComponent = owner.GetComponent<AudioComponent>();
        }

        public override void OnTrigger(GameObject triggered)
        {
            System.Diagnostics.Debug.WriteLine("Lina trafiona");
            audioComponent.PlaySound("cut");
            component.dropBridge = true;
            base.OnTrigger(null);
        }
    }
}
