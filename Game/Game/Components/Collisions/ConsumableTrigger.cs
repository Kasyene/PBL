using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PBLGame.SceneGraph;

namespace Game.Components.Collisions
{
    public class ConsumableTrigger : Trigger
    {
        public ConsumableTrigger(GameObject owner) : base(owner)
        {
        }

        public override void OnTrigger()
        {
            if (owner.tag.Contains("Serce"))
            {
                Debug.WriteLine("Increase Hp of player");
            }

            foreach (var ownerCollider in owner.colliders)
            {
                ownerCollider.isTrigger = false;
                ownerCollider.isReadyToBeDisposed = true;
            }

            owner.Dispose();
        }
    }
}
