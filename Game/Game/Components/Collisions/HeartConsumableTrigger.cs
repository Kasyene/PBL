using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PBLGame.SceneGraph;

namespace Game.Components.Collisions
{
    class HeartConsumableTrigger : ConsumableTrigger
    {
        public HeartConsumableTrigger(GameObject owner) : base(owner)
        {
        }

        public override void OnTrigger()
        {
            Debug.WriteLine("Increase Hp of player");
            base.OnTrigger();
        }
    }
}
