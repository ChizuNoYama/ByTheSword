using System;
using System.Diagnostics;
using ByTheSword.Scripts.Controllers;
using ByTheSword.Scripts.Models;
using ByTheSword.Scripts.Utilities;
using Godot;

namespace ByTheSword.Scripts.Entities;

public partial class Enemy : Entity
{
    private Vector2 _targetLocation;
    private Vector2[] _navigationPath;
    private int _currentNavPathIndex;
    

    public override void _Ready()
    {
        base._Ready();
        
        this.RootDungeonSceneController =  this.GetOwner<DungeonSceneController>();
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
        this.RootDungeonSceneController?.RegisterEntity(this);
        
        _diceRoll = new Random();
        _currentNavPathIndex = 0;

        this.Health = 30;
        this.MaxHealth = 30;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        
        // Do nothing for now
    }

    public override void ApplyDamage(int damageAmount)
    {
        base.ApplyDamage(damageAmount);

        if (this.Health <= 0)
        {
            this.RootDungeonSceneController.DeregisterEntity(this);
            this.Free();
        }
    }

    public override int GetArmorClass()
    {
        //TODO: Will come from natural or Actual Armor if wearing any
        int armorClass = 7;

        return armorClass;
    }

    public override void Attack(Entity target)
    {
        int attackRoll = _diceRoll.Next(1, 21); // TODO: Add modifiers
        Console.WriteLine($"Enemy rolled {attackRoll} against my {target.GetArmorClass()}");
        if (target.GetArmorClass() < attackRoll)
        {
            // TODO: This will come from weapon from EquipmentManager.
            int damageAmount = 5;
            target.ApplyDamage(damageAmount);
            Console.WriteLine($"Enemy dealt {damageAmount}");
        }
        Console.WriteLine();
    }

    public override void ProcessTurn(Entity targetEntity = null)
    {
        // Entity entity _rootSceneController.GetEntity() // change this to
        // TODO: 1. Do I have a target? (Forced getting the Player as a target. Will change this to get companions or other factions as well.
        // TODO: 1a. Move closer to my target, or move aimlessly, or come up with a patrol route for them if needed.
        // TODO: 2. Peek target tile and compute What my next move will be

        if (targetEntity != null)
        {
            if (_targetLocation != targetEntity.Position || _navigationPath.Length != 0) // Have a buffer so that location is not calculated repeatedly.
            {
                // Recalculate path
                _targetLocation = targetEntity.Position;

                Vector2I myMapPosition = this.RootDungeonSceneController.GetMapPosition(this.Position);
                Vector2I targetMapPosition = this.RootDungeonSceneController.GetMapPosition(targetEntity.Position);
                
                _navigationPath = this.RootDungeonSceneController.GridNav.GetPointPath(myMapPosition, targetMapPosition);
                _currentNavPathIndex = 0;
            }

            if (_navigationPath.Length == 0)
            {
                Debug.WriteLine($"No path to target");
            }
            else
            {
                if (_currentNavPathIndex != _navigationPath.Length - 1)
                {
                    _currentNavPathIndex++;
                }
                
                Vector2 targetCell = _navigationPath[_currentNavPathIndex];

                CellData cellData = this.RootDungeonSceneController.PeekTargetCell(targetCell);
                this.PerformActionOnCell(cellData);
                
            }
            // Move towards target
        }
        
        this.IsTurnFinished = true;
    }

    private void PerformActionOnCell(CellData cellData)
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

    public override bool IsEnemyTo(Entity target)
    {
        return true;
    }
}