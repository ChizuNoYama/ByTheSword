using System;
using ByTheSword.Scripts.Controllers;
using Godot;

namespace ByTheSword.Scripts.Entities;

public partial class Enemy : CharacterBody2D, IEntity
{
    private int _health;
    private Random _diceRoll;
    private SceneController _rootSceneController;

    public override void _Ready()
    {
        base._Ready();
        
        _rootSceneController =  this.GetOwner<SceneController>();
        if(_rootSceneController == null)
        {
            _rootSceneController = this.GetTree().CurrentScene as SceneController;
            if (_rootSceneController != null)
            {
                this.Owner = _rootSceneController;
            }
            else
            {
                throw new Exception("Root scene cannot be found");
            }
        }
		
        _rootSceneController?.RegisterEntity(this);

        _health = 30;
        _diceRoll = new Random();
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        
        // Do nothing for now
    }

    public void ApplyDamage(int damageAmount)
    {
        _health -= damageAmount;

        if (_health <= 0)
        {
            _rootSceneController.DeregisterEntity(this);
            this.Free();
        }
    }

    public Vector2 GetPosition()
    {
        return this.Position;
    }

    public int GetArmorClass()
    {
        //TODO: Will come from natural or Actual Armor if wearing any
        int armorClass = 7;

        return armorClass;
    }

    public void Attack(IEntity target)
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

    public bool IsAlive()
    {
        return _health > 0;
    }

    public bool IsEnemyTo(IEntity target)
    {
        return true;
    }
}