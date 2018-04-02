using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Game.Input.Devices
{
    public class MouseInput : IInput
    {
        private readonly Dictionary<SupportedMouseButtons, MouseKey> _keys;
        private Vector2 _currentPosition;
        private Vector2 _lastPosition;
        private MouseState _state;
        private MouseState _lastState;

        internal MouseInput()
        {
            this._lastState = Mouse.GetState();
            this._state = this._lastState;

            this._keys = new Dictionary<SupportedMouseButtons, MouseKey>();
            foreach (SupportedMouseButtons k in Enum.GetValues(typeof(SupportedMouseButtons)))
            {
                this._keys.Add(k, new MouseKey(k));
            }
        }

        public delegate void MouseKeyEvent(MouseKey key);
        public event MouseKeyEvent WasPressed;
        public event MouseKeyEvent WasReleased;

        public Vector2 PositionsDelta => (this._lastPosition - this._currentPosition);

        public Vector2 Position
        {
            get => this._currentPosition;
            set
            {
                this._currentPosition = value;
                Mouse.SetPosition(Convert.ToInt32(value.X), Convert.ToInt32(value.Y));
            }
        }


        // indexer, gets the key state for the mouse key specified in dictionary
        public MouseKey this[SupportedMouseButtons key]
        {
            get => this._keys[key];
            internal set => this._keys[key] = value;
        }

        public void Update()
        {
            this._lastState = this._state;
            this._state = Mouse.GetState();

            if (this._lastState.X != this._state.X || this._lastState.Y != this._state.Y)
            {
                this._lastPosition = _currentPosition;
                this._currentPosition = new Vector2(this._state.X, this._state.Y);
            }


            foreach (var key in this._keys.Keys)
            {
                switch (key)
                {
                    case SupportedMouseButtons.Left:
                        this[key].Update(this._state.LeftButton == ButtonState.Pressed);
                        break;
                  
                    case SupportedMouseButtons.Right:
                        this[key].Update(this._state.RightButton == ButtonState.Pressed);
                        break;

                    default:
                        break;
                }

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
