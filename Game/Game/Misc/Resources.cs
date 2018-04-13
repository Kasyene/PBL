
using Microsoft.Xna.Framework.Content;

namespace Game.Misc
{
    static class Resources
    {
        public static ContentManager Content { get; private set; }

        public static void Init(ContentManager _content)
        {
            Content = _content;
        }
    }
}
