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
            component = _component;
        }

        public override void OnTrigger()
        {
            System.Diagnostics.Debug.WriteLine("Lina trafiona");
            component.dropBridge = true;
            foreach (var ownerCollider in owner.colliders)
            {
                ownerCollider.isTrigger = false;
                ownerCollider.isReadyToBeDisposed = true;
            }
            owner.Dispose();
        }
    }
}
