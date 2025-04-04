namespace Utils
{
    public static class Constants
    {
        // Scene names
        public const string SCENE_LOBBY = "Lobby";
        public const string SCENE_GAME  = "Game";

        // Addressables Address
        public const string ADDRESS_LOADING_PREFAB = "LoadingPrefab";
        public const string PLAYERS_ATLAS = "PlayersAtlas";
        public const string BET_BUTTON_PREFAB_ADDRESS = "BetButtonPrefabAddress";
        public const string PLAYER_PREFAB_ADDRESS = "PlayerPrefabAddress";
        public const string CARD_ATLAS = "CardAtlas";
        public const string CARD_BACK_ADDRESS = "CardBack";
        public const string CARD_PREFAB_ADDRESS = "CardPrefabAddress";
        public const string UI_ATLAS = "UIAtlas";



        
        // Loading Progress Weights (used to compute overall progress)
        public const float UI_LOADING_WEIGHT    = 0.30f;
        public const float AUDIO_LOADING_WEIGHT = 0.30f;
        public const float SCENE_LOADING_WEIGHT = 0.40f;
    }
}