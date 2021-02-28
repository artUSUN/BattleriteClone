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
        public const string ROOM_MATCH_DURATION = "MatchDuration"; //string
        public const string ROOM_POINTS_TO_WIN   = "PointsToWin"; //string
        public const string ROOM_UNIT_RESPAWN_DURATION = "UnitRespawnDuration"; //string
        public const string ROOM_UNIT_MISSLE_DAMAGE = "UnitMissleDamage"; //string
        public const string ROOM_UNIT_ATTACK_SPEED = "AttackSpeed"; //string
        public const string ROOM_UNIT_HP = "UnitHP"; //string
        public const string ROOM_UNIT_ABILITY_ROLL_CD = "RollCooldown"; //string
        public const string ROOM_POWERUP_HEAL_POWER = "HealPowerupPower"; //string



        //RPC consts
        public const byte ROOM_START_MATCH_BEGIN = 1; //null
    }
}