using Game.Components.Collisions;
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

namespace PBLGame.Misc
{
    class ContentLoader
    {
        private ContentManager Content;
        private GameObject root;

        private List<GameObject> triggers;

        Model heartModel;

        Texture2D playerTex;
        Texture2D playerNormal;
        Texture2D rangedEnemyTex;
        Texture2D rangedEnemyNormal;
        Texture2D meleeEnemyTex;
        Texture2D meleeEnemyNormal;
        Texture2D bulletEnemyTex;
        Texture2D bulletEnemyNormal;
        Texture2D heartTexture;

        Effect standardEffect;
        Effect animatedEffect;
        Effect refractionEffect;

        public ContentLoader(ContentManager content, GameObject root)
        {
            this.Content = content;
            this.root = root;
            triggers = new List<GameObject>();
            LoadContent();
        }

        public void LoadContent()
        {
            standardEffect = Content.Load<Effect>("Standard");
            animatedEffect = Content.Load<Effect>("StandardAnimated");
            refractionEffect = Content.Load<Effect>("Refraction");
            playerTex = Content.Load<Texture2D>("models/player/borowikTex");
            playerNormal = Content.Load<Texture2D>("models/player/borowikNormal");
            rangedEnemyTex = Content.Load<Texture2D>("models/enemies/muchomorRzucajacy/muchomorRzucajacyTex");
            rangedEnemyNormal = Content.Load<Texture2D>("models/enemies/muchomorRzucajacy/muchomorRzucajacyNormal");
            meleeEnemyTex = Content.Load<Texture2D>("models/enemies/muchomorStadny/muchomorStadnyTex");
            meleeEnemyNormal = Content.Load<Texture2D>("models/enemies/muchomorStadny/muchomorStadnyNormal");
            bulletEnemyTex = Content.Load<Texture2D>("models/enemies/muchomorRzucajacy/muchomorRzucajacyTex");
            bulletEnemyNormal = Content.Load<Texture2D>("models/enemies/muchomorRzucajacy/muchomorRzucajacyNormal");
            heartModel = Content.Load<Model>("apteczka");
        }

        public void SetAsColliderAndTrigger()
        {
            foreach(GameObject obj in triggers)
            {
                obj.SetAsColliderAndTrigger(new HitTrigger(obj));
            }
        }

        public void LoadPlayer(GameObject player, GameObject cameraCollision)
        {
            GameObject playerLeg = new GameObject("Leg");
            GameObject playerHat = new GameObject("Hat");
            triggers.Add(playerHat);
            GameObject playerLegWalk = new GameObject("Leg");
            GameObject playerHatWalk = new GameObject("Hat");

            // Load anim models
            player.AddChildNode(playerLeg);
            player.AddChildNode(playerHat);

            // models without anims have problems i guess ; /
            playerLeg.AddComponent(new ModelAnimatedComponent("models/player/borowikNozkaChod", Content, animatedEffect, playerTex, playerNormal));
            playerLeg.AddComponent(new AnimationManager(playerLeg));
            playerHat.AddComponent(new ModelAnimatedComponent("models/player/borowikKapeluszChod", Content, animatedEffect, playerTex, playerNormal));
            playerHat.AddComponent(new AnimationManager(playerHat));

            // ENABLE DYNAMIC COLLISION ON PLAYER HAT
            playerHat.GetComponent<ModelAnimatedComponent>().ColliderDynamicUpdateEnable();

            // IDLE
            playerLeg.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikNozkaIdle", Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "idle");
            playerHat.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikKapeluszIdle", Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "idle");

            // WALK
            playerLeg.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikNozkaChod", Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "walk");
            playerHat.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikKapeluszChod", Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "walk");

            // ATTACK MAIN
            playerLeg.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikNozkaSlash", Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "slash");
            playerHat.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikKapeluszSlash", Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "slash");

            // ATTACK LEFT
            playerLeg.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikNozkaSlashLewo", Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "slashL");
            playerHat.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikKapeluszSlashLewo", Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "slashL");

            // ATTACK RIGHT
            playerLeg.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikNozkaSlashPrawo", Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "slashR");
            playerHat.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikKapeluszSlashPrawo", Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "slashR");

            // THROW
            playerLeg.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikNozkaRzutKapeluszem", Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "throw");
            playerHat.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikKapeluszRzutKapeluszem", Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "throw");

            // JUMPATTACK1
            playerLeg.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikNozkaRzutKapeluszem", Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "jumpAttack1");
            playerHat.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikKapeluszRzutKapeluszem", Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "jumpAttack1");


            // JUMPATTACK2
            playerLeg.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikNozkaRzutKapeluszem", Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "jumpAttack2");
            playerHat.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikKapeluszRzutKapeluszem", Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "jumpAttack2");

            // TODO: ANIM LOAD SYSTEM / SELECTOR
            playerHat.GetComponent<AnimationManager>().PlayAnimation("idle");
            playerLeg.GetComponent<AnimationManager>().PlayAnimation("idle");

            player.AddComponent(new Player(player));
            root.AddChildNode(player);

            player.AddChildNode(cameraCollision);
            player.AddChildNode(GameServices.GetService<Camera>());
            player.RotationZ = 1.5f;

            GameServices.AddService(player);
        }

        public void LoadRangedEnemy(GameObject rangedEnemy1)
        {
            GameObject rangedEnemyHat = new GameObject("Hat");
            GameObject rangedEnemyHatWalk = new GameObject("Hat");
            GameObject rangedEnemyLeg = new GameObject("Leg");
            GameObject rangedEnemyLegWalk = new GameObject("Leg");

            triggers.Add(rangedEnemyHat);

            // Load anim models
            rangedEnemy1.AddChildNode(rangedEnemyLeg);
            rangedEnemy1.AddChildNode(rangedEnemyHat);

            // models without anims have problems i guess ; /
            rangedEnemyLeg.AddComponent(new ModelAnimatedComponent("models/enemies/muchomorRzucajacy/muchomorRzucajacyNozkaChod", Content, animatedEffect, rangedEnemyTex, rangedEnemyNormal));
            rangedEnemyLeg.AddComponent(new AnimationManager(rangedEnemyLeg));
            rangedEnemyHat.AddComponent(new ModelAnimatedComponent("models/enemies/muchomorRzucajacy/muchomorRzucajacyKapeluszChod", Content, animatedEffect, rangedEnemyTex, rangedEnemyNormal));
            rangedEnemyHat.AddComponent(new AnimationManager(rangedEnemyHat));

            // ENABLE DYNAMIC COLLISION ON ENEMY HAT
            rangedEnemyHat.GetComponent<ModelAnimatedComponent>().ColliderDynamicUpdateEnable();

            // IDLE
            rangedEnemyLeg.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/enemies/muchomorRzucajacy/muchomorRzucajacyNozkaIdle", Content, animatedEffect, rangedEnemyTex, rangedEnemyNormal).AnimationClips[0], "idle");
            rangedEnemyHat.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/enemies/muchomorRzucajacy/muchomorRzucajacyKapeluszIdle", Content, animatedEffect, rangedEnemyTex, rangedEnemyNormal).AnimationClips[0], "idle");

            // WALK
            rangedEnemyLeg.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/enemies/muchomorRzucajacy/muchomorRzucajacyNozkaChod", Content, animatedEffect, rangedEnemyTex, rangedEnemyNormal).AnimationClips[0], "walk");
            rangedEnemyHat.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/enemies/muchomorRzucajacy/muchomorRzucajacyKapeluszChod", Content, animatedEffect, rangedEnemyTex, rangedEnemyNormal).AnimationClips[0], "walk");

            // GOTHIT
            rangedEnemyLeg.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/enemies/muchomorRzucajacy/muchomorRzucajacyNozkaOberwal", Content, animatedEffect, rangedEnemyTex, rangedEnemyNormal).AnimationClips[0], "gotHit");
            rangedEnemyHat.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/enemies/muchomorRzucajacy/muchomorRzucajacyKapeluszOberwal", Content, animatedEffect, rangedEnemyTex, rangedEnemyNormal).AnimationClips[0], "gotHit");

            // DEATH
            rangedEnemyLeg.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/enemies/muchomorRzucajacy/muchomorRzucajacyNozkaUmarl", Content, animatedEffect, rangedEnemyTex, rangedEnemyNormal).AnimationClips[0], "death");
            rangedEnemyHat.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/enemies/muchomorRzucajacy/muchomorRzucajacyKapeluszUmarl", Content, animatedEffect, rangedEnemyTex, rangedEnemyNormal).AnimationClips[0], "death");

            // TODO: ANIM LOAD SYSTEM / SELECTOR
            rangedEnemyLeg.GetComponent<AnimationManager>().PlayAnimation("idle");
            rangedEnemyHat.GetComponent<AnimationManager>().PlayAnimation("idle");
            rangedEnemy1.AddComponent(new RangedEnemy(rangedEnemy1));
            rangedEnemy1.GetComponent<RangedEnemy>().ObjectSide = Side.Enemy;

            //bullet
            rangedEnemy1.GetComponent<RangedEnemy>().bulletEnemyNormal = bulletEnemyNormal;
            rangedEnemy1.GetComponent<RangedEnemy>().bulletEnemyTex = bulletEnemyTex;
            rangedEnemy1.GetComponent<RangedEnemy>().bulletModel = Content.Load<Model>("models/enemies/muchomorRzucajacy/Kulka");
            rangedEnemy1.GetComponent<RangedEnemy>().standardEffect = standardEffect;

            //heart
            rangedEnemy1.GetComponent<RangedEnemy>().heartTex = heartTexture;
            rangedEnemy1.GetComponent<RangedEnemy>().heartModel = heartModel;

            root.AddChildNode(rangedEnemy1);
            rangedEnemy1.Position = new Vector3(-20f, 40f, -550f);
        }

        public void LoadMeleeEnemy(GameObject meleeEnemy1)
        {
            GameObject meleeEnemyModel = new GameObject("meleeEnemy");

            triggers.Add(meleeEnemyModel);
            // Load anim models
            meleeEnemy1.AddChildNode(meleeEnemyModel);

            // models without anims have problems i guess ; /
            meleeEnemyModel.AddComponent(new ModelAnimatedComponent("models/enemies/muchomorStadny/muchomorStadnyChod", Content, animatedEffect, meleeEnemyTex, meleeEnemyNormal));
            meleeEnemyModel.AddComponent(new AnimationManager(meleeEnemyModel));

            // IDLE
            meleeEnemyModel.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/enemies/muchomorStadny/muchomorStadnyIdle", Content, animatedEffect, meleeEnemyTex, meleeEnemyNormal).AnimationClips[0], "idle");

            // WALK
            meleeEnemyModel.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/enemies/muchomorStadny/muchomorStadnyChod", Content, animatedEffect, meleeEnemyTex, meleeEnemyNormal).AnimationClips[0], "walk");

            // ATTACK
            meleeEnemyModel.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/enemies/muchomorStadny/muchomorStadnyAtak", Content, animatedEffect, meleeEnemyTex, meleeEnemyNormal).AnimationClips[0], "attack");

            // gotHit
            meleeEnemyModel.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/enemies/muchomorStadny/muchomorStadnyOberwal", Content, animatedEffect, meleeEnemyTex, meleeEnemyNormal).AnimationClips[0], "gotHit");

            // DEATH
            meleeEnemyModel.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/enemies/muchomorStadny/muchomorStadnyUmarl", Content, animatedEffect, meleeEnemyTex, meleeEnemyNormal).AnimationClips[0], "death");

            // TODO: ANIM LOAD SYSTEM / SELECTOR
            meleeEnemyModel.GetComponent<AnimationManager>().PlayAnimation("idle");
            meleeEnemy1.AddComponent(new MeleeEnemy(meleeEnemy1));
            meleeEnemy1.GetComponent<MeleeEnemy>().ObjectSide = Side.Enemy;

            //heart
            meleeEnemy1.GetComponent<MeleeEnemy>().standardEffect = standardEffect;
            meleeEnemy1.GetComponent<MeleeEnemy>().heartTex = heartTexture;
            meleeEnemy1.GetComponent<MeleeEnemy>().heartModel = heartModel;

            root.AddChildNode(meleeEnemy1);
            meleeEnemy1.Position = new Vector3(100f, 40f, -350f);
        }
    }
}
