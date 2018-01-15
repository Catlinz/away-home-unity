using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityModule : ActorModule {

	#region PUBLIC FIELDS
	public int activeComputerResources;

	public int activeEnergyDrain;
	#endregion

	#region PROTECTED_VARS
	/// <summary>Whether or not the module is currently active.</summary>
	protected bool isActivated = false;
	#endregion

	/// <summary>
	/// Try and activate the module.  If there is not enough energy or CPU to do so, it will fail.
	/// If the CPU and/or energy levels fall too low, the module will be deactivated.
	/// </summary>
	/// <returns>
	/// A <c>ModuleResult</c> indicating if the activation was successful.
	/// <list type="bullet">
	/// <item>
	/// <description><c>AlreadyActive</c> if the module is already activated.</description>
	/// </item>
	/// <item>
	/// <description><c>InvalidAsset</c> if the module asset is null or cannot be converted to an <c>ActiveModuleAsset</c></description>
	/// </item>
	/// <item>
	/// <description><c>InvalidSystem</c> if the ship reference is null.</description>
	/// </item>
	/// <item>
	/// <description><c>InsufficientPower</c> if there is not enough power to activate the module.</description>
	/// </item>
	/// <item>
	/// <description><c>InsufficientCpu</c> if there is not enough CPU to activate the module.</description>
	/// </item>
	/// </list>
	/// </returns>
	public virtual ModuleResult ActivateModule() {
		if (isActivated) { 
			return ModuleResult.AlreadyActive;
		}

		if (!system) {
			return ModuleResult.InvalidSystem;
		}

		// Try and allocate the required CPU and energy for the module to activate.
		if (system.computer.AllocateCpu(activeComputerResources)) {
			if (system.power.Reserve(activeEnergyDrain)) {
				isActivated = true;
				return ModuleResult.Success;
			}
			else { // Not enough power
				system.computer.DeallocateCpu(activeComputerResources);
				return ModuleResult.InsufficientPower;
			}
		}
		else { // Not enough CPU
			return ModuleResult.InsufficientCpu;
		}
	}

	/// <summary>
	/// Try and deactivate the module.
	/// </summary>
	/// <returns>
	/// A <c>ModuleResult</c> indicating the result of the deactivation.
	/// <list type="bullet">
	/// <item>
	/// <description><c>AlreadyInactive</c> if the module is already deactivated.</description>
	/// </item>
	/// <item>
	/// <description><c>InvalidAsset</c> if the module asset is null or cannot be converted to an <c>ActiveModuleAsset</c></description>
	/// </item>
	/// <item>
	/// <description><c>InvalidSystem</c> if the ship reference is null.</description>
	/// </item>
	/// <item>
	/// <description><c>InsufficientPower</c> if there is not enough power to activate the module.</description>
	/// </item>
	/// <item>
	/// <description><c>InsufficientCpu</c> if there is not enough CPU to activate the module.</description>
	/// </item>
	/// </list>
	/// </returns>
	public virtual ModuleResult DeactivateModule() {
		if (!isActivated) { return ModuleResult.AlreadyInactive; }

		if (!system) {
			return ModuleResult.InvalidSystem;
		}

		system.power.Free(activeEnergyDrain);
		system.computer.DeallocateCpu(activeComputerResources);

		return ModuleResult.Success;
	}

	/// <summary>Makes sure the module is deactivated before disabling.</summary>
	/// <seealso cref="ShipModuleClass.DisableModule"/>
	public override ModuleResult DisableModule() {
		DeactivateModule();
		return base.DisableModule();
	}

	/// <summary>Allocates the idle CPU and reserves the idle energy required.</summary>
	/// <seealso cref="ShipModuleClass.EnableModule"/>
	public override ModuleResult EnableModule() {
		return base.EnableModule();
	}
}
