using System;
using AnimationAux;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Game.Misc;
using Game.Misc.Time;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PBLGame.Input;
using PBLGame.MainGame;
using PBLGame.Misc;
using PBLGame.Misc.Anim;
using PBLGame.SceneGraph;
using System.IO;
using Game.Components;
using Game.Components.Collisions;
using Game.Components.Enemies;
using Game.MainGame;
using PBLGame.Input.Devices;

namespace PBLGame
{
    enum GameState
    {
        MainMenu,
        LevelTutorial,
        LevelOne
    }

    public class ShroomGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        Resolution resolution;
        SpriteBatch spriteBatch;
        Skybox skybox;

        private readonly InputManager inputManager;
        public static Texture2D missingTexture;
        public static Lights.DirectionalLight directionalLight;
        public static List<Lights.PointLight> pointLights;
        public static RenderTarget2D shadowRenderTarget;
        public static RenderTarget2D screenRenderTarget;
        public static RenderTargetCube refractionTarget;

        private float gammaValue = 0.9f;
        private bool areCollidersAndTriggersSet;

        private float textDisplayTime = 3f;
        private string actualDialogueText = "";
        private Texture2D hpTexture;
        private Texture2D timeTexture;

        GameObject root;
        GameObject heart;
        GameObject heart2;
        GameObject mapRoot;

        GameObject refractiveObject;

        GameObject player;
        GameObject playerLeg;
        GameObject playerHat;
        GameObject playerLegWalk;
        GameObject playerHatWalk;
        GameObject enemy1;
        GameObject meleeEnemyLeg;
        GameObject meleeEnemyHat;
        GameObject meleeEnemyLegWalk;
        GameObject meleeEnemyHatWalk;
        Camera camera;

        Texture2D playerTex;
        Texture2D playerNormal;
        Texture2D rangedEnemyTex;
        Texture2D rangedEnemyNormal;
        Texture2D apteczkaTexture;

        Effect standardEffect;
        Effect animatedEffect;
        Effect refractionEffect;
        Effect outlineEffect;
        SpriteFont dialoguesFont;
        const int shadowMapWidthHeight = 2048;

        GameState actualGameState;

        public ShroomGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            inputManager = InputManager.Instance;
            Content.RootDirectory = "Content";
            resolution = new Resolution();
            graphics.PreferredBackBufferHeight = 768;
            graphics.PreferredBackBufferWidth = 1366;
            //actualGameState = GameState.LevelTutorial;
            actualGameState = GameState.LevelOne;
        }

        protected override void Initialize()
        {
            base.Initialize();
            GameServices.AddService<GraphicsDevice>(GraphicsDevice);
            GameServices.AddService<GraphicsDeviceManager>(graphics);
            GameServices.AddService<Resolution>(resolution);
        }

        protected override void LoadContent()
        {
            Resources.Init(Content);

            spriteBatch = new SpriteBatch(GraphicsDevice);
            shadowRenderTarget = new RenderTarget2D(graphics.GraphicsDevice, shadowMapWidthHeight, shadowMapWidthHeight,
                                                    false, SurfaceFormat.Single, DepthFormat.Depth24);
            screenRenderTarget = new RenderTarget2D(graphics.GraphicsDevice, graphics.GraphicsDevice.Viewport.Width,
                                                    graphics.GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            refractionTarget = new RenderTargetCube(this.GraphicsDevice, shadowMapWidthHeight, true, SurfaceFormat.Color, DepthFormat.Depth24);


            standardEffect = Content.Load<Effect>("Standard");
            animatedEffect = Content.Load<Effect>("StandardAnimated");
            refractionEffect = Content.Load<Effect>("Refraction");
            outlineEffect = Content.Load<Effect>("Outline");
            outlineEffect.Parameters["ScreenSize"].SetValue(
               new Vector2(GraphicsDevice.Viewport.Bounds.Width, GraphicsDevice.Viewport.Bounds.Height));
            dialoguesFont = Content.Load<SpriteFont>("Dialogues");

            skybox = new Skybox("skybox/Sunset", Content);
            playerTex = Content.Load<Texture2D>("models/player/borowikTex");
            playerNormal = Content.Load<Texture2D>("models/player/borowikNormal");
            rangedEnemyTex = Content.Load<Texture2D>("models/enemies/muchomorRzucajacy/muchomorRzucajacyTex");
            rangedEnemyNormal = Content.Load<Texture2D>("models/enemies/muchomorRzucajacy/muchomorRzucajacyNormal");
            apteczkaTexture = Content.Load<Texture2D>("apteczkaTex");

            root = new GameObject();
            heart = new GameObject("Serce");
            heart2 = new GameObject("Serce");
            mapRoot = new GameObject();

            refractiveObject = new GameObject();
            root.AddChildNode(refractiveObject);

            player = new GameObject("player");
            playerLeg = new GameObject("Leg");
            playerHat = new GameObject("Hat");
            playerLegWalk = new GameObject("Leg");
            playerHatWalk = new GameObject("Hat");
            player.AddComponent(new Player(player));

            enemy1 = new GameObject("enemy1");
            meleeEnemyHat = new GameObject("Hat");
            meleeEnemyHatWalk = new GameObject("Hat");
            meleeEnemyLeg = new GameObject("Leg");
            meleeEnemyLegWalk = new GameObject("Leg");

            camera = new Camera();
            camera.SetCameraTarget(player);

            pointLights = new List<Lights.PointLight>();
            directionalLight = new Lights.DirectionalLight();
            missingTexture = Content.Load<Texture2D>("Missing");
            Model apteczka = Content.Load<Model>("apteczka");

            // Add static models
            heart.AddComponent(new ModelComponent(apteczka, standardEffect, apteczkaTexture));
            heart2.AddComponent(new ModelComponent(apteczka, standardEffect, apteczkaTexture));

            root.AddChildNode(heart);
            root.AddChildNode(heart2);
            heart.TransformationsOrder = TransformationOrder.ScalePositionRotation;
            heart2.TransformationsOrder = TransformationOrder.ScalePositionRotation;
            heart.Position = new Vector3(15.0f, 4.0f, -10.0f);
            heart2.Position = new Vector3(-15.0f, 4.0f, -10.0f);
            heart.Scale = new Vector3(0.4f);
            heart2.Scale = new Vector3(0.4f);

        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        protected override void Update(GameTime gameTime)
        {
            //resolutionChange
            if (inputManager.Keyboard[Keys.P])
            {
                resolution.SetResolution(800, 400);
                screenRenderTarget = new RenderTarget2D(graphics.GraphicsDevice, graphics.GraphicsDevice.Viewport.Width,
                                                   graphics.GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            }

            switch (actualGameState)
            {
                case GameState.MainMenu:
                    break;
                case GameState.LevelTutorial:
                    if (!areCollidersAndTriggersSet)
                    {
                        LoadTutorial();
                    }
                    break;
                case GameState.LevelOne:
                    if (!areCollidersAndTriggersSet)
                    {
                        LoadLevel1();
                    }
                    break;
            }

            if (!areCollidersAndTriggersSet)
            {
                root.CreateColliders();
                playerHat.SetAsColliderAndTrigger();
                meleeEnemyHat.SetAsColliderAndTrigger();
                heart.SetAsTrigger(new ConsumableTrigger(heart));
                heart2.SetAsTrigger(new ConsumableTrigger(heart2));
                areCollidersAndTriggersSet = true;
            }

            // Our Timer Class
            Timer.Update(gameTime);
            inputManager.Update();
            camera.Update();
            // Player update
            player.Update();
            player.Update(gameTime);
            enemy1.Update(gameTime);

            if (inputManager.Keyboard[Keys.Escape])
            {
                Exit();
            }
            // TEMP ANIMATION CHANGER
            // TODO: CREATE ANIMATION CHANGING CLASS N' METHODS :)

            if (areCollidersAndTriggersSet)
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

                if (inputManager.Keyboard[Keys.W].IsDown || inputManager.Keyboard[Keys.S].IsDown || inputManager.Keyboard[Keys.A].IsDown || inputManager.Keyboard[Keys.D].IsDown)
                {
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

                base.Update(gameTime);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            switch(actualGameState)
            {
                case GameState.MainMenu:
                    break;

                case GameState.LevelTutorial:
                case GameState.LevelOne:
                    CreateShadowMap();
                    //skybox.SetSkyBoxTexture(refractionTarget);
                    GraphicsDevice.SetRenderTarget(screenRenderTarget);
                    DrawWithShadow(camera.CalcViewMatrix());
                    DrawOutline();
                    DrawHpBar();
                    DrawTimeBar();

                    DrawText(gameTime);
                    break;
            }
            base.Draw(gameTime);
        }

        void CreateShadowMap()
        {
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            GraphicsDevice.SetRenderTarget(shadowRenderTarget);

            GraphicsDevice.Clear(Color.White);

            root.Draw(camera, true);
        }

        void DrawWithShadow(Matrix viewMatrix)
        {
            GraphicsDevice.Clear(Color.White);
            DrawSkybox();
            camera.ViewMatrix = viewMatrix;
            root.Draw(camera);
            GraphicsDevice.SetRenderTarget(null);
        }

        void DrawOutline()
        {
            outlineEffect.Parameters["GammaValue"].SetValue(gammaValue);
            spriteBatch.Begin(0, BlendState.Opaque, null, null, null, outlineEffect);
            spriteBatch.Draw(screenRenderTarget, Vector2.Zero, Color.White);
            spriteBatch.End();
        }

        void DrawRefraction()
        {
            for (int i = 0; i < 6; i++)
            {
                // render the scene to all cubemap faces
                CubeMapFace cubeMapFace = (CubeMapFace)i;

                switch (cubeMapFace)
                {
                    case CubeMapFace.NegativeX:
                        {
                            camera.ViewMatrix = Matrix.CreateLookAt(refractiveObject.Position, refractiveObject.Position + Vector3.Left, Vector3.Up);
                            break;
                        }
                    case CubeMapFace.NegativeY:
                        {
                            camera.ViewMatrix = Matrix.CreateLookAt(refractiveObject.Position, refractiveObject.Position + Vector3.Down, Vector3.Forward);
                            break;
                        }
                    case CubeMapFace.NegativeZ:
                        {
                            camera.ViewMatrix = Matrix.CreateLookAt(refractiveObject.Position, refractiveObject.Position + Vector3.Backward, Vector3.Up);
                            break;
                        }
                    case CubeMapFace.PositiveX:
                        {
                            camera.ViewMatrix = Matrix.CreateLookAt(refractiveObject.Position, refractiveObject.Position + Vector3.Right, Vector3.Up);
                            break;
                        }
                    case CubeMapFace.PositiveY:
                        {
                            camera.ViewMatrix = Matrix.CreateLookAt(refractiveObject.Position, refractiveObject.Position + Vector3.Up, Vector3.Backward);
                            break;
                        }
                    case CubeMapFace.PositiveZ:
                        {
                            camera.ViewMatrix = Matrix.CreateLookAt(refractiveObject.Position, refractiveObject.Position + Vector3.Forward, Vector3.Up);
                            break;
                        }
                }
                camera.fieldOfView = MathHelper.PiOver2;
                GraphicsDevice.SetRenderTarget(refractionTarget, cubeMapFace);
                GraphicsDevice.Clear(Color.White);
                DrawWithShadow(camera.ViewMatrix);
            }
            camera.fieldOfView = MathHelper.PiOver4;
        }

        void DrawSkybox()
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            RasterizerState originalRasterizerState = graphics.GraphicsDevice.RasterizerState;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            graphics.GraphicsDevice.RasterizerState = rasterizerState;
            Vector3 scale;
            Quaternion rotation;
            Vector3 cameraPosition;
            camera.WorldTransformations.Decompose(out scale, out rotation, out cameraPosition);
            skybox.Draw(Matrix.CreateLookAt(cameraPosition, player.Position, Vector3.UnitY),
                Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 1f, 0.1f, 4000f), cameraPosition);
            graphics.GraphicsDevice.RasterizerState = originalRasterizerState;
        }

        void DrawText(GameTime gameTime)
        {
            textDisplayTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (textDisplayTime < 0f || actualDialogueText == "")
            {
                actualDialogueText = DialogueString.GetActualDialogueString();
                textDisplayTime = 3f;
            }
            spriteBatch.Begin();
            spriteBatch.DrawString(dialoguesFont, actualDialogueText,
                new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height - 50f),
                Color.Snow, 0.0f, dialoguesFont.MeasureString(actualDialogueText) / 2, 1.0f, SpriteEffects.None, 0.5f);
            spriteBatch.End();
        }

        void DrawHpBar()
        {
            spriteBatch.Begin();
            spriteBatch.Draw(hpTexture, new Rectangle(20, graphics.GraphicsDevice.Viewport.Height - 60, player.GetComponent<Player>().GetHP() * 2, 20), Color.White);
            spriteBatch.End();
        }

        void DrawTimeBar()
        {
            spriteBatch.Begin();
            spriteBatch.Draw(timeTexture, new Rectangle(20, graphics.GraphicsDevice.Viewport.Height - 30, player.GetComponent<Player>().GetTimeEnergy() * 2, 20), Color.White);
            spriteBatch.End();
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
                    ModelComponent newModel = new ModelComponent(new Model(GraphicsDevice, bones, meshes), standardEffect, bigTex, bigNormalTex);
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

        void LoadLevel1()
        {
            Model hierarchiaStrefa1 = Content.Load<Model>("Level1/levelStrefa1");
            Texture2D hierarchiaStrefa1Tex = Content.Load<Texture2D>("Level1/levelStrefa1Tex");
            Texture2D hierarchiaStrefa1Normal = Content.Load<Texture2D>("Level1/levelStrefa1Normal");

            List<GameObject> strefa1List = SplitModelIntoSmallerPieces(hierarchiaStrefa1, hierarchiaStrefa1Tex, hierarchiaStrefa1Normal);
            CreateHierarchyOfLevel(strefa1List, mapRoot);
            AssignTagsForMapElements(strefa1List);

            Model dzwignia = Content.Load<Model>("Level1/levelStrefa1Dzwignia");
            List<GameObject> dzwigniaList = SplitModelIntoSmallerPieces(dzwignia, hierarchiaStrefa1Tex, hierarchiaStrefa1Normal);
            CreateHierarchyOfLevel(dzwigniaList, mapRoot);
            AssignTagsForMapElements(dzwigniaList);

            Model hierarchiaStrefa2 = Content.Load<Model>("Level1/levelStrefa2");
            Texture2D hierarchiaStrefa2Tex = Content.Load<Texture2D>("Level1/levelStrefa2Tex");
            Texture2D hierarchiaStrefa2Normal = Content.Load<Texture2D>("Level1/levelStrefa2Normal");

            List<GameObject> strefa2List = SplitModelIntoSmallerPieces(hierarchiaStrefa2, hierarchiaStrefa2Tex, hierarchiaStrefa2Normal);
            CreateHierarchyOfLevel(strefa2List, mapRoot);
            AssignTagsForMapElements(strefa2List);

            GameObject plat1 = new GameObject();
            Model platforma1 = Content.Load<Model>("Level1/levelStrefa2Platforma1");
            Vector3 position;
            Vector3 scale;
            Quaternion quat;
            platforma1.Meshes[0].ParentBone.Transform.Decompose(out scale, out quat, out position);
            plat1.Position = position;
            plat1.Scale = scale;
            plat1.SetModelQuat(quat);
            ModelComponent modelPlat1 = new ModelComponent(platforma1, standardEffect, hierarchiaStrefa2Tex, hierarchiaStrefa2Normal);
            plat1.AddComponent(modelPlat1);
            mapRoot.AddChildNode(plat1);

            GameObject plat2 = new GameObject();
            Model platforma2 = Content.Load<Model>("Level1/levelStrefa2Platforma2");
            platforma2.Meshes[0].ParentBone.Transform.Decompose(out scale, out quat, out position);
            plat2.Position = position;
            plat2.Scale = scale;
            plat2.SetModelQuat(quat);
            ModelComponent modelPlat2 = new ModelComponent(platforma2, standardEffect, hierarchiaStrefa2Tex, hierarchiaStrefa2Normal);
            plat2.AddComponent(modelPlat2);
            mapRoot.AddChildNode(plat2);

            Model hierarchiaStrefa3 = Content.Load<Model>("Level1/levelStrefa3");
            Texture2D hierarchiaStrefa3Tex = Content.Load<Texture2D>("Level1/levelStrefa3Tex");
            Texture2D hierarchiaStrefa3Normal = Content.Load<Texture2D>("Level1/levelStrefa3Normal");

            List<GameObject> strefa3List = SplitModelIntoSmallerPieces(hierarchiaStrefa3, hierarchiaStrefa3Tex, hierarchiaStrefa3Normal);
            CreateHierarchyOfLevel(strefa3List, mapRoot);
            AssignTagsForMapElements(strefa3List);

            Model most = Content.Load<Model>("Level1/levelStrefa3Most");
            List<GameObject> mostList = SplitModelIntoSmallerPieces(most, hierarchiaStrefa3Tex, hierarchiaStrefa3Normal);
            CreateHierarchyOfLevel(mostList, mapRoot);
            AssignTagsForMapElements(mostList);

            Model kolek1 = Content.Load<Model>("Level1/levelStrefa3Kolek1");
            List<GameObject> kolek1List = SplitModelIntoSmallerPieces(kolek1, hierarchiaStrefa3Tex, hierarchiaStrefa3Normal);
            CreateHierarchyOfLevel(kolek1List, mapRoot);
            AssignTagsForMapElements(kolek1List);

            Model kolek2 = Content.Load<Model>("Level1/levelStrefa3Kolek2");
            List<GameObject> kolek2List = SplitModelIntoSmallerPieces(kolek2, hierarchiaStrefa3Tex, hierarchiaStrefa3Normal);
            CreateHierarchyOfLevel(kolek2List, mapRoot);
            AssignTagsForMapElements(kolek2List);

            Model lina = Content.Load<Model>("Level1/levelStrefa3Lina");
            List<GameObject> linaList = SplitModelIntoSmallerPieces(lina, hierarchiaStrefa3Tex, hierarchiaStrefa3Normal);
            CreateHierarchyOfLevel(linaList, mapRoot);
            AssignTagsForMapElements(linaList);

            Model hierarchiaStrefa4 = Content.Load<Model>("Level1/levelStrefa4");
            Texture2D hierarchiaStrefa4Tex = Content.Load<Texture2D>("Level1/levelStrefa4Tex");
            Texture2D hierarchiaStrefa4Normal = Content.Load<Texture2D>("Level1/levelStrefa4Normal");

            List<GameObject> strefa4List = SplitModelIntoSmallerPieces(hierarchiaStrefa4, hierarchiaStrefa4Tex, hierarchiaStrefa4Normal);
            CreateHierarchyOfLevel(strefa4List, mapRoot);
            AssignTagsForMapElements(strefa4List);

            //pointLights.Add(new Lights.PointLight(new Vector3(0.0f, 8.0f, 0.0f)));
            pointLights.Add(new Lights.PointLight(new Vector3(15.0f, 8.0f, -60.0f)));

            root.AddChildNode(mapRoot);

            Model refr = Content.Load<Model>("Level1/levelStrefa4Rzezba");
            refr.Meshes[0].ParentBone.Transform.Decompose(out scale, out quat, out position);
            refractiveObject.Position = position;
            refractiveObject.Scale = scale;
            DrawRefraction();
            ModelComponent refract = new ModelComponent(refr, refractionEffect,
                Content.Load<Texture2D>("Level1/levelStrefa4RzezbaTex"), Content.Load<Texture2D>("Level1/levelStrefa4RzezbaNormal"));
            refract.refractive = true;
            refractiveObject.AddComponent(refract);

            LoadPlayer();
            player.Position = new Vector3(0f, 60f, 0f);

            LoadEnemies1();
        }

        void LoadEnemies1()
        {
            enemy1.AddChildNode(meleeEnemyLeg);
            enemy1.AddChildNode(meleeEnemyHat);

            // ENEMY
            meleeEnemyLeg.AddComponent(new ModelAnimatedComponent("models/enemies/muchomorRzucajacy/muchomorRzucajacyNozkaChod", Content, animatedEffect, rangedEnemyTex, rangedEnemyNormal));
            meleeEnemyLeg.AddComponent(new AnimationManager(meleeEnemyLeg));
            meleeEnemyHat.AddComponent(new ModelAnimatedComponent("models/enemies/muchomorRzucajacy/muchomorRzucajacyKapeluszChod", Content, animatedEffect, rangedEnemyTex, rangedEnemyNormal));
            meleeEnemyHat.AddComponent(new AnimationManager(meleeEnemyHat));
            enemy1.AddComponent(new MeleeEnemy(enemy1));

            //anims TODO: CHANGE MODEL COMPONENT TO SOMETHING ELSE : )

            root.AddChildNode(enemy1);
            
            enemy1.Position = new Vector3(-8f, 40f, -250f);
            new DialogueString("I not have too much time, I need to find Borowikus quickly, there is no time to waste");
        }

        void LoadTutorial()
        {
            Model hierarchiaStrefa1 = Content.Load<Model>("LevelTut/zamekStrefa1");
            Texture2D hierarchiaStrefa1Tex = Content.Load<Texture2D>("LevelTut/zamekStrefa1Tex");
            Texture2D hierarchiaStrefa1Normal = Content.Load<Texture2D>("LevelTut/zamekStrefa1Normal");

            List<GameObject> strefa1List = SplitModelIntoSmallerPieces(hierarchiaStrefa1, hierarchiaStrefa1Tex, hierarchiaStrefa1Normal);
            CreateHierarchyOfLevel(strefa1List, mapRoot);
            AssignTagsForMapElements(strefa1List);

            GameObject plat1 = new GameObject();
            Model platforma1 = Content.Load<Model>("LevelTut/zamekStrefa1Platforma");
            Vector3 position;
            Vector3 scale;
            Quaternion quat;
            platforma1.Meshes[0].ParentBone.Transform.Decompose(out scale, out quat, out position);
            plat1.Position = position;
            plat1.Scale = scale;
            plat1.SetModelQuat(quat);
            ModelComponent modelPlat1 = new ModelComponent(platforma1, standardEffect, hierarchiaStrefa1Tex, hierarchiaStrefa1Normal);
            plat1.AddComponent(modelPlat1);
            mapRoot.AddChildNode(plat1);

            Model hierarchiaStrefa2 = Content.Load<Model>("LevelTut/zamekStrefa2");
            Texture2D hierarchiaStrefa2Tex = Content.Load<Texture2D>("LevelTut/zamekStrefa2Tex");
            Texture2D hierarchiaStrefa2Normal = Content.Load<Texture2D>("LevelTut/zamekStrefa2Normal");

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

            Model hierarchiaStrefa3 = Content.Load<Model>("LevelTut/zamekStrefa3");
            Texture2D hierarchiaStrefa3Tex = Content.Load<Texture2D>("LevelTut/zamekStrefa3Tex");
            Texture2D hierarchiaStrefa3Normal = Content.Load<Texture2D>("LevelTut/zamekStrefa3Normal");

            List<GameObject> strefa3List = SplitModelIntoSmallerPieces(hierarchiaStrefa3, hierarchiaStrefa3Tex, hierarchiaStrefa3Normal);
            CreateHierarchyOfLevel(strefa3List, mapRoot);
            AssignTagsForMapElements(strefa3List);

            Model hierarchiaStrefa4 = Content.Load<Model>("LevelTut/zamekStrefa4");
            Texture2D hierarchiaStrefa4Tex = Content.Load<Texture2D>("LevelTut/zamekStrefa4Tex");
            Texture2D hierarchiaStrefa4Normal = Content.Load<Texture2D>("LevelTut/zamekStrefa4Normal");

            List<GameObject> strefa4List = SplitModelIntoSmallerPieces(hierarchiaStrefa4, hierarchiaStrefa4Tex, hierarchiaStrefa4Normal);
            CreateHierarchyOfLevel(strefa4List, mapRoot);
            AssignTagsForMapElements(strefa4List);

            root.AddChildNode(mapRoot);

            Model refr = Content.Load<Model>("LevelTut/zamekStrefa4Rzezby");
            refr.Meshes[0].ParentBone.Transform.Decompose(out scale, out quat, out position);
            //  Debug.WriteLine("Position of new model " + position + " Rotation " + quat);
            refractiveObject.Position = position;
            refractiveObject.Scale = scale;
            DrawRefraction();
            ModelComponent refract = new ModelComponent(refr, refractionEffect,
                Content.Load<Texture2D>("LevelTut/zamekStrefa4RzezbyTex"), Content.Load<Texture2D>("LevelTut/zamekStrefa4RzezbyNormal"));
            refract.refractive = true;
            refractiveObject.AddComponent(refract);

            pointLights = new List<Lights.PointLight>();

            pointLights.Add(new Lights.PointLight(new Vector3(-20.0f, 150.0f, -620.0f), new Vector3(2.8f, 0.0002f, 0.00004f)));
            pointLights.Add(new Lights.PointLight(new Vector3(-20.0f, 150.0f, -1300.0f), new Vector3(2.8f, 0.0002f, 0.00004f)));
            pointLights.Add(new Lights.PointLight(new Vector3(-20.0f, 150.0f, -1900.0f), new Vector3(2.8f, 0.0002f, 0.00004f)));

            LoadPlayer();
            player.Position = new Vector3(0f, 60f, -800f);
            LoadTutorialEnemies();
        }

        void LoadTutorialEnemies()
        {

        }

        void LoadPlayer()
        {
            hpTexture = apteczkaTexture;
            timeTexture = playerNormal;

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

            // ATTACK
            playerLeg.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikNozkaSlash", Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "slash");
            playerHat.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikKapeluszSlash", Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "slash");

            // THROW
            playerLeg.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikNozkaRzutKapeluszem", Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "throw");
            playerHat.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikKapeluszRzutKapeluszem", Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "throw");

            // TODO: ANIM LOAD SYSTEM / SELECTOR
            playerHat.GetComponent<AnimationManager>().PlayAnimation("idle");
            playerLeg.GetComponent<AnimationManager>().PlayAnimation("idle");

            root.AddChildNode(player);

            player.AddChildNode(camera);
            player.RotationZ = 1.5f;

            GameServices.AddService(player);

        }
    }
}
