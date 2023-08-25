using System.Numerics;
using ByTheSword.Scripts.Entities;

namespace ByTheSword.Scripts.Models;

public class CellData
{
    public IEntity Entity { get; set; }
    public bool IsNoGoZone { get; set; }
    // TODO: List of items in cell
}