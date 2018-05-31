using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PBLGame.Input;
using PBLGame.Input.Devices;
using PBLGame.Misc.Anim;
using PBLGame.SceneGraph;

namespace PBLGame.MainGame
{
    class Player : Pawn
    {
        private readonly InputManager inputManager;
        private float playerSpeed;
        private int hp = 100;
        private int timeEnergy = 100;
        GameObject playerHat;
        GameObject playerLeg;

        public Player(GameObject parent) : base()
        {
            parentGameObject = parent;
            inputManager = InputManager.Instance;
            playerSpeed = 0.1f;
            playerHat = parentGameObject.FindChildNodeByTag("Hat");
            playerLeg = parentGameObject.FindChildNodeByTag("Leg");
        }

        public override void Update(GameTime time)
        {
            base.Update(time);
            PlayerAttacks();
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
                if (playerHat.GetComponent<AnimationManager>().defaultKey != "walk")
                {
                    playerHat.GetComponent<AnimationManager>().SetDefaultAnimation("walk");
                    playerLeg.GetComponent<AnimationManager>().SetDefaultAnimation("walk");
                }
            }
            else if (inputManager.Keyboard[Keys.W] && inputManager.Keyboard[Keys.D])
            {
                MoveWD(playerSpeed);
                if (playerHat.GetComponent<AnimationManager>().defaultKey != "walk")
                {
                    playerHat.GetComponent<AnimationManager>().SetDefaultAnimation("walk");
                    playerLeg.GetComponent<AnimationManager>().SetDefaultAnimation("walk");
                }
            }
            else if (inputManager.Keyboard[Keys.S] && inputManager.Keyboard[Keys.A])
            {
                MoveSA(playerSpeed);
                if (playerHat.GetComponent<AnimationManager>().defaultKey != "walk")
                {
                    playerHat.GetComponent<AnimationManager>().SetDefaultAnimation("walk");
                    playerLeg.GetComponent<AnimationManager>().SetDefaultAnimation("walk");
                }
            }
            else if (inputManager.Keyboard[Keys.S] && inputManager.Keyboard[Keys.D])
            {
                MoveSD(playerSpeed);
                if (playerHat.GetComponent<AnimationManager>().defaultKey != "walk")
                {
                    playerHat.GetComponent<AnimationManager>().SetDefaultAnimation("walk");
                    playerLeg.GetComponent<AnimationManager>().SetDefaultAnimation("walk");
                }
            }
            else if (inputManager.Keyboard[Keys.A] && inputManager.Keyboard[Keys.D])
            {
                playerHat.GetComponent<AnimationManager>().SetDefaultAnimation("idle");
                playerLeg.GetComponent<AnimationManager>().SetDefaultAnimation("idle");
                return;
            }
            else if (inputManager.Keyboard[Keys.W] && inputManager.Keyboard[Keys.S])
            {
                playerHat.GetComponent<AnimationManager>().SetDefaultAnimation("idle");
                playerLeg.GetComponent<AnimationManager>().SetDefaultAnimation("idle");
                return;
            }
            else if (inputManager.Keyboard[Keys.W])
            {
                MoveForward(playerSpeed);
                if (playerHat.GetComponent<AnimationManager>().defaultKey != "walk")
                {
                    playerHat.GetComponent<AnimationManager>().SetDefaultAnimation("walk");
                    playerLeg.GetComponent<AnimationManager>().SetDefaultAnimation("walk");
                }
            }
            else if (inputManager.Keyboard[Keys.S])
            {
                MoveBack(playerSpeed);
                if (playerHat.GetComponent<AnimationManager>().defaultKey != "walk")
                {
                    playerHat.GetComponent<AnimationManager>().SetDefaultAnimation("walk");
                    playerLeg.GetComponent<AnimationManager>().SetDefaultAnimation("walk");
                }
            }
            else if (inputManager.Keyboard[Keys.A])
            {
                MoveRight(playerSpeed);
                if (playerHat.GetComponent<AnimationManager>().defaultKey != "walk")
                {
                    playerHat.GetComponent<AnimationManager>().SetDefaultAnimation("walk");
                    playerLeg.GetComponent<AnimationManager>().SetDefaultAnimation("walk");
                }
            }
            else if (inputManager.Keyboard[Keys.D])
            {
                MoveLeft(playerSpeed);
                if (playerHat.GetComponent<AnimationManager>().defaultKey != "walk")
                {
                    playerHat.GetComponent<AnimationManager>().SetDefaultAnimation("walk");
                    playerLeg.GetComponent<AnimationManager>().SetDefaultAnimation("walk");
                }
            }
            else if (true)
            {
                //TODO CHANGE CONDITION
                playerHat.GetComponent<AnimationManager>().SetDefaultAnimation("idle");
                playerLeg.GetComponent<AnimationManager>().SetDefaultAnimation("idle");
            }

            if (parentGameObject.isGrounded && inputManager.Keyboard[Keys.Space])
            {
                isJumping = true;
            }
            Rotate(inputManager.Mouse.PositionsDelta.X * 0.01f);
        }

        private void PlayerAttacks()
        {
            if (inputManager.Mouse[SupportedMouseButtons.Left].WasPressed && playerHat.GetComponent<AnimationManager>().isReady)
            {
                playerHat.GetComponent<AnimationManager>().PlayAnimation("slash");
                playerLeg.GetComponent<AnimationManager>().PlayAnimation("slash");
            }

            if (inputManager.Mouse[SupportedMouseButtons.Right].WasPressed && playerHat.GetComponent<AnimationManager>().isReady)
            {
                playerHat.GetComponent<AnimationManager>().PlayAnimation("throw");
                playerLeg.GetComponent<AnimationManager>().PlayAnimation("throw");
            }
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
