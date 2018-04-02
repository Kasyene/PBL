﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace PBLGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class ShroomGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        BasicEffect effect;
        Texture2D checkerboardTexture;
        SceneGraph.SceneNode root;
        SceneGraph.SceneNode node;
        SceneGraph.SceneNode node2;
        SceneGraph.Camera camera;

        public ShroomGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            root = new SceneGraph.SceneNode();
            node = new SceneGraph.SceneNode();
            node2 = new SceneGraph.SceneNode();
            camera = new SceneGraph.Camera();
            camera.SetCameraTarget(root);

            root.AddChildNode(node);
            root.AddChildNode(node2);
            root.AddChildNode(camera);
            Model model = Content.Load<Model>("apteczka");
            Model budda = Content.Load<Model>("Knuckles");
            node.AddEntity(new SceneGraph.ModelEntity(model));
            node2.AddEntity(new SceneGraph.ModelEntity(budda));

            // We aren't using the content pipeline, so we need
            // to access the stream directly:
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            root.Draw(camera);
            root.PositionX += 1.0f;
            root.RotationZ += 0.1f;
            Debug.WriteLine(root.PositionX);
            node.TransformationsOrder = SceneGraph.TransformationOrder.ScalePositionRotation;
            node.PositionY = 15.0f;
            node.RotationX = node.RotationX + 0.01f;
            node.Scale = new Vector3(0.2f);
            node2.RotationZ = node2.RotationZ + 0.01f; ;
            node2.RotationY = -2.0f;
            node2.Scale = new Vector3(1.4f);

            base.Draw(gameTime);
        }
    }
}
