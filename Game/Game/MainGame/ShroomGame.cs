using AnimationAux;
using Microsoft.Xna.Framework;
﻿using System.Collections.Generic;
using System.Diagnostics;
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

        Texture2D checkerboardTexture;
        SceneGraph.GameObject root;
        SceneGraph.GameObject heart;
        SceneGraph.GameObject heart2;
        SceneGraph.GameObject box;
        SceneGraph.GameObject sphere;
        SceneGraph.GameObject cone;
        SceneGraph.GameObject levelOne;
        List<SceneGraph.GameObject> walls;

        SceneGraph.GameObject player;
        SceneGraph.GameObject playerModel;
        private SceneGraph.GameObject playerDance;
        SceneGraph.Camera camera;

        public ShroomGame()
        {
            graphics = new GraphicsDeviceManager(this);
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

            root = new SceneGraph.GameObject();
            heart = new SceneGraph.GameObject();
            heart2 = new SceneGraph.GameObject();
            box = new SceneGraph.GameObject();
            sphere = new SceneGraph.GameObject();
            cone = new SceneGraph.GameObject();
            levelOne = new SceneGraph.GameObject();
            player = new SceneGraph.GameObject();
            camera = new SceneGraph.Camera();
            camera.SetCameraTarget(player);

            Model apteczka = Content.Load<Model>("apteczka");
            Model hierarchia = Content.Load<Model>("level_newXD");

            // Load anim model
            player.AddComponent(new SceneGraph.ModelAnimatedComponent("test/borowikSlashLewo", Content));
            player.AddComponent(new Player(player));
            List<GameObject> hiererchyList = SplitModelIntoSmallerPieces(hierarchia);
            CreateHierarchyOfLevel(hiererchyList, levelOne);

            heart.AddComponent(new SceneGraph.ModelComponent(apteczka));
            heart2.AddComponent(new SceneGraph.ModelComponent(apteczka));


            // TODO: ANIM LOAD SYSTEM / SELECTOR
            AnimationClip animationClip = player.GetComponent<ModelAnimatedComponent>().AnimationClips[0];
            AnimationPlayer animationPlayer = player.GetComponent<ModelAnimatedComponent>().PlayClip(animationClip);
            animationPlayer.Looping = true;

            // Add static models
            heart.AddComponent(new SceneGraph.ModelComponent(apteczka));
            heart2.AddComponent(new SceneGraph.ModelComponent(apteczka));


            root.AddChildNode(heart);
            root.AddChildNode(heart2);
            root.AddChildNode(player);
            root.AddChildNode(box);
            root.AddChildNode(levelOne);
            box.AddChildNode(sphere);
            sphere.AddChildNode(cone);
            player.AddChildNode(camera);
            heart.TransformationsOrder = SceneGraph.TransformationOrder.ScalePositionRotation;
            heart2.TransformationsOrder = SceneGraph.TransformationOrder.ScalePositionRotation;
            heart.Position = new Vector3(15.0f, 1.0f, -10.0f);
            heart2.Position = new Vector3(-15.0f, 1.0f, -10.0f);
            heart.Scale = new Vector3(0.2f);
            heart2.Scale = new Vector3(0.2f);
            player.Position = new Vector3(0f, 3f, 0f);
            player.Scale = new Vector3(0.1f);

            //CreateLevel();

            root.CreateColliders();

            // We aren't using the content pipeline, so we need to access the stream directly:
            using (var stream = TitleContainer.OpenStream("Content/checkerboard.png"))
            {
                checkerboardTexture = Texture2D.FromStream(this.GraphicsDevice, stream);
            }
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

            root.Draw(camera);

            base.Draw(gameTime);
        }

        private void CreateLevel()
        {
            Model sciana = Content.Load<Model>("wall");
            walls = new List<SceneGraph.GameObject>();
            for (int i = 0; i < 5; i++)
            {
                walls.Add(new SceneGraph.GameObject());
            }

            foreach (var wall in walls)
            {
                wall.AddComponent(new SceneGraph.ModelComponent(sciana));
                root.AddChildNode(wall);
            }

            walls[0].Position = new Vector3(0.0f, 4.0f, 0.0f);
            walls[1].Position = new Vector3(0.0f, -6.0f, -10.0f);
            walls[2].Position = new Vector3(0.0f, -6.0f, -20.0f);
            walls[3].Position = new Vector3(0.0f, -6.0f, -30.0f);
            walls[4].Position = new Vector3(0.0f, 4.0f, -40.0f);
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
                    ModelComponent newModel = new ModelComponent(new Model(GraphicsDevice, bones, meshes));
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
    }
}
