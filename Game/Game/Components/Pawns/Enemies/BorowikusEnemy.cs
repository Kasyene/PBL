﻿using Microsoft.Xna.Framework;
using PBLGame;
using PBLGame.MainGame;
using PBLGame.Misc.Anim;
using PBLGame.SceneGraph;

namespace Game.Components.Pawns.Enemies
{
    class BorowikusEnemy : Enemy
    {
        public bool running = false;
        private GameObject enemyHat;
        private GameObject enemyLeg;

        public BorowikusEnemy(GameObject parent) : base(parent)
        {
            ObjectSide = Side.Enemy;
            enemySpeed = 0.2f;
            this.Hp = 99999;
            enemyHat = parentGameObject.FindChildNodeByTag("Hat");
            enemyLeg = parentGameObject.FindChildNodeByTag("Leg");
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override void ReceiveHit()
        {
            base.ReceiveHit();
            running = true;
        }

        public override void Update(GameTime time)
        {
            base.Update(time);
        }

        protected override void Attack()
        {
            base.Attack();
        }

        protected override void EnemyBehaviour()
        {
            if (!GameServices.GetService<ShroomGame>().levelOneCompleted)
            {
                if (parentGameObject.PositionZ < -1200 && !GameServices.GetService<ShroomGame>().levelOneCompleted)
                {
                    running = false;
                    GameServices.GetService<ShroomGame>().tutorialCompleted = true;
                    GameServices.GetService<GameObject>().GetComponent<Player>().canUseQ = true;
                    GameServices.GetService<GameObject>().GetComponent<Player>().canUseR = true;
                    GameServices.GetService<ShroomGame>().door1.closed = true;
                    GameServices.GetService<ShroomGame>().door2.closed = true;
                }
                if (!running)
                {
                    Vector3 playerPosition = GameServices.GetService<GameObject>().Position;
                    LookAtTarget(playerPosition, parentGameObject.Position);
                    distance = Vector3.Distance(playerPosition, this.parentGameObject.Position);
                    heightDifference = System.Math.Abs(playerPosition.Y - this.parentGameObject.Position.Y);
                    if (enemyHat.GetComponent<AnimationManager>().defaultKey != "idle")
                    {
                        enemyHat.GetComponent<AnimationManager>().SetDefaultAnimation("idle");
                        enemyLeg.GetComponent<AnimationManager>().SetDefaultAnimation("idle");
                    }
                }
                else
                {
                    if (parentGameObject.PositionZ > -450) LookAtTarget(new Vector3(-30f, 0f, -500f), parentGameObject.Position);
                    else LookAtTarget(new Vector3(0f, 0f, -1500f), parentGameObject.Position);
                    GameServices.GetService<GameObject>().GetComponent<Player>().TimeEnergy = 0;
                    Movement();
                    if (enemyHat.GetComponent<AnimationManager>().defaultKey != "walk")
                    {
                        enemyHat.GetComponent<AnimationManager>().SetDefaultAnimation("walk");
                        enemyLeg.GetComponent<AnimationManager>().SetDefaultAnimation("walk");
                    }
                }
            }
        }

        protected override void Movement()
        {
            Move(new Vector3(0f, 0f, enemySpeed));
        }
    }
}