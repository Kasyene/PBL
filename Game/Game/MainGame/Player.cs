using Microsoft.Xna.Framework.Graphics;
using PBLGame.Input;

namespace PBLGame.MainGame
{
    class Player : Pawn
    {
        private readonly InputManager inputManager;

        public Player() : base()
        {
            inputManager = InputManager.Instance;
        }

        public override void Update()
        {
            base.Update();
            PlayerMovement();

        }

        private void PlayerMovement()
        {
            //TODO playermovement
        }
    }
}
