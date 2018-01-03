using UnityEngine;

/// <summary>
/// Interface for classes to implement to be able to modify a System component on 
/// an actor of some sort, ship, turret, vehicle, etc.
/// </summary>
public interface ISystemModifier
{
    /// <summary>The name of the modifier, for UI display.</summary>
    string Name { get; }
    /// <summary>A multiplicative factor to add to the modified stat.</summary>
    float Multiplier { get; }
    /// <summary>A flat amount to add to the modified stat.</summary>
    float Delta { get; }

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
/// A list that holds a number of ISystemModifiers and has methods for getting total 
/// modifiers and whatnot.
/// </summary>
[System.Serializable]
public class SystemModifierList
{
    [SerializeField]
    private ISystemModifier[] _modifiers;

    // Cached values based on the list of modifiers.
    private float _cachedMultiplier;
    private float _cachedDelta;

    public SystemModifierList() {
        _modifiers = null;
        _cachedMultiplier = float.NaN;
        _cachedDelta = float.NaN;
    }

    /// <summary>
    /// Adds a new ISystemModifier to the list.
    /// </summary>
    public void Add(ISystemModifier modifier) {
        _cachedMultiplier = float.NaN;
        _cachedDelta = float.NaN;
        _modifiers = AHArray.Added(_modifiers, modifier);
    }

    /// <summary>
    /// Get the total value of the sum of all the delta values for all the modifiers
    /// </summary>
    /// <returns></returns>
    public float GetDelta() {
        if (float.IsNaN(_cachedDelta)) {
            CacheValues();
        }

        return _cachedDelta;
    }

    /// <summary>
    /// Get the total value of the sum of all the multipliers for all the modifiers.
    /// </summary>
    public float GetMultiplier() {
        if (float.IsNaN(_cachedMultiplier)) {
            CacheValues();
        }

        return _cachedMultiplier;
    }

    /// <summary>
    /// Remove an existing modifier from the list.  Uses the 
    /// <c>Equals()</c> method of the ISystemModifier.
    /// </summary>
    /// <returns>True if a modifier was found and removed.</returns>
    bool Remove(ISystemModifier modifier) {
        if (_modifiers == null) { return false; }

        int numMod = _modifiers.Length;
        for (int i = 0; i < numMod; ++i) {
            if (_modifiers[i].Equals(modifier)) {
                _cachedDelta = float.NaN;
                _cachedMultiplier = float.NaN;
                _modifiers = AHArray.Removed(_modifiers, i);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Cache the total multiplier and delta values.
    /// </summary>
    private void CacheValues() {

        float sumMulti = 0f;
        float sumDelta = 0f;

        int numMod = (_modifiers != null) ? _modifiers.Length : 0;
        for (int i = 0; i < numMod; ++i) {
            sumMulti += _modifiers[i].Multiplier;
            sumDelta += _modifiers[i].Delta;
        }

        _cachedMultiplier = sumMulti;
        _cachedDelta = sumDelta;
    }
}
