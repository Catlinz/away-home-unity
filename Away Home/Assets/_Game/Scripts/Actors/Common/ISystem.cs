/// <summary>
/// Delegate to listen for when the a system receives damage.
/// </summary>
/// <param name="percentDamage">The percentage of damage the system has currently [0-1].</param>
public delegate void SystemDamaged(float percentDamage);

/// <summary>
/// An interface that defines what a System Component needs to implement.  Needs to
/// implement the add and remove modifier functions, so the systems stats can be 
/// modified by things like modules or crew.
/// </summary>
public interface ISystem
{
    /// <summary>
    /// Adds a modifier to the System Component.
    /// </summary>
    /// <param name="modifier">The SystemModifier to add.</param>
    void AddModifier(SystemModifier modifier);

    /// <summary>
    /// Removes a modifier from the System Component.
    /// </summary>
    /// <param name="modifier">The ISystemModifier to remove.  Uses Equals().</param>
    void RemoveModifier(SystemModifier modifier);

    /// <summary>
    /// Replaces an existing modifier with a new one, or adds a new one if it doesn't exist yet.
    /// </summary>
    /// <param name="modifier">The SystemModifier to add or replace.</param>
    void ReplaceModifier(SystemModifier modifier);
}