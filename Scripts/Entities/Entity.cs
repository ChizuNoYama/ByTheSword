using System;
using ByTheSword.Scripts.Controllers;
using Godot;

namespace ByTheSword.Scripts.Entities;

public abstract partial class Entity : CharacterBody2D
{
    protected DungeonSceneController RootDungeonSceneController { get; set; }
    protected Random _diceRoll{ get; set;  }
    public int Health { get; protected set; }
    public int MaxHealth { get; protected set; }
    public virtual bool IsTurnFinished { get; protected set; }
    
    public virtual void ApplyDamage(int amount)
    {
        this.Health -= amount;
    }

    public virtual void HealHealth(int amount)
    {
        this.Health += amount;
    }
    
    public virtual bool IsAlive()
    {
        return this.Health > 0;
    }

    protected virtual void OnRoundEnd()
    {
        this.IsTurnFinished = false;
    }
    
    public abstract bool IsEnemyTo(Entity target);

    public abstract void Attack(Entity target);

    public abstract int GetArmorClass();

    public abstract void ProcessTurn(Entity entity = null);

}