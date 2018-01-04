/// <summary>
/// An interface that defines what a System Component needs to implement.  Needs to
/// implement the add and remove modifier functions, so the systems stats can be 
/// modified by things like modules or crew.
/// </summary>
public interface ISystem<T>
{
    /// <summary>
    /// Adds a modifier to the System Component.
    /// </summary>
    /// <param name="modifier">The SystemModifier to add.</param>
    void AddModifierTo(ISystemModifier modifier);

    /// <summary>
    /// Removes a modifier from the System Component.
    /// </summary>
    /// <param name="modifier">The ISystemModifier to remove.  Uses Equals().</param>
    void RemoveModifier(ISystemModifier modifier);
}