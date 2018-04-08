﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using PBLGame.Input;
using PBLGame.MainGame;
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
        List<SceneGraph.GameObject> walls;

        Player player;
        SceneGraph.GameObject playerModel;
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
            spriteBatch = new SpriteBatch(GraphicsDevice);

            root = new SceneGraph.GameObject();
            heart = new SceneGraph.GameObject();
            heart2 = new SceneGraph.GameObject();
            playerModel = new SceneGraph.GameObject();
            player = new Player();
            camera = new SceneGraph.Camera();
            camera.SetCameraTarget(player);

            Model apteczka = Content.Load<Model>("apteczka");
            Model budda = Content.Load<Model>("Knuckles");
            Model wall = Content.Load<Model>("wall");

            heart.AddEntity(new SceneGraph.ModelComponent(apteczka));
            heart2.AddEntity(new SceneGraph.ModelComponent(apteczka));
            playerModel.AddEntity(new SceneGraph.ModelComponent(budda));

            root.AddChildNode(heart);
            root.AddChildNode(heart2);       
            root.AddChildNode(player);
            player.AddChildNode(playerModel);
            player.AddChildNode(camera);
            heart.TransformationsOrder = SceneGraph.TransformationOrder.ScalePositionRotation;
            heart2.TransformationsOrder = SceneGraph.TransformationOrder.ScalePositionRotation;
            heart.Position = new Vector3(15.0f, 1.0f, -10.0f);
            heart2.Position = new Vector3(-15.0f, 1.0f, -10.0f);
            heart.Scale = new Vector3(0.2f);
            heart2.Scale = new Vector3(0.2f);

            createLevel();

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
            base.Update(gameTime);
            inputManager.Update();
            camera.Update();
            player.Update();

            if (inputManager.Keyboard[Keys.Escape])
            {
                Exit();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            root.Draw(camera);
            playerModel.RotationY = -MathHelper.PiOver2;
            playerModel.Scale = new Vector3(1.4f);

            base.Draw(gameTime);
        }

        private void createLevel()
        {
            Model sciana = Content.Load<Model>("wall");
            walls = new List<SceneGraph.GameObject>();
            for (int i = 0; i < 5; i++)
            {
                walls.Add(new SceneGraph.GameObject());
            }

            foreach (var wall in walls)
            {
                wall.AddEntity(new SceneGraph.ModelComponent(sciana));
                root.AddChildNode(wall);
            }
            walls[0].Position = new Vector3(0.0f, 4.0f, 0.0f);
            walls[1].Position = new Vector3(0.0f, -6.0f, -10.0f);
            walls[2].Position = new Vector3(0.0f, -6.0f, -20.0f);
            walls[3].Position = new Vector3(0.0f, -6.0f, -30.0f);
            walls[4].Position = new Vector3(0.0f, 4.0f, -40.0f);
        }
    }
}
