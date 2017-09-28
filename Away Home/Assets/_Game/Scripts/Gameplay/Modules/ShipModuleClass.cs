using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The base class for all Ship Modules to inherit from.
/// </summary>
public class ShipModuleClass : MonoBehaviour {

	/// <summary>The basic types of modules that can be created.</summary>
	public enum ModuleType { Passive, Active };

    #region PUBLIC_PROPS
    /// <summary>Get the asset that was used to construct the Module.</summary>
    public InstallableModuleAsset Asset {
        get { return moduleAsset; }
    }

    /// <summary>The amount of CPU resources consumed by the Module when enabled.</summary>
    public int IdleCpuUsage {
        get { return moduleAsset.idleCpuUsage; }
    }

    /// <summary>The amount of power consumed by the Module when enabled.</summary>
    public int IdleEnergyDrain {
        get { return moduleAsset.idleEnergyDrain; }
    }

    /// <summary>Whether or not the Module is currently enabled.</summary>
    public bool IsEnabled {
        get { return isEnabled; }
    }
    #endregion

    #region PROTECTED_VARS
    /// <summary>Whether or not the module is currently enabled.</summary>
    protected bool isEnabled;

    /// <summary>The asset that was used to construct the Module from.</summary>
    protected InstallableModuleAsset moduleAsset;

    /// <summary>A reference to the computer system the module uses when enabled.</summary>
    protected ComputerSystem computer;

    /// <summary>A reference to the power system the module uses when enabled.</summary>
    protected PowerSystem power;
    #endregion

    /// <summary>
    /// Test whether or not the module can target the provided target.
    /// </summary>
    /// <param name="newTarget">The targetable object to test.</param>
    /// <returns>True if it is a valid target for the module.</returns>
    public virtual bool CanTarget(ITargetable newTarget) {
        return false;
    }

    /// <summary>Frees up the Idle CPU and Energy from the ships systems.</summary>
    /// <param name="ship">The ship to disable the module on.</param>
    public virtual void DisableOnShip(ShipActorComponent ship) {
        if (computer != null && power != null) {
            computer.DeallocateCpu(IdleCpuUsage);
            power.Free(IdleEnergyDrain);
            computer = null;
            power = null;
        }

        isEnabled = false;
	}

	/// <summary>Allocates the idle CPU and reserves the idle energy required.</summary>
	/// <param name="ship">The ship to enable the Module for.</param>
    /// <returns>True if the module was successfully enabled (or was already enabled).</returns>
	public virtual bool EnableOnShip(ShipActorComponent ship) {
        if (isEnabled) { return true; }

		if (ship.computer.AllocateCpu(IdleCpuUsage)) {
            if (ship.power.Reserve(IdleEnergyDrain)) {
                computer = ship.computer;
                power = ship.power;
                isEnabled = true;
            }
            else {
                ship.computer.DeallocateCpu(IdleCpuUsage);
            }
        }

        return isEnabled;
	}

	/// <summary>
	/// Gets the type of the module.
	/// </summary>
	/// <returns>The module type.  By default, returns Passive.</returns>
	public virtual ModuleType GetModuleType() { 
		return ModuleType.Passive;
	}

    /// <summary>
    /// Test to see if the module is currently targeting the provided targetable object.
    /// </summary>
    /// <param name="target">The targetable object to test.</param>
    /// <returns>True if the module is targeting the provided target.</returns>
    public virtual bool HasTarget(ITargetable testTarget) {
        return false;
    }

	/// <summary>Initializes the Module component from an asset for the module.</summary>
	/// <param name="asset">The module asset to initialize from.</param>
	/// <param name="socket">The ShipSocket that the module is being installed on.</param>
	public virtual void InitFromAssetInSocket(InstallableModuleAsset asset, ShipSocket socket) {
		moduleAsset = asset;
        socket.SetModule(this);
	}

    /// <summary>
    /// Sets the current target for the module, if it can have a target.
    /// </summary>
    /// <param name="newTarget">The new targetable object to target.</param>
    public virtual void SetTarget(ITargetable newTarget) {
        return;
    }
}
