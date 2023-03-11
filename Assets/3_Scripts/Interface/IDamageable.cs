public interface IDamagable
{
    bool IsAlive { get; }

    void Damage(int damage);
}