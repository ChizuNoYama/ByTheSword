using System;
using ByTheSword.Scripts.Entities;
using ByTheSword.Scripts.Models;
using Godot;

namespace ByTheSword.Scripts.Controllers;

public partial class SceneController : Node2D
{
    private TileMap _map;
    
    public override void _Ready()
    {
        base._Ready();
        _map = this.GetNode<TileMap>("TileMap");
    }

    public CellType PeekTile(Vector2 targetPosition)
    {
        // Check for walls;
        var mapPosition = _map.LocalToMap(targetPosition);
        TileData tileData = _map.GetCellTileData(1, mapPosition);
        if (tileData != null)
        {
           return CellType.NoGoZone;
        }

        // Check for enemies
        // this.GetNode<Enemy>("Enemy");
        
        return CellType.None;
    }
}