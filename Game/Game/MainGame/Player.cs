using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PBLGame.Input;
using PBLGame.SceneGraph;

namespace PBLGame.MainGame
{
    class Player : Pawn
    {
        private readonly InputManager inputManager;
        private float playerSpeed;

        public Player() : base()
        {
            inputManager = InputManager.Instance;
            playerSpeed = 0.5f;
        }

        public override void Update()
        {
            base.Update();
            PlayerMovement();     
        }

        private void PlayerMovement()
        {
            if (inputManager.Keyboard[Keys.W])
            {
                MoveForward(playerSpeed);              
            }
            if (inputManager.Keyboard[Keys.S])
            {
                MoveBack(playerSpeed);
            }
            if (inputManager.Keyboard[Keys.A])
            {
                MoveRight(playerSpeed);
            }
            if (inputManager.Keyboard[Keys.D])
            {
                MoveLeft(playerSpeed);
            }
        }
    }
}
