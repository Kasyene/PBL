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

        public override void OnTrigger(GameObject triggered)
        {
            owner.Dispose();
        }
    }
}
