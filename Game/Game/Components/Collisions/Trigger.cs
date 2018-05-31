using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PBLGame.SceneGraph;

namespace Game.Components.Collisions
{
    public class Trigger : Component
    {
        public PBLGame.SceneGraph.GameObject owner;

        public Trigger(GameObject owner)
        {
            this.owner = owner;
        }

        public virtual void OnTrigger()
        {

        }
    }
}
