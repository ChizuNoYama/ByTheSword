using System;
using System.Diagnostics;
using Godot;
using ByTheSword.Scripts.Controllers;
using ByTheSword.Scripts.Models;
using ByTheSword.Scripts.Utilities;
using TurnTimer = System.Timers.Timer;
using Vector2 = Godot.Vector2;

namespace ByTheSword.Scripts.Entities;

public partial class Player : Entity
{
	// private MovementController _movementController = new MovementController();
	private TurnTimer _turnTimer = new TurnTimer();
	private bool _timerStopped;
	
	// TODO: Equipment Manager
	// TODO: Inventory Manager
	// TODO: Spells???
	// TODO: Stats and Attributes
		
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.RootDungeonSceneController =  this.GetOwner<DungeonSceneController>() ?? this.GetTree().CurrentScene as DungeonSceneController;
		if(this.RootDungeonSceneController == null)
		{
			this.RootDungeonSceneController = this.GetTree().CurrentScene as DungeonSceneController;
			if (this.RootDungeonSceneController != null)
			{
				this.Owner = this.RootDungeonSceneController;
			}
			else
			{
				throw new Exception("Root scene cannot be found");
			}
		}
		
		RootDungeonSceneController.OnRoundEnded += () =>
		{
			this.IsTurnFinished = false;
		};
		RootDungeonSceneController?.RegisterEntity(this);
		
		_turnTimer.Interval = Constants.PLAYER_TURN_TIMER_INTERVAL;
		_turnTimer.AutoReset = false;
		_turnTimer.Elapsed += (obj, args) =>_timerStopped = true;
		_timerStopped = true;

		_diceRoll = new Random();
		this.IsTurnFinished = false;
		
		this.Health= 30;
		this.MaxHealth = 30;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		if (Input.IsKeyPressed(Key.Escape))
		{
			//TODO: Create a confirmation window. So when settings are set up we can just show the window wherever
			this.GetTree().Quit();
		}

		if (!_timerStopped || this.IsTurnFinished)
		{
			return;
		}
		
		if (Input.IsActionPressed(Constants.ACTION_STEP_RIGHT))
		{
			Vector2 targetPosition = this.Position + Vector2.Right * Constants.MOVEMENT_STEP;
			CellData cellData = RootDungeonSceneController.PeekTargetCell(targetPosition);
			this.PerformActionOnCell(cellData);
			this.EndMyTurn();
		}

		if (Input.IsActionPressed(Constants.ACTION_STEP_LEFT))
		{
			Vector2 targetPosition = this.Position + Vector2.Left * Constants.MOVEMENT_STEP;
			CellData cellData = RootDungeonSceneController.PeekTargetCell(targetPosition);
			this.PerformActionOnCell(cellData);
			this.EndMyTurn();
		}

		if (Input.IsActionPressed(Constants.ACTION_STEP_UP))
		{
			Vector2 targetPosition = this.Position + Vector2.Up * Constants.MOVEMENT_STEP;
			CellData cellData = RootDungeonSceneController.PeekTargetCell(targetPosition);
			this.PerformActionOnCell(cellData);
			this.EndMyTurn();
		}

		if (Input.IsActionPressed(Constants.ACTION_STEP_DOWN))
		{
			Vector2 targetPosition = this.Position + Vector2.Down * Constants.MOVEMENT_STEP;
			CellData cellData = RootDungeonSceneController.PeekTargetCell(targetPosition);
			this.PerformActionOnCell(cellData);
			this.EndMyTurn();
		}

		if (Input.IsActionPressed(Constants.ACTION_SKIP_TURN))
		{
			this.EndMyTurn();
		}
	}

	private void PerformActionOnCell(CellData cellData)
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
				this.Position = cellData.GlobalPosition;
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
		this.IsTurnFinished = true;
		
		this.RootDungeonSceneController.ProcessRound();
	}

	public override void ApplyDamageToSelf(int damageAmount)
	{
		base.ApplyDamageToSelf(damageAmount);

		Console.ForegroundColor = ConsoleColor.Red;
		Console.WriteLine($"Current health: {this.Health}");
		Console.ResetColor();
		if (this.Health <= 0)
		{
			Console.WriteLine("I died.");
			this.GetTree().Quit();
		}
	}

	public override void Attack(Entity target)
	{
		int attackRoll = _diceRoll.Next(1, 21); // TODO: Add modifiers when it is set up
		Console.WriteLine($"I rolled {attackRoll} against their {target.GetArmorClass()}"); // TODO: Emit this to a message via a service in the SceneController
		if (target.GetArmorClass() < attackRoll)
		{
			int damageAmount = 5; // TODO: This will come from weapon from EquipmentManager
			target.ApplyDamageToSelf(damageAmount);
			
			Console.WriteLine($"I dealt {damageAmount}");
			if (!target.IsAlive())
			{
				Console.WriteLine($"I killed {nameof(target)}");
			}
		}
		
		Console.WriteLine();
	}

	public override bool IsEnemyTo(Entity target)
	{
		return true; //TODO: Once Factions are in place, this will check Faction relation
	}

	public override int GetArmorClass()
	{
		return 10;
	}

	public override void ProcessTurn(Entity target = null)
	{
		// Maybe I will implement a way to make the player auto explore like in other roguelikes.
		// For now. Do nothing since this is the player
	}
}
