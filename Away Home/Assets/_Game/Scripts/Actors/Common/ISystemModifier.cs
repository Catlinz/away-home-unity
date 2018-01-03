using System.Collections.Generic;

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

[System.Serializable]
public struct SerializableSystemModifier
{
    public string type;
    public string name;
    public float delta;
    public float multiplier;

    public static SerializableSystemModifier From(ISystemModifier modifier) {
        string modType = modifier.GetType().ToString();
        return new SerializableSystemModifier() {
            type = modType,
            name = modifier.Name,
            delta = modifier.Delta,
            multiplier = modifier.Multiplier
        };
    }

    public ISystemModifier Get() {
        System.Type modType = System.Type.GetType(type);
        return (ISystemModifier)System.Activator.CreateInstance(modType, new System.Object[] { delta, multiplier, name });
    }
}


/// <summary>
/// A list that holds a number of ISystemModifiers and has methods for getting total 
/// modifiers and whatnot.
/// </summary>
[System.Serializable]
public class SystemModifierList
{
    private List<ISystemModifier> _modifiers;

    // Cached values based on the list of modifiers.
    private float _cachedMultiplier;
    private float _cachedDelta;

    public SerializableSystemModifier[] serializedList;

    public SystemModifierList() {
        _modifiers = new List<ISystemModifier>();
        _cachedMultiplier = float.NaN;
        _cachedDelta = float.NaN;
    }

    /// <summary>
    /// Adds a new ISystemModifier to the list.
    /// </summary>
    public void Add(ISystemModifier modifier) {
        _cachedMultiplier = float.NaN;
        _cachedDelta = float.NaN;
        _modifiers.Add(modifier);
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

    public void OnAfterDeserialize() {
        // Unity has just written new data into the seralizedList field.
        _modifiers.Clear();
        for (int i = 0; i < serializedList.Length; ++i) {
            System.Type modType = System.Type.GetType(serializedList[i].type);
            _modifiers.Add(serializedList[i].Get());
        }
        serializedList = null;
    }

    // Used to make the class serializeable.
    public void OnBeforeSerialize() {
        // Unity is about to read the serializedList's field's contents.
        serializedList = new SerializableSystemModifier[_modifiers.Count];
        int index = 0;
        foreach (ISystemModifier mod in _modifiers) {
            serializedList[index] = SerializableSystemModifier.From(_modifiers[index]);
            ++index;
        }
    }

    /// <summary>
    /// Remove an existing modifier from the list.  Uses the 
    /// <c>Equals()</c> method of the ISystemModifier.
    /// </summary>
    /// <returns>True if a modifier was found and removed.</returns>
    bool Remove(ISystemModifier modifier) {
        if (_modifiers.Count == 0) { return false; }

        int index = 0;
        foreach(ISystemModifier mod in _modifiers) {
            if (mod.Equals(modifier)) { break; }
            ++index;
        }

        if (index < _modifiers.Count) {
            _modifiers.RemoveAt(index);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Cache the total multiplier and delta values.
    /// </summary>
    private void CacheValues() {

        float sumMulti = 0f;
        float sumDelta = 0f;

        foreach (ISystemModifier mod in _modifiers) {
            sumMulti += mod.Multiplier;
            sumDelta += mod.Delta;
        }

        _cachedMultiplier = sumMulti;
        _cachedDelta = sumDelta;
    }
}
