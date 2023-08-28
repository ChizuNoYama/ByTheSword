using ByTheSword.Scripts.Entities;
using Vector2 = Godot.Vector2;

namespace ByTheSword.Scripts.Models;

public class CellData
{
    public Vector2 GlobalPosition { get; set; }
    public Entity Entity { get; set; }
    public bool IsNoGoZone { get; set; }
    // TODO: List of items in cell
}