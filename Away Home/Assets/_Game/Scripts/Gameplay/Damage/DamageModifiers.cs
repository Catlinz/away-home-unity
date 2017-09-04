using UnityEngine;

[System.Serializable]
public class DamageModifiers {

	/// <summary>The resistance to explosive damage.</summary>
	public ModifiableFloat explosive;
	/// <summary>The resistance to kinetic (physical) damage.</summary>
	public ModifiableFloat kinetic;
	/// <summary>The resistance to plasma energy damage.</summary>
	public ModifiableFloat plasma;
	/// <summary>The resistance to thermal energy damage.</summary>
	public ModifiableFloat thermal;

	/// <summary>Default constructor, initializes all values to 1.0f</summary>
	public DamageModifiers() {
		explosive = 1.0f;
		kinetic = 1.0f;
		plasma = 1.0f;
		thermal = 1.0f;
	}
}
