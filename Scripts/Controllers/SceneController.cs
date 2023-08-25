using System;
using System.Collections.Generic;
using System.Linq;
using ByTheSword.Scripts.Entities;
using ByTheSword.Scripts.Models;
using Godot;
using Godot.Collections;
using Microsoft.VisualBasic;
using Constants = ByTheSword.Scripts.Utilities.Constants;

namespace ByTheSword.Scripts.Controllers;

public partial class SceneController : Node2D
{
	private TileMap _map;
	private CombatController _combatController;
	private List<IEntity> _entities;

	public override void _EnterTree()
	{
		base._EnterTree();
		_entities = new List<IEntity>();
	}

	public override void _Ready()
	{
		base._Ready();
		_map = this.GetNode<TileMap>("TileMap");
		_combatController = new CombatController();
	}

	public void RegisterEntity(IEntity entity)
	{
		_entities.Add(entity);
	}

	public void DeregisterEntity(IEntity entity)
	{
		_entities.Remove(entity);
	}

	public CellData PeekTargetCell(Vector2 targetPosition)
	{
		CellData cellData = new CellData();
		
		// Check for walls;
		var mapPosition = _map.LocalToMap(targetPosition);
		TileData wallData = _map.GetCellTileData(Constants.MAP_WALL_LAYER, mapPosition);
		TileData floorPitData = _map.GetCellTileData(Constants.MAP_FLOOR_PIT_LAYER, mapPosition);
		
		if (wallData != null || floorPitData != null)
		{
			cellData.IsNoGoZone = true;
		}

		// Check for Enitity
		IEntity entity = _entities.FirstOrDefault(e => e.GetPosition() == targetPosition);
		if(entity != null)
		{
			cellData.Entity = entity;
		}
		
		return cellData;
	}
}
