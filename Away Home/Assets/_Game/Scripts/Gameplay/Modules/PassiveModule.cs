using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A component that represents a passive module that is installed on a ship.
/// A passive module simply provides a flat or percentage boost to a ship stat.
/// </summary>
public class PassiveModule : ShipModuleClass {

	/// <summary>Frees up the Idle CPU and Energy from the ships systems.</summary>
	/// <param name="ship">The ship to disable the module on.</param>
	public override void DisableOnShip(ShipActorComponent ship) {
		base.DisableOnShip(ship);
	}

    /// <summary>Allocates the idle CPU and reserves the idle energy required.</summary>
    /// <param name="ship">The ship to enable the Module for.</param>
	public override bool EnableOnShip(ShipActorComponent ship) {
		return base.EnableOnShip(ship);
	}

	/// <summary>Initializes the Module component from an asset for the module.</summary>
    /// <param name="asset">The module asset to initialize from.</param>
    /// <param name="socket">The ShipSocket that the module is being installed on.</param>
	public override void InitFromAssetInSocket(InstallableModuleAsset asset, ShipSocket socket) {
		base.InitFromAssetInSocket(asset, socket);
	}
}
