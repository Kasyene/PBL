﻿using Microsoft.Xna.Framework;
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
using PBLGame.Input.Devices;
using Game.Components.MapElements;
using Game.Components.Pawns.Enemies;

namespace PBLGame
{
    public enum GameState
    {
        MainMenu,
        Options,
        Credits,
        Dead,
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

        const int NumberOfOptionButtons = 9,
          BiggestResolutionButtonIndex = 0,
          BigResolutionButtonIndex = 1,
          MediumResolutionButton = 2,
          SmallResolutionButton = 3,
          FullscreenButtonIndex = 4,
          InvertAxisXButtonIndex = 5,
          InvertAxisYButtonIndex = 6,
          BackButtonIndex = 7,
          ExitOptionButtonIndex = 8;

        const int NumberOfDeadButtons = 2,
          RestartButtonIndex = 0,
          ExitDeadButtonindex = 1;

        Color[] deadButtonColor = new Color[NumberOfDeadButtons];
        Rectangle[] deadButtonRectangle = new Rectangle[NumberOfDeadButtons];
        BState[] deadButtonState = new BState[NumberOfDeadButtons];
        Texture2D[] deadButtonTexture = new Texture2D[NumberOfDeadButtons];
        Color[] menuButtonColor = new Color[NumberOfMenuButtons];
        Rectangle[] menuButtonRectangle = new Rectangle[NumberOfMenuButtons];
        BState[] menuButtonState = new BState[NumberOfMenuButtons];
        Texture2D[] menuButtonTexture = new Texture2D[NumberOfMenuButtons];
        Texture2D menuTexture;
        Texture2D optionsTexture;
        Texture2D creditsTexture;
        Texture2D deadTexture;
        Color[] menuOptionButtonColor = new Color[NumberOfOptionButtons];
        Rectangle[] menuOptionButtonRectangle = new Rectangle[NumberOfOptionButtons];
        BState[] menuOptionButtonState = new BState[NumberOfOptionButtons];
        Texture2D[] menuOptionButtonTexture = new Texture2D[NumberOfOptionButtons];
        Color backButtonColor = new Color();
        Rectangle backButtonRectangle = new Rectangle();
        BState backButtonState = new BState();
        Texture2D backButtonTexture;
        static public int mouseXAxis = 1, mouseYAxis = 1;
        bool mousePressed, prevMousePressed = false;
        bool fullscreen = false;
        int mouseX, mouseY;
        #endregion

        #region GameProgress
        public bool levelOneCompleted = false;
        public bool tutorialCompleted = false;
        public bool usedQ = false;
        public bool usedE = false;
        public bool usedR = false;
        public bool cutsceneLoaded = false;
        public bool roomEntranceCutscene = false;
        public bool playerShouldNotMove = false;
        public bool bossFight = false;
        public bool gameComplete = false;
        public float endTimer = 0.0f;

        public DoorComponent door1;
        public DoorComponent door2;
        public GameObject boss;
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
        public bool areCollidersAndTriggersSet;

        #region HUD
        private float textDisplayTime = 3f;
        private string[] actualDialogueText;
        private Texture2D barsFront;
        private Texture2D barsBack;
        private Texture2D hpTexture;
        private Texture2D timeTexture;
        private Texture2D[] icons;

        private Texture2D bossBarFront;
        private Texture2D bossBarBack;
        private Texture2D bossHpTexture;
        #endregion

        Cutscene cutscene;
        private Texture2D actualCutsceneTexture;
        public float cutsceneDisplayTime;

        public GameObject root;

        public GameObject refractiveObject;
        public List<Component> updateComponents;

        public GameObject player;
        public List<GameObject> enemyList;
        public Camera camera;
        public GameObject cameraCollision;
        public GameObject MusicGameObject;

        public MusicManager musicManager => MusicGameObject.GetComponent<MusicManager>();

        Effect outlineEffect;
        SpriteFont dialoguesFont;
        const int shadowMapWidthHeight = 2048;

        public static GameState actualGameState;
        public static GameState lastGameState;
        public static double loadLevelTime = 0.0;

        private Vector2 nativResolution; 

        public ShroomGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            inputManager = InputManager.Instance;
            Content.RootDirectory = "Content";
            resolution = new Resolution();
            nativResolution = new Vector2(1280, 720);
            graphics.PreferredBackBufferHeight = (int)nativResolution.Y;
            graphics.PreferredBackBufferWidth = (int)nativResolution.X;
            //actualGameState = GameState.LevelOne;
            actualGameState = GameState.MainMenu;
            //actualGameState = GameState.LevelTutorial;
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
            resolution.SetFullscreen(true);
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
            optionsTexture = Content.Load<Texture2D>("Menus/opcjeTlo");
            creditsTexture = Content.Load<Texture2D>("Menus/autorzy");
            deadTexture = Content.Load<Texture2D>("Menus/youDied");

            deadButtonTexture[RestartButtonIndex] = Content.Load<Texture2D>("Menus/restart");
            deadButtonTexture[ExitDeadButtonindex] = Content.Load<Texture2D>("Menus/menuExit");

            menuButtonTexture[StartButtonIndex] = Content.Load<Texture2D>("Menus/menuStart");
            menuButtonTexture[OptionsButtonIndex] = Content.Load<Texture2D>("Menus/menuOptions");
            menuButtonTexture[CreditsButtonIndex] = Content.Load<Texture2D>("Menus/menuCredits");
            menuButtonTexture[ExitButtonIndex] = Content.Load<Texture2D>("Menus/menuExit");

            menuOptionButtonTexture[FullscreenButtonIndex] = Content.Load<Texture2D>("Menus/full");
            menuOptionButtonTexture[BackButtonIndex] = Content.Load<Texture2D>("Menus/inneBack");
            menuOptionButtonTexture[ExitOptionButtonIndex] = Content.Load<Texture2D>("Menus/menuExit");
            menuOptionButtonTexture[BiggestResolutionButtonIndex] = Content.Load<Texture2D>("Menus/roz1");
            menuOptionButtonTexture[BigResolutionButtonIndex] = Content.Load<Texture2D>("Menus/roz2");
            menuOptionButtonTexture[MediumResolutionButton] = Content.Load<Texture2D>("Menus/roz3");
            menuOptionButtonTexture[SmallResolutionButton] = Content.Load<Texture2D>("Menus/roz4");
            menuOptionButtonTexture[InvertAxisXButtonIndex] = Content.Load<Texture2D>("Menus/aX");
            menuOptionButtonTexture[InvertAxisYButtonIndex] = Content.Load<Texture2D>("Menus/aY");

            backButtonTexture = Content.Load<Texture2D>("Menus/inneBack");

            skybox = new Skybox("skybox/SkyBox", Content);

            hpTexture = Content.Load<Texture2D>("hud/paskiZycie");
            timeTexture = Content.Load<Texture2D>("hud/paskiCzas");
            barsFront = Content.Load<Texture2D>("hud/paskiPrzod");
            barsBack = Content.Load<Texture2D>("hud/paskiTyl");
            icons = new Texture2D[9];
            icons[0] = Content.Load<Texture2D>("hud/ikonaStop");
            icons[1] = Content.Load<Texture2D>("hud/ikonaCofnij");
            icons[2] = Content.Load<Texture2D>("hud/ikonaTep");
            icons[3] = Content.Load<Texture2D>("hud/ikonaStopSzara");
            icons[4] = Content.Load<Texture2D>("hud/ikonaCofnijSzara");
            icons[5] = Content.Load<Texture2D>("hud/ikonaTepSzara");

            bossBarFront = Content.Load<Texture2D>("hud/pasekKrolaPrzod");
            bossBarBack = Content.Load<Texture2D>("hud/pasekKrolaTyl");
            bossHpTexture = Content.Load<Texture2D>("hud/pasekKrolaZycie");

            actualDialogueText = new string[3];
            actualDialogueText[0] = ""; 

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
            updateComponents = new List<Component>();
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
                    if (musicManager.currentKey != "menu")
                    {
                        musicManager.StopSong();
                        musicManager.PlaySong("menu");
                        musicManager.IsRepeating = true;
                    }   
                    break;
                case GameState.Options:
                    break;
                case GameState.LevelTutorial:
                    if (bossFight)
                    {
                        //moved to player class
                    }
                    else
                    {
                        musicManager.StopSong();
                        musicManager.PlaySong("5823");
                        musicManager.IsRepeating = true;
                    }
                    break;
                case GameState.Dead:
                    break;
                case GameState.LevelOne:
                    musicManager.StopSong();
                    musicManager.PlaySong("5823");
                    musicManager.IsRepeating = true;
                    break;
            }
        }


        protected override void Update(GameTime gameTime)
        {
            endTimer -= 0.1f;
            if (cutsceneDisplayTime > -0.1f)
            {
                playerShouldNotMove = true;
            }
            else
            {
                playerShouldNotMove = false;
            }

            if (gameComplete && cutsceneDisplayTime < 0.0f && cutscene == null && endTimer < 0f)
            {
                gameComplete = false;
                levelOneCompleted = false;
                tutorialCompleted = false;
                roomEntranceCutscene = false;
                actualGameState = GameState.MainMenu;
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
                    prevMousePressed = mousePressed;
                    mousePressed = inputManager.Mouse[SupportedMouseButtons.Left].IsDown;
                    UpdateMainMenuButtons();
                    break;

                case GameState.Options:
                    OnLevelLoad(actualGameState);
                    prevMousePressed = mousePressed;
                    mousePressed = inputManager.Mouse[SupportedMouseButtons.Left].IsDown;
                    UpdateOptionsButtons();
                    break;

                case GameState.Credits:
                    OnLevelLoad(actualGameState);
                    prevMousePressed = mousePressed;
                    mousePressed = inputManager.Mouse[SupportedMouseButtons.Left].IsDown;
                    UpdateCreditsButton();   
                    break;

                case GameState.Dead:
                    OnLevelLoad(actualGameState);
                    prevMousePressed = mousePressed;
                    mousePressed = inputManager.Mouse[SupportedMouseButtons.Left].IsDown;
                    UpdateDeadButtons();
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
                        if (player.GetComponent<Player>() != null)
                        {
                            outlineEffect.Parameters["TimeStop"].SetValue(player.GetComponent<Player>().timeStop);
                            if (!GameServices.GetService<GameObject>().GetComponent<Player>().timeStop)
                            {
                                foreach (Component comp in updateComponents)
                                {
                                    comp.Update(gameTime);
                                }
                            }
                        }
                        foreach (GameObject obj in enemyList)
                        {
                            obj.Update(gameTime);
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
                        if (player.GetComponent<Player>() != null)
                        {
                            outlineEffect.Parameters["TimeStop"].SetValue(player.GetComponent<Player>().timeStop);
                            if (!GameServices.GetService<GameObject>().GetComponent<Player>().timeStop)
                            {
                                foreach (Component comp in updateComponents)
                                {
                                    comp.Update(gameTime);
                                }
                            }
                        }
                        foreach (GameObject obj in enemyList)
                        {
                            obj.Update(gameTime);
                        }
                        base.Update(gameTime);
                    }
                    break;
            }

            if (inputManager.Keyboard[Keys.Add])
            {
                if (gammaValue < 2.5f) gammaValue += 0.01f;
            }
            if (inputManager.Keyboard[Keys.Subtract])
            {
                if (gammaValue > 0.5f) gammaValue -= 0.01f;
            }

            if (inputManager.Keyboard[Keys.Space].WasPressed && cutsceneDisplayTime > 0.0f)
            {
                if (Timer.gameTime.TotalGameTime.TotalSeconds - loadLevelTime > 6)
                {
                    cutsceneDisplayTime = 0.0f;
                    textDisplayTime = 0.0f;
                }
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
                case GameState.Dead:
                    DrawDead();
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

        private void DrawDead()
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            spriteBatch.Draw(deadTexture, new Rectangle(new Point(0, 0), new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight)), Color.White);
            for (int i = 0; i < NumberOfDeadButtons; i++)
            {
                spriteBatch.Draw(deadButtonTexture[i], deadButtonRectangle[i], deadButtonColor[i]);
            }
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
                cutscene = Cutscene.GetActualCutscene();
                if(cutscene != null)
                {
                    actualCutsceneTexture = cutscene.texture;
                    cutsceneDisplayTime = cutscene.time;
                    new DialogueString(cutscene.text[0]);
                    new DialogueString(cutscene.text[1]);
                    new DialogueString(cutscene.text[2]);
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
                DrawBossHP();
            }
            else
            {
                spriteBatch.Begin(0, BlendState.Opaque, null, null, null);
                spriteBatch.Draw(actualCutsceneTexture, new Rectangle(new Point(0, 0), new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight)), Color.White);
                if (Timer.gameTime.TotalGameTime.TotalSeconds - loadLevelTime > 6)
                {
                    spriteBatch.DrawString(dialoguesFont, "Press space to skip",
                    new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, 50f),
                    Color.Snow, 0.0f, dialoguesFont.MeasureString("Press space to skip") / 2,
                    graphics.GraphicsDevice.Viewport.Width / nativResolution.X, SpriteEffects.None, 0.5f);
                }
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
            if (textDisplayTime < 0f || actualDialogueText[0] == "")
            {
                actualDialogueText[0] = DialogueString.GetActualDialogueString();
                actualDialogueText[1] = DialogueString.GetActualDialogueString();
                actualDialogueText[2] = DialogueString.GetActualDialogueString();
                if(cutscene !=null)
                {
                    textDisplayTime = cutscene.time;
                }
                else
                {
                    textDisplayTime = 6f;
                }
            }
            spriteBatch.Begin();
            spriteBatch.DrawString(dialoguesFont, actualDialogueText[0],
                new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height - 80f),
                Color.Snow, 0.0f, dialoguesFont.MeasureString(actualDialogueText[0]) / 2, 
                graphics.GraphicsDevice.Viewport.Width / nativResolution.X, SpriteEffects.None, 0.5f);
            spriteBatch.DrawString(dialoguesFont, actualDialogueText[1],
               new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height - 50f),
               Color.Snow, 0.0f, dialoguesFont.MeasureString(actualDialogueText[1]) / 2, 
               graphics.GraphicsDevice.Viewport.Width / nativResolution.X, SpriteEffects.None, 0.5f);
            spriteBatch.DrawString(dialoguesFont, actualDialogueText[2],
               new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height - 20f),
               Color.Snow, 0.0f, dialoguesFont.MeasureString(actualDialogueText[2]) / 2, 
               graphics.GraphicsDevice.Viewport.Width / nativResolution.X, SpriteEffects.None, 0.5f);
            spriteBatch.End();
        }

        void DrawBossHP()
        {
            if(bossFight && boss != null)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(bossBarBack, new Rectangle((graphics.GraphicsDevice.Viewport.Width / 2) - (boss.GetComponent<BossEnemy>().MaxHp * 8)/2 , 10, boss.GetComponent<BossEnemy>().MaxHp * 8, 80), Color.White);
                spriteBatch.Draw(bossHpTexture, new Rectangle(((graphics.GraphicsDevice.Viewport.Width / 2) - (boss.GetComponent<BossEnemy>().MaxHp * 8) / 2) + (100 - boss.GetComponent<BossEnemy>().Hp), 10, boss.GetComponent<BossEnemy>().Hp * 8, 80), Color.White);
                spriteBatch.Draw(bossBarFront, new Rectangle((graphics.GraphicsDevice.Viewport.Width / 2) - (boss.GetComponent<BossEnemy>().MaxHp * 8) / 2, 10, boss.GetComponent<BossEnemy>().MaxHp * 8, 80), Color.White);
                spriteBatch.End();
            }
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
                if (player.GetComponent<Player>().canUseQ)
                {
                    if (player.GetComponent<Player>().GetTimeEnergy() > 1)
                    {
                        spriteBatch.Draw(icons[0], new Rectangle(graphics.GraphicsDevice.Viewport.Width - 380, graphics.GraphicsDevice.Viewport.Height - 90, 103, 63), Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(icons[3], new Rectangle(graphics.GraphicsDevice.Viewport.Width - 380, graphics.GraphicsDevice.Viewport.Height - 90, 103, 63), Color.White);
                    }
                }
                if (player.GetComponent<Player>().canUseE)
                {
                    if (player.GetComponent<Player>().GetTimeEnergy() >= 5)
                    {
                        spriteBatch.Draw(icons[1], new Rectangle(graphics.GraphicsDevice.Viewport.Width - 260, graphics.GraphicsDevice.Viewport.Height - 90, 103, 63), Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(icons[4], new Rectangle(graphics.GraphicsDevice.Viewport.Width - 260, graphics.GraphicsDevice.Viewport.Height - 90, 103, 63), Color.White);
                    }
                }
                if (player.GetComponent<Player>().canUseR)
                {
                    if (player.GetComponent<Player>().GetTimeEnergy() >= 3)
                    {
                        spriteBatch.Draw(icons[2], new Rectangle(graphics.GraphicsDevice.Viewport.Width - 140, graphics.GraphicsDevice.Viewport.Height - 90, 103, 63), Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(icons[5], new Rectangle(graphics.GraphicsDevice.Viewport.Width - 140, graphics.GraphicsDevice.Viewport.Height - 90, 103, 63), Color.White);
                    }
                }
                spriteBatch.End();
            }
        }

        private void CalcButtonSize()
        {
            int BUTTON_HEIGHT = Window.ClientBounds.Height / 9;
            int BUTTON_WIDTH = Window.ClientBounds.Width / 5;
            int x = Window.ClientBounds.Width / 2 - BUTTON_WIDTH / 2;
            int y = (Window.ClientBounds.Height / 2 - NumberOfMenuButtons / 2 * BUTTON_HEIGHT - (NumberOfMenuButtons % 2) * BUTTON_HEIGHT / 2) + BUTTON_HEIGHT;
            for (int i = 0; i < NumberOfMenuButtons; i++)
            {
                menuButtonState[i] = BState.UP;
                menuButtonColor[i] = Color.White;
                menuButtonRectangle[i] = new Rectangle(x, y, BUTTON_WIDTH, BUTTON_HEIGHT);
                y += BUTTON_HEIGHT + 20;
            }
            y = (2*Window.ClientBounds.Height / 3 - NumberOfDeadButtons / 2 * BUTTON_HEIGHT - (NumberOfDeadButtons % 2) * BUTTON_HEIGHT / 2) + BUTTON_HEIGHT;
            for (int i = 0; i < NumberOfDeadButtons; i++)
            {
                deadButtonState[i] = BState.UP;
                deadButtonColor[i] = Color.White;
                deadButtonRectangle[i] = new Rectangle(x, y, BUTTON_WIDTH, BUTTON_HEIGHT);
                y += BUTTON_HEIGHT + 20;
            }
            x = (Window.ClientBounds.Width / 2 - 4 / 2 * BUTTON_WIDTH - (4 % 2) * BUTTON_WIDTH / 2);
            y = 2* BUTTON_HEIGHT;
            for (int i = 0; i < 4; i++)
            {
                menuOptionButtonState[i] = BState.UP;
                menuOptionButtonColor[i] = Color.White;
                menuOptionButtonRectangle[i] = new Rectangle(x, y, BUTTON_WIDTH, BUTTON_HEIGHT);
                x += BUTTON_WIDTH + 20;
            }
            y += BUTTON_HEIGHT + 20;
            menuOptionButtonState[FullscreenButtonIndex] = BState.UP;
            menuOptionButtonColor[FullscreenButtonIndex] = Color.White;
            x = Window.ClientBounds.Width / 2 - BUTTON_WIDTH / 2;
            menuOptionButtonRectangle[FullscreenButtonIndex] = new Rectangle(x, y, BUTTON_WIDTH, BUTTON_HEIGHT);
            y = (Window.ClientBounds.Height / 2)  + BUTTON_HEIGHT;
            for (int i = 5; i < 7; i++)
            {
                menuOptionButtonState[i] = BState.UP;
                menuOptionButtonColor[i] = Color.White;
                menuOptionButtonRectangle[i] = new Rectangle(menuOptionButtonRectangle[i-5].X, y, BUTTON_WIDTH, BUTTON_HEIGHT);
            }
            y = (Window.ClientBounds.Height) - 2 * BUTTON_HEIGHT;
            for (int i = 7; i < 9; i++)
            {
                menuOptionButtonState[i] = BState.UP;
                menuOptionButtonColor[i] = Color.White;
                menuOptionButtonRectangle[i] = new Rectangle(menuOptionButtonRectangle[i - 5].X, y, BUTTON_WIDTH, BUTTON_HEIGHT);
            }
            y = (Window.ClientBounds.Height / 2 - NumberOfMenuButtons / 2 * BUTTON_HEIGHT - (NumberOfMenuButtons % 2) * BUTTON_HEIGHT / 2) + BUTTON_HEIGHT;
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
                if (HitImageAlpha(menuButtonRectangle[i], menuButtonTexture[i], (int)inputManager.Mouse.Position.X, (int)inputManager.Mouse.Position.Y))
                {
                    if (mousePressed)
                    {
                        menuButtonState[i] = BState.DOWN;
                        menuButtonColor[i] = Color.DarkOliveGreen;
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
                        menuButtonColor[i] = Color.DarkKhaki;
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
                if (HitImageAlpha(menuOptionButtonRectangle[i], menuOptionButtonTexture[i], (int)inputManager.Mouse.Position.X, (int)inputManager.Mouse.Position.Y))
                {
                    if (mousePressed)
                    {
                        menuOptionButtonState[i] = BState.DOWN;
                        menuOptionButtonColor[i] = Color.DarkOliveGreen;
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
                        menuOptionButtonColor[i] = Color.DarkKhaki;
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
            if (HitImageAlpha(backButtonRectangle, backButtonTexture, (int)inputManager.Mouse.Position.X, (int)inputManager.Mouse.Position.Y))
            {
                if (mousePressed)
                {
                    backButtonState = BState.DOWN;
                    backButtonColor = Color.DarkOliveGreen;
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
                    backButtonColor = Color.DarkKhaki;
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

        void UpdateDeadButtons()
        {
            for (int i = 0; i < NumberOfDeadButtons; i++)
            {
                if (HitImageAlpha(deadButtonRectangle[i], deadButtonTexture[i], (int)inputManager.Mouse.Position.X, (int)inputManager.Mouse.Position.Y))
                {
                    if (mousePressed)
                    {
                        deadButtonState[i] = BState.DOWN;
                        deadButtonColor[i] = Color.DarkOliveGreen;
                    }
                    else if (!mousePressed && prevMousePressed)
                    {
                        if (deadButtonState[i] == BState.DOWN)
                        {
                            deadButtonState[i] = BState.JUST_RELEASED;
                        }
                    }
                    else
                    {
                        deadButtonState[i] = BState.HOVER;
                        deadButtonColor[i] = Color.DarkKhaki;
                    }
                }
                else
                {
                    deadButtonState[i] = BState.UP;
                    deadButtonColor[i] = Color.White;
                }

                if (deadButtonState[i] == BState.JUST_RELEASED)
                {
                    DeadButtonActions(i);
                }
            }
        }

        private void DeadButtonActions(int i)
        {
            switch (i)
            {
                case RestartButtonIndex:
                    areCollidersAndTriggersSet = false;
                    actualGameState = lastGameState;
                    break;
                case ExitDeadButtonindex:
                    Exit();
                    break;
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
                    loadLevelTime = Timer.gameTime.TotalGameTime.TotalSeconds;
                    areCollidersAndTriggersSet = false;
                    new Cutscene(Content.Load<Texture2D>("Cutscene/1.3"), 18f, "Player: Your Majesty, I am ready for your orders.",
                        "King: Loyal knight, the secret service of our Kingdom has revealed a conspiracy against the Crown!",
                        "Player: How is this possible? Who would dare to stand against You my King?");
                    new Cutscene(Content.Load<Texture2D>("Cutscene/1.1"), 18f, "King: Nobody suspected that. The traitor turned out to be one of the knights, Borovikus",
                        "Player: Borovikus? It is not possible, he was always the most loyal one!",
                        "King: The evidence is irrefutable. Borovikus is plotting with the enemy. You must stop him!");
                    new Cutscene(Content.Load<Texture2D>("Cutscene/1.2"), 18f, "Player: What is your command, Lord?",
                        "King: Go after him and kill! Kill the traitor. Bring his hat to me as a proof.",
                        "Player: Yes Sir. Your wish is my command.");
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
                case BiggestResolutionButtonIndex:
                    resolution.SetResolution(1920, 1080);
                    screenRenderTarget = new RenderTarget2D(graphics.GraphicsDevice, graphics.GraphicsDevice.Viewport.Width,
                                                graphics.GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);
                    CalcButtonSize();
                    break;
                case BigResolutionButtonIndex:
                    resolution.SetResolution(1280, 720);
                    screenRenderTarget = new RenderTarget2D(graphics.GraphicsDevice, graphics.GraphicsDevice.Viewport.Width,
                                                graphics.GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);
                    CalcButtonSize();
                    break;
                case MediumResolutionButton:
                    resolution.SetResolution(1024, 768);
                    screenRenderTarget = new RenderTarget2D(graphics.GraphicsDevice, graphics.GraphicsDevice.Viewport.Width,
                                                graphics.GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);
                    CalcButtonSize();
                    break;
                case SmallResolutionButton:
                    resolution.SetResolution(800, 600);
                    screenRenderTarget = new RenderTarget2D(graphics.GraphicsDevice, graphics.GraphicsDevice.Viewport.Width,
                                                graphics.GraphicsDevice.Viewport.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);
                    CalcButtonSize();
                    break;
                case FullscreenButtonIndex:
                    fullscreen = !fullscreen;
                    resolution.SetFullscreen(fullscreen);
                    CalcButtonSize();
                    break;
                case InvertAxisXButtonIndex:
                    mouseXAxis *= -1;
                    break;
                case InvertAxisYButtonIndex:
                    mouseYAxis *= -1;
                    break;
                case BackButtonIndex:
                    if (lastGameState != GameState.MainMenu)
                    {
                        Mouse.SetPosition(GameServices.GetService<GraphicsDevice>().Viewport.Width / 2,
                        GameServices.GetService<GraphicsDevice>().Viewport.Height / 2);
                    }
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
