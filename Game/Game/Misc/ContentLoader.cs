﻿using Game.Components.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PBLGame.SceneGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using PBLGame.Misc.Anim;
using PBLGame.MainGame;
using Game.Components.Pawns.Enemies;
using Game.Components.Enemies;
using Game.Components;
using Game.MainGame;
using Game.Components.Coliisions;
using Game.Components.MapElements;
using Game.Misc.Time;

namespace PBLGame.Misc
{
    class ContentLoader
    {
        private ShroomGame game;

        private List<GameObject> triggers;

        #region Textures
        Texture2D playerTex;
        Texture2D playerNormal;
        Texture2D borowikusTex;
        Texture2D borowikusNormal;
        Texture2D kingTex;
        Texture2D kingNormal;
        Texture2D rangedEnemyTex;
        Texture2D rangedEnemyNormal;
        Texture2D meleeEnemyTex;
        Texture2D meleeEnemyNormal;
        Texture2D bulletEnemyTex;
        Texture2D bulletEnemyNormal;
        Texture2D heartTexture;
        #endregion

        #region Models
        Model heartModel;
        Model bulletModel;
        ModelAnimatedComponent meleeIdle;
        ModelAnimatedComponent meleeWalk;
        ModelAnimatedComponent meleeAttack;
        ModelAnimatedComponent meleeGotHit;
        ModelAnimatedComponent meleeDeath;

        ModelAnimatedComponent kingHatIdle;
        ModelAnimatedComponent kingHatWalk;
        ModelAnimatedComponent kingHatDeath;
        ModelAnimatedComponent kingHatGotHit;
        ModelAnimatedComponent kingHatSlashR;
        ModelAnimatedComponent kingHatSlashL;
        ModelAnimatedComponent kingHatSlash;
        ModelAnimatedComponent kingHatSpin;
        ModelAnimatedComponent kingHatBaczek;
        ModelAnimatedComponent kingHatWbicie;

        ModelAnimatedComponent kingLegIdle;
        ModelAnimatedComponent kingLegWalk;
        ModelAnimatedComponent kingLegDeath;
        ModelAnimatedComponent kingLegGotHit;
        ModelAnimatedComponent kingLegSlashR;
        ModelAnimatedComponent kingLegSlashL;
        ModelAnimatedComponent kingLegSlash;
        ModelAnimatedComponent kingLegSpin;
        ModelAnimatedComponent kingLegBaczek;
        ModelAnimatedComponent kingLegWbicie;

        ModelAnimatedComponent rangedHatIdle;
        ModelAnimatedComponent rangedHatWalk;
        ModelAnimatedComponent rangedHatGotHit;
        ModelAnimatedComponent rangedHatDeath;

        ModelAnimatedComponent rangedLegIdle;
        ModelAnimatedComponent rangedLegWalk;
        ModelAnimatedComponent rangedLegGotHit;
        ModelAnimatedComponent rangedLegDeath;
        #endregion

        Effect standardEffect;
        Effect animatedEffect;
        Effect refractionEffect;

        public ContentLoader(ShroomGame game)
        {
            this.game = game;
            triggers = new List<GameObject>();
            LoadContent();
        }

        public void LoadContent()
        {
            heartTexture = game.Content.Load<Texture2D>("apteczkaTex");
            standardEffect = game.Content.Load<Effect>("Standard");
            animatedEffect = game.Content.Load<Effect>("StandardAnimated");
            refractionEffect = game.Content.Load<Effect>("Refraction");
            playerTex = game.Content.Load<Texture2D>("models/player/borowikTex");
            playerNormal = game.Content.Load<Texture2D>("models/player/borowikNormal");
            borowikusTex = game.Content.Load<Texture2D>("models/player/borowikusTex");
            borowikusNormal = game.Content.Load<Texture2D>("models/player/borowikusNormal");
            kingTex = game.Content.Load<Texture2D>("models/enemies/nowyKrol/krolTex");
            kingNormal = game.Content.Load<Texture2D>("models/enemies/nowyKrol/krolNormal");
            rangedEnemyTex = game.Content.Load<Texture2D>("models/enemies/muchomorRzucajacy/muchomorRzucajacyTex");
            rangedEnemyNormal = game.Content.Load<Texture2D>("models/enemies/muchomorRzucajacy/muchomorRzucajacyNormal");
            meleeEnemyTex = game.Content.Load<Texture2D>("models/enemies/muchomorStadny/muchomorStadnyTex");
            meleeEnemyNormal = game.Content.Load<Texture2D>("models/enemies/muchomorStadny/muchomorStadnyNormal");
            bulletEnemyTex = game.Content.Load<Texture2D>("models/enemies/muchomorRzucajacy/muchomorRzucajacyTex");
            bulletEnemyNormal = game.Content.Load<Texture2D>("models/enemies/muchomorRzucajacy/muchomorRzucajacyNormal");

            heartModel = game.Content.Load<Model>("apteczka");
            bulletModel = game.Content.Load<Model>("models/enemies/muchomorRzucajacy/Kulka");

            kingHatIdle = new ModelAnimatedComponent("models/enemies/nowyKrol/krolIdleKapelusz", game.Content, animatedEffect, kingTex, kingNormal);
            kingHatWalk = new ModelAnimatedComponent("models/enemies/nowyKrol/krolChodKapelusz", game.Content, animatedEffect, kingTex, kingNormal);
            kingHatDeath = new ModelAnimatedComponent("models/enemies/nowyKrol/krolUmarlKapelusz", game.Content, animatedEffect, kingTex, kingNormal);
            kingHatGotHit = new ModelAnimatedComponent("models/enemies/nowyKrol/krolOberwalKapelusz", game.Content, animatedEffect, kingTex, kingNormal);
            kingHatSlashR = new ModelAnimatedComponent("models/enemies/nowyKrol/krolSlashPrawoKapelusz", game.Content, animatedEffect, kingTex, kingNormal);
            kingHatSlashL = new ModelAnimatedComponent("models/enemies/nowyKrol/krolSlashLewoKapelusz", game.Content, animatedEffect, kingTex, kingNormal);
            kingHatSlash = new ModelAnimatedComponent("models/enemies/nowyKrol/krolSlashPrzodKapelusz", game.Content, animatedEffect, kingTex, kingNormal);
            kingHatSpin = new ModelAnimatedComponent("models/enemies/nowyKrol/krolSpinKapelusz", game.Content, animatedEffect, kingTex, kingNormal);
            kingHatBaczek = new ModelAnimatedComponent("models/enemies/nowyKrol/krolBaczekKapelusz", game.Content, animatedEffect, kingTex, kingNormal);
            kingHatWbicie = new ModelAnimatedComponent("models/enemies/nowyKrol/krolWbicieKapelusz", game.Content, animatedEffect, kingTex, kingNormal);

            kingLegIdle = new ModelAnimatedComponent("models/enemies/nowyKrol/krolIdleNozka", game.Content, animatedEffect, kingTex, kingNormal);
            kingLegWalk = new ModelAnimatedComponent("models/enemies/nowyKrol/krolChodNozka", game.Content, animatedEffect, kingTex, kingNormal);
            kingLegDeath = new ModelAnimatedComponent("models/enemies/nowyKrol/krolUmarlNozka", game.Content, animatedEffect, kingTex, kingNormal);
            kingLegGotHit = new ModelAnimatedComponent("models/enemies/nowyKrol/krolOberwalNozka", game.Content, animatedEffect, kingTex, kingNormal);
            kingLegSlash = new ModelAnimatedComponent("models/enemies/nowyKrol/krolSlashPrzodNozka", game.Content, animatedEffect, kingTex, kingNormal);
            kingLegSlashR = new ModelAnimatedComponent("models/enemies/nowyKrol/krolSlashPrawoNozka", game.Content, animatedEffect, kingTex, kingNormal);
            kingLegSlashL = new ModelAnimatedComponent("models/enemies/nowyKrol/krolSlashLewoNozka", game.Content, animatedEffect, kingTex, kingNormal);
            kingLegSpin = new ModelAnimatedComponent("models/enemies/nowyKrol/krolSpinNozka", game.Content, animatedEffect, kingTex, kingNormal);
            kingLegBaczek = new ModelAnimatedComponent("models/enemies/nowyKrol/krolBaczekNozka", game.Content, animatedEffect, kingTex, kingNormal);
            kingLegWbicie = new ModelAnimatedComponent("models/enemies/nowyKrol/krolWbicieNozka", game.Content, animatedEffect, kingTex, kingNormal);

            meleeIdle = new ModelAnimatedComponent("models/enemies/muchomorStadny/muchomorStadnyIdle", game.Content, animatedEffect, meleeEnemyTex, meleeEnemyNormal);
            meleeWalk = new ModelAnimatedComponent("models/enemies/muchomorStadny/muchomorStadnyChod", game.Content, animatedEffect, meleeEnemyTex, meleeEnemyNormal);
            meleeAttack = new ModelAnimatedComponent("models/enemies/muchomorStadny/muchomorStadnyAtak", game.Content, animatedEffect, meleeEnemyTex, meleeEnemyNormal);
            meleeGotHit = new ModelAnimatedComponent("models/enemies/muchomorStadny/muchomorStadnyOberwal", game.Content, animatedEffect, meleeEnemyTex, meleeEnemyNormal);
            meleeDeath = new ModelAnimatedComponent("models/enemies/muchomorStadny/muchomorStadnyUmarl", game.Content, animatedEffect, meleeEnemyTex, meleeEnemyNormal);

            rangedHatIdle = new ModelAnimatedComponent("models/enemies/muchomorRzucajacy/muchomorRzucajacyKapeluszIdle", game.Content, animatedEffect, rangedEnemyTex, rangedEnemyNormal);
            rangedHatWalk = new ModelAnimatedComponent("models/enemies/muchomorRzucajacy/muchomorRzucajacyKapeluszChod", game.Content, animatedEffect, rangedEnemyTex, rangedEnemyNormal);
            rangedHatGotHit = new ModelAnimatedComponent("models/enemies/muchomorRzucajacy/muchomorRzucajacyKapeluszOberwal", game.Content, animatedEffect, rangedEnemyTex, rangedEnemyNormal);
            rangedHatDeath = new ModelAnimatedComponent("models/enemies/muchomorRzucajacy/muchomorRzucajacyKapeluszUmarl", game.Content, animatedEffect, rangedEnemyTex, rangedEnemyNormal);

            rangedLegIdle = new ModelAnimatedComponent("models/enemies/muchomorRzucajacy/muchomorRzucajacyNozkaIdle", game.Content, animatedEffect, rangedEnemyTex, rangedEnemyNormal);
            rangedLegWalk = new ModelAnimatedComponent("models/enemies/muchomorRzucajacy/muchomorRzucajacyNozkaChod", game.Content, animatedEffect, rangedEnemyTex, rangedEnemyNormal);
            rangedLegGotHit = new ModelAnimatedComponent("models/enemies/muchomorRzucajacy/muchomorRzucajacyNozkaOberwal", game.Content, animatedEffect, rangedEnemyTex, rangedEnemyNormal);
            rangedLegDeath = new ModelAnimatedComponent("models/enemies/muchomorRzucajacy/muchomorRzucajacyNozkaUmarl", game.Content, animatedEffect, rangedEnemyTex, rangedEnemyNormal);
        }

        public void SetAsColliderAndTrigger()
        {
            foreach (GameObject obj in triggers)
            {
                obj.SetAsColliderAndTrigger(new HitTrigger(obj));
            }
        }

        public void LoadPlayer(GameObject player, GameObject cameraCollision)
        {
            GameObject playerLeg = new GameObject("Leg");
            GameObject playerHat = new GameObject("Hat");
            triggers.Add(playerHat);

            // Load anim models
            player.AddChildNode(playerLeg);
            player.AddChildNode(playerHat);

            // models without anims have problems i guess ; /
            playerLeg.AddComponent(new ModelAnimatedComponent("models/player/borowikNozkaChod", game.Content, animatedEffect, playerTex, playerNormal));
            playerLeg.AddComponent(new AnimationManager(playerLeg));
            playerHat.AddComponent(new ModelAnimatedComponent("models/player/borowikKapeluszChod", game.Content, animatedEffect, playerTex, playerNormal));
            playerHat.AddComponent(new AnimationManager(playerHat));

            // ENABLE DYNAMIC COLLISION ON PLAYER HAT
            playerHat.GetComponent<ModelAnimatedComponent>().ColliderDynamicUpdateEnable();

            // IDLE
            playerLeg.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikNozkaIdle", game.Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "idle");
            playerHat.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikKapeluszIdle", game.Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "idle");

            // WALK
            playerLeg.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikNozkaChod", game.Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "walk");
            playerHat.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikKapeluszChod", game.Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "walk");

            // ATTACK MAIN
            playerLeg.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikNozkaSlash", game.Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "slash");
            playerHat.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikKapeluszSlash", game.Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "slash");

            // ATTACK LEFT
            playerLeg.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikNozkaSlashLewo", game.Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "slashL");
            playerHat.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikKapeluszSlashLewo", game.Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "slashL");

            // ATTACK RIGHT
            playerLeg.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikNozkaSlashPrawo", game.Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "slashR");
            playerHat.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikKapeluszSlashPrawo", game.Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "slashR");

            // THROW
            playerLeg.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikNozkaRzutKapeluszem", game.Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "throw");
            playerHat.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikKapeluszRzutKapeluszem", game.Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "throw");

            // JUMPATTACK1
            playerLeg.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikNozkaRzutKapeluszem", game.Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "jumpAttack1");
            playerHat.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikKapeluszRzutKapeluszem", game.Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "jumpAttack1");


            // JUMPATTACK2
            playerLeg.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikNozkaRzutKapeluszem", game.Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "jumpAttack2");
            playerHat.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikKapeluszRzutKapeluszem", game.Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "jumpAttack2");

            // JUMP START
            playerLeg.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikNozkaSkok1", game.Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "jumpStart");
            playerHat.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikKapeluszSkok1", game.Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "jumpStart");

            // JUMP FALL
            playerLeg.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikNozkaSkok2", game.Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "jumpFall");
            playerHat.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikKapeluszSkok2", game.Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "jumpFall");

            // JUMP LAND
            playerLeg.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikNozkaSkok3", game.Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "jumpLand");
            playerHat.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikKapeluszSkok3", game.Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "jumpLand");

            // TODO: ANIM LOAD SYSTEM / SELECTOR
            playerHat.GetComponent<AnimationManager>().PlayAnimation("idle");
            playerLeg.GetComponent<AnimationManager>().PlayAnimation("idle");

            player.AddComponent(new Player(player));
            game.root.AddChildNode(player);

            player.AddChildNode(cameraCollision);
            player.AddChildNode(GameServices.GetService<Camera>());
            player.RotationZ = 1.5f;

            GameServices.RemoveService<GameObject>();
            GameServices.AddService(player);
        }

        public GameObject LoadBorowikus()
        {
            GameObject borowikus = new GameObject("borowikus");
            GameObject borowikusLeg = new GameObject("Leg");
            GameObject borowikusHat = new GameObject("Hat");
            triggers.Add(borowikusHat);

            // Load anim models
            borowikus.AddChildNode(borowikusLeg);
            borowikus.AddChildNode(borowikusHat);

            // models without anims have problems i guess ; /
            borowikusLeg.AddComponent(new ModelAnimatedComponent("models/player/borowikNozkaChod", game.Content, animatedEffect, borowikusTex, borowikusNormal));
            borowikusLeg.AddComponent(new AnimationManager(borowikusLeg));
            borowikusHat.AddComponent(new ModelAnimatedComponent("models/player/borowikKapeluszChod", game.Content, animatedEffect, borowikusTex, bulletEnemyNormal));
            borowikusHat.AddComponent(new AnimationManager(borowikusHat));

            // ENABLE DYNAMIC COLLISION ON BOROWIKUS HAT
            borowikusHat.GetComponent<ModelAnimatedComponent>().ColliderDynamicUpdateEnable();

            // IDLE
            borowikusLeg.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikNozkaIdle", game.Content, animatedEffect, borowikusTex, borowikusNormal).AnimationClips[0], "idle");
            borowikusHat.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikKapeluszIdle", game.Content, animatedEffect, borowikusTex, borowikusNormal).AnimationClips[0], "idle");

            // WALK
            borowikusLeg.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikNozkaChod", game.Content, animatedEffect, borowikusTex, borowikusNormal).AnimationClips[0], "walk");
            borowikusHat.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikKapeluszChod", game.Content, animatedEffect, borowikusTex, borowikusNormal).AnimationClips[0], "walk");

            // ATTACK MAIN
            borowikusLeg.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikNozkaSlash", game.Content, animatedEffect, borowikusTex, borowikusNormal).AnimationClips[0], "slash");
            borowikusHat.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikKapeluszSlash", game.Content, animatedEffect, borowikusTex, borowikusNormal).AnimationClips[0], "slash");

            // ATTACK LEFT
            borowikusLeg.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikNozkaSlashLewo", game.Content, animatedEffect, borowikusTex, borowikusNormal).AnimationClips[0], "slashL");
            borowikusHat.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikKapeluszSlashLewo", game.Content, animatedEffect, borowikusTex, borowikusNormal).AnimationClips[0], "slashL");

            // ATTACK RIGHT
            borowikusLeg.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikNozkaSlashPrawo", game.Content, animatedEffect, borowikusTex, borowikusNormal).AnimationClips[0], "slashR");
            borowikusHat.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikKapeluszSlashPrawo", game.Content, animatedEffect, borowikusTex, borowikusNormal).AnimationClips[0], "slashR");

            // THROW
            borowikusLeg.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikNozkaRzutKapeluszem", game.Content, animatedEffect, borowikusTex, playerNormal).AnimationClips[0], "throw");
            borowikusHat.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikKapeluszRzutKapeluszem", game.Content, animatedEffect, borowikusTex, playerNormal).AnimationClips[0], "throw");

            // JUMPATTACK1
            borowikusLeg.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikNozkaRzutKapeluszem", game.Content, animatedEffect, borowikusTex, borowikusNormal).AnimationClips[0], "jumpAttack1");
            borowikusHat.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikKapeluszRzutKapeluszem", game.Content, animatedEffect, borowikusTex, borowikusNormal).AnimationClips[0], "jumpAttack1");


            // JUMPATTACK2
            borowikusLeg.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikNozkaRzutKapeluszem", game.Content, animatedEffect, borowikusTex, borowikusNormal).AnimationClips[0], "jumpAttack2");
            borowikusHat.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikKapeluszRzutKapeluszem", game.Content, animatedEffect, borowikusTex, borowikusNormal).AnimationClips[0], "jumpAttack2");

            // JUMP START
            borowikusLeg.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikNozkaSkok1", game.Content, animatedEffect, borowikusTex, borowikusNormal).AnimationClips[0], "jumpStart");
            borowikusHat.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikKapeluszSkok1", game.Content, animatedEffect, borowikusTex, borowikusNormal).AnimationClips[0], "jumpStart");

            // JUMP FALL
            borowikusLeg.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikNozkaSkok2", game.Content, animatedEffect, borowikusTex, borowikusNormal).AnimationClips[0], "jumpFall");
            borowikusHat.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikKapeluszSkok2", game.Content, animatedEffect, borowikusTex, borowikusNormal).AnimationClips[0], "jumpFall");

            // JUMP LAND
            borowikusLeg.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikNozkaSkok3", game.Content, animatedEffect, borowikusTex, borowikusNormal).AnimationClips[0], "jumpLand");
            borowikusHat.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikKapeluszSkok3", game.Content, animatedEffect, borowikusTex, borowikusNormal).AnimationClips[0], "jumpLand");

            borowikusHat.GetComponent<AnimationManager>().PlayAnimation("idle");
            borowikusLeg.GetComponent<AnimationManager>().PlayAnimation("idle");

            // TODO: CHANGE CLASS TO BOROWIKUS
            borowikus.AddComponent(new BorowikusEnemy(borowikus));
            game.root.AddChildNode(borowikus);
            game.enemyList.Add(borowikus);

            borowikus.RotationZ = 1.5f;

            return borowikus;
        }

        public GameObject LoadKing()
        {
            GameObject king = new GameObject("king");
            game.boss = king;
            GameObject kingHat = new GameObject("Hat");
            GameObject kingLeg = new GameObject("Leg");

            triggers.Add(kingHat);

            // Load anim models
            king.AddChildNode(kingLeg);
            king.AddChildNode(kingHat);

            // models without anims have problems i guess ; /
            kingLeg.AddComponent(new ModelAnimatedComponent("models/enemies/nowyKrol/krolChodNozka", game.Content, animatedEffect, kingTex, kingNormal));
            kingLeg.AddComponent(new AnimationManager(kingLeg));
            kingHat.AddComponent(new ModelAnimatedComponent("models/enemies/nowyKrol/krolChodKapelusz", game.Content, animatedEffect, kingTex, kingNormal));
            kingHat.AddComponent(new AnimationManager(kingHat));

            // ENABLE DYNAMIC COLLISION ON ENEMY HAT
            kingHat.GetComponent<ModelAnimatedComponent>().ColliderDynamicUpdateEnable();

            // IDLE
            kingLeg.GetComponent<AnimationManager>().AddAnimation(kingLegIdle.AnimationClips[0], "idle");
            kingHat.GetComponent<AnimationManager>().AddAnimation(kingHatIdle.AnimationClips[0], "idle");

            // WALK
            kingLeg.GetComponent<AnimationManager>().AddAnimation(kingLegWalk.AnimationClips[0], "walk");
            kingHat.GetComponent<AnimationManager>().AddAnimation(kingHatWalk.AnimationClips[0], "walk");

            // GOTHIT
            kingLeg.GetComponent<AnimationManager>().AddAnimation(kingLegGotHit.AnimationClips[0], "gotHit");
            kingHat.GetComponent<AnimationManager>().AddAnimation(kingHatGotHit.AnimationClips[0], "gotHit");

            // DEATH
            kingLeg.GetComponent<AnimationManager>().AddAnimation(kingLegDeath.AnimationClips[0], "death");
            kingHat.GetComponent<AnimationManager>().AddAnimation(kingHatDeath.AnimationClips[0], "death");

            // SLASH PRZOD
            kingLeg.GetComponent<AnimationManager>().AddAnimation(kingLegSlash.AnimationClips[0], "slash");
            kingHat.GetComponent<AnimationManager>().AddAnimation(kingHatSlash.AnimationClips[0], "slash");

            // SLASH LEWO
            kingLeg.GetComponent<AnimationManager>().AddAnimation(kingLegSlashL.AnimationClips[0], "slashL");
            kingHat.GetComponent<AnimationManager>().AddAnimation(kingHatSlashL.AnimationClips[0], "slashL");

            // SLASH PRAWO
            kingLeg.GetComponent<AnimationManager>().AddAnimation(kingLegSlashR.AnimationClips[0], "slashR");
            kingHat.GetComponent<AnimationManager>().AddAnimation(kingHatSlashR.AnimationClips[0], "slashR");

            // SPIN
            kingLeg.GetComponent<AnimationManager>().AddAnimation(kingLegSpin.AnimationClips[0], "spin");
            kingHat.GetComponent<AnimationManager>().AddAnimation(kingHatSpin.AnimationClips[0], "spin");

            // BACZEK
            kingLeg.GetComponent<AnimationManager>().AddAnimation(kingLegBaczek.AnimationClips[0], "baczek");
            kingHat.GetComponent<AnimationManager>().AddAnimation(kingHatBaczek.AnimationClips[0], "baczek");

            // WBICIE
            kingLeg.GetComponent<AnimationManager>().AddAnimation(kingLegWbicie.AnimationClips[0], "wbicie");
            kingHat.GetComponent<AnimationManager>().AddAnimation(kingHatWbicie.AnimationClips[0], "wbicie");

            // TODO: CHANGE TO KING CLASS
            kingLeg.GetComponent<AnimationManager>().PlayAnimation("idle");
            kingHat.GetComponent<AnimationManager>().PlayAnimation("idle");
            king.AddComponent(new BossEnemy(king));
            king.GetComponent<BossEnemy>().ObjectSide = Side.Enemy;


            game.root.AddChildNode(king);
            game.enemyList.Add(king);
            return king;
        }

        public GameObject LoadRangedEnemy()
        {
            GameObject rangedEnemy = new GameObject("rangedEnemy");
            GameObject rangedEnemyHat = new GameObject("Hat");
            GameObject rangedEnemyLeg = new GameObject("Leg");

            triggers.Add(rangedEnemyHat);

            // Load anim models
            rangedEnemy.AddChildNode(rangedEnemyLeg);
            rangedEnemy.AddChildNode(rangedEnemyHat);

            // models without anims have problems i guess ; /
            rangedEnemyLeg.AddComponent(new ModelAnimatedComponent("models/enemies/muchomorRzucajacy/muchomorRzucajacyNozkaChod", game.Content, animatedEffect, rangedEnemyTex, rangedEnemyNormal));
            rangedEnemyLeg.AddComponent(new AnimationManager(rangedEnemyLeg));
            rangedEnemyHat.AddComponent(new ModelAnimatedComponent("models/enemies/muchomorRzucajacy/muchomorRzucajacyKapeluszChod", game.Content, animatedEffect, rangedEnemyTex, rangedEnemyNormal));
            rangedEnemyHat.AddComponent(new AnimationManager(rangedEnemyHat));

            // ENABLE DYNAMIC COLLISION ON ENEMY HAT
            rangedEnemyHat.GetComponent<ModelAnimatedComponent>().ColliderDynamicUpdateEnable();

            // IDLE
            rangedEnemyLeg.GetComponent<AnimationManager>().AddAnimation(rangedLegIdle.AnimationClips[0], "idle");
            rangedEnemyHat.GetComponent<AnimationManager>().AddAnimation(rangedHatIdle.AnimationClips[0], "idle");

            // WALK
            rangedEnemyLeg.GetComponent<AnimationManager>().AddAnimation(rangedLegWalk.AnimationClips[0], "walk");
            rangedEnemyHat.GetComponent<AnimationManager>().AddAnimation(rangedHatWalk.AnimationClips[0], "walk");

            // GOTHIT
            rangedEnemyLeg.GetComponent<AnimationManager>().AddAnimation(rangedLegGotHit.AnimationClips[0], "gotHit");
            rangedEnemyHat.GetComponent<AnimationManager>().AddAnimation(rangedHatGotHit.AnimationClips[0], "gotHit");

            // DEATH
            rangedEnemyLeg.GetComponent<AnimationManager>().AddAnimation(rangedLegDeath.AnimationClips[0], "death");
            rangedEnemyHat.GetComponent<AnimationManager>().AddAnimation(rangedHatDeath.AnimationClips[0], "death");

            // TODO: ANIM LOAD SYSTEM / SELECTOR
            rangedEnemyLeg.GetComponent<AnimationManager>().PlayAnimation("idle");
            rangedEnemyHat.GetComponent<AnimationManager>().PlayAnimation("idle");
            rangedEnemy.AddComponent(new RangedEnemy(rangedEnemy));
            rangedEnemy.GetComponent<RangedEnemy>().ObjectSide = Side.Enemy;

            //bullet
            rangedEnemy.GetComponent<RangedEnemy>().bulletEnemyNormal = bulletEnemyNormal;
            rangedEnemy.GetComponent<RangedEnemy>().bulletEnemyTex = bulletEnemyTex;
            rangedEnemy.GetComponent<RangedEnemy>().bulletModel = bulletModel;
            rangedEnemy.GetComponent<RangedEnemy>().standardEffect = standardEffect;

            //heart
            rangedEnemy.GetComponent<RangedEnemy>().heartTex = heartTexture;
            rangedEnemy.GetComponent<RangedEnemy>().heartModel = heartModel;

            game.root.AddChildNode(rangedEnemy);
            game.enemyList.Add(rangedEnemy);
            return rangedEnemy;
        }

        public GameObject LoadMeleeEnemy()
        {
            GameObject meleeEnemy = new GameObject("meleeEnemy");
            GameObject meleeEnemyModel = new GameObject("meleeEnemy");

            triggers.Add(meleeEnemyModel);
            // Load anim models
            meleeEnemy.AddChildNode(meleeEnemyModel);

            // models without anims have problems i guess ; /
            meleeEnemyModel.AddComponent(new ModelAnimatedComponent("models/enemies/muchomorStadny/muchomorStadnyChod", game.Content, animatedEffect, meleeEnemyTex, meleeEnemyNormal));
            meleeEnemyModel.AddComponent(new AnimationManager(meleeEnemyModel));

            // IDLE
            meleeEnemyModel.GetComponent<AnimationManager>().AddAnimation(meleeIdle.AnimationClips[0], "idle");

            // WALK
            meleeEnemyModel.GetComponent<AnimationManager>().AddAnimation(meleeWalk.AnimationClips[0], "walk");

            // ATTACK
            meleeEnemyModel.GetComponent<AnimationManager>().AddAnimation(meleeAttack.AnimationClips[0], "attack");

            // gotHit
            meleeEnemyModel.GetComponent<AnimationManager>().AddAnimation(meleeGotHit.AnimationClips[0], "gotHit");

            // DEATH
            meleeEnemyModel.GetComponent<AnimationManager>().AddAnimation(meleeDeath.AnimationClips[0], "death");

            // TODO: ANIM LOAD SYSTEM / SELECTOR
            meleeEnemyModel.GetComponent<AnimationManager>().PlayAnimation("idle");
            meleeEnemy.AddComponent(new MeleeEnemy(meleeEnemy));
            meleeEnemy.GetComponent<MeleeEnemy>().ObjectSide = Side.Enemy;

            //heart
            meleeEnemy.GetComponent<MeleeEnemy>().standardEffect = standardEffect;
            meleeEnemy.GetComponent<MeleeEnemy>().heartTex = heartTexture;
            meleeEnemy.GetComponent<MeleeEnemy>().heartModel = heartModel;

            game.root.AddChildNode(meleeEnemy);
            game.enemyList.Add(meleeEnemy);
            return meleeEnemy;
        }

        private List<GameObject> SplitModelIntoSmallerPieces(Model bigModel, Texture2D bigTex = null, Texture2D bigNormalTex = null)
        {
            if (bigModel.Meshes.Count >= 1)
            {
                List<GameObject> result = new List<GameObject>();
                for (int i = 0; i < bigModel.Meshes.Count; i++)
                {
                    List<ModelBone> bones = new List<ModelBone>();
                    List<ModelMesh> meshes = new List<ModelMesh>();
                    bones.Add(bigModel.Meshes[i].ParentBone);
                    meshes.Add(bigModel.Meshes[i]);
                    ModelComponent newModel = new ModelComponent(new Model(game.GraphicsDevice, bones, meshes), standardEffect, bigTex, bigNormalTex);
                    GameObject newObj = new GameObject();
                    Vector3 position;
                    Vector3 scale;
                    Quaternion quat;
                    newModel.model.Meshes[0].ParentBone.Transform.Decompose(out scale, out quat, out position);
                    //  Debug.WriteLine("Position of new model " + position + " Rotation " + quat);
                    newObj.Position = position;
                    newObj.Scale = scale;
                    newObj.SetModelQuat(quat);
                    newObj.AddComponent(newModel);
                    newObj.name = bigModel.Meshes[i].Name;
                    newObj.Update();
                    result.Add(newObj);
                }
                return result;
            }
            else
            {
                throw new Exception("There is no mesh in this model !!");
            }
        }

        private void CreateHierarchyOfLevel(List<GameObject> mapa, GameObject rootMapy)
        {
            foreach (var newObj in mapa)
            {
                if (newObj.GetComponent<ModelComponent>().model.Bones[0].Children.Count > 0)
                {
                    for (int i = 0; i < newObj.GetComponent<ModelComponent>().model.Bones[0].Children.Count; i++)
                    {
                        foreach (var gameObject in mapa)
                        {
                            if (gameObject.GetComponent<ModelComponent>().model.Bones[0].Name ==
                                newObj.GetComponent<ModelComponent>().model.Bones[0].Children[i].Name &&
                                gameObject.Parent == null)
                            {
                                newObj.AddChildNode(gameObject);
                            }
                        }
                    }
                }
            }

            foreach (var newObj in mapa)
            {
                if (newObj.Parent == null)
                {
                    rootMapy.AddChildNode(newObj);
                }
            }
        }

        private void AssignTagsForMapElements(List<GameObject> mapa)
        {
            List<String> Groundy = new List<string>();
            Groundy.Add("Most");
            Groundy.Add("Platforma");
            Groundy.Add("Ground");
            Groundy.Add("OwinietaLina");
            Groundy.Add("Tron");

            List<String> Walle = new List<string>();
            Walle.Add("Brama");
            Walle.Add("Dzwignia");
            Walle.Add("Kolek");
            //Walle.Add("Lina");
            Walle.Add("Flag");
            Walle.Add("OwinietaLina");
            Walle.Add("Pal");
            Walle.Add("Wall");

            tagAssigner(mapa, Walle, "Wall");
            tagAssigner(mapa, Groundy, "Ground");

        }

        private void tagAssigner(List<GameObject> mapa, List<String> otagowaneNazwy, string tag)
        {
            foreach (var gameObj in mapa)
            {
                if (gameObj.tag != tag && otagowaneNazwy.Any(s => gameObj.name.Contains(s)))
                {
                    gameObj.tag = tag;
                }
            }
        }

        public void LoadLevel1()
        {
            if (!game.cutsceneLoaded)
            {
                new Cutscene(game.Content.Load<Texture2D>("Cutscene/1.4"), 12f, "Narrator: After the weird fight with Borovikus our hero found the throne room door locked.",
                    "Narrator: While he was looking for a way to break in, his attention was drawn by something unusual.");
                new Cutscene(game.Content.Load<Texture2D>("Cutscene/1.4"), 12f, "Player: Is it the newest painting by Michelshroomgelo? I know this scene, but I remember it differently...",
                    "Narrator: Our brave hero were lost in memories of scene depicted on the painting.");
            }
            game.cutsceneLoaded = false;
            GameObject mapRoot = new GameObject();
            ResetMap();

            game.updateComponents = new List<Component>();
            Model hierarchiaStrefa1 = game.Content.Load<Model>("Level1/levelStrefa1");
            Texture2D hierarchiaStrefa1Tex = game.Content.Load<Texture2D>("Level1/levelStrefa1Tex");
            Texture2D hierarchiaStrefa1Normal = game.Content.Load<Texture2D>("Level1/levelStrefa1Normal");

            List<GameObject> strefa1List = SplitModelIntoSmallerPieces(hierarchiaStrefa1, hierarchiaStrefa1Tex, hierarchiaStrefa1Normal);
            CreateHierarchyOfLevel(strefa1List, mapRoot);
            AssignTagsForMapElements(strefa1List);

            Model dzwignia = game.Content.Load<Model>("Level1/levelStrefa1Dzwignia");
            List<GameObject> dzwigniaList = SplitModelIntoSmallerPieces(dzwignia, hierarchiaStrefa1Tex, hierarchiaStrefa1Normal);
            CreateHierarchyOfLevel(dzwigniaList, mapRoot);
            AssignTagsForMapElements(dzwigniaList);

            LeverComponent lever = null;
            foreach (GameObject obj in dzwigniaList)
            {
                if (obj.name == "Zebatka1")
                {
                    lever = new LeverComponent(obj);
                    obj.AddComponent(lever);
                    game.updateComponents.Add(lever);
                }
            }

            foreach (GameObject obj in dzwigniaList)
            {
                if (obj.name == "Uchwyt1")
                {
                    obj.CreateColliders();
                    LeverTrigger comp = new LeverTrigger(obj, lever);
                    obj.SetAsTrigger(comp);
                }
            }

            GameObject gate = new GameObject("Wall");
            Model gateModel = game.Content.Load<Model>("Level1/levelStrefa1Brama");
            Vector3 position;
            Vector3 scale;
            Quaternion quat;
            gateModel.Meshes[0].ParentBone.Transform.Decompose(out scale, out quat, out position);
            gate.Position = position;
            gate.Scale = scale;
            gate.SetModelQuat(quat);
            gate.name = gateModel.Meshes[0].Name;
            ModelComponent modelGate = new ModelComponent(gateModel, standardEffect, hierarchiaStrefa1Tex, hierarchiaStrefa1Normal);
            gate.AddComponent(modelGate);
            GateComponent gateComponent = new GateComponent(gate, lever);
            gate.AddComponent(gateComponent);
            game.updateComponents.Add(gateComponent);
            mapRoot.AddChildNode(gate); //TU ODKOMENTOWA BY BRAMA SIĘ POJAWIŁA

            Model hierarchiaStrefa2 = game.Content.Load<Model>("Level1/levelStrefa2");
            Texture2D hierarchiaStrefa2Tex = game.Content.Load<Texture2D>("Level1/levelStrefa2Tex");
            Texture2D hierarchiaStrefa2Normal = game.Content.Load<Texture2D>("Level1/levelStrefa2Normal");

            List<GameObject> strefa2List = SplitModelIntoSmallerPieces(hierarchiaStrefa2, hierarchiaStrefa2Tex, hierarchiaStrefa2Normal);
            CreateHierarchyOfLevel(strefa2List, mapRoot);
            AssignTagsForMapElements(strefa2List);

            GameObject plat1 = new GameObject();
            Model platforma1 = game.Content.Load<Model>("Level1/levelStrefa2Platforma1");
            platforma1.Meshes[0].ParentBone.Transform.Decompose(out scale, out quat, out position);
            plat1.Position = position;
            plat1.Scale = scale;
            plat1.SetModelQuat(quat);
            plat1.name = platforma1.Meshes[0].Name;
            ModelComponent modelPlat1 = new ModelComponent(platforma1, standardEffect, hierarchiaStrefa2Tex, hierarchiaStrefa2Normal);
            plat1.AddComponent(modelPlat1);
            PlatformComponent plat1Comp = new PlatformComponent(plat1, plat1.Position - new Vector3(150f, 0f, 0f), 1.7f, 3f);
            plat1.AddComponent(plat1Comp);
            game.updateComponents.Add(plat1Comp);
            mapRoot.AddChildNode(plat1);

            GameObject plat2 = new GameObject();
            Model platforma2 = game.Content.Load<Model>("Level1/levelStrefa2Platforma2");
            platforma2.Meshes[0].ParentBone.Transform.Decompose(out scale, out quat, out position);
            plat2.Position = position;
            plat2.Scale = scale;
            plat2.SetModelQuat(quat);
            plat2.name = platforma2.Meshes[0].Name;
            ModelComponent modelPlat2 = new ModelComponent(platforma2, standardEffect, hierarchiaStrefa2Tex, hierarchiaStrefa2Normal);
            plat2.AddComponent(modelPlat2);
            PlatformComponent plat2Comp = new PlatformComponent(plat2, plat2.Position - new Vector3(150f, 0f, 0f), 0.7f, 4f);
            plat2.AddComponent(plat2Comp);
            game.updateComponents.Add(plat2Comp);
            mapRoot.AddChildNode(plat2);

            List<GameObject> list = new List<GameObject>();
            list.Add(plat1);
            list.Add(plat2);
            AssignTagsForMapElements(list);

            Model hierarchiaStrefa3 = game.Content.Load<Model>("Level1/levelStrefa3");
            Texture2D hierarchiaStrefa3Tex = game.Content.Load<Texture2D>("Level1/levelStrefa3Tex");
            Texture2D hierarchiaStrefa3Normal = game.Content.Load<Texture2D>("Level1/levelStrefa3Normal");

            List<GameObject> strefa3List = SplitModelIntoSmallerPieces(hierarchiaStrefa3, hierarchiaStrefa3Tex, hierarchiaStrefa3Normal);
            CreateHierarchyOfLevel(strefa3List, mapRoot);
            AssignTagsForMapElements(strefa3List);

            Model most = game.Content.Load<Model>("Level1/levelStrefa3Most");
            List<GameObject> mostList = SplitModelIntoSmallerPieces(most, hierarchiaStrefa3Tex, hierarchiaStrefa3Normal);
            CreateHierarchyOfLevel(mostList, mapRoot);
            AssignTagsForMapElements(mostList);

            BridgeComponent bridgeComp = null;
            foreach (GameObject obj in mostList)
            {
                if (obj.name == "Most1")
                {
                    bridgeComp = new BridgeComponent(obj);
                    obj.AddComponent(bridgeComp);
                    game.updateComponents.Add(bridgeComp);
                }
            }

            Model kolek1 = game.Content.Load<Model>("Level1/levelStrefa3Kolek1");
            List<GameObject> kolek1List = SplitModelIntoSmallerPieces(kolek1, hierarchiaStrefa3Tex, hierarchiaStrefa3Normal);
            CreateHierarchyOfLevel(kolek1List, mapRoot);
            AssignTagsForMapElements(kolek1List);

            Model kolek2 = game.Content.Load<Model>("Level1/levelStrefa3Kolek2");
            List<GameObject> kolek2List = SplitModelIntoSmallerPieces(kolek2, hierarchiaStrefa3Tex, hierarchiaStrefa3Normal);
            CreateHierarchyOfLevel(kolek2List, mapRoot);
            AssignTagsForMapElements(kolek2List);

            Model lina = game.Content.Load<Model>("Level1/levelStrefa3Lina");
            List<GameObject> linaList = SplitModelIntoSmallerPieces(lina, hierarchiaStrefa3Tex, hierarchiaStrefa3Normal);
            CreateHierarchyOfLevel(linaList, mapRoot);
            AssignTagsForMapElements(linaList);

            foreach (GameObject obj in linaList)
            {
                if (obj.name == "Lina1")
                {
                    obj.CreateColliders();
                    RopeCutConsumableTrigger comp = new RopeCutConsumableTrigger(obj, bridgeComp);
                    obj.SetAsTrigger(comp);
                    game.updateComponents.Add(comp);
                }
            }

            Model hierarchiaStrefa4 = game.Content.Load<Model>("Level1/levelStrefa4");
            Texture2D hierarchiaStrefa4Tex = game.Content.Load<Texture2D>("Level1/levelStrefa4Tex");
            Texture2D hierarchiaStrefa4Normal = game.Content.Load<Texture2D>("Level1/levelStrefa4Normal");

            List<GameObject> strefa4List = SplitModelIntoSmallerPieces(hierarchiaStrefa4, hierarchiaStrefa4Tex, hierarchiaStrefa4Normal);
            CreateHierarchyOfLevel(strefa4List, mapRoot);
            AssignTagsForMapElements(strefa4List);

            Model podest = game.Content.Load<Model>("Level1/levelStrefa4Podest");
            Texture2D podestTex = game.Content.Load<Texture2D>("Level1/levelStrefa4PodestTex");
            Texture2D podestNormal = game.Content.Load<Texture2D>("Level1/levelStrefa4PodestNormal");

            List<GameObject> podestList = SplitModelIntoSmallerPieces(podest, podestTex, podestNormal);
            CreateHierarchyOfLevel(podestList, mapRoot);
            AssignTagsForMapElements(podestList);

            GameObject endTrigger = new GameObject("endTrigger");
            EndLevelTrigger trigger = new EndLevelTrigger(endTrigger, new Vector3(1100f, 10f, -1800f), new Vector3(1300f, 100f, -1500f), game);
            endTrigger.AddComponent(trigger);
            endTrigger.CreateColliders();
            endTrigger.SetAsTrigger();
            mapRoot.AddChildNode(endTrigger);

            ShroomGame.pointLights.Add(new Lights.PointLight(new Vector3(0.0f, 8.0f, 0.0f)));
            ShroomGame.pointLights.Add(new Lights.PointLight(new Vector3(90.0f, -25.0f, -1350.0f), new Vector3(1.8f, 0.0002f, 0.00004f)));

            game.root.AddChildNode(mapRoot);

            Model refr = game.Content.Load<Model>("Level1/levelStrefa4Rzezba");
            refr.Meshes[0].ParentBone.Transform.Decompose(out scale, out quat, out position);
            game.refractiveObject.Position = position;
            game.refractiveObject.Scale = scale;
            game.DrawRefraction();
            ModelComponent refract = new ModelComponent(refr, refractionEffect,
                game.Content.Load<Texture2D>("Level1/levelStrefa4RzezbaTex"), game.Content.Load<Texture2D>("Level1/levelStrefa4RzezbaNormal"));
            refract.refractive = true;
            game.refractiveObject.AddComponent(refract);

            LoadPlayer(game.player, game.cameraCollision);
            game.player.Position = new Vector3(0f, 60f, 0f);
            game.player.RotationZ = MathHelper.Pi - MathHelper.PiOver4;

            GameObject enemyRanged1 = LoadRangedEnemy();
            enemyRanged1.Position = new Vector3(400f, 40f, -600f);
            GameObject enemy1 = LoadMeleeEnemy();
            enemy1.Position = new Vector3(0f, 40f, -600f);
            GameObject enemy2 = LoadMeleeEnemy();
            enemy2.Position = new Vector3(200f, 40f, -600f);
            GameObject enemy3 = LoadMeleeEnemy();
            enemy3.Position = new Vector3(600f, 40f, -550f);

            GameObject enemyRanged2 = LoadRangedEnemy();
            enemyRanged2.Position = new Vector3(100f, 40f, -1750f);
            GameObject enemy4 = LoadMeleeEnemy();
            enemy4.Position = new Vector3(200f, 40f, -1750f);
            GameObject enemy5 = LoadMeleeEnemy();
            enemy5.Position = new Vector3(400f, 40f, -1850f);


            GameObject enemy6 = LoadMeleeEnemy();
            enemy6.Position = new Vector3(600f, 40f, -1720f);
        }

        public void LoadTutorial()
        {
            ResetMap();
            if(game.roomEntranceCutscene)
            {
                new Cutscene(GameServices.GetService<ShroomGame>().Content.Load<Texture2D>("Cutscene/3,0"), 12f, "Narrator: When our hero came to his senses, he found the door to the throne room wide open.",
                "Narrator: He noticed that King pushed weakened Borovikus to defense.");
                new Cutscene(GameServices.GetService<ShroomGame>().Content.Load<Texture2D>("Cutscene/3,0"), 12f, "King: Finally you are here! Help me, kill the traitor!",                "Narrator: But our hero had already figured out who the real traitor was.");
            }
            GameObject mapRoot = new GameObject();
            Model hierarchiaStrefa1 = game.Content.Load<Model>("LevelTut/zamekStrefa1");
            Texture2D hierarchiaStrefa1Tex = game.Content.Load<Texture2D>("LevelTut/zamekStrefa1Tex");
            Texture2D hierarchiaStrefa1Normal = game.Content.Load<Texture2D>("LevelTut/zamekStrefa1Normal");

            List<GameObject> strefa1List = SplitModelIntoSmallerPieces(hierarchiaStrefa1, hierarchiaStrefa1Tex, hierarchiaStrefa1Normal);
            CreateHierarchyOfLevel(strefa1List, mapRoot);
            AssignTagsForMapElements(strefa1List);

            GameObject plat1 = new GameObject();
            Model platforma1 = game.Content.Load<Model>("LevelTut/zamekStrefa1Platforma");
            Vector3 position;
            Vector3 scale;
            Quaternion quat;
            platforma1.Meshes[0].ParentBone.Transform.Decompose(out scale, out quat, out position);
            plat1.Position = position;
            plat1.Scale = scale;
            plat1.SetModelQuat(quat);
            plat1.name = platforma1.Meshes[0].Name;
            ModelComponent modelPlat1 = new ModelComponent(platforma1, standardEffect, hierarchiaStrefa1Tex, hierarchiaStrefa1Normal);
            plat1.AddComponent(modelPlat1);
            PlatformComponent plat1Comp = new PlatformComponent(plat1, plat1.Position - new Vector3(150f, 0f, 0f), 0.1f, 1f);
            plat1.AddComponent(plat1Comp);
            game.updateComponents.Add(plat1Comp);
            mapRoot.AddChildNode(plat1);

            GameObject tutTrigger = new GameObject("tutTrigger");
            TutorialTrigger trigger = new TutorialTrigger(tutTrigger, position + new Vector3(-40f, 10f, -140f), position + new Vector3(40f, 80f, -100f), game);
            tutTrigger.AddComponent(trigger);
            tutTrigger.CreateColliders();
            tutTrigger.SetAsTrigger();
            mapRoot.AddChildNode(tutTrigger);


            List<GameObject> list = new List<GameObject>();
            list.Add(plat1);
            AssignTagsForMapElements(list);

            Model hierarchiaStrefa2 = game.Content.Load<Model>("LevelTut/zamekStrefa2");
            Texture2D hierarchiaStrefa2Tex = game.Content.Load<Texture2D>("LevelTut/zamekStrefa2Tex");
            Texture2D hierarchiaStrefa2Normal = game.Content.Load<Texture2D>("LevelTut/zamekStrefa2Normal");

            List<GameObject> strefa2List = SplitModelIntoSmallerPieces(hierarchiaStrefa2, hierarchiaStrefa2Tex, hierarchiaStrefa2Normal);
            CreateHierarchyOfLevel(strefa2List, mapRoot);
            AssignTagsForMapElements(strefa2List);

            GameObject door1 = new GameObject("Wall");
            Model door1Model = game.Content.Load<Model>("LevelTut/zamekStrefa2Drzwi1");
            door1Model.Meshes[0].ParentBone.Transform.Decompose(out scale, out quat, out position);
            door1.Position = position;
            door1.Scale = scale;
            door1.SetModelQuat(quat);
            door1.name = door1Model.Meshes[0].Name;
            ModelComponent door1ModelComp = new ModelComponent(door1Model, standardEffect, hierarchiaStrefa2Tex, hierarchiaStrefa2Normal);
            door1.AddComponent(door1ModelComp);
            DoorComponent door1Comp = new DoorComponent(door1, MathHelper.PiOver2, new Vector3(-25f, 0f, -25f));
            if (game.levelOneCompleted) door1Comp.closed = true;
            door1.AddComponent(door1Comp);
            game.updateComponents.Add(door1Comp);
            mapRoot.AddChildNode(door1);

            GameObject door2 = new GameObject("Wall");
            Model door2Model = game.Content.Load<Model>("LevelTut/zamekStrefa2Drzwi2");
            door2Model.Meshes[0].ParentBone.Transform.Decompose(out scale, out quat, out position);
            door2.Position = position;
            door2.Scale = scale;
            door2.SetModelQuat(quat);
            door2.name = door2Model.Meshes[0].Name;
            ModelComponent door2ModelComp = new ModelComponent(door2Model, standardEffect, hierarchiaStrefa2Tex, hierarchiaStrefa2Normal);
            door2.AddComponent(door2ModelComp);
            DoorComponent door2Comp = new DoorComponent(door2, -MathHelper.PiOver2, new Vector3(25f, 0f, -25f));
            if (game.levelOneCompleted) door2Comp.closed = true;
            door2.AddComponent(door2Comp);
            game.updateComponents.Add(door2Comp);
            mapRoot.AddChildNode(door2);

            GameObject door3 = new GameObject("Wall");
            Model door3Model = game.Content.Load<Model>("LevelTut/zamekStrefa2Drzwi3");
            door3Model.Meshes[0].ParentBone.Transform.Decompose(out scale, out quat, out position);
            door3.Position = position;
            door3.Scale = scale;
            door3.SetModelQuat(quat);
            door3.name = door3Model.Meshes[0].Name;
            ModelComponent door3ModelComp = new ModelComponent(door3Model, standardEffect, hierarchiaStrefa2Tex, hierarchiaStrefa2Normal);
            door3.AddComponent(door3ModelComp);
            DoorComponent door3Comp = new DoorComponent(door3, MathHelper.PiOver2, new Vector3(-45f, 0f, -45f));
            door3.AddComponent(door3Comp);
            game.door1 = door3Comp;
            game.updateComponents.Add(door3Comp);
            mapRoot.AddChildNode(door3);

            GameObject door4 = new GameObject("Wall");
            Model door4Model = game.Content.Load<Model>("LevelTut/zamekStrefa2Drzwi4");
            door4Model.Meshes[0].ParentBone.Transform.Decompose(out scale, out quat, out position);
            door4.Position = position;
            door4.Scale = scale;
            door4.SetModelQuat(quat);
            door4.name = door4Model.Meshes[0].Name;
            ModelComponent door4ModelComp = new ModelComponent(door4Model, standardEffect, hierarchiaStrefa2Tex, hierarchiaStrefa2Normal);
            door4.AddComponent(door4ModelComp);
            DoorComponent door4Comp = new DoorComponent(door4, -MathHelper.PiOver2, new Vector3(45f, 0f, -45f));
            door4.AddComponent(door4Comp);
            game.door2 = door4Comp;
            game.updateComponents.Add(door4Comp);
            mapRoot.AddChildNode(door4);

            List<GameObject> doorList = new List<GameObject>();
            list.Add(door1);
            list.Add(door2);
            list.Add(door3);
            list.Add(door4);
            AssignTagsForMapElements(doorList);

            if (!game.levelOneCompleted)
            {
                GameObject level1Trigger = new GameObject("level1Trigger");
                LoadLevel1Trigger triggerLevel = new LoadLevel1Trigger(level1Trigger, new Vector3(-120f, 10f, -900f), new Vector3(-60f, 100f, -550f), game);
                level1Trigger.AddComponent(triggerLevel);
                level1Trigger.CreateColliders();
                level1Trigger.SetAsTrigger();
                mapRoot.AddChildNode(level1Trigger);
            }

            Model hierarchiaStrefa3 = game.Content.Load<Model>("LevelTut/zamekStrefa3");
            Texture2D hierarchiaStrefa3Tex = game.Content.Load<Texture2D>("LevelTut/zamekStrefa3Tex");
            Texture2D hierarchiaStrefa3Normal = game.Content.Load<Texture2D>("LevelTut/zamekStrefa3Normal");

            List<GameObject> strefa3List = SplitModelIntoSmallerPieces(hierarchiaStrefa3, hierarchiaStrefa3Tex, hierarchiaStrefa3Normal);
            CreateHierarchyOfLevel(strefa3List, mapRoot);
            AssignTagsForMapElements(strefa3List);

            Model hierarchiaStrefa4 = game.Content.Load<Model>("LevelTut/zamekStrefa4");
            Texture2D hierarchiaStrefa4Tex = game.Content.Load<Texture2D>("LevelTut/zamekStrefa4Tex");
            Texture2D hierarchiaStrefa4Normal = game.Content.Load<Texture2D>("LevelTut/zamekStrefa4Normal");

            List<GameObject> strefa4List = SplitModelIntoSmallerPieces(hierarchiaStrefa4, hierarchiaStrefa4Tex, hierarchiaStrefa4Normal);
            CreateHierarchyOfLevel(strefa4List, mapRoot);
            AssignTagsForMapElements(strefa4List);

            game.root.AddChildNode(mapRoot);

            Model refr = game.Content.Load<Model>("LevelTut/zamekStrefa4Rzezby");
            refr.Meshes[0].ParentBone.Transform.Decompose(out scale, out quat, out position);
            game.refractiveObject.Position = position;
            game.refractiveObject.Scale = scale;
            game.DrawRefraction();
            ModelComponent refract = new ModelComponent(refr, refractionEffect,
                game.Content.Load<Texture2D>("LevelTut/zamekStrefa4RzezbyTex"), game.Content.Load<Texture2D>("LevelTut/zamekStrefa4RzezbyNormal"));
            refract.refractive = true;
            game.refractiveObject.AddComponent(refract);

            ShroomGame.pointLights = new List<Lights.PointLight>();

            ShroomGame.pointLights.Add(new Lights.PointLight(new Vector3(-20.0f, 150.0f, -620.0f), new Vector3(2.8f, 0.0002f, 0.00004f)));
            ShroomGame.pointLights.Add(new Lights.PointLight(new Vector3(-20.0f, 150.0f, -1300.0f), new Vector3(2.8f, 0.0002f, 0.00004f)));
            ShroomGame.pointLights.Add(new Lights.PointLight(new Vector3(-20.0f, 150.0f, -1900.0f), new Vector3(2.8f, 0.0002f, 0.00004f)));

            LoadPlayer(game.player, game.cameraCollision);
            GameObject borowikus = LoadBorowikus();
            GameObject king = LoadKing();
            if (game.levelOneCompleted)
            {
                game.player.Position = new Vector3(-20f, 100f, -600f);
                game.player.RotationZ = MathHelper.Pi;
                borowikus.Position = new Vector3(150f, 40f, -2090f);
                king.Position = new Vector3(0f, 40f, -1800f);
            }
            else
            {
                game.player.Position = new Vector3(-20f, 100f, -1500f);
                game.player.RotationZ = MathHelper.TwoPi;
                borowikus.Position = new Vector3(130f, 200f, 30f);
                king.Position = new Vector3(0f, 40f, -1900f);
            }
            if (!game.tutorialCompleted)
            {
                game.player.GetComponent<Player>().canUseE = false;
                game.player.GetComponent<Player>().canUseR = false;
                game.player.GetComponent<Player>().canUseQ = false;
            }


        }

        void LoadTutorialEnemies()
        {

        }

        void ResetMap()
        {
            game.areCollidersAndTriggersSet = false;
            ShroomGame.loadLevelTime = Timer.gameTime.TotalGameTime.TotalSeconds;
            Collider.ClearColliders();
            game.root = new GameObject();
            game.player = new GameObject("player");
            game.camera.SetCameraTarget(game.player);
            game.cameraCollision = new GameObject("cameraCollision");
            game.cameraCollision.AddComponent(new CameraCollisions(game.cameraCollision));
            game.enemyList = new List<GameObject>();
            game.refractiveObject = new GameObject();
            game.root.AddChildNode(game.refractiveObject);
            ShroomGame.pointLights = new List<Lights.PointLight>();
            ShroomGame.directionalLight = new Lights.DirectionalLight();
        }
    }
}
