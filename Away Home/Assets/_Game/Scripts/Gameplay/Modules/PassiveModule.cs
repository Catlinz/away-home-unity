using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A component that represents a passive module that is installed on a ship.
/// A passive module simply provides a flat or percentage boost to a ship stat.
/// </summary>
public class PassiveModule : MonoBehaviour, IShipModule {

    /// <summary>The asset that was used to construct the Module from.</summary>
	private PassiveModuleAsset moduleAsset;

    /// <summary>The amount of power consumed by the Module when enabled.</summary>
	public int IdlePowerUsage {
		get { return moduleAsset.idlePowerUsage; }
	}

    /// <summary>The amount of CPU resources consumed by the Module when enabled.</summary>
	public int IdleCpuUsage {
		get { return moduleAsset.idleCpuUsage; }
	}

    /// <summary>Called to try and enable the module a ShipActorComponent.</summary>
    /// <param name="ship">The ship to enable the Module for.</param>
	public virtual void EnableOnShip(ShipActorComponent ship) {
        ship.computer.UseCpu(IdleCpuUsage);
        ship.power.Use(IdlePowerUsage);
	}

	/// <summary>Initializes the Module component from an asset for the module.</summary>
    /// <param name="asset">The module asset to initialize from.</param>
    /// <param name="socket">The ShipSocket that the module is being installed on.</param>
	public void InitFromAssetInSocket(InstallableModuleAsset asset, ShipSocket socket) {
		moduleAsset = asset as PassiveModuleAsset;
	}
}
