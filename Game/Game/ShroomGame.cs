using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using PBLGame.Input;

namespace PBLGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class ShroomGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        InputManager inputManager;

        Texture2D checkerboardTexture;
        SceneGraph.GameObject root;
        SceneGraph.GameObject node;
        SceneGraph.GameObject player;
        SceneGraph.GameObject node2;
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
            node = new SceneGraph.GameObject();
            node2 = new SceneGraph.GameObject();
            player = new SceneGraph.GameObject();
            camera = new SceneGraph.Camera();
            camera.SetCameraTarget(root);

            root.AddChildNode(node);
            root.AddChildNode(player);
            player.AddChildNode(camera);
            player.AddChildNode(node2);
            Model model = Content.Load<Model>("apteczka");
            Model budda = Content.Load<Model>("Knuckles");
            node.AddEntity(new SceneGraph.ModelComponent(model));
            node2.AddEntity(new SceneGraph.ModelComponent(budda));

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
            // TODO: Add your update logic here
            base.Update(gameTime);
            inputManager.Update();
            camera.Update();

            if (inputManager.Keyboard[Keys.Escape])
            {
                Exit();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            root.Draw(camera);
            root.PositionX += 1.0f;
            //Debug.WriteLine(root.PositionX);
            node.TransformationsOrder = SceneGraph.TransformationOrder.ScalePositionRotation;
            node.PositionY = 15.0f;
            node.RotationX += 0.01f;
            node.Scale = new Vector3(0.2f);
            node2.RotationY = -MathHelper.PiOver2;
            node2.Scale = new Vector3(1.4f);

            base.Draw(gameTime);
        }
    }
}
