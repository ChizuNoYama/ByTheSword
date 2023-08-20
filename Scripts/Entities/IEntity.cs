using Godot;

namespace ByTheSword.Scripts.Entities;

public interface IEntity
{
    void ApplyDamage(int damageAmount);
    Vector2 GetPosition();
    int GetArmorClass();
    void Attack(IEntity target);
    bool IsAlive();

}