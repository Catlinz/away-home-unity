using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityModule : ActorModuleClass {

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
	/// <description><c>InvalidShip</c> if the ship reference is null.</description>
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

		ActiveModuleAsset asset = moduleAsset as ActiveModuleAsset;

		// Do sanity checks to make sure we have a valid module asset and ship.
		if (!moduleAsset) {
			return ModuleResult.InvalidAsset;
		}
		if (!ship) {
			return ModuleResult.InvalidShip;
		}

		// Try and allocate the required CPU and energy for the module to activate.
		if (ship.computer.AllocateCpu(asset.activeCpuUsage)) {
			if (ship.power.Reserve(asset.activeEnergyDrain)) {
				isActivated = true;
				return ModuleResult.Success;
			}
			else { // Not enough power
				ship.computer.DeallocateCpu(asset.activeCpuUsage);
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
	/// <description><c>InvalidShip</c> if the ship reference is null.</description>
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

		ActiveModuleAsset asset = moduleAsset as ActiveModuleAsset;
		// Do sanity checks to make sure we have a valid module asset and ship.
		if (!moduleAsset) {
			return ModuleResult.InvalidAsset;
		}
		if (!ship) {
			return ModuleResult.InvalidShip;
		}

		ship.power.Free(asset.activeEnergyDrain);
		ship.computer.DeallocateCpu(asset.activeCpuUsage);

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

	/// <summary>
	/// Returns that this is an Active module.
	/// </summary>
	public override ShipModuleClass.TypeFlags GetTypeFlags() {
		return ShipModuleClass.TypeFlags.Active;
	}

	/// <summary>Initializes the Module component from an asset for the module.</summary>
	/// <seealso cref="ShipModuleClass.InitFromAssetInSocket"/>
	public override ModuleResult InitFromAssetInSocket(InstallableModuleAsset asset, ShipSocket socket) {
		return base.InitFromAssetInSocket(asset, socket);
	}
}
