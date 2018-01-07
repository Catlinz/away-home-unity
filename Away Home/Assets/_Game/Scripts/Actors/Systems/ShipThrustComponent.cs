using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StructuralComponent))]
public class ShipThrustComponent : MonoBehaviour, ISystem {

    #region PUBLIC FIELDS
    [Header("Thrust")]
	/// <summary>The amount of thrust produced by the engine (Newtons).</summary>
	public ModifiableFloat thrust;

	/// <summary>The amount of thrust provided to turn or slide the ship (Newtons).</summary>
	public ModifiableFloat maneuveringThrust;
    #endregion

	#region PUBLIC PROPERTIES
	/// <summary>The max velocity based on the mass and thrust.</summary>
	public float MaxVelocity {
		get { return thrust / structure.mass * Units.ThrustToMaxVel; }
	}

	/// <summary>The max rotational velocity based on the mass and the thrust.</summary>
	public float MaxRotationalVelocity {
		get { return maneuveringThrust / structure.mass * Units.ThrustToMaxVel; }
	}
	#endregion

	#region PRIVATE FIELDS
	/** The list of modifiers applied to the system. */
    private SystemModifierList _modifiers;

	/** A reference to the StructuralComponent, for the mass. */
	private StructuralComponent structure;
    #endregion

	#region PUBLIC METHODS
	/// <summary>Returns the accleration for the given percentage of current thrust [0-1].</summary>
	public float Acceleration(float percentThrust) {
		return (thrust * percentThrust) / structure.mass;
	}

	/// <summary>Returns the lateral acceleration for the given percentage of current thrust [0-1].</summary>
	public float LateralAcceleration(float percentThrust) {
		return (maneuveringThrust * 2 * percentThrust) / structure.mass; 
	}

	/// <summary>Returns the rotational acceleration for the percentage of current thrust [0-1].</summary>
	public float RotationalAcceleration(float percentThrust) {
		return (maneuveringThrust * percentThrust) / ((structure.mass * 0.0833333f) * (structure.width * structure.width + structure.length * structure.length));
	}
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
        _modifiers = new SystemModifierList(2);
    }

	// Get the StructuralComponent, we need it for the mass calculations.
	void Start() {
		structure = gameObject.GetComponent<StructuralComponent>();
	}
    #endregion

	#region PRIVATE_METHODS
    ///<summary>
    ///Recalculates the specified stat based on the current modifiers.
    ///</summary>
    private void RecalculateStat(ModifiableStat stat) {
        float multiplier, delta;
        _modifiers.Get(stat, out multiplier, out delta);

        switch (stat) {
			case ModifiableStat.Thrust:
				thrust.added = delta;
				thrust.modifier = multiplier;
				break;
			case ModifiableStat.ManeuveringThrust:
				maneuveringThrust.added = delta;
				maneuveringThrust.modifier = multiplier;
				break;
            default:
                break;
        }

        // TODO Implement the OverclockDamage modifier.
    }
    #endregion
}
