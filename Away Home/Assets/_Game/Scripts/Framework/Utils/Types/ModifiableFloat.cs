
/// <summary>
/// A floating point value that can be modified by adding a value to it and/or 
/// mutiplying it by a modifier.
/// </summary>
[System.Serializable]
public struct ModifiableFloat {

    /// <summary>The base value of the float.</summary>
	public float initial;
    /// <summary>A value to add to the base value.</summary>
    public float added;
    /// <summary>A value to multiply the (base + added) value by.</summary>
	public float modifier;

	/// <summary>
    /// The current value of the float, with the modifiers applied.
    /// </summary>
	public float Current { get { return (initial + added) * modifier; } }

    /// <summary>
    /// Construct a new float with the specified initial value.
    /// </summary>
    /// <param name="value">The initial value of the float.</param>
	public ModifiableFloat(float value) {
		initial = value;
		modifier = 1.0f;
        added = 0.0f;
	}

    /// <summary>
    /// Redefine equality as whether or not the Current values are equal.
    /// </summary>
    /// <param name="obj">The other object to compare against.</param>
    /// <returns>True if both objects are ModifiableFloats and are Currently equal.</returns>
    public override bool Equals(object obj) {
        if (obj.GetType() != this.GetType()) { return false; }
        else { return Current == ((ModifiableFloat)obj).Current;  }
    }

    /// <summary></summary>
    /// <returns>The hash code of the Current value.</returns>
    public override int GetHashCode() {
        return Current.GetHashCode();
    }

    /// <summary>Convert from a float to a ModifiableFloat.</summary>
    /// <param name="value">The float value to convert from.</param>
    public static implicit operator ModifiableFloat(float value) {
		return new ModifiableFloat(value);
	}
    /// <summary>Convert from a ModifiableFloat to a float.</summary>
    /// <param name="value">The ModifiableFloat to convert to a float.</param>
	public static implicit operator float(ModifiableFloat value) {
		return value.Current;
	}

    /// <summary>Compare Current values of each ModifiableFloat.</summary>
	public static bool operator <(ModifiableFloat a, ModifiableFloat b) {
		return a.Current < b.Current;
	}
    /// <summary>Compare Current values of each ModifiableFloat.</summary>
	public static bool operator <=(ModifiableFloat a, ModifiableFloat b) {
		return a.Current <= b.Current;
	}

    /// <summary>Compare Current values of each ModifiableFloat.</summary>
	public static bool operator >(ModifiableFloat a, ModifiableFloat b) {
		return a.Current > b.Current;
	}
    /// <summary>Compare Current values of each ModifiableFloat.</summary>
	public static bool operator >=(ModifiableFloat a, ModifiableFloat b) {
		return a.Current >= b.Current;
	}

    /// <summary>Compare Current values of each ModifiableFloat.</summary>
	public static bool operator ==(ModifiableFloat a, ModifiableFloat b) {
		return a.Current == b.Current;
	}
    /// <summary>Compare Current values of each ModifiableFloat.</summary>
	public static bool operator !=(ModifiableFloat a, ModifiableFloat b) {
		return a.Current != b.Current;
	}
}
