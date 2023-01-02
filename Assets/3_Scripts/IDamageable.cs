public interface IDamagaeble
{
    bool IsAlive { get; }

    void Damage(int damage);
}