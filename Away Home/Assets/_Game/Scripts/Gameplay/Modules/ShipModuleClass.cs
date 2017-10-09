using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The base class for all Ship Modules to inherit from.
/// </summary>
public class ShipModuleClass : MonoBehaviour {

	/// <summary>A list of flags that can describe a type of module.</summary>
	[System.Flags] public enum TypeFlags { None = 0, Passive = 0x1, Active = 0x2, Tracking = 0x4, Trigger = 0x8 };

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

    /// <summary>A reference to the ship the module is enabled on.</summary>
    protected ShipActorComponent ship;
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
	/// <returns>
	/// A <c>ModuleResult</c> to indicate the result of disabling the module.
	/// <list type="bullet">
	/// <item>
	/// <description><c>AlreadyDisabled</c> if the module is already disabled.</description>
	/// </item>
	/// <item>
	/// <description><c>InvalidShip</c> if the ship reference is null already.</description>
	/// </item>
	/// </list>
	/// </returns>
	public virtual ModuleResult DisableModule() {
		if (!isEnabled) { return ModuleResult.AlreadyDisabled; }

		isEnabled = false;
		if (ship != null) {
			ship.computer.DeallocateCpu(IdleCpuUsage);
			ship.power.Free(IdleEnergyDrain);
			ship = null;
			return ModuleResult.Success;
		}
		else {
			return ModuleResult.InvalidShip;
		}
	}

	/// <summary>Allocates the idle CPU and reserves the idle energy required.</summary>
	/// <returns>
	/// A <c>ModuleResult</c> to indicate the result of disabling the module.
	/// <list type="bullet">
	/// <item>
	/// <description><c>AlreadyEnabled</c> if the module is already enabled.</description>
	/// </item>
	/// <item>
	/// <description><c>InvalidShip</c> if the ship reference is null.</description>
	/// </item>
	/// <item>
	/// <description><c>InsufficientPower</c> if there is not enough power to enable the module.</description>
	/// </item>
	/// <item>
	/// <description><c>InsufficientCpu</c> if there is not enough CPU to enable the module.</description>
	/// </item>
	/// </list>
	/// </returns>
	public virtual ModuleResult EnableModule() {
        if (isEnabled) { return ModuleResult.AlreadyEnabled; }

		ship = gameObject.GetComponentInParent<ShipActorComponent>();
		if (!ship) { return ModuleResult.InvalidShip; }

		if (ship.computer.AllocateCpu(IdleCpuUsage)) {
			if (ship.power.Reserve(IdleEnergyDrain)) {
				isEnabled = true;
				return ModuleResult.Success;
			}
			else { // Not enough power to enable module.
				ship.computer.DeallocateCpu(IdleCpuUsage);
				ship = null;
				return ModuleResult.InsufficientPower;
			}
		}
		else { // Not enough CPU to enable module.
			ship = null;
			return ModuleResult.InsufficientCpu;
		}
	}

	/// <summary>
	/// Gets the details about the type of the module.
	/// </summary>
	/// <returns>The module type flags.</returns>
	public virtual TypeFlags GetTypeFlags() { 
		return TypeFlags.None;
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
	/// <returns>ModuleResult.Success</returns>
	public virtual ModuleResult InitFromAssetInSocket(InstallableModuleAsset asset, ShipSocket socket) {
		moduleAsset = asset;
        socket.SetModule(this);
		return ModuleResult.Success;
	}

    /// <summary>
    /// Sets the current target for the module, if it can have a target.
    /// </summary>
    /// <param name="newTarget">The new targetable object to target.</param>
    public virtual void SetTarget(ITargetable newTarget) {
        return;
    }
}
