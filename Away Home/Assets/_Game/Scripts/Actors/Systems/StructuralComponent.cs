using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructuralComponent : MonoBehaviour, ISystem {

	#region PUBLIC_FIELDS
	/// <summary>The mass of the object. (Kilograms)</summary>
	public ModifiableInt mass;

	[Header("Size")]
	/// <summary>The length of the object. (Meters)</summary>
	public int length;

	/// <summary>The width of the object. (Meters)</summary>
	public int width;

	/// <summary>The height of the object. (Meters)</summary>
	public int height;
	#endregion

	#region PRIVATE FIELDS
	/** The list of modifiers applied to the system. */
    private SystemModifierList _modifiers;
    #endregion

	#region INTERFACE METHODS
    public void AddModifier(SystemModifier modifier) {
        _modifiers.Add(modifier);
        RecalculateStat(modifier.stat);
    }

    public void RemoveModifier(SystemModifier modifier) {
        // TODO Fill this in.
        if (_modifiers.Remove(modifier)) {
            RecalculateStat(modifier.stat);
        }
    }

    public void ReplaceModifier(SystemModifier modifier) {
        _modifiers.Replace(modifier);
        RecalculateStat(modifier.stat);
    }
	#endregion
	
	#region UNITY HOOKS
	// Initialize empty SystemModifierList.
    void Awake() {
        _modifiers = new SystemModifierList(1);
    }

    #endregion

	#region PRIVATE METHODS
    ///<summary>
    ///Recalculates the specified stat based on the current modifiers.
    ///</summary>
    private void RecalculateStat(ModifiableStat stat) {
        float multiplier, delta;
        _modifiers.Get(stat, out multiplier, out delta);

        switch (stat) {
			case ModifiableStat.Mass:
				mass.added = (int)delta;
				mass.modifier = multiplier;
				break;
            default:
                break;
        }

        // TODO Implement the OverclockDamage modifier.
    }
    #endregion
}
