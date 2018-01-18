using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// A component that represents a passive module that is installed on a ship.
/// A passive module simply provides a flat or percentage boost to a ship stat.
/// </summary>
public class PassiveModule : ActorModule
{

    #region FIELDS
    /// <summary>The name of the type of component to apply the modifier to.</summary>
    public string component;

    /// <summary>The modifier to apply to the provided component.</summary>
    public SystemModifier modifier;

    /** The actual type of the component */
    private System.Type _componentType;
    #endregion

    #region MODULE METHODS
    /// <summary>Frees up the Idle CPU and Energy from the ships systems.</summary>
    /// <seealso cref="ActorModule.DisableModule"/>
    public override ModuleResult DisableModule(DisabledReason reason = DisabledReason.User) {
        // First, deallocate the resources the module is using.
		ModuleResult disabled = base.DisableModule(reason);

		if (disabled == ModuleResult.Success) {
            // Then, remove the modifier from the component.
            SystemComponent comp = GetComponent(_componentType) as SystemComponent;
            if (comp) {
                comp.RemoveModifier(modifier);
            }
            else {
                Debug.LogError("[PassiveModule] Failed to enable, SystemComponent[" + component + "] not found.");
            }
            return disabled;
        }
		else {
			return disabled;
		}
	}

    /// <summary>Allocates the idle CPU and reserves the idle energy required.</summary>
	/// <seealso cref="ActorModule.EnableModule"/>
	public override ModuleResult EnableModule() {
        // First, get the power and CPU needed.
		ModuleResult enabled = base.EnableModule();

		if (enabled == ModuleResult.Success) {
            // Now, try and get the component we will be modifying and apply the 
            // modifier to it.
            SystemComponent comp = GetComponentInParent(_componentType) as SystemComponent;
            if (comp) {
                comp.ReplaceModifier(modifier);
                return enabled;
            }
            else {
                Debug.LogError("[PassiveModule] Failed to enable, SystemComponent[" + component + "] not found.");
                base.DisableModule();
                return ModuleResult.InvalidSystem;
            }
		}
		else {
			return enabled;
		}
	}
    #endregion

    #region UNITY METHODS
    private void Awake() {
        // Try and get the actual type of the component for efficiency.
        Assembly asm = typeof(SystemComponent).Assembly;
        _componentType = asm.GetType(component);

        if (_componentType == null) {
            Debug.LogError("[PassiveModule] Failed to get component type for '" + component + "'.");
        }
    }

    #endregion
}
