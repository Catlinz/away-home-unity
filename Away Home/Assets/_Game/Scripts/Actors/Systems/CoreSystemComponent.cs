using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreSystemComponent : MonoBehaviour {

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

    [Header("Setup")]
    /** The list of hardpoints for Targeted types of modules. */
    public Hardpoint[] targeted;

    /** The list of hardpoints for Utility types of modules. */
    public Hardpoint[] utility;

    /** The list of hardpoints for passive types of modules. */
    public Hardpoint[] passive;

    /** The list of hardpoints for structural types of modules. */
    public Hardpoint[] structures;

    protected SystemModifierList _modifiers;
    #endregion

    #region EVENTS
    /// <summary>Event that is fired when a module is installed, removed, enabled or disabled.</summary>
    public event ActorModule.StatusChanged onModuleChange;
    #endregion

    #region MODULE METHODS
    /// <summary>Check to see if we have the resources required to enable a module.</summary>
    public bool CanEnableModule(ActorModule module) {
        return (computer.IdleResources >= module.idleComputerResources) &&
                (power.FreeEnergy >= module.idleEnergyDrain);
    }

    /// <summary>
    /// Do some checks before installing a module in a hardpoint to make sure
    /// that we can actually install it.
    /// </summary>
    public OperationResult CanInstallModuleIn(Hardpoint hardPoint, GameObject prefab) {
        if (!hardPoint.IsEmpty) {
            return OperationResult.Fail("Hardpoint is already occupied.");
        }

        ActorModule module = prefab.GetComponent<ActorModule>();
        if (module == null) {
            return OperationResult.Fail("Could not load Module from Prefab.");
        }
        if (!hardPoint.IsCompatible(module.socket)) {
            return OperationResult.Fail("Module is not compatible with the hardpoint socket.");
        }
        return OperationResult.OK();
    }

    /// <summary>
    /// Clears the provided Hardpoint of any Module and resets it to empty.
    /// </summary>
    public void ClearHardpoint(Hardpoint hardpoint) {
        hardpoint.Clear();
    }

    /// <summary>
    /// Disable the module that is currently installed in the provided hardpoint.
    /// Can return a Partial result if module was already disabled.
    /// </summary>
    public OperationResult DisableModule(Hardpoint hardpoint) {
        if (hardpoint.IsEmpty) {
            return OperationResult.Fail("No Module to disable.");
        }

        ActorModule module = hardpoint.Module;
        if (!module.IsEnabled) {
            return OperationResult.Partial("Module is already disabled.");
        }

        module.DisableModule();

        if (onModuleChange != null) {
            onModuleChange(ActorModule.Change.Disabled, module);
        }
        return OperationResult.OK();
    }

    /// <summary>
    /// Try and enable the module installed in the provided Hardpoint.
    /// Can return a Partial result if the module exists, but there is not 
    /// enough energy or computer resources to enable the module.
    /// </summary>
    public OperationResult EnableModule(Hardpoint hardpoint) {
        if (hardpoint.IsEmpty) {
            return OperationResult.Fail("No Module to disable.");
        }

        ActorModule module = hardpoint.Module;

        if (CanEnableModule(module)) {
            module.EnableModule();

            if (onModuleChange != null) {
                onModuleChange(ActorModule.Change.Enabled, module);
            }
            return OperationResult.OK();
        }
        else {
            if (module.idleEnergyDrain > power.FreeEnergy) {
                return OperationResult.Partial("Not enough energy to enable Module.");
            }
            else {
                return OperationResult.Partial("Not enough computer resources to enable Module.");
            }
        }
    }

    public Hardpoint[] GetAllHardpoints() {
        int count = ((targeted != null) ? targeted.Length : 0) +
                    ((utility != null) ? utility.Length : 0) +
                    ((passive != null) ? passive.Length : 0) +
                    ((structures != null) ? structures.Length : 0);

        Hardpoint[] all = new Hardpoint[count];

        int offset = 0;
        if (targeted != null) {
            for (int i = 0; i < targeted.Length; ++i) {
                all[i] = targeted[i];
            }
            offset += targeted.Length;
        }

        if (utility != null) {
            for (int i = 0; i < utility.Length; ++i) {
                all[i + offset] = utility[i];
            }
            offset += utility.Length;
        }

        if (passive != null) {
            for (int i = 0; i < passive.Length; ++i) {
                all[i + offset] = passive[i];
            }
            offset += passive.Length;
        }

        if (structures != null) {
            for (int i = 0; i < structures.Length; ++i) {
                all[i + offset] = structures[i];
            }
        }

        return all;
    }

    /// <summary>
    /// Find and return the Hardpoint by name.
    /// </summary>
    public Hardpoint GetHardpoint(string name) {
        // Search through the Targeted hardpoints.
        int numTargeted = (targeted != null) ? targeted.Length : 0;
        for (int i = 0; i < numTargeted; ++i) {
            if (targeted[i].name == name) { return targeted[i]; }
        }

        // Search through the utility hardpoints.
        int numUtility = (utility != null) ? utility.Length : 0;
        for (int i = 0; i < numUtility; ++i) {
            if (utility[i].name == name) { return utility[i]; }
        }

        // Search through the passive hardpoints.
        int numPassive = (passive != null) ? passive.Length : 0;
        for (int i = 0; i < numPassive; ++i) {
            if (passive[i].name == name) { return passive[i]; }
        }

        // Search through the structural hardpoints.
        int numStructural = (structures != null) ? structures.Length : 0;
        for (int i = 0; i < numStructural; ++i) {
            if (structures[i].name == name) { return structures[i]; }
        }

        // Return null if we didn't find it.
        return null;
    }

    /// <summary>
    /// Find and return the Hardpoint by installed module.
    /// </summary>
    public Hardpoint GetHardpoint(ActorModule module) {
        // Search through the Targeted hardpoints.
        int numTargeted = (targeted != null) ? targeted.Length : 0;
        for (int i = 0; i < numTargeted; ++i) {
            if (targeted[i].Module == module) { return targeted[i]; }
        }

        // Search through the utility hardpoints.
        int numUtility = (utility != null) ? utility.Length : 0;
        for (int i = 0; i < numUtility; ++i) {
            if (utility[i].Module == module) { return utility[i]; }
        }

        // Search through the passive hardpoints.
        int numPassive = (passive != null) ? passive.Length : 0;
        for (int i = 0; i < numPassive; ++i) {
            if (passive[i].Module == module) { return passive[i]; }
        }

        // Search through the structural hardpoints.
        int numStructural = (structures != null) ? structures.Length : 0;
        for (int i = 0; i < numStructural; ++i) {
            if (structures[i].Module == module) { return structures[i]; }
        }

        // Return null if we didn't find it.
        return null;
    }

    /// <summary>
    /// Try and install a module in a provided hardpoint.  If the module is 
    /// installed, we will try to enable it.
    /// </summary>
    public OperationResult InstallModuleIn(string hardpointName, GameObject prefab) {
        return InstallModuleIn(GetHardpoint(hardpointName), prefab);
    }
    public OperationResult InstallModuleIn(Hardpoint hardpoint, GameObject prefab) {
        if (hardpoint == null) {
            return OperationResult.Fail("No Hardpoint to install Module in.");
        }
        if (prefab == null) {
            return OperationResult.Fail("No Module to install.");
        }

        // See if we can install the module in the hardpoint.
        OperationResult canInstall = CanInstallModuleIn(hardpoint, prefab);
        if (canInstall != OperationResult.Status.OK) {
            return canInstall; /* Return the reason we failed to install. */
        }

        // Instantiate the module from the prefab.
        ActorModule module = ActorModule.Instantiate(prefab, hardpoint, gameObject);
        if (onModuleChange != null) {
            onModuleChange(ActorModule.Change.Installed, module);
        }

        // Try and enable the module, if we can.
        return EnableModule(hardpoint);
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
        if (hardpoint == null) { return null; }

        DisableModule(hardpoint); // Don't care if this fails.

        ActorModule module = hardpoint.Module;

        // Reset the hardpoint that the module was installed in.
        ClearHardpoint(hardpoint);

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

    #region HOOKS
    void Awake() {
        _modifiers = new SystemModifierList(4);
    }
	
	// Update is called once per frame
	void Update () {
		power.Tick(Time.deltaTime);
        computer.Tick(Time.deltaTime);
	}
    #endregion

    #region PRIVATE_METHODS
    /// <summary>
	/// Gets the array of hardpoints for the specific type of socket.
	/// </summary>
	private Hardpoint[] GetHardPoints(HPSocket socket) {
        int intSocket = (int)socket;

        if ((intSocket & (int)HardpointType.Targeted) != 0) {
            return targeted;
        }
        else if ((intSocket & (int)HardpointType.Utility) != 0) {
            return utility;
        }
        else if ((intSocket & (int)HardpointType.Passive) != 0) {
            return passive;
        }
        else if ((intSocket & (int)HardpointType.Structural) != 0) {
            return structures;
        }
        else {
            return null;
        }
    }

    
    #endregion
}
