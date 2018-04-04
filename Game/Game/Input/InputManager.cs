using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PBLGame.Input.Devices;
using Microsoft.Xna.Framework.Input;

namespace PBLGame.Input
{
    public class InputManager
    {
        private readonly List<IInput> _devices;
        internal InputManager()
        {
            this._devices = new List<IInput>();
            this._devices.Add(this.Keyboard = new KeyboardInput());
            this._devices.Add(this.Mouse = new MouseInput());
        }
        public KeyboardInput Keyboard { get; internal set; }
        public MouseInput Mouse { get; internal set; }

        public void Update()
        {
            foreach (var device in this._devices)
            {
                device.Update();
            }
        }
    }
}
