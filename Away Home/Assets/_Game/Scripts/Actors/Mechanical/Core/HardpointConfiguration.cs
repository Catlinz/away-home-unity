using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HardpointConfiguration {

    #region FIELDS
    [Header("Setup")]
    /** The list of hardpoints for Targeted types of modules. */
    public Hardpoint[] targeted;

    /** The list of hardpoints for Utility types of modules. */
    public Hardpoint[] utility;

    /** The list of hardpoints for passive types of modules. */
    public Hardpoint[] passive;

    /** The list of hardpoints for structural types of modules. */
    public Hardpoint[] structures;

    /// <summary>Are there any active modules?</summary>
    public bool HasActiveModules {
        get { return _totalActive > 0; }
    }

    /// <summary>Are there any modules that have been disabled by something other than the user?</summary>
    public bool HasAutoDisabledModules {
        get { return _autoDisabled > 0; }
    }

    private System.Random _rand = new System.Random();

    private int _totalActive = 0;
    private int _autoDisabled = 0;
    #endregion

    #region EVENTS
    /// <summary>Event that is triggered when a change to a module occurs.</summary>
    public ActorModule.StatusChanged onChange;
    #endregion

    /// <summary>
    /// Do some checks before installing a module in a hardpoint to make sure
    /// that we can actually install it.
    /// </summary>
    public ModuleResult CanInstallModuleInto(Hardpoint hardpoint, GameObject prefab) {
        if (!hardpoint.IsEmpty) {
			return ModuleResult.HardpointNotEmpty;
        }
        if (prefab == null) {
            return ModuleResult.NoModule;
        }

        ActorModule module = prefab.GetComponent<ActorModule>();
        if (module == null) {
			return ModuleResult.NoModule;
        }
        if (!hardpoint.IsCompatible(module.socket)) {
			return ModuleResult.IncompatibleSocket;
        }
        return ModuleResult.Success;
    }

	/// <summary>
    /// Clears the provided Hardpoint of any Module and resets it to empty.
    /// </summary>
    public void Clear(Hardpoint hardpoint) {
        hardpoint.Clear();
    }

	/// <summary>
	/// Try and disable the module in the specified hardpoint.
	/// </summary>
	public ModuleResult DisableModuleIn(Hardpoint hardpoint, ActorModule.DisabledReason reason = ActorModule.DisabledReason.User) {
        if (hardpoint == null) {
            return ModuleResult.InvalidHardpoint;
        }
		if (hardpoint.IsEmpty) {
			return ModuleResult.NoModule;
		}
		else if (!hardpoint.Module.IsEnabled) {
			return ModuleResult.AlreadyDisabled;
		}
		else {
            ModuleResult result = hardpoint.Module.DisableModule(reason);
            if (result == ModuleResult.Success) {
                --_totalActive;
                if (reason == ActorModule.DisabledReason.ResourceLoss) {
                    ++_autoDisabled;
                }
                if (onChange != null) {
                    onChange(ActorModule.Change.Disabled, hardpoint.Module);
                }
            }
            return result;
		}
	}

    /// <summary>
    /// Try and disable a random module.  It will try and disable the more resource hungry ones first,
    /// then move onto the other ones.  It will return true once it has disabled one, or false if there 
    /// were none to disable.
    /// </summary>
    public bool DisableRandomModule() {
        return (DisableRandomModuleInArray(targeted) ||
                DisableRandomModuleInArray(utility) ||
                DisableRandomModuleInArray(passive) ||
                DisableRandomModuleInArray(structures)); 
    }

	/// <summary>
	/// Try and enable the module in the specified hardpoint.
	/// </summary>
	public ModuleResult EnableModuleIn(Hardpoint hardpoint) {
		if (hardpoint.IsEmpty) {
			return ModuleResult.NoModule;
		}
		else {
            ActorModule.DisabledReason disabledReason = hardpoint.Module.DisabledBy;

			ModuleResult res = hardpoint.Module.EnableModule();
            if (res == ModuleResult.Success) {
                ++_totalActive;
                if (disabledReason != ActorModule.DisabledReason.User) {
                    --_autoDisabled;
                }
                if (onChange != null) {
                    onChange(ActorModule.Change.Enabled, hardpoint.Module);
                }
            }
            return res;
		}
	}

    /// <summary>
    /// Try and enable a random module that was previously disabled by resource loss.
    /// Will try and enable lower resource using modules first, then move up to more 
    /// resource hungry modules.
    /// </summary>
    public bool EnableRandomModule() {
        return (EnableRandomModuleInArray(structures) ||
                EnableRandomModuleInArray(passive) || 
                EnableRandomModuleInArray(utility) ||
                EnableRandomModuleInArray(targeted));
    }

	/// <summary>
    /// Find and return the Hardpoint by name.
    /// </summary>
    public Hardpoint Get(string name) {
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
    public Hardpoint Get(ActorModule module) {
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

	// Get an array of all the hardpoints.  
	// TODO Remove this.
	public Hardpoint[] GetAll() {
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
	/// Gets the array of hardpoints for the specific type of socket.
	/// </summary>
	public Hardpoint[] GetAllCompatible(HPSocket socket) {
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

	/// <summary>
	/// Get the module that is currently in the specified hardpoint.
	/// </summary>
	public ActorModule GetModule(string hardpoint) {
		Hardpoint hp = Get(hardpoint);
		return (hp != null) ? hp.Module : null;
	}

	/// <summary>
	/// Get the module that is currently in the specified hardpoint.
	/// </summary>
	public ActorModule GetModule(Hardpoint hardpoint) {
		return (hardpoint != null) ? hardpoint.Module : null;
	}

	/// <summary>
	/// Register that a module was installed into a hardpoint after the fact.
	/// </summary>
	public void RegisterInstalledModuleIn(Hardpoint hardpoint, ActorModule module) {
        // TODO: Do something here.
        if (onChange != null) {
            onChange(ActorModule.Change.Installed, module);
        }
    }

    /// <summary>
    /// Try and remove the currently installed module from the provided Hardpoint.
    /// </summary>
    /// <param name="reason">The reason why the module was being removed.</param>
    /// <param name="prefab">Holds the prefab that was used to create the module, or null.</param>
    public ModuleResult RemoveModuleFrom(Hardpoint hardpoint, ActorModule.Change reason, out GameObject prefab) {
        ModuleResult result = DisableModuleIn(hardpoint);
        if (result != ModuleResult.Success && result != ModuleResult.AlreadyDisabled) {
            prefab = null;
            return result;
        }

        ActorModule module = hardpoint.Module;
        Clear(hardpoint);

        if (onChange != null) {
            onChange(reason, module);
        }

        prefab = module.DestroyModule();
        return ModuleResult.Success;
    }

    private bool DisableRandomModuleInArray(Hardpoint[] hardpoints) {
        int count = (hardpoints != null) ? hardpoints.Length : 0;
        if (count > 0) {
            int index = _rand.Next(0, count);
            for (int i = 0; i < count; ++i) {
                if (hardpoints[index].IsOnline) {
                    DisableModuleIn(hardpoints[index], ActorModule.DisabledReason.ResourceLoss);
                    return true;
                }

                index = (index + 1) % count;
            }
        }
        return false;
    }

    private bool EnableRandomModuleInArray(Hardpoint[] hardpoints) {
        int count = (hardpoints != null) ? hardpoints.Length : 0;
        if (count > 0) {
            int index = _rand.Next(0, count);
            for (int i = 0; i < count; ++i) {
                if (hardpoints[index].IsOffline && hardpoints[index].Module.DisabledBy == ActorModule.DisabledReason.ResourceLoss) {
                    if (EnableModuleIn(hardpoints[index]) == ModuleResult.Success) {
                        return true;
                    }
                }

                index = (index + 1) % count;
            }
        }
        return false;
    }
}
