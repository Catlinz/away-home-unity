using UnityEngine;

/// <summary>
/// The PowerSystem represents the power system of a ship or 
/// other powered object.  It has a capacity and a recharge, 
/// and can be damaged, in which case it is less efficient.
/// </summary>
[System.Serializable]
public class PowerSystem {

    /// <summary>
    /// Delegate to listen for when reserved energy is lost, or free energy is gained.
    /// </summary>
    /// <param name="energyLost">The deficit of energy (if negative) or amount of free energy (if positive).</param>
    public delegate void EnergyChanged(float energy);
    /// <summary>Event generated when reserved energy is lost.</summary>
    public event EnergyChanged onEnergyChanged;

    /// <summary>Event generated when the system takes damage.</summary>
    public event SystemDamaged onDamaged;

    /// <summary>The max energy the system can store.</summary>
    public ModifiableFloat energyCapacity;
    /// <summary>The rate at which energy is gained per second.</summary>
    public ModifiableFloat energyRecharge;

    /// <summary>The amount of energy required for enabled Modules.</summary>
    public int reservedEnergy;
    /// <summary>The amount of energy the system current has.</summary>
    public float currentEnergy;

    /// <summary>The damage of the system [0 - 1].  More damage = less efficiency.</summary>
    public float damage;

    /// <summary>The amount that the system is currently overclocked by.</summary>
    public float overclock;

    ///<summary>The current energy capacity with overclock and damage modifiers applied.</summary>
    public float EnergyCapacity {
        get {
            if (damage > 0.0f) {
                return energyCapacity * overclock * (1.0f - (damage * 0.5f)); 
            }
            else { return energyCapacity * overclock;  }
        }
    }

    ///<summary>The current energy recharge with overclock and damage modifiers applied.</summary>
    public float EnergyRecharge {
        get {
            if (damage > 0.0f) {
                return energyRecharge * overclock * (1.0f - (damage * 0.5f));
            }
            else { return energyRecharge * overclock; }
        }
    }

    /// <summary>The free energy that is currently available for use.</summary>
    public float FreeEnergy {
        get { return currentEnergy - reservedEnergy; }
    }

    /// <summary>Get the current health of the system (0 - 100).</summary>
    public float Health {
        get { return (1.0f - damage) * 100.0f; }
    }

    /// <summary>Whether the Power system is currently overclocked or not.</summary>
    public bool IsOverclocked {
        get { return overclock > 1.0f;  }
    }

    /// <summary>Keeps track of when the energy should tick up.</summary>
    private float timeAccum;

    /// <summary>Default constructor.</summary>
    public PowerSystem() {
        energyCapacity = 0;
        energyRecharge = 0;
        reservedEnergy = 0;
        currentEnergy = 0;
        damage = 0.0f;
        overclock = 1.0f;
    }

    /// <summary>Copy constructor.</summary>
    public PowerSystem(PowerSystem src) {
        energyCapacity = src.energyCapacity;
        energyRecharge = src.energyRecharge;
        reservedEnergy = src.reservedEnergy;
        currentEnergy = src.currentEnergy;
        damage = src.damage;
        overclock = src.overclock;
    }

    /// <summary>
    /// Adds energy to the currentEnergy levels of the PowerSystem.
    /// Calls any onUsablePowerGained delegates with new amount of free energy.
    /// </summary>
    /// <param name="energy">The amount of energy to add.</param>
    public void Add(float energy) {
        float capacity = EnergyCapacity;
        if (currentEnergy < capacity) {
            currentEnergy = Mathf.Min(currentEnergy + energy, capacity);
            if (onEnergyChanged != null) { onEnergyChanged(FreeEnergy); }
        }
        
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
                if (onEnergyChanged != null) { onEnergyChanged((float)currentEnergy - reservedEnergy); }
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
        if (onEnergyChanged != null) { onEnergyChanged(FreeEnergy); }
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
    /// Set the current percent damage the system has taken.
    /// </summary>
    /// <param name="newDamage">The percent damage the system has taken [0-1].</param>
    public void SetDamage(float newDamage) {
        float oldDamage = damage;
        damage = newDamage;
        if (oldDamage < newDamage) {
            // Check for and take care of any energy loss due to loss of capacity.
            CheckCapacity();

            if (onDamaged != null) {
                onDamaged(damage);
            }            
        }
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

        // Check for and take care of any energy loss due to loss of capacity.
        CheckCapacity();
    }

    /// <summary>
    /// Set the modifiers to the energyRecharge for the PowerSystem.
    /// </summary>
    /// <param name="added">The value to set the flat modifier to.</param>
    /// <param name="modifier">The value to set the multiplicative modifier to.</param>
    public void SetEnergyRecharge(float added = 0.0f, float modifier = 1.0f) {
        energyRecharge.added = added;
        energyRecharge.modifier = modifier;
    }

    /// <summary>
    /// Set the percentage of overclocking for the power system.  Overclocking 
    /// increases the power capacity and recharge rate, but also damages the power system.
    /// </summary>
    /// <param name="overclockPercent">The percentage of overclocking to do [0-1].</param>
    public void SetOverclock(float overclockPercent) {
        overclock = overclockPercent;

        // Check for and take care of any energy loss due to loss of capacity.
        CheckCapacity();
    }

    /// <summary>
    /// Ticks the power system to recharge the power.  It will 
    /// accumulate power until it is >= to the rechargeRate/sec and 
    /// then add it and reset to zero.
    /// </summary>
    /// <param name="deltaTime">The amount of time passed, in seconds.</param>
    public void Tick(float deltaTime) {
        // Increment the time accumulator.
        timeAccum += deltaTime;
        
        // If >= 1 second, then do processing.
        if (timeAccum >= 1.0f) {
            float rechargeRate = EnergyRecharge;
            float capacity = EnergyCapacity;

            // If is overclocked, then apply damage based on overclock amount.
            if (IsOverclocked) {
                // Apply damage
                UpdateDamage((overclock - 1.0f) * timeAccum);
            }

            // Increment the amount of energy we have stored.
            if (currentEnergy < capacity) {
                Add(rechargeRate * timeAccum);
            }

            timeAccum = 0.0f;
        }
    }

    /// <summary>
    /// Update the current amount of damage to the system.  If we are adding 
    /// damage, we check to see if we need to consume any energy that we no longer have.
    /// </summary>
    /// <param name="damageDelta">The amount of damage to add or remove.</param>
    public void UpdateDamage(float damageDelta) {
        damage = Mathf.Max(damage + damageDelta, 0);

        // If the system took damage, take care of any energy loss.
        if (damageDelta > 0.0f) {
            CheckCapacity();

            // Broadcast that the system took damage to any listeners.
            if (onDamaged != null) {
                onDamaged(damage);
            }
        }
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

        // Check for and take care of any energy loss due to loss of capacity.
        CheckCapacity();
    }

    /// <summary>
    /// Update the modifiers to the energyRecharge for the PowerSystem.
    /// </summary>
    /// <param name="addDelta">The amount to change the flat modifier by.</param>
    /// <param name="modifierDelta">The amount to change the multiplicative modifier by.</param>
    public void UpdateEnergyRecharge(float addDelta = 0.0f, float modifierDelta = 0.0f) {
        energyRecharge.added += addDelta;
        energyRecharge.modifier += modifierDelta;
    }

    /// <summary>
    /// Checks to see if the current stored energy is > the energy capacity.
    /// If it is, then we consume the excess energy.
    /// </summary>
    private void CheckCapacity() {
        float capacity = EnergyCapacity;
        if (capacity < currentEnergy) {
            Consume(currentEnergy - capacity);
        }
    }
}
