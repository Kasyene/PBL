using System;
using AnimationAux;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PBLGame.Input;
using PBLGame.MainGame;
using PBLGame.Misc;

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
            spriteBatch = new SpriteBatch(GraphicsDevice);

            root = new SceneGraph.GameObject();
            heart = new SceneGraph.GameObject();
            heart2 = new SceneGraph.GameObject();
            playerModel = new SceneGraph.GameObject();
            player = new Player();
            camera = new SceneGraph.Camera();
            camera.SetCameraTarget(player);

            Model apteczka = Content.Load<Model>("apteczka");
            //Model budda2 = Content.Load<Model>("dude/dude");
            //Model budda = Content.Load<Model>("Knuckles");

            // Load anim model
            playerModel.AddComponent(new SceneGraph.ModelAnimatedComponent("Knuckles", Content));

            // TODO: ANIM LOAD SYSTEM / SELECTOR
            AnimationClip animationClip = playerModel.GetModelAnimatedComponent().AnimationClips[0];
            AnimationPlayer animationPlayer = playerModel.GetModelAnimatedComponent().PlayClip(animationClip);
            animationPlayer.Looping = true;

            // Add static models
            heart.AddComponent(new SceneGraph.ModelComponent(apteczka));
            heart2.AddComponent(new SceneGraph.ModelComponent(apteczka));
            

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
            inputManager.Update();
            camera.Update();
            // Player update
            player.Update();
            // Anim update
            playerModel.Update(gameTime);
            // TODO: AUTOMATIC COMPONENT UPDATE REWORK!!!!!
            playerModel.GetModelAnimatedComponent().Update(gameTime);
            //
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

            playerModel.RotationY = MathHelper.PiOver2;
            playerModel.Scale = new Vector3(0.03f);

            base.Draw(gameTime);
        }
    }
}
