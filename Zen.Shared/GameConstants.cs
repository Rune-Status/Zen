namespace Zen.Shared
{
    public class GameConstants
    {
        public const string Title = "Zen";
        public const int Port = 43594;
        public const int Version = 530;

        public const int MaxPlayers = 2048;
        public const int MaxNpcs = 5000;

        public const int SpawnX = 3093;
        public const int SpawnY = 3493;

        public const string WorkingDirectory = @"../../../Zen.Data/";
        public const string CacheDirectory = WorkingDirectory + "Cache/";
        public const string LandscapeDirectory = WorkingDirectory + "Landscape/";
        public const string CharacterDirectory = WorkingDirectory + "Characters/";
    }
}