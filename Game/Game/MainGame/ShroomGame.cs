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
using Game.Components.Enemies;

namespace PBLGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class ShroomGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Skybox skybox;

        private readonly InputManager inputManager;
        public static Texture2D missingTexture;
        public static Texture2D missingNormalMap;
        public static Lights.DirectionalLight directionalLight;
        public static List<Lights.PointLight> pointLights;
        public static RenderTarget2D shadowRenderTarget;
        public static RenderTarget2D screenRenderTarget;

        private float gammaValue = 1.1f;
        private bool areCollidersAndTriggersSet;
        private int counterOfUpdatesToCreateCollidersAndTriggers = 0;

        private float textDisplayTime = 3f;
        private string actualDialogueText = "";
        private Texture2D hpTexture;
        private Texture2D timeTexture;

        GameObject root;
        GameObject heart;
        GameObject heart2;
        GameObject levelOne;

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

        Effect standardEffect;
        Effect animatedEffect;
        Effect outlineEffect;
        SpriteFont dialoguesFont;
        const int shadowMapWidthHeight = 2048;

        public ShroomGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            inputManager = InputManager.Instance;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
            GameServices.AddService<GraphicsDevice>(GraphicsDevice);
        }

        protected override void LoadContent()
        {
            // TODO: WRITE CONTENT MANAGER EXTENSION
            Resources.Init(Content);

            spriteBatch = new SpriteBatch(GraphicsDevice);
            shadowRenderTarget = new RenderTarget2D(graphics.GraphicsDevice, shadowMapWidthHeight, shadowMapWidthHeight,
                                                    false, SurfaceFormat.Single, DepthFormat.Depth24);
            screenRenderTarget = new RenderTarget2D(graphics.GraphicsDevice, graphics.GraphicsDevice.Viewport.Width,
                graphics.GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);

            standardEffect = Content.Load<Effect>("Standard");
            animatedEffect = Content.Load<Effect>("StandardAnimated");
            outlineEffect = Content.Load<Effect>("Outline");
            outlineEffect.Parameters["ScreenSize"].SetValue(
               new Vector2(GraphicsDevice.Viewport.Bounds.Width, GraphicsDevice.Viewport.Bounds.Height));
            dialoguesFont = Content.Load<SpriteFont>("Dialogues");
            skybox = new Skybox("skybox/Sunset", Content);
            root = new GameObject();
            heart = new GameObject();
            heart2 = new GameObject();
            levelOne = new GameObject();

            player = new GameObject();
            playerLeg = new GameObject("Leg");
            playerHat = new GameObject("Hat");
            playerLegWalk = new GameObject("Leg");
            playerHatWalk = new GameObject("Hat");

            enemy1 = new GameObject();
            meleeEnemyHat = new GameObject("Hat");
            meleeEnemyHatWalk = new GameObject("Hat");
            meleeEnemyLeg = new GameObject("Leg");
            meleeEnemyLegWalk = new GameObject("Leg");

            camera = new Camera();
            camera.SetCameraTarget(player);

            pointLights = new List<Lights.PointLight>();
            directionalLight = new Lights.DirectionalLight();
            missingTexture = Content.Load<Texture2D>("Missing");
            missingNormalMap = Content.Load<Texture2D>("Level/level1Normal");
            Model apteczka = Content.Load<Model>("apteczka");
            Texture2D apteczkaTexture = Content.Load<Texture2D>("apteczkaTex");
            Model hierarchia = Content.Load<Model>("Level/level1");
            Texture2D hierarchiaTex = Content.Load<Texture2D>("Level/level1Tex");
            Texture2D hierarchiaNormalTex = Content.Load<Texture2D>("Level/level1Normal");
            Texture2D playerTex = Content.Load<Texture2D>("models/player/borowikTex");
            Texture2D playerNormal = Content.Load<Texture2D>("models/player/borowikNormal");
            Texture2D rangedEnemyTex = Content.Load<Texture2D>("models/enemies/muchomorRzucajacy/muchomorRzucajacyTex");
            Texture2D rangedEnemyNormal = Content.Load<Texture2D>("models/enemies/muchomorRzucajacy/muchomorRzucajacyNormal");
            hpTexture = apteczkaTexture;
            timeTexture = playerNormal;

            // Load anim model
            player.AddChildNode(playerLeg);
            player.AddChildNode(playerHat);
            enemy1.AddChildNode(meleeEnemyLeg);
            enemy1.AddChildNode(meleeEnemyHat);
            
            // models without anims have problems i guess ; /
            playerLeg.AddComponent(new ModelAnimatedComponent("models/player/borowikNozkaChod", Content, animatedEffect, playerTex, playerNormal));
            playerLeg.AddComponent(new AnimationManager(playerLeg));
            playerHat.AddComponent(new ModelAnimatedComponent("models/player/borowikKapeluszChod", Content, animatedEffect, playerTex, playerNormal));
            playerHat.AddComponent(new AnimationManager(playerHat));
            player.AddComponent(new Player(player));
            meleeEnemyLeg.AddComponent(new ModelAnimatedComponent("models/enemies/muchomorRzucajacy/muchomorRzucajacyNozkaChod", Content, animatedEffect, rangedEnemyTex, rangedEnemyNormal));
            meleeEnemyLeg.AddComponent(new AnimationManager(meleeEnemyLeg));
            meleeEnemyHat.AddComponent(new ModelAnimatedComponent("models/enemies/muchomorRzucajacy/muchomorRzucajacyKapeluszChod", Content, animatedEffect, rangedEnemyTex, rangedEnemyNormal));
            meleeEnemyHat.AddComponent(new AnimationManager(meleeEnemyHat));
            enemy1.AddComponent(new MeleeEnemy(enemy1));

            //anims TODO: CHANGE MODEL COMPONENT TO SOMETHING ELSE : )
            // WALK
            playerLeg.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikNozkaChod", Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "walk");
            playerHat.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikKapeluszChod", Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "walk");

            // ATTACK
            playerLeg.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikNozkaSlash", Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "slash");
            playerHat.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikKapeluszSlash", Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "slash");

            // THROW
            playerLeg.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikNozkaRzutKapeluszem", Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "throw");
            playerHat.GetComponent<AnimationManager>().AddAnimation(new ModelAnimatedComponent("models/player/borowikKapeluszRzutKapeluszem", Content, animatedEffect, playerTex, playerNormal).AnimationClips[0], "throw");

            List<GameObject> hiererchyList = SplitModelIntoSmallerPieces(hierarchia, hierarchiaTex, hierarchiaNormalTex);
            CreateHierarchyOfLevel(hiererchyList, levelOne);
            AssignTagsForMapElements(hiererchyList);

            // TODO: ANIM LOAD SYSTEM / SELECTOR
            playerHat.GetComponent<AnimationManager>().PlayAnimation("walk");
            playerLeg.GetComponent<AnimationManager>().PlayAnimation("walk");
  

            // Add static models
            heart.AddComponent(new ModelComponent(apteczka, standardEffect, apteczkaTexture));
            heart2.AddComponent(new ModelComponent(apteczka, standardEffect, apteczkaTexture));

            pointLights.Add(new Lights.PointLight(new Vector3(0.0f, 8.0f, 0.0f)));
            pointLights.Add(new Lights.PointLight(new Vector3(15.0f, 8.0f, 60.0f)));

            root.AddChildNode(heart);
            root.AddChildNode(heart2);
            root.AddChildNode(player);
            root.AddChildNode(enemy1);
            root.AddChildNode(levelOne);
            player.AddChildNode(camera);
            heart.TransformationsOrder = TransformationOrder.ScalePositionRotation;
            heart2.TransformationsOrder = TransformationOrder.ScalePositionRotation;
            heart.Position = new Vector3(15.0f, 4.0f, -10.0f);
            heart2.Position = new Vector3(-15.0f, 4.0f, -10.0f);
            heart.Scale = new Vector3(0.2f);
            heart2.Scale = new Vector3(0.2f);
            player.Position = new Vector3(0f, 40f, 0f);
            enemy1.Position = new Vector3(-8f, 3f, -150f);
            enemy1.Scale = new Vector3(0.4f);
            player.RotationZ = 1.5f;
            player.Scale = new Vector3(0.4f);
            GameServices.AddService(player);
            new DialogueString("I not have too much time, I need to find Borowikus quickly, there is no time to waste");

        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        protected override void Update(GameTime gameTime)
        {
            if (!areCollidersAndTriggersSet)
            {
                counterOfUpdatesToCreateCollidersAndTriggers++;
                if (counterOfUpdatesToCreateCollidersAndTriggers > 10)
                {
                    root.CreateColliders();
                    heart.SetAsTrigger();
                    heart2.SetAsTrigger();
                    areCollidersAndTriggersSet = true;
                }
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

          
            //if (inputManager.Keyboard[Keys.Q])
            //{
            //    playerHat.GetComponent<AnimationManager>().PlayAnimation("walk");
            //    playerLeg.GetComponent<AnimationManager>().PlayAnimation("walk");
            //    anim = Anim.walk;
            //}

            if (inputManager.Keyboard[Keys.E] && playerHat.GetComponent<AnimationManager>().isReady)
            {
                playerHat.GetComponent<AnimationManager>().PlayAnimation("slash");
                playerLeg.GetComponent<AnimationManager>().PlayAnimation("slash");
                playerHat.GetComponent<AnimationManager>().isReady = false;
                playerLeg.GetComponent<AnimationManager>().isReady = false;

            }

            if (inputManager.Keyboard[Keys.Q] && playerHat.GetComponent<AnimationManager>().isReady)
            {
                playerHat.GetComponent<AnimationManager>().PlayAnimation("throw");
                playerLeg.GetComponent<AnimationManager>().PlayAnimation("throw");
                playerHat.GetComponent<AnimationManager>().isReady = false;
                playerLeg.GetComponent<AnimationManager>().isReady = false;
            }

            //if (inputManager.Keyboard[Keys.LeftAlt])
            //{
            //    playerHat.GetComponent<AnimationManager>().PlayAnimation("walk", true);
            //    playerLeg.GetComponent<AnimationManager>().PlayAnimation("walk", true);
            //    anim = Anim.walk;
            //}


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            CreateShadowMap();
            DrawWithShadow();

            DrawHpBar();
            DrawTimeBar();

            DrawText(gameTime);
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

        void DrawWithShadow()
        {
            GraphicsDevice.SetRenderTarget(screenRenderTarget);
            DrawSkybox();
            root.Draw(camera);
            GraphicsDevice.SetRenderTarget(null);

            outlineEffect.Parameters["GammaValue"].SetValue(gammaValue);
            spriteBatch.Begin(0, BlendState.Opaque, null, null, null, outlineEffect);
            spriteBatch.Draw(screenRenderTarget, Vector2.Zero, Color.White);
            spriteBatch.End();
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
                Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 1f, 0.1f, 4000f), cameraPosition - new Vector3(0f,0f,skybox.size/2));
            graphics.GraphicsDevice.RasterizerState = originalRasterizerState;
        }

        void DrawText(GameTime gameTime)
        {
            textDisplayTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if(textDisplayTime < 0f || actualDialogueText == "")
            {
                actualDialogueText = DialogueString.GetActualDialogueString();
                textDisplayTime = 3f;
            }            
            spriteBatch.Begin();
            spriteBatch.DrawString(dialoguesFont, actualDialogueText,
                new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height - 50f),
                Color.Snow, 0.0f, dialoguesFont.MeasureString(actualDialogueText) /2, 1.0f, SpriteEffects.None, 0.5f);
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
            Groundy.Add("podloga");
            Groundy.Add("schodki");
            Groundy.Add("most");
            Groundy.Add("platforma");
            Groundy.Add("podest");
            Groundy.Add("schody");

            List<String> Walle = new List<string>();
            Walle.Add("brama");
            Walle.Add("sciany");
            Walle.Add("sciany");
            Walle.Add("podpory");
            Walle.Add("wieza");

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
                 //   Debug.WriteLine(gameObj.tag);
                    if (gameObj.childs.Count > 0)
                    {
                        foreach (var gameObjChild in gameObj.childs)
                        {
                            gameObjChild.tag = tag;
                         //   Debug.WriteLine(gameObjChild.tag);
                        }
                    }
                }
            }
        }
    }
}
