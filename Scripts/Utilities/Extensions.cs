using Godot;

namespace ByTheSword.Scripts.Utilities;

public static class Extensions
{
    public static Vector2I ToVector2I(this Vector2 initialVector)
    {
        return new Vector2I((int)initialVector.X, (int)initialVector.Y);
    }
}