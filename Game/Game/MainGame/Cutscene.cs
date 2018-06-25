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
        public string[] text;
        static Queue<Cutscene> cutsceneQueue = new Queue<Cutscene>();

        public Cutscene(Texture2D _texture, float _time, string _text1 = "", string _text2 = "", string _text3 = "")
        {
            texture = _texture;
            time = _time;
            text = new string[3];
            text[0] = _text1;
            text[1] = _text2;
            text[2] = _text3;
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
