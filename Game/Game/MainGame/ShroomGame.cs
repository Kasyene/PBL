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
        public static PBLGame.Lights.DirectionalLight directionalLight;

        SceneGraph.GameObject root;
        SceneGraph.GameObject heart;
        SceneGraph.GameObject heart2;
        SceneGraph.GameObject levelOne;

        SceneGraph.GameObject player;
        SceneGraph.GameObject playerDance;
        SceneGraph.Camera camera;

        Effect standardEffect;
        const int shadowMapWidthHeight = 2048;
        RenderTarget2D shadowRenderTarget;

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
        }

        protected override void LoadContent()
        {
            // TODO: WRITE CONTENT MANAGER EXTENSION
            Resources.Init(Content);

            spriteBatch = new SpriteBatch(GraphicsDevice);

            shadowRenderTarget = new RenderTarget2D(graphics.GraphicsDevice, shadowMapWidthHeight, shadowMapWidthHeight,
                                                    false, SurfaceFormat.Single, DepthFormat.Depth24);

            standardEffect = Content.Load<Effect>("Standard");
            root = new SceneGraph.GameObject();
            heart = new SceneGraph.GameObject();
            heart2 = new SceneGraph.GameObject();
            levelOne = new SceneGraph.GameObject();
            player = new SceneGraph.GameObject();
            playerDance = new SceneGraph.GameObject();
            camera = new SceneGraph.Camera();
            camera.SetCameraTarget(player);

            directionalLight = new PBLGame.Lights.DirectionalLight();
            missingTexture = Content.Load<Texture2D>("Missing");
            Model apteczka = Content.Load<Model>("apteczka");
            Texture2D apteczkaTexture = Content.Load<Texture2D>("apteczkaTex");
            Model hierarchia = Content.Load<Model>("level_newXD");

            // Load anim model
            player.AddComponent(new SceneGraph.ModelAnimatedComponent("test/borowik", Content));
            player.AddComponent(new Player(player));
            playerDance.AddComponent(new SceneGraph.ModelAnimatedComponent("test/borowikChod", Content));
            List<GameObject> hiererchyList = SplitModelIntoSmallerPieces(hierarchia);
            CreateHierarchyOfLevel(hiererchyList, levelOne);
            AssignTagsForGroundElements(hiererchyList);

            // TODO: ANIM LOAD SYSTEM / SELECTOR
            AnimationClip animationClip = playerDance.GetComponent<ModelAnimatedComponent>().AnimationClips[0];
            AnimationPlayer animationPlayer = player.GetComponent<ModelAnimatedComponent>().PlayClip(animationClip);
            animationPlayer.Looping = true;

            // Add static models
            heart.AddComponent(new SceneGraph.ModelComponent(apteczka, standardEffect, apteczkaTexture));
            heart2.AddComponent(new SceneGraph.ModelComponent(apteczka, standardEffect, apteczkaTexture));

            root.AddChildNode(heart);
            root.AddChildNode(heart2);
            root.AddChildNode(player);
            root.AddChildNode(levelOne);
            player.AddChildNode(camera);
            heart.TransformationsOrder = SceneGraph.TransformationOrder.ScalePositionRotation;
            heart2.TransformationsOrder = SceneGraph.TransformationOrder.ScalePositionRotation;
            heart.Position = new Vector3(15.0f, 2.0f, -10.0f);
            heart2.Position = new Vector3(-15.0f, 2.0f, -10.0f);
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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            CreateShadowMap();
            root.Draw(camera);

            base.Draw(gameTime);
        }

        void CreateShadowMap()
        {
            GraphicsDevice.SetRenderTarget(shadowRenderTarget);

            GraphicsDevice.Clear(Color.White);

            root.Draw(camera, true);

            GraphicsDevice.SetRenderTarget(null);
        }

        private List<GameObject> SplitModelIntoSmallerPieces(Model bigModel)
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
                    ModelComponent newModel = new ModelComponent(new Model(GraphicsDevice, bones, meshes), standardEffect);
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
                throw new System.Exception("There is no mesh in this model !!");
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

        private void AssignTagsForGroundElements(List<GameObject> mapa)
        {
            List<String> otagowaneNazwy = new List<string>();
            otagowaneNazwy.Add("podloga");
            otagowaneNazwy.Add("schodek");
            foreach (var gameObj in mapa)
            {
                if (otagowaneNazwy.Any(s => gameObj.name.Contains(s)))
                {
                    gameObj.tag = "Ground";
                }
            }
        }
    }
}
