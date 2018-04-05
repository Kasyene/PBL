using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PBLGame.Input;

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
                this.Translate(new Vector3(0f, 0f, playerSpeed));
            }
            if (inputManager.Keyboard[Keys.S])
            {
                this.Translate(new Vector3(0f, 0f, -playerSpeed));
            }
            if (inputManager.Keyboard[Keys.A])
            {
                this.Translate(new Vector3(playerSpeed, 0f, 0f));
            }
            if (inputManager.Keyboard[Keys.D])
            {
                this.Translate(new Vector3(-playerSpeed, 0f, 0f));
            }
        }
    }
}
