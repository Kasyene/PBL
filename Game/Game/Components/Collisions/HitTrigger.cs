using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Components.Collisions;
using PBLGame.MainGame;
using PBLGame.SceneGraph;

namespace Game.Components.Collisions
{
    public class HitTrigger : Trigger
    {
        public List<GameObject> opponentsHit;

        public void ClearBoxList()
        {
            opponentsHit.Clear();
        }

        public HitTrigger(GameObject owner) : base(owner)
        {
            opponentsHit = new List<GameObject>();
        }

        public override void OnTrigger(GameObject whoHaveIHit)
        {
            if (owner.Parent.GetComponent<Pawn>().isAttacking && whoHaveIHit.CheckIfPawn() && whoHaveIHit.GetComponent<Pawn>().ObjectSide != owner.Parent.GetComponent<Pawn>().ObjectSide 
                && whoHaveIHit.GetComponent<Pawn>().ObjectSide != Side.Other && !opponentsHit.Contains(whoHaveIHit))
            {
                opponentsHit.Add(whoHaveIHit);
                whoHaveIHit.GetComponent<Pawn>().Hp -= owner.Parent.GetComponent<Pawn>().getDamage();
                whoHaveIHit.GetComponent<Pawn>().ReceiveHit();
                //TODO change isattacking after this instance of animation is finished in owner
                owner.Parent.GetComponent<Pawn>().isAttacking = false;
            }
        }
    }
}
