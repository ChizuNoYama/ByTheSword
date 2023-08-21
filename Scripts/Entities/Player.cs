using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Godot;
using ByTheSword.Scripts.Controllers;
using ByTheSword.Scripts.Models;
using ByTheSword.Scripts.Utilities;
using TurnTimer = System.Timers.Timer;
using Vector2 = Godot.Vector2;

namespace ByTheSword.Scripts.Entities;

public partial class Player : Area2D, IEntity
{
	private MovementController _movementController = new MovementController();
	private TurnTimer _turnTimer = new TurnTimer();
	private List<IEntity> _nearbyEnemies = new List<IEntity>();
	private List<Vector2> _nearbyNoGoZone = new List<Vector2>();
	private int _health;
	private bool _isMyTurn;
	private Random _diceRoll;
	private SceneController _rootSceneController;
	
	// TODO: Equipment Manager
	// TODO: Inventory Manager
	// TODO: Spells???
	// TODO: Stats and Attributes
		
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.CollisionLayer = 1;
		this.BodyEntered += this.OnBodyEntered;
		this.BodyExited += this.OnBodyExited;
		
		_rootSceneController =  this.GetOwner<SceneController>();
		if(_rootSceneController == null)
		{
			throw new Exception("Root scene controller not found");
		}
		
		_turnTimer.Interval = Constants.TURN_TIMER_INTERVAL;
		_turnTimer.AutoReset = false;
		_turnTimer.Elapsed += (obj, args) =>_isMyTurn = true;

		_health = 30;
		_diceRoll = new Random();
		_isMyTurn = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//TODO: Create a confirmation window. So when settings are set up we can just show the window wherever
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		if (Input.IsKeyPressed(Key.Escape))
		{
			this.GetTree().Quit();
		}

		if (!_isMyTurn)
		{
			return;
		}
		
		if (Input.IsActionPressed(Constants.ACTION_STEP_RIGHT))
		{
			CellType cellType = _rootSceneController.PeekTile(this.Position + Vector2.Right * Constants.MOVEMENT_STEP);
			switch (cellType)
			{
				case CellType.None:
				case CellType.Item:
					//TODO: have this be global move. or something. figure this shit out
					this.Position = _movementController.StepRight(this.Position);
					break;
				case CellType.Enemy:
					// TODO: Add to Combat since the player is wanting to attack this enemy. Pass the target space to CombatService/Manager
					// IEntity targetEntity = _nearbyEnemies.FirstOrDefault(e => e.GetPosition() - this.Position == new Vector2(Constants.MOVEMENT_STEP, 0));
					// if (targetEntity != null)
					// {
					// 	if (targetEntity.IsAlive())
					// 	{
					// 		if (targetEntity.IsEnemyTo(this))
					// 		{	
					// 			// TODO: Maybe will have a CombatManager to handle who attacks who first in combat
					// 			this.Attack(targetEntity);
					// 			targetEntity.Attack(this);
					// 		}
					// 	}
					// }
					break;
				case CellType.NoGoZone:
					break;
			}

			this.EndMyTurn();
		}

		if (Input.IsActionPressed(Constants.ACTION_STEP_LEFT))
		{
			CellType cellType = _rootSceneController.PeekTile(this.Position + Vector2.Left * Constants.MOVEMENT_STEP);
			switch (cellType)
			{
				case CellType.None:
				case CellType.Item:
					//TODO: have this be global move. or something. figure this shit out
					this.Position = _movementController.StepLeft(this.Position);
					break;
				case CellType.Enemy:
					// TODO: Add to Combat since the player is wanting to attack this enemy. Pass the target space to CombatService/Manager
					break;
				case CellType.NoGoZone:
					break;
			}

			this.EndMyTurn();
		}

		if (Input.IsActionPressed(Constants.ACTION_STEP_UP))
		{
			CellType cellType = _rootSceneController.PeekTile(this.Position + Vector2.Up * Constants.MOVEMENT_STEP);
			switch (cellType)
			{
				case CellType.None:
				case CellType.Item:
					//TODO: have this be global move. or something. figure this shit out
					this.Position = _movementController.StepUp(this.Position);
					break;
				case CellType.Enemy:
					// TODO: Add to Combat since the player is wanting to attack this enemy. Pass the target space to CombatService/Manager
					break;
				case CellType.NoGoZone:
					break;
			}

			this.EndMyTurn();
		}

		if (Input.IsActionPressed(Constants.ACTION_STEP_DOWN))
		{
			CellType cellType = _rootSceneController.PeekTile(this.Position + Vector2.Down * Constants.MOVEMENT_STEP);
			switch (cellType)
			{
				case CellType.None:
				case CellType.Item:
					//TODO: have this be global move. or something. figure this shit out
					this.Position = _movementController.StepDown(this.Position);
					break;
				case CellType.Enemy:
					// TODO: Add to Combat since the player is wanting to attack this enemy. Pass the target space to CombatService/Manager
					break;
				case CellType.NoGoZone:
					break;
			}
			
			

			this.EndMyTurn();
		}
	}

	private void OnBodyEntered(Node2D node)
	{
		if (node is PhysicsBody2D body)
		{
			var what = this.Position.Normalized();
			if (body is IEntity entity)
			{
				if(entity.IsEnemyTo(this) && entity.IsAlive())
				{
					_nearbyEnemies.Add(entity);
				}
			}
		}

		// if (node is TileMap map)
		// {
			// Get the direction of the character and this is where the cell is.
			// Vector2I cellPosition = map.GetNeighborCell(new Vector2I((int)this.Position.X, (int)this.Position.Y), TileSet.CellNeighbor.LeftSide);
			// var what = this.Position.Normalized();
			// if (map.TileSet.GetPhysicsLayerCollisionLayer(0) == Utility.GetCollisionValueFromIndices(Constants.WALLS_COLLISION_LAYER))
			// {
			// 	
			// }
			
			// Console.WriteLine("No go zone nearby");
		// }
	}

	public void OnBodyExited(Node2D node)
	{
		
	}

	private void EndMyTurn()
	{
		_turnTimer.Start();
		_isMyTurn = false;
	}

	public void ApplyDamage(int damageAmount)
	{
		_health -= damageAmount;

		if (_health <= 0)
		{
			Console.WriteLine("I dead");
			this.GetTree().Quit();
		}
	}

	public Vector2 GetPosition()
	{
		return this.Position;
	}

	public void Attack(IEntity target)
	{
		int attackRoll = _diceRoll.Next(1, 21); // TODO: Add modifiers when it is set up
		Console.WriteLine($"I rolled {attackRoll} against their {target.GetArmorClass()}"); // TODO: Emit this to a message via a service in the parent
		if (target.GetArmorClass() < attackRoll)
		{
			int damageAmount = 5; // TODO: This will come from weapon from EquipmentManager
			target.ApplyDamage(damageAmount);
			
			Console.WriteLine($"I dealt {damageAmount}");
			if (!target.IsAlive())
			{
				_nearbyEnemies.Remove(target);
			}
		}
		
		Console.WriteLine();
	}

	public bool IsAlive()
	{
		return _health > 0;
	}

	public int GetArmorClass()
	{
		//TODO: this will come from amor from EquipmentManager
		int armorClass = 10;
		return armorClass;
	}

	public bool IsEnemyTo(IEntity target)
	{
		return false;
	}
}
