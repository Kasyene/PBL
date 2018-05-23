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

        internal Resolution()
        {
        }

        public void InitializeResolution(int width, int height)
        {
            GameServices.GetService<GraphicsDeviceManager>().PreferredBackBufferWidth = width;
            GameServices.GetService<GraphicsDeviceManager>().PreferredBackBufferHeight = height;
        }

        public void SetResolution(int width, int height)
        {
            GameServices.GetService<GraphicsDeviceManager>().PreferredBackBufferWidth = width;
            GameServices.GetService<GraphicsDeviceManager>().PreferredBackBufferHeight = height;
            GameServices.GetService<GraphicsDeviceManager>().ApplyChanges();
        }

        public void SetResolutionAndFullscreen(int width, int height, bool fullscreen)
        {
            GameServices.GetService<GraphicsDeviceManager>().PreferredBackBufferWidth = width;
            GameServices.GetService<GraphicsDeviceManager>().PreferredBackBufferHeight = height;
            GameServices.GetService<GraphicsDeviceManager>().IsFullScreen = fullscreen;
            GameServices.GetService<GraphicsDeviceManager>().ApplyChanges();
        }

        public void SetFullscreen(bool fullscreen)
        {
            GameServices.GetService<GraphicsDeviceManager>().IsFullScreen = fullscreen;
            GameServices.GetService<GraphicsDeviceManager>().ApplyChanges();
        }
    }
}
