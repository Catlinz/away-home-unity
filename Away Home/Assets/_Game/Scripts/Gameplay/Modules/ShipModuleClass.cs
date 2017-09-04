using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The base class for all Ship Modules to inherit from.
/// </summary>
public class ShipModuleClass : MonoBehaviour {

	/// <summary>The basic types of modules that can be created.</summary>
	public enum ModuleType { Passive, Active };

	/// <summary>The asset that was used to construct the Module from.</summary>
	private InstallableModuleAsset moduleAsset;

	/// <summary>The amount of power consumed by the Module when enabled.</summary>
	public int IdleEnergyDrain {
		get { return moduleAsset.idleEnergyDrain; }
	}

	/// <summary>The amount of CPU resources consumed by the Module when enabled.</summary>
	public int IdleCpuUsage {
		get { return moduleAsset.idleCpuUsage; }
	}

	/// <summary>Frees up the Idle CPU and Energy from the ships systems.</summary>
	/// <param name="ship">The ship to disable the module on.</param>
	public virtual void DisableOnShip(ShipActorComponent ship) {
		ship.computer.DeallocateCpu(IdleCpuUsage);
		ship.power.Free(IdleEnergyDrain);
	}

	/// <summary>Allocates the idle CPU and reserves the idle energy required.</summary>
	/// <param name="ship">The ship to enable the Module for.</param>
	public virtual void EnableOnShip(ShipActorComponent ship) {
		ship.computer.AllocateCpu(IdleCpuUsage);
		ship.power.Reserve(IdleEnergyDrain);
	}

	/// <summary>
	/// Gets the type of the module.
	/// </summary>
	/// <returns>The module type.  By default, returns Passive.</returns>
	public virtual ModuleType GetModuleType() { 
		return ModuleType.Passive;
	}

	/// <summary>Initializes the Module component from an asset for the module.</summary>
	/// <param name="asset">The module asset to initialize from.</param>
	/// <param name="socket">The ShipSocket that the module is being installed on.</param>
	public virtual void InitFromAssetInSocket(InstallableModuleAsset asset, ShipSocket socket) {
		moduleAsset = asset;
	}
}
