/// <summary>
/// Interface for classes to implement to be able to modify a System component on 
/// an actor of some sort, ship, turret, vehicle, etc.
/// </summary>
public interface ISystemModifier
{
    /// <summary>A multiplicative factor to add to the modified stat.</summary>
    float Multiplier { get; }
    /// <summary>A flat amount to add to the modified stat.</summary>
    float Amount { get; }

    /// <summary>
    /// Check if two ISystemModifier are the same.  Not all ISystemModifier 
    /// need to be able to be equal.
    /// </summary>
    /// <param name="other">The other SystemModifier to check.</param>
    /// <returns></returns>
    bool Equals(ISystemModifier other);

    /// TODO Add method to draw the UI part of the modifier?
}


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
    void AddModifier(ISystemModifier modifier);

    /// <summary>
    /// Removes a modifier from the System Component.
    /// </summary>
    /// <param name="modifier">The ISystemModifier to remove.  Uses Equals().</param>
    void RemoveModifier(ISystemModifier modifier);
}

/// <summary>
/// A list that holds a number of ISystemModifiers and has methods for getting total 
/// modifiers and whatnot.
/// </summary>
public class SystemModifierList
{

}
