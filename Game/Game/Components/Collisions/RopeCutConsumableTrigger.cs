using PBLGame.SceneGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Components.Collisions
{
    class RopeCutConsumableTrigger : ConsumableTrigger
    {
        BridgeComponent component;

        public RopeCutConsumableTrigger(GameObject owner, BridgeComponent _component) : base(owner)
        {
            component = _component;
        }

        public override void OnTrigger(GameObject triggered)
        {
            System.Diagnostics.Debug.WriteLine("Lina trafiona");
            component.dropBridge = true;
            base.OnTrigger(null);
        }
    }
}
