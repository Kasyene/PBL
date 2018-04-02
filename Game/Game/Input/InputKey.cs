using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Input
{
    public class InputKey
    {
        public bool WasPressed { get; internal set; }
        public bool IsDown { get; internal set; }
        public bool WasReleased { get; internal set; }

        public void Update(bool heldDown)
        {
            if (heldDown)
            {
                if (this.WasPressed)
                {
                    this.WasPressed = false;
                }
                else if (!this.IsDown)
                {
                    this.WasPressed = true;
                }
                this.IsDown = true;
                this.WasReleased = false;
            }
            else
            {
                if (this.WasReleased)
                {
                    this.WasReleased = false;
                }
                else if (this.IsDown)
                {
                    this.WasReleased = true;
                }

                this.IsDown = false;
                this.WasPressed = false;
            }
        }
    }
}
