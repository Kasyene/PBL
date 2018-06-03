using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PBLGame.MainGame;
using PBLGame.SceneGraph;

namespace Game.Components.Collisions
{
    class HeartConsumableTrigger : ConsumableTrigger
    {
        private int value;
        public HeartConsumableTrigger(GameObject owner) : base(owner)
        {
            value = 3;
        }

        public override void OnTrigger()
        {
            var player = GameServices.GetService<GameObject>().GetComponent<Player>();
            if (player.Hp < player.MaxHp)
            {
                Debug.WriteLine("INCREASE HP");
                base.OnTrigger();
                if (player.Hp >= (player.MaxHp - value))
                {
                    GameServices.GetService<GameObject>().GetComponent<Player>().Hp = player.MaxHp;
                }
                else
                {
                    GameServices.GetService<GameObject>().GetComponent<Player>().Hp = player.Hp + value;
                }
            }
            
        }
    }
}
