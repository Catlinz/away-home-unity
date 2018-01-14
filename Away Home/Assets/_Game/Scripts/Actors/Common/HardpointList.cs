using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HardpointList {

	[Header("Setup")]
    /** The list of hardpoints for Targeted types of modules. */
    public Hardpoint[] targeted;

    /** The list of hardpoints for Utility types of modules. */
    public Hardpoint[] utility;

    /** The list of hardpoints for passive types of modules. */
    public Hardpoint[] passive;

    /** The list of hardpoints for structural types of modules. */
    public Hardpoint[] structures;

	/// <summary>
    /// Do some checks before installing a module in a hardpoint to make sure
    /// that we can actually install it.
    /// </summary>
    public ModuleResult CanInstallModuleInto(Hardpoint hardpoint, GameObject prefab) {
        if (!hardpoint.IsEmpty) {
			return ModuleResult.HardpointNotEmpty;
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
	public ModuleResult DisableModuleIn(Hardpoint hardpoint) {
		if (hardpoint.IsEmpty) {
			return ModuleResult.NoModule;
		}
		else if (!hardpoint.Module.IsEnabled) {
			return ModuleResult.AlreadyDisabled;
		}
		else {
			return hardpoint.Module.DisableModule();
		}
	}

	/// <summary>
	/// Try and enable the module in the specified hardpoint.
	/// </summary>
	public ModuleResult EnableModuleIn(Hardpoint hardpoint) {
		if (hardpoint.IsEmpty) {
			return ModuleResult.NoModule;
		}
		else {
			return hardpoint.Module.EnableModule();
		}
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
	/// Do a pre-install setup and checking to install a module into a hardpoint.
	/// </summary>
	public ModuleResult InstallModuleIn(Hardpoint hardpoint, GameObject prefab) {
		if (hardpoint == null) {
			return ModuleResult.InvalidHardpoint;
		}
		else if (prefab == null) {
			return ModuleResult.NoModule;
		}
		else {
			ModuleResult res = CanInstallModuleInto(hardpoint, prefab);
			if (res == ModuleResult.Success) {
				// TODO: Do something here.
			}
			return res;
		}
	}
}
