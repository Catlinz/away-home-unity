using System.Collections.Generic;

public enum ModifiableStat
{
    None = 0,

    EnergyCapacity,
    EnergyRecharge,

    ComputerResources,

    Thrust,
    ManeuveringThrust,

    Mass,

    OverclockDamage
}

/// <summary>
/// Base for classes to implement to be able to modify a System component on 
/// an actor of some sort, ship, turret, vehicle, etc.
/// </summary>
public abstract class SystemModifier
{
    /// <summary>A multiplicative factor to add to the modified stat.</summary>
    public float multiplier;
    /// <summary>A flat amount to add to the modified stat.</summary>
    public float delta;
    /// <summary>The value that the modifier is modifying.</summary>
    public ModifiableStat stat;

    /// <summary>
    /// Check if two ISystemModifier are the same.  Not all ISystemModifier 
    /// need to be able to be equal.
    /// </summary>
    /// <param name="other">The other SystemModifier to check.</param>
    /// <returns></returns>
    public abstract bool Equals(SystemModifier other);

    /// TODO Add method to draw the UI part of the modifier?
}


/// <summary>
/// A list that holds a number of ISystemModifiers and has methods for getting total 
/// modifiers and whatnot.
/// </summary>
public class SystemModifierList
{
    // Class used to store the caches for the modifiers.
    private struct ModifierCache
    {
        public ModifiableStat stat;
        public float multiplier;
        public float delta;
        public bool isValid;
    }

    private List<SystemModifier> _modifiers;

    // Cached values based on the list of modifiers.
    private ModifierCache[] _cache;

    public SystemModifierList(int initialCacheSize = 1) {
        _modifiers = new List<SystemModifier>();
        _cache = new ModifierCache[initialCacheSize];
        
        // Initialize the cache.
        for (int i = 0; i < _cache.Length; ++i) {
            _cache[i] = new ModifierCache() {
                stat = ModifiableStat.None,
                isValid = false
            };
        }
    }

    /// <summary>
    /// Adds a new ISystemModifier to the list.
    /// </summary>
    public void Add(SystemModifier modifier) {
        ResetCache(modifier.stat);
        _modifiers.Add(modifier);
    }

    /// <summary>
    /// Get the total multiplier and delta for the specified ModifiableStat.
    /// </summary>
    /// <param name="getFor">The ModifiableStat to get the modifiers for.</param>
    /// <param name="multiplier">The sum of all the multipliers for the stat.</param>
    /// <param name="delta">The sum of all the deltas for the stat.</param>
    public void Get(ModifiableStat getFor, out float multiplier, out float delta) {
        if (_modifiers.Count > 0) {

            int cacheSize = _cache.Length;
            for (int i = 0; i < cacheSize; ++i) {

                if (_cache[i].stat == getFor) {
                    if (!_cache[i].isValid) {
                        CacheValues(getFor, i);
                    }
                    multiplier = _cache[i].multiplier;
                    delta = _cache[i].delta;
                    return;
                }
            }
        }

        // If modifier(s) for stat are not found...
        multiplier = 1f;
        delta = 0f;
    }

    /// <summary>
    /// Get the total value of the sum of all the delta values for all the modifiers
    /// </summary>
    /// <returns></returns>
    public float GetDelta(ModifiableStat getFor) {
        float delta, _;
        Get(getFor, out _, out delta);
        return delta;
    }

    /// <summary>
    /// Get the total value of the sum of all the multipliers for all the modifiers.
    /// </summary>
    public float GetMultiplier(ModifiableStat getFor) {
        float _, multiplier;
        Get(getFor, out multiplier, out _);
        return multiplier;
    }

    /// <summary>
    /// Get the index of a SystemModifier in the list.
    /// </summary>
    /// <returns>The index of the modifier or -1 if not found.</returns>
    public int IndexOf(SystemModifier modifier) {
        int index = 0;
        foreach (SystemModifier mod in _modifiers) {
            if (mod.Equals(modifier)) { return index; }
            ++index;
        }

        return -1;
    }

    /// <summary>
    /// Remove an existing modifier from the list.  Uses the 
    /// <c>Equals()</c> method of the ISystemModifier.
    /// </summary>
    /// <returns>True if a modifier was found and removed.</returns>
    public bool Remove(SystemModifier modifier) {
        if (_modifiers.Count == 0) { return false; }

        int index = IndexOf(modifier);

        if (index != -1) {
            ResetCache(modifier.stat);
            _modifiers.RemoveAt(index);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Either replaces an existing modifier, or adds a new one if it doesn't exist yet.
    /// </summary>
    /// <returns>True if a modifier was replaced, false if a new one was added.</returns>
    public bool Replace(SystemModifier modifier) {
        ResetCache(modifier.stat);

        if (_modifiers.Count > 0) {
            // try and find the modifier to replace.
            int index = IndexOf(modifier);
            if (index != -1) {
                _modifiers[index] = modifier;
                return true;
            }
        }
        
        // Otherwise, just add the modifier.
        _modifiers.Add(modifier);
        return false;
    }

    /// <summary>
    /// Cache the total multiplier and delta values.
    /// </summary>
    private void CacheValues(ModifiableStat cacheFor, int index) {

        float sumMulti = 1f;
        float sumDelta = 0f;

        foreach (SystemModifier mod in _modifiers) {
            if (mod.stat == cacheFor) {
                sumMulti += mod.multiplier;
                sumDelta += mod.delta;
            }
        }

        _cache[index].multiplier = sumMulti;
        _cache[index].delta = sumDelta;
    }

    /// <summary>
    /// Resets the cache for the specified ModifiableStat
    /// </summary>
    private void ResetCache(ModifiableStat resetFor) {
        int cacheSize = _cache.Length;
        for (int i = 0; i < cacheSize; ++i) {
            if (_cache[i].stat == resetFor) {
                _cache[i].isValid = false;
                return;
            }
        }

        // If here, need to create Cache.
        if (_cache[cacheSize - 1].stat != ModifiableStat.None) {
            // Have to add an entry to the array first.
            _cache = AHArray.Added(_cache, new ModifierCache() {
                stat = ModifiableStat.None,
                isValid = false
            });
            cacheSize += 1;
        }

        // Use the next available entry as the cache.
       for (int i = 0; i < cacheSize; ++i) {
            if (_cache[i].stat == ModifiableStat.None) {
                _cache[i].stat = resetFor;
                break;
            }
        }
    }
}
