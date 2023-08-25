using Godot;

namespace ByTheSword.Scripts.Entities;

public interface IEntity //TODO: Test changing this to an abstract class. Expanding with interface may break DRY rule
{
    void ApplyDamage(int damageAmount);
    Vector2 GetPosition();
    int GetArmorClass();
    void Attack(IEntity target);
    bool IsAlive();
    bool IsEnemyTo(IEntity target);

}