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
            this.transform.transformOrder = TransformationOrder.ScalePositionRotation;
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
                this.Rotation  = new Vector3(this.RotationX, this.RotationY, this.RotationZ + playerSpeed/5);
            }
            if (inputManager.Keyboard[Keys.D])
            {
                this.Rotation = new Vector3(this.RotationX, this.RotationY, this.RotationZ - playerSpeed/5);
            }
        }
    }
}
