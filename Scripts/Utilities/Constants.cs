namespace ByTheSword.Scripts.Utilities;

public static class Constants
{
    public const float MOVEMENT_STEP = 32f;
    public const double TURN_TIMER_INTERVAL = 150; // in milliseconds
    
    public const string ACTION_STEP_RIGHT = "step_right";
    public const string ACTION_STEP_LEFT = "step_left";
    public const string ACTION_STEP_UP = "step_up";
    public const string ACTION_STEP_DOWN = "step_down";

    public const string IS_ENEMY_META_IDENTIFIER = "is_enemy";

    public const uint WALLS_COLLISION_LAYER = 1;
    public const uint PLAYER_COLLISION_LAYER = 2;
    public const uint ENEMY_COLLISION_LAYER = 3;
    public const uint ITEMS_COLLISION_LAYER = 4;

    public const int MAP_FLOOR_LAYER = 0;
    public const int MAP_WALL_LAYER = 1;
    public const int MAP_FURNITURE_LAYER = 2;
    public const int MAP_ITEMS_LAYER = 3;
    public const int MAP_FLOOR_PIT_LAYER = 4;
}