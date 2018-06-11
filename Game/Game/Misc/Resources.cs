using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Game.Misc
{
    static class Resources
    {
        public static ContentManager Content { get; private set; }
        public static Vector3 CameraVector3;

        public static void Init(ContentManager _content)
        {
            Content = _content;
        }
    }
}
