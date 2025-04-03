namespace Utils
{
    public static class Constants
    {
        // Scene names
        public const string SCENE_LOBBY = "Lobby";
        public const string SCENE_GAME  = "Game";

        // Addressables Address
        public const string ADDRESS_LOADING_PREFAB = "LoadingPrefab";
        
        // Loading Progress Weights (used to compute overall progress)
        public const float UI_LOADING_WEIGHT    = 0.30f;
        public const float AUDIO_LOADING_WEIGHT = 0.30f;
        public const float SCENE_LOADING_WEIGHT = 0.40f;
    }
}