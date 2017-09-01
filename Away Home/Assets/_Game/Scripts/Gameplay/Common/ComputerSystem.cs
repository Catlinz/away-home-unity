using System.Collections;
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
    public delegate void CpuResourcesGained(float freeCpu);
    /// <summary>Event generated when free energy is gained.</summary>
    public event CpuResourcesGained onCpuResourcesGained;

    /// <summary>The amount of CPU resources given by the system.</summary>
    public ModifiableFloat cpu;
    /// <summary>The current amount of CPU resources being used.</summary>
    public int cpuUsage;

    /// <summary>The health of the ComputerSystem.  Damaged computer = less resources.</summary>
    public float health;

    /// <summary>The amount of free CPU resources available.</summary>
    public float FreeCpuResources {
        get { return cpu - (float)cpuUsage; }
    }

    /// <summary>Default constructor.</summary>
    public ComputerSystem() {
        cpu = 0;
        cpuUsage = 0;
        health = 100;
    }

    /// <summary>Copy constructor.</summary>
    public ComputerSystem(ComputerSystem src) {
        cpu = src.cpu;
        cpuUsage = src.cpuUsage;
        health = src.health;
    }

    /// <summary>
    /// Free up some CPU resources.
    /// </summary>
    /// <param name="cpu">The amount of CPU resources to free.</param>
    public void FreeCpu(int cpu) {
        cpuUsage -= cpu;
        if (cpuUsage < 0) { cpuUsage = 0; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cpu"></param>
    public void UseCpu(int cpu) {
        cpuUsage += cpu;
    }



}
