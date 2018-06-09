using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace PBLGame.Input.Devices
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
        public delegate void MouseScrollEvent(int ammount);
        public event MouseKeyEvent WasPressed;
        public event MouseKeyEvent WasReleased;
        public event MouseScrollEvent Scroll;
        public int ScrollTotal { get; internal set; }
        public int ScrollValue { get; internal set; }


        public Vector2 PositionsDelta => (this._lastPosition - this._currentPosition);

        public Vector2 Position
        {
            get
            {
               return this._currentPosition;
            }
            set
            {
                this._currentPosition = value;
                Mouse.SetPosition(Convert.ToInt32(value.X), Convert.ToInt32(value.Y));
            }
        }


        // indexer, gets the key state for the mouse key specified in dictionary
        public MouseKey this[SupportedMouseButtons key]
        {
            get{ return this._keys[key];}
            internal set
            {
                this._keys[key] = value;
            }
        }

        public void Update()
        {
            this._lastState = this._state;
            this._state = Mouse.GetState();

            this._lastPosition = new Vector2(GameServices.GetService<GraphicsDevice>().Viewport.Width / 2, GameServices.GetService<GraphicsDevice>().Viewport.Height / 2);
            this._currentPosition = new Vector2(this._state.X, this._state.Y);
            Mouse.SetPosition(GameServices.GetService<GraphicsDevice>().Viewport.Width / 2, GameServices.GetService<GraphicsDevice>().Viewport.Height / 2);

            this.ScrollTotal = this._state.ScrollWheelValue;
            this.ScrollValue = this._state.ScrollWheelValue - this._lastState.ScrollWheelValue;
            if (this.ScrollValue != 0)
            //To podobnie jak wyżej
            {
                Scroll?.Invoke(this.ScrollValue);
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

                    case SupportedMouseButtons.Middle:
                        this[key].Update(this._state.MiddleButton == ButtonState.Pressed);
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
