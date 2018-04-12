using Microsoft.Xna.Framework;

namespace Game.Misc.Time
{
    static class Timer
    {
        public static GameTime gameTime { get; private set; }
        private static bool isInitialized = false;

        public static void Update(GameTime _gameTime)
        {
            gameTime = _gameTime;
        }
    }
}
