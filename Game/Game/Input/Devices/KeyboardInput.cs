using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Game.Input.Devices
{
    public class KeyboardInput : IInput
    {
        private readonly Dictionary<Keys, KeyboardKey> _keys;
        private KeyboardState _state;

        internal KeyboardInput()
        {
            this._keys = new Dictionary<Keys, KeyboardKey>();
            foreach (Keys k in Enum.GetValues(typeof(Keys)))
            {
                this._keys.Add(k, new KeyboardKey(k));
            }

        }

        public delegate void KeyboardEvent(KeyboardKey key);
        public event KeyboardEvent WasPressed;
        public event KeyboardEvent WasReleased;

        // indexer, gets the key state for the keyboard key specified in dictionary
        public KeyboardKey this[Keys key]
        {
            get => this._keys[key];
            internal set => this._keys[key] = value;
        }

        public void Update()
        {
            this._state = Keyboard.GetState();

            foreach (var key in this._keys.Keys)
            {
                this[key].Update(this._state.IsKeyDown(key));
                if (this[key].WasPressed)
                {
                    WasPressed?.Invoke(this[key]);
                }
                else if (this[key].WasReleased)
                {
                    WasReleased?.Invoke(this[key]);
                }
            }
        }
    }
}
