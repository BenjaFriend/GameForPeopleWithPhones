public static class Constants
{
    public static class RewiredInputActions
    {
        public const string GyroHorizontal = "Horizontal";
        public const string GyroForward = "Forward";
    }

    public static class SceneNames
    {
        public const string Launcher = "Launcher";
        public const string LobbyRoom = "Lobby Room";
        public const string CanGame = "CanDemo";
    }

    public enum EVENT_ID : byte
    {
        CAN_BROKE = 0,
        CAN_SHAKE = 1,
        START_COUNTDOWN_FINISHED = 3,
        COUNTDOWN_TICK = 4
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