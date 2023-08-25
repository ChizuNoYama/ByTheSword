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
	private int _health;
	private bool _isMyTurn;
	private bool _timerStopped;
	private Random _diceRoll;
	private SceneController _rootSceneController;
	
	// TODO: Equipment Manager
	// TODO: Inventory Manager
	// TODO: Spells???
	// TODO: Stats and Attributes
		
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_rootSceneController =  this.GetOwner<SceneController>();
		if(_rootSceneController == null)
		{
			_rootSceneController = this.GetTree().CurrentScene as SceneController;
		}
		
		_rootSceneController?.RegisterEntity(this);
		
		this.BodyEntered += this.OnBodyEntered;
		this.BodyExited += this.OnBodyExited;
		
		
		_turnTimer.Interval = Constants.TURN_TIMER_INTERVAL;
		_turnTimer.AutoReset = false;
		_turnTimer.Elapsed += (obj, args) =>_timerStopped = true;
		_timerStopped = true;

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

		if (!_isMyTurn && !_timerStopped)
		{
			return;
		}
        
		if (Input.IsActionPressed(Constants.ACTION_STEP_RIGHT))
		{
			Vector2 targetPosition = this.Position + Vector2.Right * Constants.MOVEMENT_STEP;
			CellData cellData = _rootSceneController.PeekTargetCell(targetPosition);
			this.PerformActionOnCell(cellData, targetPosition);
			this.EndMyTurn();
		}

		if (Input.IsActionPressed(Constants.ACTION_STEP_LEFT))
		{
			Vector2 targetPosition = this.Position + Vector2.Left * Constants.MOVEMENT_STEP;
			CellData cellData = _rootSceneController.PeekTargetCell(targetPosition);
			this.PerformActionOnCell(cellData, targetPosition);
			this.EndMyTurn();
		}

		if (Input.IsActionPressed(Constants.ACTION_STEP_UP))
		{
			Vector2 targetPosition = this.Position + Vector2.Up * Constants.MOVEMENT_STEP;
			CellData cellData = _rootSceneController.PeekTargetCell(targetPosition);
			this.PerformActionOnCell(cellData, targetPosition);
			this.EndMyTurn();
		}

		if (Input.IsActionPressed(Constants.ACTION_STEP_DOWN))
		{
			Vector2 targetPosition = this.Position + Vector2.Down * Constants.MOVEMENT_STEP;
			CellData cellData = _rootSceneController.PeekTargetCell(targetPosition);
			this.PerformActionOnCell(cellData, targetPosition);
			this.EndMyTurn();
		}
	}

	private void PerformActionOnCell(CellData cellData, Vector2 targetPosition)
	{
		if (_timerStopped)
		{
			if (cellData.IsNoGoZone)
			{
				// Do nothing. Waste a turn
				return;
			}

			if (cellData.Entity != null)
			{
				if (cellData.Entity.IsEnemyTo(this))
				{
					this.Attack(cellData.Entity);
				}
				
				//TODO: other possible actions like talk or whatever.
				
			}
			else
			{
				// Move to target cell position
				this.Position = targetPosition;
			}
		}
	}
	

	private void OnBodyEntered(Node2D node)
	{
		
	}

	private void OnBodyExited(Node2D node)
	{
		
	}

	private void EndMyTurn()
	{
		_turnTimer.Start();
		_timerStopped = false;
	}

	public void ApplyDamage(int damageAmount)
	{
		_health -= damageAmount;

		if (_health <= 0)
		{
			Console.WriteLine("I died.");
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
				Console.WriteLine($"I killed {nameof(target)}");
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
