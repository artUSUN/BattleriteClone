namespace Source.Code.MyPhoton
{
    public static class GlobalConst
    {
        //Player Properties consts
        public const string PLAYER_CARD_POSITION_ID = "PlayerCardID"; //int
        public const string PLAYER_LOADED_LEVEL = "PlayerLoadedLevel"; //bool
        public const string PLAYER_READY = "PlayerReady"; //bool
        public const string PLAYER_FACTION = "PlayerFaction"; //int
        public const string PLAYER_UNIT_PREFAB_NAME = "PlayerUnitPrefabName"; //string

        //Room Properties consts
        public const string ROOM_MATCH_DURATION = "MatchDuration"; //float
        public const string ROOM_POINTS_TO_WIN   = "PointsToWin"; //float
        public const string ROOM_UNIT_RESPAWN_DURATION = "UnitRespawnDuration"; //float
        public const string ROOM_UNIT_MISSLE_DAMAGE = "UnitMissleDamage"; //float
        public const string ROOM_UNIT_ATTACK_SPEED = "AttackSpeed"; //float
        public const string ROOM_UNIT_HP = "UnitHP"; //float
        public const string ROOM_UNIT_ABILITY_ROLL_CD = "RollCooldown"; //float
        public const string ROOM_POWERUP_HEAL_POWER = "HealPowerupPower"; //float

        public const string GAME_STATE = "GameState"; //byte = 0 - preGame, 1 - game, 2 - gameEnded

        public static string GetFactionScoresKey(int factionID)
        {
            return "Faction" + factionID + "Scores";
        }

        //RPC consts
        public const byte ROOM_START_MATCH_BEGIN = 1; //null
        public const byte GAME_PRE_GAME_TIMER_STARTED = 2; //current time (float)
    }
}