using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using ByTheSword.Scripts.Controllers;
using ByTheSword.Scripts.Utilities;
using TurnTimer = System.Timers.Timer;

namespace ByTheSword.Scripts.Entities;

public partial class Player : Area2D, IEntity
{
	private MovementController _movementController = new MovementController();
	private TurnTimer _turnTimer = new TurnTimer();
	private List<IEntity> _nearbyEntities = new List<IEntity>();
	private int _health;
	private bool _isMyTurn;
	private Random _diceRoll;
	
	// TODO: Equipment Manager
	// TODO: Inventory Manager
	// TODO: Spells???
	// TODO: Stats and Attributes
		
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.CollisionLayer = 1;
		this.BodyEntered += this.OnBodyEntered;
		
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
			IEntity targetEntity = _nearbyEntities.FirstOrDefault(e => e.GetPosition().Y == this.Position.Y
																	   && e.GetPosition().X == this.Position.X + Constants.MOVEMENT_STEP);
			if (targetEntity != null)
			{
				this.Attack(targetEntity);
				targetEntity.Attack(this);
			}
			else
			{
				this.Position = _movementController.StepRight(this.Position);
			}
			this.EndMyTurn();
		}

		if (Input.IsActionPressed(Constants.ACTION_STEP_LEFT))
		{
			this.Position = _movementController.StepLeft(this.Position);
			this.EndMyTurn();
		}

		if (Input.IsActionPressed(Constants.ACTION_STEP_UP))
		{
			this.Position=  _movementController.StepUp(this.Position);
			this.EndMyTurn();
		}

		if (Input.IsActionPressed(Constants.ACTION_STEP_DOWN))
		{
			this.Position = _movementController.StepDown(this.Position);
			this.EndMyTurn();
		}
	}

	private void OnBodyEntered(Node2D node)
	{
		this.TrackNearbyEnemy(node);
	}

	private void TrackNearbyEnemy(Node2D node)
	{
		// TODO: Testing checking if enemy by from IEntity
		// if (node is IEntity)
		// {
		// 	(IEntity)node.IsEnemy();
		// }
		
		Variant isEnemy = node.GetMeta(Constants.IS_ENEMY_META_IDENTIFIER);
		if (isEnemy.AsBool())
		{
			if (node is IEntity)
			{
				IEntity enemyEntity = (IEntity) node;
				_nearbyEntities.Add(enemyEntity);
			}
		}
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
		int attackRoll = _diceRoll.Next(1, 21); // TODO: Add modifiers
		Console.WriteLine($"I rolled {attackRoll} against their {target.GetArmorClass()}");
		if (target.GetArmorClass() < attackRoll)
		{
			// TODO: This will come from weapon from EquipmentManager.
			int damageAmount = 5;
			target.ApplyDamage(damageAmount);
			Console.WriteLine($"I dealt {damageAmount}");
			if (!target.IsAlive())
			{
				_nearbyEntities.Remove(target);
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
}
