using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using PBLGame.Input;
using PBLGame.MainGame;
using SkinnedModel;

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
        Player player;
        SceneGraph.GameObject playerModel;
        SceneGraph.Camera camera;

        private AnimationPlayer animationPlayer;

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

            // Anim Load
            SkinningData skinningData = budda.Tag as SkinningData;

            if(skinningData == null)
                throw new InvalidOperationException("This model does not contain a SkinningData tag.");

            // Create animation player and decode an animation clip
            animationPlayer = new AnimationPlayer(skinningData);

            AnimationClip clip = skinningData.AnimationClips["Take 001"];

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
            heart.PositionX = 15.0f;
            heart2.PositionX = 30f;
            heart.Scale = new Vector3(0.2f);
            heart2.Scale = new Vector3(0.2f);
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
    }
}
