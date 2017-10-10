using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A component that represents a passive module that is installed on a ship.
/// A passive module simply provides a flat or percentage boost to a ship stat.
/// </summary>
public class PassiveModule : ShipModuleClass { 

	/// <summary>Frees up the Idle CPU and Energy from the ships systems.</summary>
	/// <seealso cref="ShipModuleClass.DisableModule"/>
	public override ModuleResult DisableModule() {
		return base.DisableModule();
	}

    /// <summary>Allocates the idle CPU and reserves the idle energy required.</summary>
	/// <seealso cref="ShipModuleClass.EnableModule"/>
	public override ModuleResult EnableModule() {
		return base.EnableModule();
	}

	/// <summary>
	/// Returns that this is a Passive module.
	/// </summary>
	public override ShipModuleClass.TypeFlags GetTypeFlags() {
		return ShipModuleClass.TypeFlags.Passive;
	}

	/// <summary>Initializes the Module component from an asset for the module.</summary>
	/// <seealso cref="ShipModuleClass.InitFromAssetInSocket"/>
	public override ModuleResult InitFromAssetInSocket(InstallableModuleAsset asset, ShipSocket socket) {
		return base.InitFromAssetInSocket(asset, socket);
	}
}
