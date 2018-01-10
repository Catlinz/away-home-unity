using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CoreSystemComponent))]
public class ShipThrustComponent : SystemComponent {

    #region FIELDS
    [Header("Thrust")]
	/// <summary>The amount of thrust produced by the engine (Newtons).</summary>
	public ModifiableFloat thrust;

	/// <summary>The amount of thrust provided to turn or slide the ship (Newtons).</summary>
	public ModifiableFloat maneuveringThrust;
    #endregion

	#region PROPERTIES
	/// <summary>The max velocity based on the mass and thrust.</summary>
	public float MaxVelocity {
		get {
            return thrust / _sys.mass * Units.ThrustToMaxVel;
        }
	}

	/// <summary>The max rotational velocity based on the mass and the thrust.</summary>
	public float MaxRotationalVelocity {
		get {
            return maneuveringThrust / _sys.mass * Units.ThrustToMaxVel;
        }
	}
    #endregion

    #region SYSTEM CONTROL
    /// <summary>
    /// Make sure to recalculate all the stats when the system is activated.
    /// </summary>
    protected override OperationResult SystemActivate() {
        thrust.modifier = 1f;
        maneuveringThrust.modifier = 1f;

        RecalculateAllStats();
        return OperationResult.OK();
    }

    /// <summary>
    /// Set the stat modifiers to 0 in order to remove all thrust when inactive.
    /// </summary>
    protected override OperationResult SystemDeactivate() {
        thrust.modifier = 0f;
        maneuveringThrust.modifier = 0f;
        return OperationResult.OK();
    }

    protected override void SystemStart() {
        SystemDeactivate();
    }
    #endregion

    #region PUBLIC METHODS
    /// <summary>Returns the accleration for the given percentage of current thrust [0-1].</summary>
    public float Acceleration(float percentThrust) {
		return (thrust * percentThrust) / _sys.mass;
	}

	/// <summary>Returns the lateral acceleration for the given percentage of current thrust [0-1].</summary>
	public float LateralAcceleration(float percentThrust) {
		return (maneuveringThrust * 2 * percentThrust) / _sys.mass; 
	}

	/// <summary>Returns the rotational acceleration for the percentage of current thrust [0-1].</summary>
	public float RotationalAcceleration(float percentThrust) {
		return (maneuveringThrust * percentThrust) / ((_sys.mass * 0.0833333f) * (_sys.width * _sys.width + _sys.length * _sys.length));
	}
    #endregion

    #region MODIFIER METHODS
    private void RecalculateAllStats() {
        float multiplier, delta;

        // Recalculate the Thrust
        _modifiers.Get(ModifiableStat.Thrust, out multiplier, out delta);
        thrust.added = delta;
        thrust.modifier = multiplier;

        // Recalculate the Maneuvering Thrust
        _modifiers.Get(ModifiableStat.ManeuveringThrust, out multiplier, out delta);
        maneuveringThrust.added = delta;
        maneuveringThrust.modifier = multiplier;

    }
    ///<summary>
    ///Recalculates the specified stat based on the current modifiers.
    ///</summary>
    override protected void RecalculateStat(ModifiableStat stat) {
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

    #region UNITY HOOKS
    // Initialize empty SystemModifierList.
    void Awake() {
        _modifiers = new SystemModifierList(2);
    }
    #endregion
}
