using System.Collections.Generic;
using System.Linq;
using ByTheSword.Scripts.Entities;
using ByTheSword.Scripts.Models;
using Godot;
using Godot.Collections;
using Constants = ByTheSword.Scripts.Utilities.Constants;

namespace ByTheSword.Scripts.Controllers;

public partial class DungeonSceneController : Node2D
{
	
	[Signal]
	public delegate void OnRoundEndedEventHandler();
	private TileMap _map;
	// private CombatController _combatController;
	private List<Entity> _entities;
	public AStarGrid2D GridNav { get; private set; }

	public override void _EnterTree()
	{
		base._EnterTree();
		_entities = new List<Entity>();
	}

	public override void _Ready()
	{
		base._Ready();
		_map = this.GetNode<TileMap>("TileMap");
		// _combatController = new CombatController();
		
		// Go through each tile in the TileMap and mark no go zones for the GridNav.
		// While this is happening, have a loading screen so that everything is already done and ready for path computation.
		this.InitializeNavigation();
	}

	private void InitializeNavigation()
	{
		GD.Print("Initializing cells");

		Array<Vector2I> wallCellsArray = _map.GetUsedCells(Constants.MAP_WALL_LAYER);
		HashSet<Vector2I> excludeCellsHash = (wallCellsArray + _map.GetUsedCells(Constants.MAP_FLOOR_PIT_LAYER)).ToHashSet();

		wallCellsArray.Sort();
		// mapCells.AddRange(_map.GetUsedCells(Constants.MAP_FLOOR_PIT_LAYER));
		Vector2I firstMapCell = wallCellsArray[0];
		Vector2I lastWallCell = wallCellsArray[^1];
		
		this.GridNav = new AStarGrid2D();
		this.GridNav.CellSize = new Vector2(32, 32);
		this.GridNav.Region = new Rect2I(firstMapCell, lastWallCell);
		this.GridNav.DefaultComputeHeuristic = AStarGrid2D.Heuristic.Manhattan;
		this.GridNav.DiagonalMode = AStarGrid2D.DiagonalModeEnum.Never;
		this.GridNav.Update();
		foreach (Vector2I cell in excludeCellsHash)
		{
			// GD.Print($"({cell.X},{cell.Y}): {this.GridNav.IsInBoundsv(cell)}");
			if (this.GridNav.IsInBoundsv(cell))
			{
				this.GridNav.SetPointSolid(cell);
			}
			// this.GridNav.SetPointSolid(cell);
		}
	}

	public void RegisterEntity(Entity entity)
	{
		_entities.Add(entity);
	}

	public void DeregisterEntity(Entity entity)
	{
		_entities.Remove(entity);
	}

	public CellData PeekTargetCell(Vector2 targetPosition)
	{
		CellData cellData = new CellData();
		
		// Check for walls and pits;
		var mapPosition = _map.LocalToMap(targetPosition);
		TileData wallData = _map.GetCellTileData(Constants.MAP_WALL_LAYER, mapPosition);
		TileData floorPitData = _map.GetCellTileData(Constants.MAP_FLOOR_PIT_LAYER, mapPosition);
		
		if (wallData != null || floorPitData != null)
		{
			cellData.IsNoGoZone = true;
		}

		// Check for Enitity
		Entity entity = _entities.FirstOrDefault(e => e.Position == targetPosition);
		if(entity != null)
		{
			cellData.Entity = entity;
		}

		cellData.GlobalPosition = targetPosition;
		
		return cellData;
	}

	public void ProcessRound()
	{
		Entity playerEntity = null;
		List<Entity> entitiesTurnPending = _entities.Where(x =>
		{
			if (x is Player)
			{
				playerEntity = x;
			}
			return !x.IsTurnFinished && !(x is Player);
		}).ToList();

		foreach (var entity in entitiesTurnPending)
		{
			if (playerEntity != null)
			{
				if(entity.IsEnemyTo(playerEntity))
					entity.ProcessTurn(playerEntity);
			}
			else
				entity.ProcessTurn();
		}

		this.EmitSignal(SignalName.OnRoundEnded);
	}

	public Vector2I GetMapPosition(Vector2 globalPosition)
	{
		Vector2 localPosition = _map.ToLocal(globalPosition);
		return _map.LocalToMap(localPosition);
	}
}
