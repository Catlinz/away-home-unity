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


	public ModuleSystem() {

	}

	/// <summary>
	/// Check to see if a module can be enabled, that is, if there is enough 
	/// resources for it to be enabled.
	/// </summary>
	/// <param name="module">The ship module to enable.</param>
	/// <param name="ship">The ship that the ModuleSystem is attached to.</param>
	/// <returns>True if the ship has enough resources to enable the module.</returns>
	public bool CanEnable(ShipModuleClass module, ShipActorComponent ship) {
		return (ship.computer.IdleCpu >= module.IdleCpuUsage) && (ship.power.FreeEnergy >= module.IdleEnergyDrain);
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
            return new OperationResult(OperationResult.Status.FAIL, "Cannot install Module, socket is already occupied.");
        }
		if (socket.maxEnergyOutput < moduleAsset.idleEnergyDrain) {
			return new OperationResult(OperationResult.Status.FAIL, "Cannot install Module in socket, not enough power output.");
		} 
		else if (socket.maxCpuBandwidth < moduleAsset.idleCpuUsage) { 
			return new OperationResult(OperationResult.Status.FAIL, "Cannot install Module in socket, not enough CPU bandwidth.");
		}
		else {
			return OperationResult.OK;
		}
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
		if (module.GetModuleType() == ShipModuleClass.ModuleType.Passive) {
			//AddPassiveModule(module);
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

        // Remove the module from the socket.
        socket.SetModule(null);

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
        if (socket.Module == null) { return new OperationResult(OperationResult.Status.FAIL, "No module to disable."); }

        ShipModuleClass module = socket.Module;
        if (!module.IsEnabled) {
            return new OperationResult(OperationResult.Status.PARTIAL, "Module is already disabled.");
        }

        module.DisableOnShip(ship);

        if (onModuleDisabled != null) { onModuleDisabled(module); }
        return OperationResult.OK;
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
            return new OperationResult(OperationResult.Status.FAIL, "No module to disable.");
        }

        ShipModuleClass module = socket.Module;

		if (CanEnable(module, ship)) {
			module.EnableOnShip(ship);
            
            // Notify any ModuleEnabled delegates.
            if (onModuleEnabled != null) { onModuleEnabled(module); }

			return OperationResult.OK;
		}
		else {
			return new OperationResult(OperationResult.Status.PARTIAL, "Not enough power or CPU bandwidth to enable the Module.");
		}
	}
}
