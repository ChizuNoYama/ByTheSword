using System;
using ByTheSword.Scripts.Controllers;
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

    public override void ProcessTurn(Entity target = null) //TODO: Find out who should call this method. Maybe the Scene should since it will take care of turns between all the NPCs in the scene
    {
        // Entity entity _rootSceneController.GetEntity() // change this to
        // TODO: 1. Do I have a target? (Forced getting the Player as a target. Will change this to get companions or other factions as well.
        // TODO: 1a. Move closer to my target, or move aimlessly, or come up with a patrol route for them if needed.
        // TODO: 2. Peek target tile and compute What my next move will be

        if (target != null)
        {
            if (_targetLocation != target.Position || _navigationPath.Length != 0) // Have a buffer so that location is not calculated repeatedly.
            {
                // Recalculate path
                _targetLocation = target.Position;
                _navigationPath = this.RootDungeonSceneController.GridNav.GetPointPath(this.Position.ToVector2I(), target.Position.ToVector2I());
                _currentNavPathIndex = 0;
            }
            
            // Move towards target
            this.Position = _navigationPath[_currentNavPathIndex];
            _currentNavPathIndex++;
        }
        
        this.TurnFinished = true;
    }

    public override bool IsEnemyTo(Entity target)
    {
        return true;
    }
}