namespace PBLGame.Input.Devices
{
    public class MouseKey : InputKey
    {
        internal MouseKey(SupportedMouseButtons key)
        {
            this.Key = key;
        }

        public SupportedMouseButtons Key { get; internal set; }

        public static implicit operator bool(MouseKey key)
        {
            return key.IsDown;
        }
    }
}
