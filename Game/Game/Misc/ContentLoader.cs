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
using Game.Components;
using Game.MainGame;
using Game.Components.Coliisions;
using Game.Components.MapElements;

namespace PBLGame.Misc
{
    class ContentLoader
    {
        private ShroomGame game;

        private List<GameObject> triggers;

        #region Textures
        Texture2D playerTex;
        Texture2D playerNormal;
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
            rangedEnemyTex = game.Content.Load<Texture2D>("models/enemies/muchomorRzucajacy/muchomorRzucajacyTex");
            rangedEnemyNormal = game.Content.Load<Texture2D>("models/enemies/muchomorRzucajacy/muchomorRzucajacyNormal");
            meleeEnemyTex = game.Content.Load<Texture2D>("models/enemies/muchomorStadny/muchomorStadnyTex");
            meleeEnemyNormal = game.Content.Load<Texture2D>("models/enemies/muchomorStadny/muchomorStadnyNormal");
            bulletEnemyTex = game.Content.Load<Texture2D>("models/enemies/muchomorRzucajacy/muchomorRzucajacyTex");
            bulletEnemyNormal = game.Content.Load<Texture2D>("models/enemies/muchomorRzucajacy/muchomorRzucajacyNormal");

            heartModel = game.Content.Load<Model>("apteczka");
            bulletModel = game.Content.Load<Model>("models/enemies/muchomorRzucajacy/Kulka");

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

            // TODO: ANIM LOAD SYSTEM / SELECTOR
            playerHat.GetComponent<AnimationManager>().PlayAnimation("idle");
            playerLeg.GetComponent<AnimationManager>().PlayAnimation("idle");

            player.AddComponent(new Player(player));
            game.root.AddChildNode(player);

            player.AddChildNode(cameraCollision);
            player.AddChildNode(GameServices.GetService<Camera>());
            player.RotationZ = 1.5f;

            GameServices.AddService(player);
        }

        public GameObject LoadRangedEnemy()
        {
            GameObject rangedEnemy = new GameObject();
            GameObject rangedEnemyHat = new GameObject("Hat");
            GameObject rangedEnemyHatWalk = new GameObject("Hat");
            GameObject rangedEnemyLeg = new GameObject("Leg");
            GameObject rangedEnemyLegWalk = new GameObject("Leg");

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
            GameObject meleeEnemy = new GameObject();
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

            List<String> Walle = new List<string>();
            Walle.Add("Brama");
            Walle.Add("Dzwignia");
            Walle.Add("Kolek");
            Walle.Add("Lina");
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
                    //Debug.WriteLine(gameObj.tag);
                }
            }
        }

        public void LoadLevel1()
        {
            GameObject mapRoot = new GameObject();
            new Cutscene(game.Content.Load<Texture2D>("Cutscene/1.1"), 3f);
            new Cutscene(game.Content.Load<Texture2D>("Cutscene/1.3"), 2f);
            new Cutscene(game.Content.Load<Texture2D>("Cutscene/1.2"), 2f);

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

            GameObject enemyRanged1 = LoadRangedEnemy();
            enemyRanged1.Position = new Vector3(-20f, 40f, -550f);
            GameObject enemyRanged2 = LoadRangedEnemy();
            enemyRanged2.Position = new Vector3(100f, 40f, -1750f);

            GameObject enemy1 = LoadMeleeEnemy();
            enemy1.Position = new Vector3(100f, 40f, -350f);
            GameObject enemy2 = LoadMeleeEnemy();
            enemy2.Position = new Vector3(150f, 40f, -350f);

            new DialogueString("I not have too much time, I need to find Borowikus quickly, there is no time to waste");
        }

        public void LoadTutorial()
        {
            GameObject mapRoot = new GameObject();
            game.updateComponents = new List<Component>();
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
            mapRoot.AddChildNode(plat1);

            GameObject tutTrigger = new GameObject();
            TutorialTrigger trigger = new TutorialTrigger(tutTrigger, position + new Vector3(-40f, 10f, -100f), position + new Vector3(40f, 100f, -140f));
            tutTrigger.AddComponent(trigger);
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

            /*Model platforma1 = Content.Load<Model>("Level1/levelStrefa2Platforma1");
            List<GameObject> platforma1List = SplitModelIntoSmallerPieces(platforma1, hierarchiaStrefa2Tex, hierarchiaStrefa2Normal);
            CreateHierarchyOfLevel(platforma1List, mapRoot);
            AssignTagsForMapElements(platforma1List);

            Model platforma2 = Content.Load<Model>("Level1/levelStrefa2Platforma2");
            List<GameObject> platforma2List = SplitModelIntoSmallerPieces(platforma2, hierarchiaStrefa2Tex, hierarchiaStrefa2Normal);
            CreateHierarchyOfLevel(platforma2List, mapRoot);
            AssignTagsForMapElements(platforma2List);*/

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
            //  Debug.WriteLine("Position of new model " + position + " Rotation " + quat);
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
            LoadTutorialEnemies();
        }

        void LoadTutorialEnemies()
        {

        }
    }
}
