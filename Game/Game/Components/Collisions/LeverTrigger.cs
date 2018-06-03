using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PBLGame.SceneGraph;

namespace Game.Components.Collisions
{
    class LeverTrigger : Trigger
    {
        LeverComponent component;
        public LeverTrigger(GameObject owner, LeverComponent _component) : base(owner)
        {
            component = _component;
        }

        public override void OnTrigger(GameObject triggered)
        {
            component.direction = true;
        }
    }
}
