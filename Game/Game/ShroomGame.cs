using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
            Model budda = Content.Load<Model>("Knuckles");

            // Anim Load for player
            SkinningData skinningData = budda.Tag as SkinningData;

            if(skinningData == null)
                throw new InvalidOperationException("This model does not contain a SkinningData tag.");
           

            // Create animation player and decode an animation clip 
            playerModel.AddComponent(new SceneGraph.ModelAnimComponent(budda, new AnimationPlayer(skinningData), skinningData.AnimationClips["Armature|ArmatureAction"]));


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

            playerModel.RotationY = -MathHelper.PiOver2;
            playerModel.Scale = new Vector3(1.4f);

            base.Draw(gameTime);
        }
    }
}
