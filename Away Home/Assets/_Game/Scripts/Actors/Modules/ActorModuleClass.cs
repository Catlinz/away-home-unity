using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The base class for all Ship Modules to inherit from.
/// </summary>
public class ActorModuleClass : MonoBehaviour {

    #region PUBLIC_FIELDS
    /// <summary>The type of hardpoint socket that is required to install the module.</summary>
    public HPSocket socket;
    #endregion

    #region PUBLIC_PROPS
    /// <summary>Get the asset that was used to construct the Module.</summary>
    public InstallableModuleAsset Asset {
        get { return moduleAsset; }
    }

    /// <summary>The amount of computer resources consumed by the Module when enabled.</summary>
    public int IdleComputerResources {
        get { return moduleAsset.idleComputerResources; }
    }

    /// <summary>The amount of energy consumed by the Module when enabled.</summary>
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

    /// <summary>The CoreSystemComponent of the actor this module is installed on.</summary>
    protected CoreSystemComponent system;
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
	/// <description><c>InvalidSystem</c> if the system reference is null already.</description>
	/// </item>
	/// </list>
	/// </returns>
	public virtual ModuleResult DisableModule() {
		if (!isEnabled) { return ModuleResult.AlreadyDisabled; }

		isEnabled = false;
		if (system != null) {
            system.computer.DeallocateCpu(IdleComputerResources);
            system.power.Free(IdleEnergyDrain);
            system = null;
			return ModuleResult.Success;
		}
		else {
			return ModuleResult.InvalidSystem;
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

		system = gameObject.GetComponentInParent<CoreSystemComponent>();
		if (!system) { return ModuleResult.InvalidSystem; }

		if (system.computer.AllocateCpu(IdleComputerResources)) {
			if (system.power.Reserve(IdleEnergyDrain)) {
				isEnabled = true;
				return ModuleResult.Success;
			}
			else { // Not enough power to enable module.
                system.computer.DeallocateCpu(IdleComputerResources);
                system = null;
				return ModuleResult.InsufficientPower;
			}
		}
		else { // Not enough CPU to enable module.
            system = null;
			return ModuleResult.InsufficientCpu;
		}
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
	/// <param name="hardpoint">The HardPoint that the module is being installed on.</param>
	/// <returns>ModuleResult.Success</returns>
	public virtual ModuleResult InitFromAssetInHardPoint(InstallableModuleAsset asset, HardPoint hardpoint) {
		moduleAsset = asset;
        hardpoint.SetModule(this);
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
