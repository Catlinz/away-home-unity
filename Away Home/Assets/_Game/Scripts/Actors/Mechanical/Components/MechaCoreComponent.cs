using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechaCoreComponent : MonoBehaviour {

    #region FIELDS
    [Header("Mass")]
    /// <summary>The mass of the object. (Kilograms)</summary>
    public ModifiableInt mass;

    [Header("Size")]
    /// <summary>The length of the object. (Meters)</summary>
    public int length;

    /// <summary>The width of the object. (Meters)</summary>
    public int width;

    /// <summary>The height of the object. (Meters)</summary>
    public int height;

    [Header("Reasources")]
    ///<summary>The power system that represents the reactor.</summary>
    public PowerSystem power;
    ///<summary>The computer system</summary>
    public ComputerSystem computer;

    [Header("Hardpoints")]
    /** The list of all the hardpoints. */
    public HardpointConfiguration hardpoints;

    /** The list of all systems on the actor. */
    public SystemList systems = new SystemList();

    protected SystemModifierList _modifiers;
    #endregion

    #region EVENTS
    /// <summary>Event that is fired when a module is installed, removed, enabled or disabled.</summary>
    public event ActorModule.StatusChanged onModuleChange;
    #endregion

    #region MODULE METHODS

    /// <summary>
    /// Disable the module that is currently installed in the provided hardpoint.
    /// Can return a Partial result if module was already disabled.
    /// </summary>
    public OperationResult DisableModule(Hardpoint hardpoint) {
        ModuleResult res = hardpoints.DisableModuleIn(hardpoint);
        switch (res) {
            case ModuleResult.NoModule:
                return OperationResult.Fail("No Module to disable");
            case ModuleResult.AlreadyDisabled:
                return OperationResult.Partial("Module is already disabled");
            default:
                if (onModuleChange != null) { 
                    onModuleChange(ActorModule.Change.Disabled, hardpoint.Module);
                }
                return OperationResult.OK();
        }
    }

    /// <summary>
    /// Try and enable the module installed in the provided Hardpoint.
    /// Can return a Partial result if the module exists, but there is not 
    /// enough energy or computer resources to enable the module.
    /// </summary>
    public OperationResult EnableModule(Hardpoint hardpoint) {
        ModuleResult res = hardpoints.EnableModuleIn(hardpoint);
        switch (res) {
            case ModuleResult.Success:
                if (onModuleChange != null) {
                    onModuleChange(ActorModule.Change.Enabled, hardpoint.Module);
                }
                return OperationResult.OK();
            case ModuleResult.NoModule:
                return OperationResult.Fail("No Module to disable");
            case ModuleResult.AlreadyEnabled:
                return OperationResult.OK("Module already enabled");
            case ModuleResult.InsufficientPower: 
                return OperationResult.Partial("Not enough energy to activate Module");
            case ModuleResult.InsufficientCpu:
                return OperationResult.Partial("Not enough CPU");
            default:
                return OperationResult.Fail("Failed because: " + res.ToString());
        }
    }

    /// <summary>
    /// Try and install a module in a provided hardpoint.  If the module is 
    /// installed, we will try to enable it.
    /// </summary>
    public OperationResult InstallModuleIn(string hardpointName, GameObject prefab) {
        return InstallModuleIn(hardpoints.Get(hardpointName), prefab);
    }
    public OperationResult InstallModuleIn(Hardpoint hardpoint, GameObject prefab) {
        ModuleResult res = hardpoints.CanInstallModuleInto(hardpoint, prefab);
        switch (res) {
            case ModuleResult.Success:
                ActorModule module = ActorModule.Instantiate(prefab, hardpoint, gameObject);
                if (onModuleChange != null) {
                    onModuleChange(ActorModule.Change.Installed, module);
                }
                hardpoints.RegisterModuleIn(hardpoint, module);
                return EnableModule(hardpoint);
            case ModuleResult.HardpointNotEmpty:
                return OperationResult.Fail("Selected hardpoint is not empty");
            case ModuleResult.IncompatibleSocket:
                return OperationResult.Fail("Module cannot be installed in selected hardpoint");
            default:
                return OperationResult.Fail("Failed to install Module: " + res.ToString());
        }
    }

    /// <summary>
    /// Remove any installed module from the provided Hardpoint, with the provided 
    /// removal reason (Uninstalled or Destroyed).  
    ///
    /// If there was a module installed, 
    /// we try to return the prefab that the module was instantiated from.
    /// </summary>
    public GameObject RemoveModuleFrom(Hardpoint hardpoint,
                                       ActorModule.Change reason = ActorModule.Change.Uninstalled) {
        // TODO Move this into HardpointList.
        if (hardpoint == null) { return null; }

        DisableModule(hardpoint); // Don't care if this fails.

        ActorModule module = hardpoint.Module;

        // Reset the hardpoint that the module was installed in.
        hardpoints.Clear(hardpoint);

        if (module != null) {
            onModuleChange(reason, module);
            return module.DestroyModule();
        }

        return null;

    }
    #endregion

    #region  MODIFIER METHODS
    /// <summary>
    /// Adds a modifier to the System Component.
    /// </summary>
    /// <param name="modifier">The SystemModifier to add.</param>
    public void AddModifier(SystemModifier modifier) {
        _modifiers.Add(modifier);
        RecalculateStat(modifier.stat);
    }

    /// <summary>
    /// Removes a modifier from the System Component.
    /// </summary>
    /// <param name="modifier">The ISystemModifier to remove.  Uses Equals().</param>
    public void RemoveModifier(SystemModifier modifier) {
        if (_modifiers.Remove(modifier)) {
            RecalculateStat(modifier.stat);
        }
    }

    /// <summary>
    /// Replaces an existing modifier with a new one, or adds a new one if it doesn't exist yet.
    /// </summary>
    /// <param name="modifier">The SystemModifier to add or replace.</param>
    public void ReplaceModifier(SystemModifier modifier) {
        _modifiers.Replace(modifier);
        RecalculateStat(modifier.stat);
    }
    ///<summary>
    ///Recalculates the specified stat based on the current modifiers.
    ///</summary>
    private void RecalculateStat(ModifiableStat stat) {
        float multiplier, delta;
        _modifiers.Get(stat, out multiplier, out delta);

        switch (stat) {
            case ModifiableStat.ComputerResources:
                computer.SetTotalCpu(delta, multiplier);
                break;
            case ModifiableStat.EnergyRecharge:
                power.SetEnergyRecharge(delta, multiplier);
                break;
            case ModifiableStat.EnergyCapacity:
                power.SetEnergyCapacity(delta, multiplier);
                break;
            case ModifiableStat.Mass:
                mass.added = (int)delta;
                mass.modifier = multiplier;
                break;
            default:
                break;
        }

        // TODO Implement the OverclockDamage modifier.
    }
    #endregion

    #region HANDLERS
    private void HandleResourcesChanged(float upOrDown) {
        if (upOrDown > 0) {
            // Try and enable any auto disabled systems.
            while (systems.HasDisabledAuto) {
                if (!systems.EnableRandom()) { break; }
            }
            
            // Then try and enable any auto disabled modules.
            while (hardpoints.HasAutoDisabledModules) {
                if (!hardpoints.EnableRandomModule()) { break; }
            }
        }
        else {
            // First, try disabling random modules until we are back in positive
            // resources.
            while ((power.FreeEnergy < 0 || computer.IdleResources < 0) && hardpoints.HasActiveModules) {
                if (!hardpoints.DisableRandomModule()) { break; }
            }

            // If still don't have enough resources, then try disabling systems
            while ((power.FreeEnergy < 0 || computer.IdleResources < 0)) {
                if (!systems.DisableRandom()) { break; }
            }
        }
    }
    #endregion

    #region HOOKS
    void Awake() {
        _modifiers = new SystemModifierList(4);
        power.onEnergyChanged += HandleResourcesChanged;
    }
	
	// Update is called once per frame
	void Update () {
		power.Tick(Time.deltaTime);
        computer.Tick(Time.deltaTime);
	}
    #endregion
}
