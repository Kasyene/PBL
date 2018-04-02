using Microsoft.Xna.Framework.Input;

namespace Game.Input.Devices
{
    public class KeyboardKey : InputKey
    {
        internal KeyboardKey(Keys key)
        {
            this.Key = key;
        }

        public Keys Key { get; internal set; }

        public static implicit operator bool(KeyboardKey key)
        {
            return key.IsDown;
        }
    }
}
