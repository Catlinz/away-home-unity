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
    /// Delegate to listen for when allocated CPU resources are lost..
    /// </summary>
    /// <param name="cpuLost">The amount of allocated CPU resources that were lost.</param>
    public delegate void AllocatedCpuLost(float cpuLost);
    /// <summary>Event generated when allocated CPU resources are lost.</summary>
    public event AllocatedCpuLost onAllocatedCpuLost;

    /// <summary>
    /// Delegate to listen for when CPU resources become available.
    /// </summary>
    /// <param name="freeCpu">The new level of free energy.</param>
    public delegate void IdleCpuGained(float idleCpu);
    /// <summary>Event generated when free energy is gained.</summary>
	public event IdleCpuGained onIdleCpuGained;

    /// <summary>The amount of CPU resources given by the system.</summary>
    public ModifiableFloat totalCpu;
    /// <summary>The current amount of CPU resources being used.</summary>
    public int allocatedCpu;

    /// <summary>The health of the ComputerSystem.  Damaged computer = less resources.</summary>
    public float health;

    /// <summary>The amount of free CPU resources available.</summary>
    public float IdleCpu {
		get { return totalCpu - (float)allocatedCpu; }
    }

    /// <summary>Default constructor.</summary>
    public ComputerSystem() {
        totalCpu = 0;
		allocatedCpu = 0;
        health = 100;
    }

    /// <summary>Copy constructor.</summary>
    public ComputerSystem(ComputerSystem src) {
        totalCpu = src.totalCpu;
		allocatedCpu = src.allocatedCpu;
        health = src.health;
    }

    /// <summary>
    /// Allocate some CPU resources.
    /// <para>If there aren't enough CPU resources to allocate, returns false.</para>
    /// </summary>
    /// <param name="cpu">The amount of CPU resources to allocate.</param>
    /// <returns>True if there were enough resources to allocate, false otherwise.</returns>
    public bool AllocateCpu(int cpu) {
		if (allocatedCpu + cpu <= totalCpu) {
			allocatedCpu += cpu;
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
		allocatedCpu -= cpu;
		if (allocatedCpu < 0) { allocatedCpu = 0; }
        if (onIdleCpuGained != null) { onIdleCpuGained(IdleCpu); }
    }

    /// <summary>
    /// Set the modifiers to the total cpu resources for the ComputerSystem.
    /// <para>Generates a AllocatedCpuLost event with the amount of allocated CPU resources lost.</para>
    /// </summary>
    /// <param name="added">The value to set the flat modifier to.</param>
    /// <param name="modifier">The value to set the multiplicative modifier to.</param>
    public void SetTotalCpu(float added = 0.0f, float modifier = 1.0f) {
        totalCpu.added = added;
        totalCpu.modifier = modifier;

		if (totalCpu < allocatedCpu) {
            if (onAllocatedCpuLost != null) {
				onAllocatedCpuLost(allocatedCpu - totalCpu);
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
        totalCpu.added += addedDelta;
        totalCpu.modifier += modifierDelta;

		if (totalCpu < allocatedCpu) {
            if (onAllocatedCpuLost != null) {
				onAllocatedCpuLost(allocatedCpu - totalCpu);
            }
        }
    }
}