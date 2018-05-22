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
        private int hp = 100;
        private int timeEnergy = 100;

        public Player(GameObject parent) : base()
        {
            parentGameObject = parent;
            inputManager = InputManager.Instance;
            playerSpeed = 0.1f;
        }

        public override void Update(GameTime time)
        {
            base.Update(time);
            PlayerMovement();     
        }

        public Player GetPlayer()
        {
            return this;
        }

        private void PlayerMovement()
        {
            if (inputManager.Keyboard[Keys.W] && inputManager.Keyboard[Keys.A])
            {
                MoveWA(playerSpeed);
            }
            else if (inputManager.Keyboard[Keys.W] && inputManager.Keyboard[Keys.D])
            {
                MoveWD(playerSpeed);
            }
            else if (inputManager.Keyboard[Keys.S] && inputManager.Keyboard[Keys.A])
            {
                MoveSA(playerSpeed);
            }
            else if (inputManager.Keyboard[Keys.S] && inputManager.Keyboard[Keys.D])
            {
                MoveSD(playerSpeed);
            }
            else if (inputManager.Keyboard[Keys.A] && inputManager.Keyboard[Keys.D])
            {
                return;
            }
            else if (inputManager.Keyboard[Keys.W] && inputManager.Keyboard[Keys.S])
            {
                return;
            }
            else if (inputManager.Keyboard[Keys.W])
            {
                MoveForward(playerSpeed);
            }
            else if (inputManager.Keyboard[Keys.S])
            {
                MoveBack(playerSpeed);
            }
            else if (inputManager.Keyboard[Keys.A])
            {
                MoveRight(playerSpeed);
            }
            else if (inputManager.Keyboard[Keys.D])
            {
                MoveLeft(playerSpeed);
            }
            else if (parentGameObject.isGrounded && inputManager.Keyboard[Keys.Space])
            {
                isJumping = true;
            }
/*            else
            {
                CheckCollider();
            }*/
            Rotate(inputManager.Mouse.PositionsDelta.X * 0.01f);
        }

        public int GetHP()
        {
            return hp;
        }

        public int GetTimeEnergy()
        {
            return timeEnergy;
        }
    }
}
