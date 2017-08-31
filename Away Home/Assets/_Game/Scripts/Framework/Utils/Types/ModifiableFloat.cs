
/**
 * A floating point number that can be modified by a modifier value.
 */
[System.Serializable]
public struct ModifiableFloat {

	public float initial; // The unmodified value of the float.
    public float added; // A flat amount to add to the initial value.
	public float modifier; // The modifier (1 = not modified).

	// Get the current value of the float with the modifier.
	public float Current { get { return (initial + added) * modifier; } }

	public ModifiableFloat(float value) {
		initial = value;
		modifier = 1.0f;
        added = 0.0f;
	}

    public override bool Equals(object obj) {
        if (obj.GetType() != this.GetType()) { return false; }
        else { return Current == ((ModifiableFloat)obj).Current;  }
    }

    public override int GetHashCode() {
        return Current.GetHashCode();
    }

    public static implicit operator ModifiableFloat(float value) {
		return new ModifiableFloat(value);
	}

	public static implicit operator float(ModifiableFloat value) {
		return value.Current;
	}

	public static bool operator <(ModifiableFloat a, ModifiableFloat b) {
		return a.Current < b.Current;
	}
	public static bool operator <=(ModifiableFloat a, ModifiableFloat b) {
		return a.Current <= b.Current;
	}

	public static bool operator >(ModifiableFloat a, ModifiableFloat b) {
		return a.Current > b.Current;
	}
	public static bool operator >=(ModifiableFloat a, ModifiableFloat b) {
		return a.Current >= b.Current;
	}

	public static bool operator ==(ModifiableFloat a, ModifiableFloat b) {
		return a.Current == b.Current;
	}
	public static bool operator !=(ModifiableFloat a, ModifiableFloat b) {
		return a.Current != b.Current;
	}
}
