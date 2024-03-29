﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The ComputerSystem represents the computer resources and state of 
/// a ship or other object.
/// </summary>
[System.Serializable]
public class ComputerSystem {
    /// <summary>
    /// Delegate to listen for when allocated Computer resources are lost or Idle computer resources are gained.
    /// </summary>
    /// <param name="cpuLost">The resource deficit (if negative) or free resources (if positive).</param>
    public delegate void ResourcesChanged(float cpuLost);
    /// <summary>Event generated when allocated Computer resources are lost or idle Computer resources are gained.</summary>
    public event ResourcesChanged onResourcesChanged;

    /// <summary>Event generated when the system takes damage.</summary>
    public event SystemComponent.Damaged onDamaged;

    /// <summary>The amount of CPU resources given by the system.</summary>
    public ModifiableFloat totalResources;
    /// <summary>The current amount of CPU resources being used.</summary>
    public int allocatedResources;

    /// <summary>The damage to the computer system.  More damage = less resources.</summary>
    public float damage;

    /// <summary>The amount that the system is currently overclocked by.</summary>
    public float overclock;

    /// <summary>The amount of free CPU resources available.</summary>
    public float IdleResources {
		get { return totalResources - (float)allocatedResources; }
    }

    /// <summary>Whether the computer system is currently overclocked or not.</summary>
    public bool IsOverclocked {
        get { return overclock >= 1.0f; }
    }

    ///<summary>The total CPU available with overclock and damage modifiers applied.</summary>
    public float TotalResources {
        get {
            if (damage > 0.0f) {
                return totalResources * overclock * (1.0f - (damage * 0.5f));
            }
            else { return totalResources * overclock; }
        }
    }

    /// <summary>Keeps track of the ticks and when damage should be applied.</summary>
    private float timeAccum;

    /// <summary>Default constructor.</summary>
    public ComputerSystem() {
        totalResources = 0;
		allocatedResources = 0;
        damage = 0;
        overclock = 1.0f;
    }

    /// <summary>Copy constructor.</summary>
    public ComputerSystem(ComputerSystem src) {
        totalResources = src.totalResources;
		allocatedResources = src.allocatedResources;
        damage = src.damage;
        overclock = src.overclock;
    }

    /// <summary>
    /// Allocate some CPU resources.
    /// <para>If there aren't enough CPU resources to allocate, returns false.</para>
    /// </summary>
    /// <param name="cpu">The amount of CPU resources to allocate.</param>
    /// <returns>True if there were enough resources to allocate, false otherwise.</returns>
    public bool AllocateCpu(int cpu) {
        if (allocatedResources + cpu <= TotalResources) {
			allocatedResources += cpu;
            return true;
        }
        else { return false; }
    }

    /// <summary>
    /// Free up some CPU resources.
    /// <para>Generates a CpuResourcesGained event with the UnallocatedCpu.</para>
    /// </summary>
    /// <param name="cpu">The amount of CPU resources to free.</param>
    public void DeallocateCpu(int cpu) {
        allocatedResources = Mathf.Max(allocatedResources - cpu, 0);
        if (onResourcesChanged != null) { onResourcesChanged(IdleResources); }
    }

    /// <summary>
    /// Set the percent damage the system has taken.
    /// </summary>
    /// <param name="newDamage">The percent damage to set [0-1].</param>
    public void SetDamage(float newDamage) {
        float oldDamage = damage;
        damage = newDamage;

        if (newDamage > oldDamage) {
            // Check to see if we lost any allocated resources to do total CPU change.
            CheckTotalCpu();

            if (onDamaged != null) {
                onDamaged(damage);
            }
        }
    }

    /// <summary>
    /// Set the percentage of overclocking for the computer system.  Overclocking 
    /// increases the available CPU resources, but also damages the computer system.
    /// </summary>
    /// <param name="overclockPercent">The percentage of overclocking to do [0-1].</param>
    public void SetOverclock(float overclockPercent) {
        overclock = overclockPercent;

        // Check for and take care of any lost resources.
        CheckTotalCpu();
    }

    /// <summary>
    /// Set the modifiers to the total cpu resources for the ComputerSystem.
    /// <para>Generates a AllocatedCpuLost event with the amount of allocated CPU resources lost.</para>
    /// </summary>
    /// <param name="added">The value to set the flat modifier to.</param>
    /// <param name="modifier">The value to set the multiplicative modifier to.</param>
    public void SetTotalCpu(float added = 0.0f, float modifier = 1.0f) {
        totalResources.added = added;
        totalResources.modifier = modifier;

        // Check to see if we lost any allocated resources to do total CPU change.
        CheckTotalCpu();
    }

    /// <summary>
    /// Ticks the computer system.  Basically just to apply damage based on 
    /// overclocking.
    /// </summary>
    /// <param name="deltaTime">The amount of time passed, in seconds.</param>
    public void Tick(float deltaTime) {
        // Increment the time accumulator.
        timeAccum += deltaTime;

        // If >= 1 second, then do processing.
        while (timeAccum >= 1.0f) {
            // If is overclocked, then apply damage based on overclock amount.
            if (IsOverclocked) {
                // Apply damage
                UpdateDamage((overclock - 1.0f) * timeAccum);
            }

            timeAccum -= 1.0f;
        }
    }

    /// <summary>
    /// Update the damage to the Computer system.  If the system took damage, some allocated resources
    /// may be lost.
    /// </summary>
    /// <param name="damageDelta">The amount of damage to apply to the system.</param>
    public void UpdateDamage(float damageDelta) {
        damage = Mathf.Max(damage + damageDelta, 0);
        if (damageDelta > 0.0f) {
            // Check to see if we lost any allocated resources to do total CPU change.
            CheckTotalCpu();
            
            if (onDamaged != null) {
                onDamaged(damage);
            }
        }
    }

    /// <summary>
    /// Update the modifiers to the total cpu resources for the ComputerSystem.
    /// <para>Generates a AllocatedCpuLost event with the amount of allocated CPU resources lost.</para>
    /// </summary>
    /// <param name="addedDelta">The amount to change the flat modifier by.</param>
    /// <param name="modifierDelta">The amount to change the multiplicative modifier by.</param>
    public void UpdateTotalCpu(float addedDelta = 0.0f, float modifierDelta = 0.0f) {
        totalResources.added += addedDelta;
        totalResources.modifier += modifierDelta;

        // Check to see if we lost any allocated resources to do total CPU change.
        CheckTotalCpu();
    }

    /// <summary>
    /// Check to see if we lost any allocated resources due to a change in the total CPU available.
    /// If so, then we broadcast an event.
    /// </summary>
    private void CheckTotalCpu() {
        float total = TotalResources;
        if (total < allocatedResources) {
            if (onResourcesChanged != null) {
                onResourcesChanged(total - allocatedResources);
            }
        }
    }
}
