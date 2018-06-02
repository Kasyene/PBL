using System.Diagnostics;
using Game.Misc.Time;
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
        private int count = 0;
        private int timeEnergy = 10;
        public bool timeStop = false;
        private bool timeEnergyRegeneration = false;
        private double previousTimeEnergyUpdate;
        GameObject playerHat;
        GameObject playerLeg;

        public Player(GameObject parent) : base()
        {
            Hp = 100;
            previousTimeEnergyUpdate = 0d;
            parentGameObject = parent;
            inputManager = InputManager.Instance;
            playerSpeed = 0.1f;
            playerHat = parentGameObject.FindChildNodeByTag("Hat");
            playerLeg = parentGameObject.FindChildNodeByTag("Leg");
        }

        public override void Update(GameTime time)
        {
            base.Update(time);

            TimeEnergyManagement(time);
            //special abilities energy management
            if (timeEnergy == 0)
            {
                timeStop = false;
            }
            // special abilities
            if (inputManager.Keyboard[Keys.Q].WasPressed)
            {
                if (timeStop || timeEnergy == 0)
                {
                    timeStop = false;
                }
                else if (!timeStop && timeEnergy > 1)
                {
                    timeStop = true;
                }


            }
            PlayerAttacks();
            PlayerMovement();

            // ANIMATION REVERSING - TEST

            if (playerHat.hatAnimationCollision)
            {
                playerHat.GetComponent<AnimationManager>().SetPlaybackMultiplier(-1);
                playerLeg.GetComponent<AnimationManager>().SetPlaybackMultiplier(-1);
                playerHat.hatAnimationCollision = false;
            }
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
                switch (count)
                {
                    case 0:
                        playerHat.GetComponent<AnimationManager>().PlayAnimation("slash");
                        playerLeg.GetComponent<AnimationManager>().PlayAnimation("slash");
                        break;
                    case 1:
                        playerHat.GetComponent<AnimationManager>().PlayAnimation("slashL");
                        playerLeg.GetComponent<AnimationManager>().PlayAnimation("slashL");
                        break;
                    case 2:
                        playerHat.GetComponent<AnimationManager>().PlayAnimation("slashR");
                        playerLeg.GetComponent<AnimationManager>().PlayAnimation("slashR");
                        break;
                    default:
                        break;
                }
                count = (count + 1) % 3;
            }

            if (inputManager.Mouse[SupportedMouseButtons.Right].WasPressed && playerHat.GetComponent<AnimationManager>().isReady)
            {
                playerHat.GetComponent<AnimationManager>().PlayAnimation("throw");
                playerLeg.GetComponent<AnimationManager>().PlayAnimation("throw");
            }
        }

        public int GetTimeEnergy()
        {
            return timeEnergy;
        }

        private void TimeEnergyManagement(GameTime time)
        {
            previousTimeEnergyUpdate += (time.ElapsedGameTime.TotalMilliseconds / 1000.0d);
            if (previousTimeEnergyUpdate > 1)
            {
                previousTimeEnergyUpdate = 0.0d;
                if (timeStop)
                {
                    timeEnergy -= 1;
                }
                else if(timeEnergy < 10)
                {
                    timeEnergy += 1;
                }
            }
           
        }
    }
}
