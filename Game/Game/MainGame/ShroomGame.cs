using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Game.Components.Audio;
using Game.Misc;
using Game.Misc.Time;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PBLGame.Input;
using PBLGame.MainGame;
using PBLGame.SceneGraph;
using Game.Components.Collisions;
using Game.MainGame;
using PBLGame.Misc;
using System;

namespace PBLGame
{
    enum GameState
    {
        MainMenu,
        Options,
        LevelTutorial,
        LevelOne
    }

    enum BState
    {
        HOVER,
        UP,
        JUST_RELEASED,
        DOWN
    }

    public class ShroomGame : Microsoft.Xna.Framework.Game
    {
        const int NUMBER_OF_BUTTONS = 4,
            START_BUTTON_INDEX = 0,
            OPTIONS_BUTTON_INDEX = 1,
            CREDITS_BUTTON_INDEX = 2,
            EXIT_BUTTON_INDEX = 3;
        Color background_color;
        Color[] button_color = new Color[NUMBER_OF_BUTTONS];
        Rectangle[] button_rectangle = new Rectangle[NUMBER_OF_BUTTONS];
        BState[] button_state = new BState[NUMBER_OF_BUTTONS];
        Texture2D[] button_texture = new Texture2D[NUMBER_OF_BUTTONS];
        Texture2D menuTexture;
        double[] button_timer = new double[NUMBER_OF_BUTTONS];
        //mouse pressed and mouse just pressed
        bool mpressed, prev_mpressed = false;
        //mouse location in window
        int mx, my;
        double frame_time;

        GraphicsDeviceManager graphics;
        Resolution resolution;
        SpriteBatch spriteBatch;
        Skybox skybox;

        private readonly InputManager inputManager;
        public static Texture2D missingTexture;
        public static Lights.DirectionalLight directionalLight;
        public static List<Lights.PointLight> pointLights;
        public static RenderTarget2D shadowRenderTarget;
        public static RenderTarget2D screenRenderTarget;
        public static RenderTargetCube refractionTarget;

        private float gammaValue = 1.0f;
        public static float fadeAmount = 0.0f;
        private bool areCollidersAndTriggersSet;

        private float textDisplayTime = 3f;
        private string actualDialogueText = "";
        private Texture2D barsFront;
        private Texture2D barsBack;
        private Texture2D hpTexture;
        private Texture2D timeTexture;
        private Texture2D icons;

        private Texture2D actualCutsceneTexture;
        private float cutsceneDisplayTime;

        public GameObject root;

        public GameObject refractiveObject;
        public List<Component> updateComponents;

        public GameObject player;
        public List<GameObject> enemyList;
        public Camera camera;
        public GameObject cameraCollision;
        public GameObject MusicGameObject;

        private MusicManager musicManager => MusicGameObject.GetComponent<MusicManager>();

        Effect outlineEffect;
        SpriteFont dialoguesFont;
        const int shadowMapWidthHeight = 2048;

        GameState actualGameState;

        public ShroomGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            inputManager = InputManager.Instance;
            Content.RootDirectory = "Content";
            resolution = new Resolution();
            graphics.PreferredBackBufferHeight = 768;
            graphics.PreferredBackBufferWidth = 1366;
            //actualGameState = GameState.LevelTutorial;
            //actualGameState = GameState.LevelOne;
            actualGameState = GameState.MainMenu;
            root = new GameObject();
        }

        protected override void Initialize()
        {
            CalcButtonSize();
            IsMouseVisible = true;
            background_color = Color.CornflowerBlue;
            base.Initialize();
            GameServices.AddService<GraphicsDevice>(GraphicsDevice);
            GameServices.AddService<GraphicsDeviceManager>(graphics);
            GameServices.AddService<Resolution>(resolution);
            GameServices.AddService<ContentLoader>(new ContentLoader(this));
            GameServices.AddService<ShroomGame>(this);
        }

        private void CalcButtonSize()
        {
            int BUTTON_HEIGHT = Window.ClientBounds.Height / 9;
            int BUTTON_WIDTH = Window.ClientBounds.Width / 4 ;
            int x = Window.ClientBounds.Width / 2 - BUTTON_WIDTH / 2;
            int y = (Window.ClientBounds.Height / 2 - NUMBER_OF_BUTTONS / 2 * BUTTON_HEIGHT - (NUMBER_OF_BUTTONS % 2) * BUTTON_HEIGHT / 2) + BUTTON_HEIGHT;
            for (int i = 0; i < NUMBER_OF_BUTTONS; i++)
            {
                button_state[i] = BState.UP;
                button_color[i] = Color.White;
                button_timer[i] = 0.0;
                button_rectangle[i] = new Rectangle(x, y, BUTTON_WIDTH, BUTTON_HEIGHT);
                y += BUTTON_HEIGHT + 20;
            }
        }

        protected override void LoadContent()
        {
            Resources.Init(Content);
            // MUSIC INIT
            MusicGameObject = new GameObject("MusicManagerContainer");
            MusicGameObject.AddComponent(new MusicManager());
            //
            spriteBatch = new SpriteBatch(GraphicsDevice);
            shadowRenderTarget = new RenderTarget2D(graphics.GraphicsDevice, shadowMapWidthHeight, shadowMapWidthHeight,
                                                    false, SurfaceFormat.Single, DepthFormat.Depth24);
            screenRenderTarget = new RenderTarget2D(graphics.GraphicsDevice, graphics.GraphicsDevice.Viewport.Width,
                                                    graphics.GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            refractionTarget = new RenderTargetCube(this.GraphicsDevice, shadowMapWidthHeight, true, SurfaceFormat.Color, DepthFormat.Depth24);

            outlineEffect = Content.Load<Effect>("Outline");
            outlineEffect.Parameters["ScreenSize"].SetValue(
               new Vector2(GraphicsDevice.Viewport.Bounds.Width, GraphicsDevice.Viewport.Bounds.Height));
            dialoguesFont = Content.Load<SpriteFont>("Dialogues");

            menuTexture = Content.Load<Texture2D>("Menus/menuTlo");
            button_texture[START_BUTTON_INDEX] = Content.Load<Texture2D>("Menus/menuStart");
            button_texture[OPTIONS_BUTTON_INDEX] = Content.Load<Texture2D>("Menus/menuOptions");
            button_texture[CREDITS_BUTTON_INDEX] = Content.Load<Texture2D>("Menus/menuCredits");
            button_texture[EXIT_BUTTON_INDEX] = Content.Load<Texture2D>("Menus/menuExit");

            skybox = new Skybox("skybox/SkyBox", Content);

            hpTexture = Content.Load<Texture2D>("hud/paskiZycie");
            timeTexture = Content.Load<Texture2D>("hud/paskiCzas");
            barsFront = Content.Load<Texture2D>("hud/paskiPrzod");
            barsBack = Content.Load<Texture2D>("hud/paskiTyl");
            icons = Content.Load<Texture2D>("hud/ikonki");
            refractiveObject = new GameObject();
            root.AddChildNode(refractiveObject);

            player = new GameObject("player");
            enemyList = new List<GameObject>();

            camera = new Camera();
            camera.SetCameraTarget(player);
            GameServices.AddService(camera);
            cameraCollision = new GameObject("cameraCollision");
            cameraCollision.AddComponent(new CameraCollisions(cameraCollision));
            
            pointLights = new List<Lights.PointLight>();
            directionalLight = new Lights.DirectionalLight();
            missingTexture = Content.Load<Texture2D>("Missing");
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private void OnLevelLoad(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.MainMenu:
                    break;
                case GameState.Options:
                    break;
                case GameState.LevelTutorial:
                    break;
                case GameState.LevelOne:
                    musicManager.PlaySong("5823");
                    musicManager.IsRepeating = true;
                    break;
            }
        }


        protected override void Update(GameTime gameTime)
        {
            //resolutionChange
            if (inputManager.Keyboard[Keys.P])
            {
                resolution.SetResolution(1920, 1080);
                screenRenderTarget = new RenderTarget2D(graphics.GraphicsDevice, graphics.GraphicsDevice.Viewport.Width,
                                                   graphics.GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);
                CalcButtonSize();
            }

            Resources.CameraVector3 = camera.Position;

            // Our Timer Class
            Timer.Update(gameTime);
            inputManager.Update();

            switch (actualGameState)
            {
                case GameState.MainMenu:
                    OnLevelLoad(actualGameState);
                    break;

                case GameState.LevelTutorial:
                    if (!areCollidersAndTriggersSet)
                    {
                        GameServices.GetService<ContentLoader>().LoadTutorial();
                        OnLevelLoad(actualGameState);
                        player.GetComponent<Pawn>().ObjectSide = Side.Player;
                        root.CreateColliders();
                        cameraCollision.SetAsTrigger();
                        GameServices.GetService<ContentLoader>().SetAsColliderAndTrigger();
                        foreach (GameObject obj in enemyList)
                        {
                            obj.GetComponent<Pawn>().ObjectSide = Side.Enemy;
                        }
                        areCollidersAndTriggersSet = true;
                    }
                    else
                    {
                        cameraCollision.Update();
                        camera.Update();
                        player.Update();
                        player.Update(gameTime);
                        outlineEffect.Parameters["TimeStop"].SetValue(player.GetComponent<Player>().timeStop);
                        foreach (GameObject obj in enemyList)
                        {
                            obj.Update(gameTime);
                        }
                        if (!GameServices.GetService<GameObject>().GetComponent<Player>().timeStop)
                        {
                            foreach (Component comp in updateComponents)
                            {
                                comp.Update(gameTime);
                            }
                        }
                        base.Update(gameTime);
                    }
                    break;

                case GameState.LevelOne:
                    if (!areCollidersAndTriggersSet)
                    {
                        GameServices.GetService<ContentLoader>().LoadLevel1();
                        OnLevelLoad(actualGameState);
                        player.GetComponent<Pawn>().ObjectSide = Side.Player;
                        root.CreateColliders();
                        cameraCollision.SetAsTrigger();
                        GameServices.GetService<ContentLoader>().SetAsColliderAndTrigger();
                        foreach (GameObject obj in enemyList)
                        {
                            obj.GetComponent<Pawn>().ObjectSide = Side.Enemy;
                        }
                        areCollidersAndTriggersSet = true;
                    }
                    else
                    {
                        cameraCollision.Update();
                        camera.Update();
                        player.Update();
                        player.Update(gameTime);
                        outlineEffect.Parameters["TimeStop"].SetValue(player.GetComponent<Player>().timeStop);
                        foreach (GameObject obj in enemyList)
                        {
                            obj.Update(gameTime);
                        }
                        if (!GameServices.GetService<GameObject>().GetComponent<Player>().timeStop)
                        {
                            foreach (Component comp in updateComponents)
                            {
                                comp.Update(gameTime);
                            }
                        }
                        base.Update(gameTime);
                    }
                    break;
            }

            if (inputManager.Keyboard[Keys.Escape])
            {
                Exit();
            }

        }

        protected override void Draw(GameTime gameTime)
        {
            switch(actualGameState)
            {
                case GameState.MainMenu:
                    DrawMenu(gameTime);
                    break;

                case GameState.LevelTutorial:
                case GameState.LevelOne:
                    CreateShadowMap();
                    GraphicsDevice.SetRenderTarget(screenRenderTarget);
                    DrawWithShadow(camera.CalcViewMatrix());
                    DrawScreen(gameTime);

                    DrawText(gameTime);
                    break;
            }
            base.Draw(gameTime);
        }

        private void DrawMenu(GameTime gameTime)
        {
            GraphicsDevice.Clear(background_color);

            spriteBatch.Begin();
            spriteBatch.Draw(menuTexture, new Rectangle(new Point(0, 0), new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight)), Color.White);
            for (int i = 0; i < NUMBER_OF_BUTTONS; i++)
            { 
                spriteBatch.Draw(button_texture[i], button_rectangle[i], button_color[i]);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        void CreateShadowMap()
        {
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            GraphicsDevice.SetRenderTarget(shadowRenderTarget);

            GraphicsDevice.Clear(Color.White);

            root.Draw(camera, true);
        }

        void DrawWithShadow(Matrix viewMatrix)
        {
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.Clear(Color.White);
            DrawSkybox();
            camera.ViewMatrix = viewMatrix;
            root.Draw(camera);
            GraphicsDevice.SetRenderTarget(null);
        }

        void DrawOutline()
        {
            outlineEffect.Parameters["GammaValue"].SetValue(gammaValue);
            outlineEffect.Parameters["FadeAmount"].SetValue(fadeAmount);
            outlineEffect.Parameters["ScreenSize"].SetValue(new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight));
            spriteBatch.Begin(0, BlendState.AlphaBlend, null, null, null, outlineEffect);
            spriteBatch.Draw(screenRenderTarget, Vector2.Zero, Color.White);
            spriteBatch.End();
        }

        void DrawScreen(GameTime gameTime)
        {
            cutsceneDisplayTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (cutsceneDisplayTime < 0f || actualCutsceneTexture == null)
            {
                Cutscene cutscene = Cutscene.GetActualCutscene();
                if(cutscene != null)
                {
                    actualCutsceneTexture = cutscene.texture;
                    cutsceneDisplayTime = cutscene.time;
                }
                else
                {
                    actualCutsceneTexture = null;
                }
            }

            if (actualCutsceneTexture == null)
            {
                DrawOutline();
                DrawHUDBars();
            }
            else
            {
                spriteBatch.Begin(0, BlendState.Opaque, null, null, null);
                spriteBatch.Draw(actualCutsceneTexture, new Rectangle(new Point(0, 0), new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight)), Color.White);
                spriteBatch.End();
            }
        }

        public void DrawRefraction()
        {
            for (int i = 0; i < 6; i++)
            {
                // render the scene to all cubemap faces
                CubeMapFace cubeMapFace = (CubeMapFace)i;

                switch (cubeMapFace)
                {
                    case CubeMapFace.NegativeX:
                        {
                            camera.ViewMatrix = Matrix.CreateLookAt(refractiveObject.Position, refractiveObject.Position + Vector3.Left, Vector3.Up);
                            break;
                        }
                    case CubeMapFace.NegativeY:
                        {
                            camera.ViewMatrix = Matrix.CreateLookAt(refractiveObject.Position, refractiveObject.Position + Vector3.Down, Vector3.Forward);
                            break;
                        }
                    case CubeMapFace.NegativeZ:
                        {
                            camera.ViewMatrix = Matrix.CreateLookAt(refractiveObject.Position, refractiveObject.Position + Vector3.Backward, Vector3.Up);
                            break;
                        }
                    case CubeMapFace.PositiveX:
                        {
                            camera.ViewMatrix = Matrix.CreateLookAt(refractiveObject.Position, refractiveObject.Position + Vector3.Right, Vector3.Up);
                            break;
                        }
                    case CubeMapFace.PositiveY:
                        {
                            camera.ViewMatrix = Matrix.CreateLookAt(refractiveObject.Position, refractiveObject.Position + Vector3.Up, Vector3.Backward);
                            break;
                        }
                    case CubeMapFace.PositiveZ:
                        {
                            camera.ViewMatrix = Matrix.CreateLookAt(refractiveObject.Position, refractiveObject.Position + Vector3.Forward, Vector3.Up);
                            break;
                        }
                }
                camera.fieldOfView = MathHelper.PiOver2;
                GraphicsDevice.SetRenderTarget(refractionTarget, cubeMapFace);
                GraphicsDevice.Clear(Color.White);
                DrawWithShadow(camera.ViewMatrix);
            }
            camera.fieldOfView = MathHelper.PiOver4;
        }

        void DrawSkybox()
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            RasterizerState originalRasterizerState = graphics.GraphicsDevice.RasterizerState;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            graphics.GraphicsDevice.RasterizerState = rasterizerState;
            Vector3 cameraPosition = camera.GetWorldPosition();
            skybox.Draw(Matrix.CreateLookAt(cameraPosition, player.Position, Vector3.UnitY),
                Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 1f, 0.1f, 4000f), cameraPosition);
            graphics.GraphicsDevice.RasterizerState = originalRasterizerState;
        }

        void DrawText(GameTime gameTime)
        {
            textDisplayTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (textDisplayTime < 0f || actualDialogueText == "")
            {
                actualDialogueText = DialogueString.GetActualDialogueString();
                textDisplayTime = 3f;
            }
            spriteBatch.Begin();
            spriteBatch.DrawString(dialoguesFont, actualDialogueText,
                new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height - 50f),
                Color.Snow, 0.0f, dialoguesFont.MeasureString(actualDialogueText) / 2, 1.0f, SpriteEffects.None, 0.5f);
            spriteBatch.End();
        }

        void DrawHUDBars()
        {
            spriteBatch.Begin();
            spriteBatch.Draw(barsBack, new Rectangle(20, graphics.GraphicsDevice.Viewport.Height - 100, player.GetComponent<Player>().MaxHp * 15, 80), Color.White);
            spriteBatch.Draw(hpTexture, new Rectangle(20 + (player.GetComponent<Player>().MaxHp -  player.GetComponent<Player>().Hp), graphics.GraphicsDevice.Viewport.Height - 100, player.GetComponent<Player>().Hp * 15, 80), Color.White);
            spriteBatch.Draw(timeTexture, new Rectangle(20 + (10 - player.GetComponent<Player>().GetTimeEnergy()), graphics.GraphicsDevice.Viewport.Height - 100, player.GetComponent<Player>().GetTimeEnergy() * 30, 80), Color.White);
            spriteBatch.Draw(barsFront, new Rectangle(20, graphics.GraphicsDevice.Viewport.Height - 100, player.GetComponent<Player>().MaxHp * 15, 80), Color.White);
            spriteBatch.Draw(icons, new Rectangle(graphics.GraphicsDevice.Viewport.Width - 380, graphics.GraphicsDevice.Viewport.Height - 120, 400, 120), Color.White);
            spriteBatch.End();
        }
    }
}
