using Microsoft.Xna.Framework.Graphics;
using PBLGame.SceneGraph;

namespace PBLGame.MainGame
{
    class Pawn : GameObject
    {
        public int Hp { get; internal set; }
        public Pawn() : base()
        {

        }
        public virtual void Damage()
        {
            this.Hp -= 1;
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
