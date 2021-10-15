//Primary Author : Maximiliam Rosén - maka4519

namespace Combat.Interfaces
{
    /// <summary>
    ///     A Interface to determined what is damageable
    /// </summary>
    public interface IDamageable
    {
        void TakeDamage(float damage);
        EntityBase GetEntity();
    }
}