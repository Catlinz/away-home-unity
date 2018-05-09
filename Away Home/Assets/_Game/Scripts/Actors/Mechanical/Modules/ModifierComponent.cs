using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// This component, when added to a component with a ActorModule, turns the module 
/// into a passive module, providing a flat boost to some system or stat.
/// A passive module simply provides a flat or percentage boost to a ship stat.
/// </summary>
public class ModifierComponent : MonoBehaviour {

    #region FIELDS
    /// <summary>The name of the type of component to apply the modifier to.</summary>
    public string component;

    /// <summary>The modifier to apply to the provided component.</summary>
    public SystemModifier modifier;

    /** The actual type of the component */
    private System.Type _componentType;
    #endregion

    #region MODIFIER_METHODS

    /// <summary>
    /// Try and apply the SystemModifier to the SystemComponent.
    /// </summary>
    /// <returns>
    /// - ModuleResult.Success if the modifier was applied
    /// - ModuleResult.InvalidSystem if the SystemComponent wasn't found.
    /// </returns>
    public ModuleResult Apply() {
        // Try and get the component we will be modifying and apply the 
        // modifier to it.
        SystemComponent comp = GetComponentInParent(_componentType) as SystemComponent;
        if (comp) {
            comp.ReplaceModifier(modifier);
            return ModuleResult.Success;
        }
        else {
            Debug.LogError("[ModifierComponent] Failed to enable, SystemComponent[" + component + "] not found.");
            return ModuleResult.InvalidSystem;
        }
    }

    /// <summary>
    /// Try and remove the SystemModifier to the SystemComponent.
    /// </summary>
    /// <returns>
    /// - ModuleResult.Success if the modifier was remove
    /// - ModuleResult.InvalidSystem if the SystemComponent wasn't found.
    /// </returns>
    public ModuleResult Remove() {
        // Try and remove the modifier from the component.
        SystemComponent comp = GetComponent(_componentType) as SystemComponent;
        if (comp) {
            comp.RemoveModifier(modifier);
            return ModuleResult.Success;
        }
        else {
            Debug.LogError("[ModifierComponent] Failed to enable, SystemComponent[" + component + "] not found.");
            return ModuleResult.InvalidSystem;
        }
    }
    #endregion

    #region UNITY METHODS
    private void Awake() {
        // Try and get the actual type of the component for efficiency.
        Assembly asm = typeof(SystemComponent).Assembly;
        _componentType = asm.GetType(component);

        if (_componentType == null) {
            Debug.LogError("[ModifierComponent] Failed to get component type for '" + component + "'.");
        }
    }

    #endregion
}
