using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using PBLGame.Input;
using PBLGame.MainGame;

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
            playerModel = new SceneGraph.GameObject();
            player = new Player();
            camera = new SceneGraph.Camera();
            camera.SetCameraTarget(root);

            Model apteczka = Content.Load<Model>("apteczka");
            Model budda = Content.Load<Model>("Knuckles");

            heart.AddEntity(new SceneGraph.ModelComponent(apteczka));
            playerModel.AddEntity(new SceneGraph.ModelComponent(budda));

            root.AddChildNode(heart);
            root.AddChildNode(player);
            player.AddChildNode(playerModel);
            player.AddChildNode(camera);



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
            heart.TransformationsOrder = SceneGraph.TransformationOrder.ScalePositionRotation;
            heart.PositionX = 15.0f;
            heart.Scale = new Vector3(0.2f);
            playerModel.RotationY = -MathHelper.PiOver2;
            playerModel.Scale = new Vector3(1.4f);

            base.Draw(gameTime);
        }
    }
}
