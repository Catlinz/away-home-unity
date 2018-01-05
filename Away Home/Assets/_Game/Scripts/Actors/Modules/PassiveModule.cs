using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A component that represents a passive module that is installed on a ship.
/// A passive module simply provides a flat or percentage boost to a ship stat.
/// </summary>
public class PassiveModule : ActorModuleClass
{

    /// <summary>Frees up the Idle CPU and Energy from the ships systems.</summary>
    /// <seealso cref="ActorModuleClass.DisableModule"/>
    public override ModuleResult DisableModule() {
		return base.DisableModule();
	}

    /// <summary>Allocates the idle CPU and reserves the idle energy required.</summary>
	/// <seealso cref="ActorModuleClass.EnableModule"/>
	public override ModuleResult EnableModule() {
		return base.EnableModule();
	}

	/// <summary>Initializes the Module component from an asset for the module.</summary>
	/// <seealso cref="ShipModuleClass.InitFromAssetInSocket"/>
	public override ModuleResult InitFromAssetInHardPoint(InstallableModuleAsset asset, HardPoint hardpoint) {
		return base.InitFromAssetInHardPoint(asset, hardpoint);
	}
}
