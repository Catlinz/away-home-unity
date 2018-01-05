using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreSystemComponent : MonoBehaviour, ISystem {

    #region PRIVATE_FIELDS
    private SystemModifierList _modifiers;
    #endregion

    #region PUBLIC_FIELDS
    ///<summary>The power system that represents the reactor.</summary>
    PowerSystem power;
    ///<summary>The computer system</summary>
    ComputerSystem computer;
    #endregion

    #region  PUBLIC METHODS
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

    #region HOOKS
    void Awake() {
        _modifiers = new SystemModifierList(3);
    }
	
	// Update is called once per frame
	void Update () {
		power.Tick(Time.deltaTime);
        computer.Tick(Time.deltaTime);
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
            case ModifiableStat.ComputerResources:
                computer.SetTotalCpu(delta, multiplier);
                break;
            case ModifiableStat.EnergyRecharge:
                power.SetEnergyRecharge(delta, multiplier);
                break;
            case ModifiableStat.EnergyCapacity:
                power.SetEnergyCapacity(delta, multiplier);
                break;
            default:
                break;
        }

        // TODO Implement the OverclockDamage modifier.
    }
    #endregion
}
