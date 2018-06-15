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
    public enum GameState
    {
        MainMenu,
        Options,
        Credits,
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
        #region Menu
        const int NumberOfMenuButtons = 4,
            StartButtonIndex = 0,
            OptionsButtonIndex = 1,
            CreditsButtonIndex = 2,
            ExitButtonIndex = 3;

        const int NumberOfOptionButtons = 3,
          FullscreenButtonIndex = 0,
          BackButtonIndex = 1,
          ExitOptionButtonIndex = 2;

        Color[] menuButtonColor = new Color[NumberOfMenuButtons];
        Rectangle[] menuButtonRectangle = new Rectangle[NumberOfMenuButtons];
        BState[] menuButtonState = new BState[NumberOfMenuButtons];
        Texture2D[] menuButtonTexture = new Texture2D[NumberOfMenuButtons];
        Texture2D menuTexture;
        Texture2D optionsTexture;
        Texture2D creditsTexture;
        Color[] menuOptionButtonColor = new Color[NumberOfOptionButtons];
        Rectangle[] menuOptionButtonRectangle = new Rectangle[NumberOfOptionButtons];
        BState[] menuOptionButtonState = new BState[NumberOfOptionButtons];
        Texture2D[] menuOptionButtonTexture = new Texture2D[NumberOfOptionButtons];
        Color backButtonColor = new Color();
        Rectangle backButtonRectangle = new Rectangle();
        BState backButtonState = new BState();
        Texture2D backButtonTexture;
        bool mousePressed, prevMousePressed = false;
        bool fullscreen = false;
        int mouseX, mouseY;
        #endregion

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

        public static GameState actualGameState;
        public static GameState lastGameState;

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
            actualGameState = GameState.LevelOne;
            //actualGameState = GameState.MainMenu;
            lastGameState = GameState.MainMenu;
            root = new GameObject();
        }

        protected override void Initialize()
        {
            CalcButtonSize();
            base.Initialize();
            GameServices.AddService<GraphicsDevice>(GraphicsDevice);
            GameServices.AddService<GraphicsDeviceManager>(graphics);
            GameServices.AddService<Resolution>(resolution);
            GameServices.AddService<ContentLoader>(new ContentLoader(this));
            GameServices.AddService<ShroomGame>(this);
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
            optionsTexture = Content.Load<Texture2D>("Menus/inneTlo");
            creditsTexture = Content.Load<Texture2D>("Menus/inneTlo");

            menuButtonTexture[StartButtonIndex] = Content.Load<Texture2D>("Menus/menuStart");
            menuButtonTexture[OptionsButtonIndex] = Content.Load<Texture2D>("Menus/menuOptions");
            menuButtonTexture[CreditsButtonIndex] = Content.Load<Texture2D>("Menus/menuCredits");
            menuButtonTexture[ExitButtonIndex] = Content.Load<Texture2D>("Menus/menuExit");

            menuOptionButtonTexture[FullscreenButtonIndex] = Content.Load<Texture2D>("Menus/menuStart");
            menuOptionButtonTexture[BackButtonIndex] = Content.Load<Texture2D>("Menus/inneBack");
            menuOptionButtonTexture[ExitOptionButtonIndex] = Content.Load<Texture2D>("Menus/menuExit");

            backButtonTexture = Content.Load<Texture2D>("Menus/inneBack");

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
                    lastGameState = actualGameState;
                    OnLevelLoad(actualGameState);
                    MouseState mouse_state = Mouse.GetState();
                    mouseX = mouse_state.X;
                    mouseY = mouse_state.Y;
                    prevMousePressed = mousePressed;
                    mousePressed = mouse_state.LeftButton == ButtonState.Pressed;
                    UpdateMainMenuButtons();
                    break;

                case GameState.Options:
                    OnLevelLoad(actualGameState);
                    mouse_state = Mouse.GetState();
                    mouseX = mouse_state.X;
                    mouseY = mouse_state.Y;
                    prevMousePressed = mousePressed;
                    mousePressed = mouse_state.LeftButton == ButtonState.Pressed;
                    UpdateOptionsButtons();
                    break;

                case GameState.Credits:
                    OnLevelLoad(actualGameState);
                    mouse_state = Mouse.GetState();
                    mouseX = mouse_state.X;
                    mouseY = mouse_state.Y;
                    prevMousePressed = mousePressed;
                    mousePressed = mouse_state.LeftButton == ButtonState.Pressed;
                    UpdateCreditsButton();
           
                    break;

                case GameState.LevelTutorial:
                    lastGameState = actualGameState;
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
                    lastGameState = actualGameState;
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
                actualGameState = GameState.Options;
            }

        }

        protected override void Draw(GameTime gameTime)
        {
            switch(actualGameState)
            {
                case GameState.Options:
                    DrawOptions();
                    IsMouseVisible = true;
                    break;
                case GameState.Credits:
                    DrawCredits();
                    IsMouseVisible = true;
                    break;
                case GameState.MainMenu:
                    DrawMenu();
                    IsMouseVisible = true;
                    break;

                case GameState.LevelTutorial:
                case GameState.LevelOne:
                    IsMouseVisible = false;
                    CreateShadowMap();
                    GraphicsDevice.SetRenderTarget(screenRenderTarget);
                    DrawWithShadow(camera.CalcViewMatrix());
                    DrawScreen(gameTime);

                    DrawText(gameTime);
                    break;
            }
            base.Draw(gameTime);
        }

        private void DrawMenu()
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            spriteBatch.Draw(menuTexture, new Rectangle(new Point(0, 0), new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight)), Color.White);
            for (int i = 0; i < NumberOfMenuButtons; i++)
            { 
                spriteBatch.Draw(menuButtonTexture[i], menuButtonRectangle[i], menuButtonColor[i]);
            }
            spriteBatch.End();
        }

        private void DrawOptions()
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            spriteBatch.Draw(optionsTexture, new Rectangle(new Point(0, 0), new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight)), Color.White);
            for (int i = 0; i < NumberOfOptionButtons; i++)
            {
                spriteBatch.Draw(menuOptionButtonTexture[i], menuOptionButtonRectangle[i], menuOptionButtonColor[i]);
            }
            spriteBatch.End();
        }

        private void DrawCredits()
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            spriteBatch.Draw(creditsTexture, new Rectangle(new Point(0, 0), new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight)), Color.White);
            spriteBatch.Draw(backButtonTexture, backButtonRectangle, backButtonColor);
            spriteBatch.End();
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
            if (areCollidersAndTriggersSet)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(barsBack, new Rectangle(20, graphics.GraphicsDevice.Viewport.Height - 100, player.GetComponent<Player>().MaxHp * 15, 80), Color.White);
                spriteBatch.Draw(hpTexture, new Rectangle(20 + (player.GetComponent<Player>().MaxHp - player.GetComponent<Player>().Hp), graphics.GraphicsDevice.Viewport.Height - 100, player.GetComponent<Player>().Hp * 15, 80), Color.White);
                spriteBatch.Draw(timeTexture, new Rectangle(20 + (10 - player.GetComponent<Player>().GetTimeEnergy()), graphics.GraphicsDevice.Viewport.Height - 100, player.GetComponent<Player>().GetTimeEnergy() * 30, 80), Color.White);
                spriteBatch.Draw(barsFront, new Rectangle(20, graphics.GraphicsDevice.Viewport.Height - 100, player.GetComponent<Player>().MaxHp * 15, 80), Color.White);
                spriteBatch.Draw(icons, new Rectangle(graphics.GraphicsDevice.Viewport.Width - 380, graphics.GraphicsDevice.Viewport.Height - 120, 400, 120), Color.White);
                spriteBatch.End();
            }
        }

        private void CalcButtonSize()
        {
            int BUTTON_HEIGHT = Window.ClientBounds.Height / 9;
            int BUTTON_WIDTH = Window.ClientBounds.Width / 4;
            int x = Window.ClientBounds.Width / 2 - BUTTON_WIDTH / 2;
            int y = (Window.ClientBounds.Height / 2 - NumberOfMenuButtons / 2 * BUTTON_HEIGHT - (NumberOfMenuButtons % 2) * BUTTON_HEIGHT / 2) + BUTTON_HEIGHT;
            for (int i = 0; i < NumberOfMenuButtons; i++)
            {
                menuButtonState[i] = BState.UP;
                menuButtonColor[i] = Color.White;
                menuButtonRectangle[i] = new Rectangle(x, y, BUTTON_WIDTH, BUTTON_HEIGHT);
                y += BUTTON_HEIGHT + 20;
            }
            y = (Window.ClientBounds.Height / 2 - NumberOfOptionButtons / 2 * BUTTON_HEIGHT - (NumberOfOptionButtons % 2) * BUTTON_HEIGHT / 2) + BUTTON_HEIGHT;
            for (int i = 0; i < NumberOfOptionButtons; i++)
            {
                menuOptionButtonState[i] = BState.UP;
                menuOptionButtonColor[i] = Color.White;
                menuOptionButtonRectangle[i] = new Rectangle(x, y, BUTTON_WIDTH, BUTTON_HEIGHT);
                y += BUTTON_HEIGHT + 20;
            }
            backButtonState = BState.UP;
            backButtonColor = Color.White;
            backButtonRectangle = menuButtonRectangle[NumberOfMenuButtons - 1];
        }

        Boolean HitImageAlpha(Rectangle rect, Texture2D tex, int x, int y)
        {
            return HitImageAlpha(0, 0, tex, tex.Width * (x - rect.X) /
                rect.Width, tex.Height * (y - rect.Y) / rect.Height);
        }

        Boolean HitImageAlpha(float tx, float ty, Texture2D tex, int x, int y)
        {
            if (HitImage(tx, ty, tex, x, y))
            {
                uint[] data = new uint[tex.Width * tex.Height];
                tex.GetData<uint>(data);
                if ((x - (int)tx) + (y - (int)ty) *
                    tex.Width < tex.Width * tex.Height)
                {
                    return ((data[
                        (x - (int)tx) + (y - (int)ty) * tex.Width
                        ] &
                                0xFF000000) >> 24) > 20;
                }
            }
            return false;
        }

        Boolean HitImage(float tx, float ty, Texture2D tex, int x, int y)
        {
            return (x >= tx &&
                x <= tx + tex.Width &&
                y >= ty &&
                y <= ty + tex.Height);
        }

        void UpdateMainMenuButtons()
        {
            for (int i = 0; i < NumberOfMenuButtons; i++)
            {
                if (HitImageAlpha(menuButtonRectangle[i], menuButtonTexture[i], mouseX, mouseY))
                {
                    if (mousePressed)
                    {
                        menuButtonState[i] = BState.DOWN;
                        menuButtonColor[i] = Color.DarkKhaki;
                    }
                    else if (!mousePressed && prevMousePressed)
                    {
                        if (menuButtonState[i] == BState.DOWN)
                        {
                            menuButtonState[i] = BState.JUST_RELEASED;
                        }
                    }
                    else
                    {
                        menuButtonState[i] = BState.HOVER;
                        menuButtonColor[i] = Color.LightYellow;
                    }
                }
                else
                {
                    menuButtonState[i] = BState.UP;
                    menuButtonColor[i] = Color.White;
                }

                if (menuButtonState[i] == BState.JUST_RELEASED)
                {
                    MainMenuButtonActions(i);
                }
            }
        }

        void UpdateOptionsButtons()
        {
            for (int i = 0; i < NumberOfOptionButtons; i++)
            {
                if (HitImageAlpha(menuOptionButtonRectangle[i], menuOptionButtonTexture[i], mouseX, mouseY))
                {
                    if (mousePressed)
                    {
                        menuOptionButtonState[i] = BState.DOWN;
                        menuOptionButtonColor[i] = Color.DarkKhaki;
                    }
                    else if (!mousePressed && prevMousePressed)
                    {
                        if (menuOptionButtonState[i] == BState.DOWN)
                        {
                            menuOptionButtonState[i] = BState.JUST_RELEASED;
                        }
                    }
                    else
                    {
                        menuOptionButtonState[i] = BState.HOVER;
                        menuOptionButtonColor[i] = Color.LightYellow;
                    }
                }
                else
                {
                    menuOptionButtonState[i] = BState.UP;
                    menuOptionButtonColor[i] = Color.White;
                }

                if (menuOptionButtonState[i] == BState.JUST_RELEASED)
                {
                    OptionsButtonActions(i);
                }
            }
        }

        void UpdateCreditsButton()
        {
            if (HitImageAlpha(backButtonRectangle, backButtonTexture, mouseX, mouseY))
            {
                if (mousePressed)
                {
                    backButtonState = BState.DOWN;
                    backButtonColor = Color.DarkKhaki;
                }
                else if (!mousePressed && prevMousePressed)
                {
                    if (backButtonState == BState.DOWN)
                    {
                        backButtonState = BState.JUST_RELEASED;
                    }
                }
                else
                {
                    backButtonState = BState.HOVER;
                    backButtonColor = Color.LightYellow;
                }
            }
            else
            {
                backButtonState = BState.UP;
                backButtonColor = Color.White;
            }

            if (backButtonState == BState.JUST_RELEASED)
            {
                BackButtonAction();
            }
        }

        void BackButtonAction()
        {
            actualGameState = lastGameState;
        }

        void MainMenuButtonActions(int i)
        {
            switch (i)
            {
                case StartButtonIndex:
                    new Cutscene(Content.Load<Texture2D>("Cutscene/1.1"), 3f);
                    new Cutscene(Content.Load<Texture2D>("Cutscene/1.3"), 2f);
                    new Cutscene(Content.Load<Texture2D>("Cutscene/1.2"), 2f);
                    actualGameState = GameState.LevelTutorial;
                    System.Diagnostics.Debug.WriteLine("START");
                    break;
                case OptionsButtonIndex:
                    System.Diagnostics.Debug.WriteLine("OPTIONS");
                    actualGameState = GameState.Options;
                    break;
                case CreditsButtonIndex:
                    System.Diagnostics.Debug.WriteLine("CREDITS");
                    actualGameState = GameState.Credits;
                    break;
                case ExitButtonIndex:
                    Exit();
                    break;
                default:
                    break;
            }
        }

        void OptionsButtonActions(int i)
        {
            switch (i)
            {
                case FullscreenButtonIndex:
                    fullscreen = !fullscreen;
                    resolution.SetFullscreen(fullscreen);
                    break;
                case BackButtonIndex:
                    actualGameState = lastGameState;
                    break;
                case ExitOptionButtonIndex:
                    Exit();
                    break;
                default:
                    break;
            }
        }
    }
}
