using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.MainGame
{
    public class Resolution
    {

        private int width = 1280;
        private int height = 720;


        private int virtualWidth = 1280;
        private int virtualHeight = 720;

        private Matrix scaleMatrix;

        private bool fullscreen = false;

        private bool dirtyMatrix = true;

        public int BaseWidth
        {
            get
            {
                return this.virtualWidth;
            }
        }

        public int BaseHeight
        {
            get
            {
                return this.virtualHeight;
            }
        }

        public int RealWidth
        {
            get
            {
                return this.width;
            }
        }

        public int RealHeight
        {
            get
            {
                return this.height;
            }
        }

        public bool Fullscreen
        {
            get
            {
                return this.fullscreen;
            }
            set
            {
                this.fullscreen = value;
            }
        }

        public Matrix Matrix
        {
            get
            {
                if (this.dirtyMatrix)
                {
                    this.RecreateScaleMatrix();
                }
                return this.scaleMatrix;
            }
        }

        public Vector2 ViewportSize
        {
            get;
            private set;
        }

        internal Resolution()
        {
        }

        public void Initialize()
        {
            this.width = GameServices.GetService<GraphicsDevice>().Viewport.Width;
            this.height = GameServices.GetService<GraphicsDevice>().Viewport.Height;
            this.dirtyMatrix = true;
            this.ApplyChanges();
        }

        public void SetResolution(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.ApplyChanges();
        }

        public void SetResolution(int width, int height, bool fullscreen)
        {
            this.width = width;
            this.height = height;
            this.fullscreen = fullscreen;
            this.ApplyChanges();
        }

        public void SetBaseResolution(int width, int height)
        {
            this.virtualWidth = width;
            this.virtualHeight = height;
            this.dirtyMatrix = true;
        }


        private void ApplyChanges()
        {
            var dmgr = GameServices.GetService<GraphicsDeviceManager>();
            if (dmgr != null)
            {
                if (this.fullscreen == false)
                {
                    if ((this.width <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
                        && (this.height <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height))
                    {
                        dmgr.PreferredBackBufferWidth = this.width;
                        dmgr.PreferredBackBufferHeight = this.height;
                        dmgr.IsFullScreen = this.fullscreen;
                        dmgr.ApplyChanges();
                    }
                }
                else
                {
                    foreach (DisplayMode dm in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
                    {
                        if ((dm.Width == width) && (dm.Height == height))
                        {
                            dmgr.PreferredBackBufferWidth = this.width;
                            dmgr.PreferredBackBufferHeight = this.height;
                            dmgr.IsFullScreen = this.fullscreen;
                            dmgr.ApplyChanges();
                        }
                    }
                }

                this.dirtyMatrix = true;

                this.width = dmgr.PreferredBackBufferWidth;
                this.height = dmgr.PreferredBackBufferHeight;
            }
        }


        private void RecreateScaleMatrix()
        {
            dirtyMatrix = false;
            scaleMatrix = Matrix.CreateScale(
                           (float)GameServices.GetService<GraphicsDevice>().Viewport.Width / virtualWidth,
                           (float)GameServices.GetService<GraphicsDevice>().Viewport.Width / virtualWidth,
                           1f);
        }


        private float GetVirtualAspectRatio()
        {
            return (float)virtualWidth / (float)virtualHeight;
        }


        internal void ApplyTotalViewport()
        {
            Viewport vp = GameServices.GetService<GraphicsDevice>().Viewport;
            vp.X = vp.Y = 0;
            vp.Width = width;
            vp.Height = height;
            GameServices.GetService<GraphicsDevice>().Viewport = vp;
        }

        internal void ApplyScaledViewport()
        {
            float targetAspectRatio = GetVirtualAspectRatio();

            int preferredWidth = GameServices.GetService<GraphicsDevice>().Viewport.Width;
            int preferredHeight = GameServices.GetService<GraphicsDevice>().Viewport.Height;

            var dmgr = GameServices.GetService<GraphicsDeviceManager>();
            if (dmgr != null)
            {
                preferredWidth = GameServices.GetService<GraphicsDeviceManager>().PreferredBackBufferWidth;
                preferredHeight = GameServices.GetService<GraphicsDeviceManager>().PreferredBackBufferHeight;
            }

            int width = preferredWidth;
            int height = (int)(width / targetAspectRatio + .5f);

            bool changed = false;

            if (height > preferredHeight)
            {
                height = preferredHeight;
                width = (int)(height * targetAspectRatio + .5f);
                changed = true;
            }

            Viewport viewport = GameServices.GetService<GraphicsDevice>().Viewport;

            viewport.X = (preferredWidth / 2) - (width / 2);
            viewport.Y = (preferredHeight / 2) - (height / 2);
            viewport.Width = width;
            viewport.Height = height;
            viewport.MinDepth = 0;
            viewport.MaxDepth = 1;

            Vector2 size = this.ViewportSize;
            size.X = viewport.X;
            size.Y = viewport.Y;
            this.ViewportSize = size;

            if (changed)
            {
                dirtyMatrix = true;
            }

            GameServices.GetService<GraphicsDevice>().Viewport = viewport;
        }
    }
}
