using System.Collections.Generic;
using Game.Components.Audio;
using Game.Components.Collisions;
using Game.Misc.Time;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PBLGame.Input;
using PBLGame.Input.Devices;
using PBLGame.Misc.Anim;
using PBLGame.SceneGraph;
using Game.MainGame;
using Microsoft.Xna.Framework.Graphics;

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
        private double previousTimePositonSaved;
        private List<Vector3> lastPositions;
        private List<int> lastHPs;
        GameObject playerHat;
        GameObject playerLeg;

        private AudioComponent audioComponent;

        private bool eWasPressed = false;
        private bool rWasPressed = false;
        private double timeSkillDoneTime = 0;
        private float timeSkillsDelay = 150;
        private double timeOfPress = 0;
        private float playerPositionYBeforeThrow;
        private int tempCount = 0;

        public bool canUseQ = true;
        public bool canUseE = true;
        public bool canUseR = true;

        public int TimeEnergy
        {
            get
            {
                return timeEnergy;
            }

            set
            {
                timeEnergy = value;
            }
        }

        public Player(GameObject parent) : base()
        {
            MaxHp = 20;
            Hp = MaxHp;
            previousTimeEnergyUpdate = 0d;
            parentGameObject = parent;
            inputManager = InputManager.Instance;
            playerSpeed = 0.138f;
            playerHat = parentGameObject.FindChildNodeByTag("Hat");
            playerLeg = parentGameObject.FindChildNodeByTag("Leg");
            lastPositions = new List<Vector3>();
            lastHPs = new List<int>();
            for (int x = 0; x < 12; x++)
            {
                lastPositions.Add(this.parentGameObject.Position);
                lastHPs.Add(Hp);

            }
            parent.AddComponent(new AudioComponent(parent));
            audioComponent = parent.GetComponent<AudioComponent>();
        }

        public override void Update(GameTime time)
        {
            base.Update(time);
            CheckIfDead();
            SaveLastPosition(time);
            TimeEnergyManagement(time);
            if (!GameServices.GetService<ShroomGame>().playerShouldNotMove)
            {
                SpecialTimeAbilities();
                PlayerAttacks();
                PlayerMovement();
            }
            
            if (playerHat.GetComponent<AnimationManager>().isCurrentAnimation("slash") || playerHat.GetComponent<AnimationManager>().isCurrentAnimation("slashL")
                || playerHat.GetComponent<AnimationManager>().isCurrentAnimation("slashR") || playerHat.GetComponent<AnimationManager>().isCurrentAnimation("throw"))
            {
                isAttacking = true;
            } else
            {
                isAttacking = false;
            }


            if (isAttacking && previousAttackAnimationId != playerHat.GetComponent<AnimationManager>().AnimID)
            {
                previousAttackAnimationId = playerHat.GetComponent<AnimationManager>().AnimID;
                playerHat.GetComponent<HitTrigger>().ClearBoxList();
            }

            if (playerHat.hatAnimationCollision)
            {
                playerHat.GetComponent<AnimationManager>().SetPlaybackMultiplier(-1);
                playerLeg.GetComponent<AnimationManager>().SetPlaybackMultiplier(-1);
                playerHat.hatAnimationCollision = false;
            }

            ThroneRoomEntranceCutscene();
        }

        private void CheckIfDead()
        {
            if(Hp <= 0)
            {
                ShroomGame.actualGameState = GameState.Dead;
            }
        }

        public Player GetPlayer()
        {
            return this;
        }

        private void PlayerMovement()
        {
            Vector3 movement = new Vector3();

            if (inputManager.Keyboard[Keys.W])
            {
                movement.Z +=  playerSpeed;
            }

            if (inputManager.Keyboard[Keys.S])
            {
                movement.Z += -playerSpeed;
            }

            if (inputManager.Keyboard[Keys.A])
            {
                movement.X += -playerSpeed;
            }

            if (inputManager.Keyboard[Keys.D])
            {
                movement.X += playerSpeed;
            }

            if (parentGameObject.isGrounded && inputManager.Keyboard[Keys.Space] && playerHat.GetComponent<AnimationManager>().isReady)
            {
                if (!isJumping)
                {
                    audioComponent?.PlaySound2D("jump");
                    isJumping = true;
                    playerLeg.GetComponent<AnimationManager>().SetPlaybackMultiplier(0.9f);
                    playerHat.GetComponent<AnimationManager>().SetPlaybackMultiplier(0.9f);
                    playerLeg.GetComponent<AnimationManager>().PlayAnimation("jumpStart",true);
                    playerHat.GetComponent<AnimationManager>().PlayAnimation("jumpStart",true);
                    playerLeg.GetComponent<AnimationManager>().SetPlaybackMultiplier(0.9f);
                    playerHat.GetComponent<AnimationManager>().SetPlaybackMultiplier(0.9f);
                    playerLeg.GetComponent<AnimationManager>().PlayAnimation("jumpLand");
                    playerHat.GetComponent<AnimationManager>().PlayAnimation("jumpLand");
                }
            }


            if (!isJumping && parentGameObject.isGrounded)
            {
                if (movement != Vector3.Zero)
                {
                    if (playerHat.GetComponent<AnimationManager>().defaultKey != "walk")
                    {
                        playerHat.GetComponent<AnimationManager>().SetDefaultAnimation("walk");
                        playerLeg.GetComponent<AnimationManager>().SetDefaultAnimation("walk");
                    }
                }
                else
                {
                    if (playerHat.GetComponent<AnimationManager>().defaultKey != "idle")
                    {
                        playerHat.GetComponent<AnimationManager>().SetDefaultAnimation("idle");
                        playerLeg.GetComponent<AnimationManager>().SetDefaultAnimation("idle");
                    }
                } 
            }
            else
            {
                //TODO: FIX JUMP FALLING ANIMATION OR LEAVE IT IN PEACE
                //if (playerHat.GetComponent<AnimationManager>().defaultKey != "jumpFall")
                //{
                //    playerHat.GetComponent<AnimationManager>().SetDefaultAnimation("jumpFall");
                //    playerLeg.GetComponent<AnimationManager>().SetDefaultAnimation("jumpFall");
                //}

                if (playerHat.GetComponent<AnimationManager>().defaultKey != "idle")
                {
                    playerHat.GetComponent<AnimationManager>().SetDefaultAnimation("idle");
                    playerLeg.GetComponent<AnimationManager>().SetDefaultAnimation("idle");
                }

                if (parentGameObject.isGrounded)
                {
                        //playerLeg.GetComponent<AnimationManager>().SetPlaybackMultiplier(0.8f);
                        //playerHat.GetComponent<AnimationManager>().SetPlaybackMultiplier(0.8f);
                        //playerLeg.GetComponent<AnimationManager>().PlayAnimation("jumpLand");
                        //playerHat.GetComponent<AnimationManager>().PlayAnimation("jumpLand");
                }
            }

            Move(movement);

            Rotate(ShroomGame.mouseXAxis * inputManager.Mouse.PositionsDelta.X * 0.01f);
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
                playerPositionYBeforeThrow = this.parentGameObject.Position.Y;
                playerHat.GetComponent<AnimationManager>().PlayAnimation("throw");
                playerLeg.GetComponent<AnimationManager>().PlayAnimation("throw");
            }
        }

        public int GetTimeEnergy()
        {
            return TimeEnergy;
        }

        private void TimeEnergyManagement(GameTime time)
        {
            previousTimeEnergyUpdate += (time.ElapsedGameTime.TotalMilliseconds / 1000.0d);

            // when timeEnergy ends
            if (TimeEnergy == 0)
            {
                timeStop = false;
            }

            // timeEnergy management
            if (previousTimeEnergyUpdate >= 1.0d && timeStop)
            {
                TimeEnergy -= 1;
                previousTimeEnergyUpdate = 0.0d;
            }
            if (previousTimeEnergyUpdate >= 1.66d && !timeStop && TimeEnergy < 10)
            {
                TimeEnergy += 1;
                previousTimeEnergyUpdate = 0.0d;
            }

        }

        private void SpecialTimeAbilities()
        {
            if (eWasPressed | rWasPressed)
            {
                ShroomGame.fadeAmount = MathHelper.Clamp((float)(Timer.gameTime.TotalGameTime.TotalMilliseconds - timeOfPress) / timeSkillsDelay, 0, 1);
            }
            else if (Timer.gameTime.TotalGameTime.TotalMilliseconds > timeSkillDoneTime + timeSkillsDelay)
            {
                ShroomGame.fadeAmount = 1.0f - MathHelper.Clamp((float)(Timer.gameTime.TotalGameTime.TotalMilliseconds - timeSkillDoneTime) / timeSkillsDelay, 0, 1);
            }

            // timeStop
            if (canUseQ)
            {
                if (inputManager.Keyboard[Keys.Q].WasPressed)
                {
                    GameServices.GetService<ShroomGame>().usedQ = true;
                    if (timeStop || TimeEnergy == 0)
                    {
                        audioComponent?.PlaySound2D("timeSpeed");
                        timeStop = false;
                    }
                    else if (!timeStop && TimeEnergy > 1)
                    {
                        audioComponent?.PlaySound2D("timeSlow");
                        timeStop = true;
                    }
                }
            }

            if (canUseE)
            {
                if (inputManager.Keyboard[Keys.E].WasPressed && TimeEnergy >= 5 && !eWasPressed)
                {
                    GameServices.GetService<ShroomGame>().usedE = true;
                    eWasPressed = true;
                    timeOfPress = Timer.gameTime.TotalGameTime.TotalMilliseconds;
                }
            }
            if (canUseR)
            {
                if (inputManager.Keyboard[Keys.R].WasPressed && TimeEnergy >= 3 && !rWasPressed && playerHat.GetComponent<AnimationManager>().isCurrentAnimation("throw"))
                {
                    GameServices.GetService<ShroomGame>().usedR = true;
                    rWasPressed = true;
                    timeOfPress = Timer.gameTime.TotalGameTime.TotalMilliseconds;
                    audioComponent?.PlaySound2D("teleport");
                }
            }

            if (eWasPressed && Timer.gameTime.TotalGameTime.TotalMilliseconds > timeOfPress + timeSkillsDelay)
            {
                timeSkillDoneTime = Timer.gameTime.TotalGameTime.TotalMilliseconds;
                eWasPressed = false;
                TimeEnergy -= 5;
                this.parentGameObject.Position = lastPositions[lastPositions.Count / 2];
                Hp = lastHPs[lastHPs.Count / 2];
                ReLocateIndicies(lastPositions);
                ReLocateIndicies(lastHPs);
            }

            if (rWasPressed && Timer.gameTime.TotalGameTime.TotalMilliseconds > timeOfPress + timeSkillsDelay)
            {
                timeSkillDoneTime = Timer.gameTime.TotalGameTime.TotalMilliseconds;
                rWasPressed = false;
                TimeEnergy -= 3;
                this.parentGameObject.Position = new Vector3((this.playerHat.GetBoundingBox().Min.X + this.playerHat.GetBoundingBox().Max.X) / 2.0f, playerPositionYBeforeThrow, (this.playerHat.GetBoundingBox().Min.Z + this.playerHat.GetBoundingBox().Max.Z) / 2.0f);
                playerHat.GetComponent<AnimationManager>().AnimationBreak();
                playerLeg.GetComponent<AnimationManager>().AnimationBreak();
                playerHat.GetComponent<AnimationManager>().PlayAnimation("idle", true);
                playerLeg.GetComponent<AnimationManager>().PlayAnimation("idle", true);
            }
        }

        private void SaveLastPosition(GameTime time)
        {
            previousTimePositonSaved += (time.ElapsedGameTime.TotalMilliseconds / 1000.0d);
            if (previousTimePositonSaved >= 0.5d)
            {
                previousTimePositonSaved = 0.0d;
                lastPositions.RemoveAt(0);
                lastPositions.Add(this.parentGameObject.Position);
                lastHPs.RemoveAt(0);
                lastHPs.Add(Hp);
            }
        }

        private void ReLocateIndicies<T>(List<T> list)
        {
            for (int x = 0; x <= list.Count / 2 - 1; x++)
            {
                list[list.Count / 2 + x] = list[x];
            }
        }

        public override void ReceiveHit()
        {
            base.ReceiveHit();
            audioComponent.PlaySound2D("damage");
        }

        private void ThroneRoomEntranceCutscene()
        {
            if (ShroomGame.actualGameState == GameState.LevelTutorial && parentGameObject.PositionZ < -1050f && parentGameObject.PositionZ > -1200f && GameServices.GetService<ShroomGame>().levelOneCompleted && !GameServices.GetService<ShroomGame>().roomEntranceCutscene)
            {
                GameServices.GetService<ShroomGame>().roomEntranceCutscene = true;
                GameServices.GetService<ShroomGame>().bossFight = true;
                GameServices.GetService<ShroomGame>().musicManager.StopSong();
                GameServices.GetService<ShroomGame>().musicManager.PlaySong("fight");
                GameServices.GetService<ShroomGame>().musicManager.IsRepeating = true;
                new Cutscene(GameServices.GetService<ShroomGame>().Content.Load<Texture2D>("Cutscene/3,0"), 12f, "Narrator: When our hero came to his senses, he found the door to the throne room wide open.",                "Narrator: He noticed that King pushed weakened Borovikus to defense.");
                new Cutscene(GameServices.GetService<ShroomGame>().Content.Load<Texture2D>("Cutscene/3,0"), 12f, "King: Finally you are here! Help me, kill the traitor!",                "Narrator: But our hero had already figured out who the real traitor was.");
            } 
        }
    }
}
