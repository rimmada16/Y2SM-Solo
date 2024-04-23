namespace Interfaces
{
    /// <summary>
    /// Interface for objects that can be healed.
    /// </summary>
    public interface IHealable
    {
        void Heal(int healAmount);
    }
}