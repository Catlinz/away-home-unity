
/// <summary>
/// A floating point value that can be modified by adding a value to it and/or 
/// mutiplying it by a modifier.
/// </summary>
[System.Serializable]
public struct ModifiableInt {

    /// <summary>The base value of the int.</summary>
	public int initial;
    /// <summary>A value to add to the base value.</summary>
    public int added;
    /// <summary>A value to multiply the (base + added) value by.</summary>
	public float modifier;

	/// <summary>
    /// The current value of the int, with the modifiers applied.
    /// </summary>
	public int Current { get { return (int)((initial + added) * modifier); } }

    /// <summary>
    /// Construct a new int with the specified initial value.
    /// </summary>
    /// <param name="value">The initial value of the int.</param>
	public ModifiableInt(int value) {
		initial = value;
		modifier = 1.0f;
        added = 0;
	}

    /// <summary>
    /// Redefine equality as whether or not the Current values are equal.
    /// </summary>
    /// <param name="obj">The other object to compare against.</param>
    /// <returns>True if both objects are ModifiableFloats and are Currently equal.</returns>
    public override bool Equals(object obj) {
        if (obj.GetType() != this.GetType()) { return false; }
        else { return Current == ((ModifiableInt)obj).Current;  }
    }

    /// <summary></summary>
    /// <returns>The hash code of the Current value.</returns>
    public override int GetHashCode() {
        return Current.GetHashCode();
    }

    /// <summary>Convert from a integer to a ModifiableInt.</summary>
    /// <param name="value">The integer value to convert from.</param>
    public static implicit operator ModifiableInt(int value) {
		return new ModifiableInt(value);
	}
    /// <summary>Convert from a ModifiableInt to a integer.</summary>
    /// <param name="value">The ModifiableInt to convert to a integer.</param>
	public static implicit operator int(ModifiableInt value) {
		return value.Current;
	}

    /// <summary>Compare Current values of each ModifiableInt.</summary>
	public static bool operator <(ModifiableInt a, ModifiableInt b) {
		return a.Current < b.Current;
	}
    /// <summary>Compare Current values of each ModifiableInt.</summary>
	public static bool operator <=(ModifiableInt a, ModifiableInt b) {
		return a.Current <= b.Current;
	}

    /// <summary>Compare Current values of each ModifiableInt.</summary>
	public static bool operator >(ModifiableInt a, ModifiableInt b) {
		return a.Current > b.Current;
	}
    /// <summary>Compare Current values of each ModifiableInt.</summary>
	public static bool operator >=(ModifiableInt a, ModifiableInt b) {
		return a.Current >= b.Current;
	}

    /// <summary>Compare Current values of each ModifiableInt.</summary>
	public static bool operator ==(ModifiableInt a, ModifiableInt b) {
		return a.Current == b.Current;
	}
    /// <summary>Compare Current values of each ModifiableInt.</summary>
	public static bool operator !=(ModifiableInt a, ModifiableInt b) {
		return a.Current != b.Current;
	}
}
