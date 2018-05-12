using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBLGame.MainGame
{
    public class DialogueString
    {
        private float sizeLimit = 35f;
        static Queue<string> dialoguesQueue = new Queue<string>();
        public DialogueString(string text)
        {
            string[] words = text.Split(' ');
            float linewidth = 0f;
            string dialogue = "";
            foreach (string word in words)
            {
                if (linewidth + word.Length < sizeLimit)
                {
                    dialogue += word + " ";
                    linewidth = dialogue.Length;
                }
                else
                {
                    dialoguesQueue.Enqueue(dialogue);
                    dialogue = word + " ";
                    linewidth = dialogue.Length;
                }
            }
            dialoguesQueue.Enqueue(dialogue);
        }

        public static string GetActualDialogueString()
        {
            if(dialoguesQueue.Count != 0)
            {
                return dialoguesQueue.Dequeue();
            }
            else
            {
                return "";
            }
    }
    }
}
