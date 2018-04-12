public static class Constants
{
    public static class RewiredInputActions
    {
        public const string GyroHorizontal = "Horizontal";
        public const string GyroForward = "Forward";
    }

    public static class Mixer
    {
        public static string Path = "Audio/MainMixer";
        public static class Mixers
        {
            public static class Master
            {
                public const string Name = "Master";
                public static class SFX
                {
                    public const string Name = Master.Name + "/SFX";
                }

                public static class Music
                {
                    public const string Name = Master.Name + "/Music";
                }
            }
        }
    }

    public static class AudioPoolSize
    {
        public const uint Music = 2;
        public const uint SFX = 10;
    }
}