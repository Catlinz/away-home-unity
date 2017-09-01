using UnityEngine;

/// <summary>
/// The PowerSystem represents the power system of a ship or 
/// other powered object.  It has a capacity and a recharge, 
/// and can be damaged, in which case it is less efficient.
/// </summary>
[System.Serializable]
public class PowerSystem {

    public ModifiableFloat energyCapacity;
    public ModifiableFloat energyRecharge;

    public int energyUsed;
    public float energy;

    public float health;

    public float AvailablePower {
        get { return energyCapacity - (float)energyUsed; }
    }

    public PowerSystem() {
        energyCapacity = 0;
        energyRecharge = 0;
        energyUsed = 0;
        energy = 0;
        health = 100;
    }

    public PowerSystem(PowerSystem src) {
        energyCapacity = src.energyCapacity;
        energyRecharge = src.energyRecharge;
        energyUsed = src.energyUsed;
        energy = src.energy;
        health = src.health;
    }

    public void Add(float energy) {
        energy += energy;
    }

    public void Consume(float energy) {
        energy -= energy;
        if (energy < 0.0f) { energy = 0.0f; }
    }

    public void Free(int energy) {
        energyUsed -= energy;
        if (energyUsed < 0) { energyUsed = 0; }
    }

    public void Use(int energy) {
        energyUsed += energy;
    }
}
