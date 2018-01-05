using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The ModuleSystem is responsible for storing the list of ShipSockets that the ship has, 
/// as well as managing all the installed modules for the ship.
/// </summary>
[System.Serializable]
public class ModuleSystem {

    /// <summary>The reason why a module was removed.</summary>
    public enum RemovedReason { Uninstalled, Destroyed };

    /// <summary>The delegate for when a new Module is installed on the ship.</summary>
    /// <param name="module">The module that was installed.</param>
    public delegate void ModuleInstalled(ShipModuleClass module);
    /// <summary>The event that is fired when a new module was installed.</summary>
    public event ModuleInstalled onModuleInstalled;

    /// <summary>The delegate for when a Module is removed from the ship.</summary>
    /// <param name="module">The module that was removed.</param>
    /// <param name="reason">The reason why the module was removed.</param>
    public delegate void ModuleRemoved(ShipModuleClass module, RemovedReason reason);
    /// <summary>The event that is fired when a Module is removed.</summary>
    public event ModuleRemoved onModuleRemoved;

    /// <summary>The delegate for when a module is enabled.</summary>
    /// <param name="module">The module that was enabled.</param>
    public delegate void ModuleEnabled(ShipModuleClass module);
    /// <summary>The event that is fired when a Module is enabled.</summary>
    public event ModuleEnabled onModuleEnabled;

    /// <summary>The delegate for when a module is disabled.</summary>
    /// <param name="module">The module that was disabled.</param>
    public delegate void ModuleDisabled(ShipModuleClass module);
    /// <summary>The even that is fired when a module is disabled.</summary>
    public event ModuleDisabled onModuleDisabled;

    /// <summary>The list of ShipSockets that the ship has.</summary>
	public ShipSocket[] sockets;

    /// <summary>
	/// The list of <c>ShipSocketGroup</c>s for the ship.  Always has at least 
    /// "Primary", "Secondary", "Utility" and "Passive" groups.
    /// </summary>
    public ShipSocketGroup[] groups;

    /// <summary>
    /// Creates a new <c>ModuleSystem</c>.  By default, it creates four socket groups: 
    /// <list type="bullet">
    /// <item>
    /// <description>Primary: For the primary weapons.</description>
    /// </item>
    /// <item>
    /// <description>Secondary: For the secondary weapons.</description>
    /// </item>
    /// <item>
    /// <description>Utility: For the utility modules.</description>
    /// </item>
    /// <item>
    /// <description>Passive: For the passive modules.</description>
    /// </item>
    /// </list>
    /// </summary>
	public ModuleSystem() {
        groups = new ShipSocketGroup[(int)ShipSocketGroup.Index.Passive + 1];
        groups[(int)ShipSocketGroup.Index.Primary] = new ShipSocketGroup("Primary", ShipSocketGroup.Flags.Trigger);
		groups[(int)ShipSocketGroup.Index.Secondary] = new ShipSocketGroup("Secondary", ShipSocketGroup.Flags.Trigger);
		groups[(int)ShipSocketGroup.Index.Utility] = new ShipSocketGroup("Utility", ShipSocketGroup.Flags.Active | ShipSocketGroup.Flags.Trigger);
		groups[(int)ShipSocketGroup.Index.Passive] = new ShipSocketGroup("Passive", ShipSocketGroup.Flags.Passive);
	}

	/// <summary>
	/// Check to see if a module can be enabled, that is, if there is enough 
	/// resources for it to be enabled.
	/// </summary>
	/// <param name="module">The ship module to enable.</param>
	/// <param name="ship">The ship that the ModuleSystem is attached to.</param>
	/// <returns>True if the ship has enough resources to enable the module.</returns>
	public bool CanEnable(ShipModuleClass module, ShipActorComponent ship) {
		return (ship.computer.IdleResources >= module.IdleCpuUsage) && (ship.power.FreeEnergy >= module.IdleEnergyDrain);
	}

	/// <summary>
	/// Check to see if we can install a module in a specific socket.  If the module 
	/// requires more CPU or power than the socket can supply, then the module cannot 
	/// be installed into it.
	/// </summary>
	/// <param name="asset">The InstallableModuleAsset to see if can be installed.</param>
	/// <param name="socket">The ShipSocket we want to install the module into.</param>
	/// <returns>
	/// An OperationResult indicating either why the module cannot be installed, or OK if the 
	/// module can be installed.
	/// </returns>
	public OperationResult CanInstallInSocket(InstallableModuleAsset moduleAsset, ShipSocket socket) {
        if (socket.Module != null) {
            return OperationResult.Fail("Cannot install Module, socket is already occupied.");
        }
		if (socket.maxEnergyOutput < moduleAsset.idleEnergyDrain) {
			return OperationResult.Fail("Cannot install Module in socket, not enough power output.");
		} 
		else if (socket.maxCpuBandwidth < moduleAsset.idleCpuUsage) { 
			return OperationResult.Fail("Cannot install Module in socket, not enough CPU bandwidth.");
		}
		else {
			return OperationResult.OK();
		}
	}

	/// <summary>
	/// Try and add the specified module / socket to the specified group.  If either the module
	/// or group cannot be found, then it returns a failed result.
	/// </summary>
	/// <param name="socketName">The name of the <c>ShipSocket</c> to add to the group.</param>
	/// <param name="groupName">The <c>ShipSocketGroup</c> to add the socket to.</param>
	/// <returns>An <c>OperationResult</c> that indicates whether or not the socket was successfully added.</returns>
	public OperationResult AddSocketToGroup(string socketName, string groupName) {
		ShipSocket toAdd = GetSocket(socketName);
		ShipSocketGroup addTo = GetSocketGroup(groupName);

		if (toAdd == null) {
			return OperationResult.Fail("Cannot find module " + socketName + " to add to group " + groupName + ".");
		}
		if (addTo == null) {
			return OperationResult.Fail("Cannot find group " + groupName + " to add the module to.");
		}

		// Make sure to remove the socket from any other socket groups first.
		ShipSocketGroup prevGroup = GetSocketGroupFor(socketName);
		if (prevGroup != null) {
			prevGroup.Remove(toAdd);
		}

		// Then add it to the new socket group.
		addTo.Add(toAdd);

		return OperationResult.OK();
	}

    /// <summary>
	/// Create a new <c>ShipSocketGroup</c> and add it to the list.
    /// </summary>
	/// <param name="newName">The name of the new <c>ShipSocketGroup</c> to create.</param>
    /// <returns>The newly created group, or null if a group with the same name already exists.</returns>
    public ShipSocketGroup CreateSocketGroup(string newName) {
        if (HasGroup(newName)) { return null; }

        ShipSocketGroup newGroup = new ShipSocketGroup(newName);
        groups = AHArray.Added(groups, newGroup);

        return newGroup;
    }

	/// <summary>
	/// Deletes the specified <c>ShipSocketGroup</c> from the list.
	/// </summary>
	/// <param name="groupName">The name of the <c>ShipSocketGroup</c> to remove.</param>
	public void DeleteSocketGroup(string groupName) {
		int index = -1;
		for (int i = 0; i < groups.Length; ++i) {
			if (groups[i].groupName == groupName) {
				index = i;
				break;
			}
		}
		if (index != -1) {
			groups = AHArray.Removed(groups, index);
		}
	}

    /// <summary>
	/// Get a <c>ShipSocketGroup</c> by name.
    /// </summary>
	/// <param name="groupName">The name of the <c>ShipSocketGroup</c> to get.</param>
	/// <returns>The <c>ShipSocketGroup</c> or null if it wasn't found.</returns>
    public ShipSocketGroup GetSocketGroup(string groupName) {
        int numGroups = groups.Length;
        for (int i = 0; i < numGroups; ++i) {
            if (groups[i].groupName == groupName) {
                return groups[i];
            }
        }
        return null;
    }

	/// <summary>
	/// Get the <c>ShipSocketGroup</c> (if any) that a <c>Socket</c> belongs to.
	/// </summary>
	/// <param name="socketName">The name of the <c>ShipSocket</c> to get the <c>ShipSocketGroup</c> for.</param>
	/// <returns>The group the socket belongs to (or null if it's not in a group)</returns>
	public ShipSocketGroup GetSocketGroupFor(string socketName) {
		for (int i = 0; i < groups.Length; ++i) {
			if (groups[i].Contains(socketName)) {
				return groups[i];
			}
		}
		return null;
	}

	/// <summary>
	/// Get the details about a ship socket by name.
	/// </summary>
	/// <param name="socketName">The name of the socket to get.</param>
	/// <returns>The ship socket, if found, otherwise returns null.</returns>
	public ShipSocket GetSocket(string socketName) {
		for (int i = 0; i < sockets.Length; ++i) {
			if (sockets[i].socketName == socketName) {
				return sockets[i];
			}
		}
		return null;
	}

    /// <summary>
    /// Get the ShipSocket with the provided Module installed.
    /// </summary>
    /// <param name="module">The module to find the socket for.</param>
    /// <returns>The ShipSocket or null if none are found.</returns>
    public ShipSocket GetSocket(ShipModuleClass module) {
        for (int i = 0; i < sockets.Length; ++i) {
            if (sockets[i].Module == module) {
                return sockets[i];
            }
        }
        return null;
    }

    /// <summary>
	/// Check to see if a <c>ShipSocketGroup</c> with the provided name exists.
    /// </summary>
	/// <param name="groupName">The name of the <c>ShipSocketGroup</c> to search for.</param>
    /// <returns>True if the the group exists already.</returns>
    public bool HasGroup(string groupName) {
        int numGroups = groups.Length;
        for (int i = 0; i < numGroups; ++i) {
            if (groups[i].groupName == groupName) { return true; }
        }
        return false;
    }

    /// <summary>
    /// Called to initialize the module after it has been created and physically added to 
    /// the ship GameObject.  This method makes sure the Module is initialized from the asset and 
    /// socket values, and added to the appropriate group or array if it is an Active or Passive module.
    /// <para>Triggers the onModuleInstalled event.</para>
    /// </summary>
    /// <param name="module">The ship module to initialise.</param>
    /// <param name="moduleAsset">The asset to initialise the module from.</param>
    /// <param name="socket">The socket that the module is installed into.</param>
	public void InitFromAssetInSocket(ShipModuleClass module, InstallableModuleAsset moduleAsset, ShipSocket socket) {
		module.InitFromAssetInSocket(moduleAsset, socket);

        // If the module is a passive module, then handle it.
		ShipModuleClass.TypeFlags typeFlags = module.GetTypeFlags();
		if ((typeFlags & ShipModuleClass.TypeFlags.Passive) == ShipModuleClass.TypeFlags.Passive) {
			groups[(int)ShipSocketGroup.Index.Passive].Add(socket);
		}
        else {
            // The module is an active module, so add it to the appriopriate activation group.
            //TODO: Add to the appropriate activation group.
        }

        // Notify any ModuleInstalled delegates.
        if (onModuleInstalled != null) { onModuleInstalled(module); }
	}

    /// <summary>
    /// Remove a module from the ship.
    /// <para>First disables the module (if enabled), then Triggers the onModuleRemoved event.</para>
    /// </summary>
    /// <param name="module">The module to remove from the ship.</param>
    /// <param name="ship">The ship to remove the module from.</param>
    /// <param name="reason">The reason the module is being removed.  Defaults to Uninstalled.</param>
    public InstallableModuleAsset RemoveModuleFrom(ShipSocket socket, ShipActorComponent ship, RemovedReason reason = RemovedReason.Uninstalled) {
        TryDisable(socket, ship);

        ShipModuleClass module = socket.Module;

        // Reset the socket that the module was in.
        ResetSocket(socket);

        if (module) {
            InstallableModuleAsset asset = module.Asset;

            GameObject.Destroy(module.gameObject);
            // Notify any of the ModuleRemoved delegates.
            if (onModuleRemoved != null) { onModuleRemoved(module, reason); }
            return asset;
        }
        return null;
    }

    /// <summary>
    /// Reset a socket when a module is removed from it.  This will remove it from any groups 
    /// and clear the socket.
    /// </summary>
    /// <param name="socket">The Socket to reset.</param>
    public void ResetSocket(ShipSocket socket) {
        // Make sure the socket isn't in any groups anymore.
        for (int i = 0; i < groups.Length; ++i) {
            groups[i].Remove(socket);
        }
        // Clear anything in the socket.
        socket.Clear();
    }

    /// <summary>
    /// Try and disable the provided module.
    /// <para>Triggers the onModuleDisabled event.</para>
    /// </summary>
    /// <param name="socket">The socket to disable the module in.</param>
    /// <param name="ship">The ship that the module is installed on.</param>
    /// <returns>
    /// A PARTIAL OperationResult with a message if the module is already disabled,
    /// a FAIL if there is no module in the socket, otherwise it returns OperationResult.OK
    /// </returns>
    public OperationResult TryDisable(ShipSocket socket, ShipActorComponent ship) {
        if (socket.Module == null) { return OperationResult.Fail("No module to disable."); }

        ShipModuleClass module = socket.Module;
        if (!module.IsEnabled) {
            return OperationResult.Partial("Module is already disabled.");
        }

        module.DisableModule();

        if (onModuleDisabled != null) { onModuleDisabled(module); }
		return OperationResult.OK();
    }

    /// <summary>
    /// Try to enable the ship module on the ship.
    /// <para>Triggers the onModuleEnabled event.</para>
    /// </summary>
    /// <param name="socket">The socket to enable the module on.</param>
    /// <param name="ship">The ship that the ModuleSystem belongs to.</param>
    /// <returns>
    /// OperationResult.OK if the module is enabled, but if it cannot be enabled, it 
    /// returns OperationResult.Status.PARTIAL and a message saying why it can't be enabled.
    /// </returns>
	public OperationResult TryEnable(ShipSocket socket, ShipActorComponent ship) {
        if (socket.Module == null) {
            return OperationResult.Fail("No module to disable.");
        }

        ShipModuleClass module = socket.Module;

		if (CanEnable(module, ship)) {
			module.EnableModule();
            
            // Notify any ModuleEnabled delegates.
            if (onModuleEnabled != null) { onModuleEnabled(module); }

			return OperationResult.OK();
		}
		else {
			return OperationResult.Partial("Not enough power or CPU bandwidth to enable the Module.");
		}
	}
}
