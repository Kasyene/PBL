using PBLGame.SceneGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Components.Collisions
{
    class RopeTrigger : Trigger
    {
        BridgeComponent component;

        public RopeTrigger(GameObject owner, BridgeComponent _component) : base(owner)
        {
            System.Diagnostics.Debug.WriteLine("Rope Trigger Żyje");
            component = _component;
        }

        public override void OnTrigger()
        {
            System.Diagnostics.Debug.WriteLine("Lina trafiona");
            component.dropBridge = true;
            owner.Dispose();
        }
    }
}
