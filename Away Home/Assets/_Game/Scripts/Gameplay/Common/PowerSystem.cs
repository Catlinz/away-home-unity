using UnityEngine;

/// <summary>
/// The PowerSystem represents the power system of a ship or 
/// other powered object.  It has a capacity and a recharge, 
/// and can be damaged, in which case it is less efficient.
/// </summary>
[System.Serializable]
public class PowerSystem {

    /// <summary>
    /// Delegate to listen for when reserved energy is lost.
    /// </summary>
    /// <param name="energyLost">The amount of reserved energy that was lost.</param>
    public delegate void ReservedEnergyLost(float energyLost);
    /// <summary>Event generated when reserved energy is lost.</summary>
    public event ReservedEnergyLost onReservedEnergyLost;

    /// <summary>
    /// Delegate to listen for when free energy is gained.
    /// </summary>
    /// <param name="freeEnergy">The new level of free energy.</param>
    public delegate void UsableEnergyGained(float freeEnergy);
    /// <summary>Event generated when free energy is gained.</summary>
    public event UsableEnergyGained onUsableEnergyGained;

    /// <summary>The max energy the system can store.</summary>
    public ModifiableFloat energyCapacity;
    /// <summary>The rate at which energy is gained per second.</summary>
    public ModifiableFloat energyRecharge;

    /// <summary>The amount of energy required for enabled Modules.</summary>
    public int reservedEnergy;
    /// <summary>The amount of energy the system current has.</summary>
    public float currentEnergy;

    /// <summary>The health of the system (out of 100).  Less health = less efficiency.</summary>
    public float health;

    /// <summary>The free energy that is currently available for use.</summary>
    public float FreeEnergy {
        get { return currentEnergy - reservedEnergy; }
    }

    /// <summary>Default constructor.</summary>
    public PowerSystem() {
        energyCapacity = 0;
        energyRecharge = 0;
        reservedEnergy = 0;
        currentEnergy = 0;
        health = 100;
    }

    /// <summary>Copy constructor.</summary>
    public PowerSystem(PowerSystem src) {
        energyCapacity = src.energyCapacity;
        energyRecharge = src.energyRecharge;
        reservedEnergy = src.reservedEnergy;
        currentEnergy = src.currentEnergy;
        health = src.health;
    }

    /// <summary>
    /// Adds energy to the currentEnergy levels of the PowerSystem.
    /// Calls any onUsablePowerGained delegates with new amount of free energy.
    /// </summary>
    /// <param name="energy">The amount of energy to add.</param>
    public void Add(float energy) {
        currentEnergy += energy;
        if (onUsableEnergyGained != null) { onUsableEnergyGained(FreeEnergy);  }
    }

    /// <summary>
    /// Consumes energy from the currentEnergy levels of the PowerSystem.
    /// If the amount of energy consumed dips below the reserved energy, 
    /// it calls any delegates registered to onCriticalPowerLost with 
    /// energyUsed - currentEnergy.
    /// <para>If energy is greater than currentEnergy, no energy is consumed and the method returns false.</para>
    /// </summary>
    /// <param name="energy">The amount of energy to consume.</param>
    /// <returns>True if there was enough energy to consume, false if no energy was consumed.</returns>
    public bool Consume(float energy) {
        // See if we have enough energy to consume.
        if (energy <= currentEnergy) {
            currentEnergy -= energy;
            if (currentEnergy < 0.0f) { currentEnergy = 0.0f; }

            // If we consumed into the reserves, call delegates.
            if (currentEnergy < reservedEnergy) {
                if (onReservedEnergyLost != null) { onReservedEnergyLost((float)reservedEnergy - currentEnergy); }
            }
            return true;
        }
        else { return false; }
    }

    /// <summary>
    /// Free some energy from the reserved energy used by installed Modules.
    /// <para>Any delegates attached to onUsablePowerGained are called with the new amount of free energy.</para>
    /// </summary>
    /// <param name="energy">The energy to free up.</param>
    public void Free(int energy) {
        reservedEnergy -= energy;
        if (reservedEnergy < 0) { reservedEnergy = 0; }
        if (onUsableEnergyGained != null) { onUsableEnergyGained(FreeEnergy); }
    }

	/// <summary>
	/// Put an amount of energy into the reserved energy for the Modules.
	/// <para>If there isn't enough energy to reserve, then the method returns false.</para>
	/// </summary>
	/// <param name="energy">The amount of energy to reserve.</param>
	/// <returns>True if the amount of energy could be reserved, false if there wasn't enough energy to reserve.</returns>
	public bool Reserve(int energy) {
		if ((reservedEnergy + energy) <= currentEnergy) {
			reservedEnergy += energy;
			return true;
		}
		else { return false; }
	}

    /// <summary>
    /// Set the modifiers to the energyCapacity for the PowerSystem.
    /// <para>If the new energyCapacity is less than currentEnergy, the difference is Consumed.</para>
    /// </summary>
    /// <param name="added">The value to set the flat modifier to.</param>
    /// <param name="modifier">The value to set the multiplicative modifier to.</param>
    public void SetEnergyCapacity(float added = 0.0f, float modifier = 1.0f) {
        energyCapacity.added = added;
        energyCapacity.modifier = modifier;

        if (energyCapacity < currentEnergy) {
            Consume(currentEnergy - energyCapacity);
        }
    }

    /// <summary>
    /// Set the modifiers to the energyRecharge for the PowerSystem.
    /// </summary>
    /// <param name="added">The value to set the flat modifier to.</param>
    /// <param name="modifier">The value to set the multiplicative modifier to.</param>
    public void SetEnergyRecharge(float added = 0.0f, float modifier = 1.0f) {
        energyRecharge.added = added;
        energyCapacity.modifier = modifier;
    }

    /// <summary>
    /// Update the modifiers to the energyCapacity for the PowerSystem.
    /// <para>If the new energyCapacity is less than currentEnergy, the difference is Consumed.</para>
    /// </summary>
    /// <param name="addDelta">The amount to change the flat modifier by.</param>
    /// <param name="modifierDelta">The amount to change the multiplicative modifier by.</param>
    public void UpdateEnergyCapacity(float addDelta = 0.0f, float modifierDelta = 0.0f) {
        energyCapacity.added += addDelta;
        energyCapacity.modifier += modifierDelta;

        if (energyCapacity < currentEnergy) {
            Consume(currentEnergy - energyCapacity);
        }
    }

    /// <summary>
    /// Update the modifiers to the energyRecharge for the PowerSystem.
    /// </summary>
    /// <param name="addDelta">The amount to change the flat modifier by.</param>
    /// <param name="modifierDelta">The amount to change the multiplicative modifier by.</param>
    public void UpdateEnergyRecharge(float addDelta = 0.0f, float modifierDelta = 0.0f) {
        energyRecharge.added += addDelta;
        energyCapacity.modifier += modifierDelta;
    }
}
