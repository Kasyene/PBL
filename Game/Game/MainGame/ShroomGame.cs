using System;
using AnimationAux;
using Microsoft.Xna.Framework;
﻿using System.Collections.Generic;
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

namespace PBLGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class ShroomGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private readonly InputManager inputManager;
        public static Texture2D missingTexture;
        public static Texture2D missingNormalMap;
        public static Lights.DirectionalLight directionalLight;
        public static List<Lights.PointLight> pointLights;
        public static RenderTarget2D shadowRenderTarget;

        GameObject root;
        GameObject heart;
        GameObject heart2;
        GameObject levelOne;

        GameObject player;
        GameObject playerLeg;
        GameObject playerHat;
        GameObject playerLegWalk;
        GameObject playerHatWalk;
        Camera camera;

        Effect standardEffect;
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


            standardEffect = Content.Load<Effect>("Standard");
            root = new GameObject();
            heart = new GameObject();
            heart2 = new GameObject();
            levelOne = new GameObject();

            player = new GameObject();
            playerLeg = new GameObject();
            playerHat = new GameObject();
            playerLegWalk = new GameObject();
            playerHatWalk = new GameObject();

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

            // Load anim model
            player.AddChildNode(playerLeg);
            player.AddChildNode(playerHat);
            
            // models without anims have problems i guess ; /
            playerLeg.AddComponent(new ModelAnimatedComponent("models/player/borowikNozkaChod", Content));
            playerHat.AddComponent(new ModelAnimatedComponent("models/player/borowikKapeluszChod", Content));
            player.AddComponent(new Player(player));

            playerLegWalk.AddComponent(new ModelAnimatedComponent("models/player/borowikNozkaChod", Content));
            playerHatWalk.AddComponent(new ModelAnimatedComponent("models/player/borowikKapeluszChod", Content));

            List<GameObject> hiererchyList = SplitModelIntoSmallerPieces(hierarchia, hierarchiaTex, hierarchiaNormalTex);
            CreateHierarchyOfLevel(hiererchyList, levelOne);
            AssignTagsForMapElements(hiererchyList);

            // TODO: ANIM LOAD SYSTEM / SELECTOR
            AnimationClip animationClipLeg = playerLegWalk.GetComponent<ModelAnimatedComponent>().AnimationClips[0];
            AnimationClip animationClipHat = playerHatWalk.GetComponent<ModelAnimatedComponent>().AnimationClips[0];
            AnimationPlayer animationPlayerLeg = playerLeg.GetComponent<ModelAnimatedComponent>().PlayClip(animationClipLeg);
            AnimationPlayer animationPlayerHat = playerHat.GetComponent<ModelAnimatedComponent>().PlayClip(animationClipHat);
            animationPlayerLeg.Looping = true;
            animationPlayerHat.Looping = true;

            // Add static models
            heart.AddComponent(new ModelComponent(apteczka, standardEffect, apteczkaTexture));
            heart2.AddComponent(new ModelComponent(apteczka, standardEffect, apteczkaTexture));

            pointLights.Add(new Lights.PointLight(new Vector3(0.0f, 8.0f, 0.0f)));
            pointLights.Add(new Lights.PointLight(new Vector3(15.0f, 8.0f, 60.0f)));

            root.AddChildNode(heart);
            root.AddChildNode(heart2);
            root.AddChildNode(player);
            root.AddChildNode(levelOne);
            player.AddChildNode(camera);
            heart.TransformationsOrder = TransformationOrder.ScalePositionRotation;
            heart2.TransformationsOrder = TransformationOrder.ScalePositionRotation;
            heart.Position = new Vector3(15.0f, 4.0f, -10.0f);
            heart2.Position = new Vector3(-15.0f, 4.0f, -10.0f);
            heart.Scale = new Vector3(0.2f);
            heart2.Scale = new Vector3(0.2f);
            player.Position = new Vector3(0f, 3f, 0f);
            player.Scale = new Vector3(0.1f);

            root.CreateColliders();
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            // Our Timer Class
            Timer.Update(gameTime);
            inputManager.Update();
            camera.Update();
            // Player update
            player.Update();
            player.Update(gameTime);

            heart.SetAtTriggers();
            heart2.SetAtTriggers();

            if (inputManager.Keyboard[Keys.Escape])
            {
                Exit();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            CreateShadowMap();
            DrawWithShadow();
            //DrawShadowMapToScreen();

            base.Draw(gameTime);
        }

        void CreateShadowMap()
        {
            GraphicsDevice.SetRenderTarget(shadowRenderTarget);

            GraphicsDevice.Clear(Color.White);

            root.Draw(camera, true);

            GraphicsDevice.SetRenderTarget(null);
        }

        void DrawWithShadow()
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
            root.Draw(camera);
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
                    Debug.WriteLine("Position of new model " + position + " Rotation " + quat);
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
                    Debug.WriteLine(gameObj.tag);
                    if (gameObj.childs.Count > 0)
                    {
                        foreach (var gameObjChild in gameObj.childs)
                        {
                            gameObjChild.tag = tag;
                            Debug.WriteLine(gameObjChild.tag);
                        }
                    }
                }
            }
        }

        void DrawShadowMapToScreen()
        {
            spriteBatch.Begin(0, BlendState.Opaque, SamplerState.PointClamp, null, null);
            spriteBatch.Draw(shadowRenderTarget, new Rectangle(0, 0, 512, 512), Color.White);
            spriteBatch.End();

            GraphicsDevice.Textures[0] = null;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        }
    }
}
