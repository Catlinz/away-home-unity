using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A component that represents a passive module that is installed on a ship.
/// A passive module simply provides a flat or percentage boost to a ship stat.
/// </summary>
public class PassiveModule : ActorModule
{

    /// <summary>Frees up the Idle CPU and Energy from the ships systems.</summary>
    /// <seealso cref="ActorModule.DisableModule"/>
    public override ModuleResult DisableModule() {
		ModuleResult disabled = base.DisableModule();
		if (disabled == ModuleResult.Success) {
			return ModuleResult.Success;
		}
		else {
			return disabled;
		}
	}

    /// <summary>Allocates the idle CPU and reserves the idle energy required.</summary>
	/// <seealso cref="ActorModule.EnableModule"/>
	public override ModuleResult EnableModule() {
		ModuleResult enabled = base.EnableModule();
		if (enabled == ModuleResult.Success) {
			return ModuleResult.Success;
		}
		else {
			return enabled;
		}
	}
}
