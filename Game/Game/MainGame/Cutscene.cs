using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.MainGame
{
    public class Cutscene
    {
        public Texture2D texture;
        public float time;
        public string text;
        static Queue<Cutscene> cutsceneQueue = new Queue<Cutscene>();

        public Cutscene(Texture2D _texture, float _time, string _text = null)
        {
            texture = _texture;
            time = _time;
            text = _text;
            cutsceneQueue.Enqueue(this);
        }

        public static Cutscene GetActualCutscene()
        {
            if (cutsceneQueue.Count != 0)
            {
                return cutsceneQueue.Dequeue();
            }
            else
            {
                return null;
            }
        }
    } 
}
