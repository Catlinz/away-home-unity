using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ModuleSystem {

	public ShipSocket[] sockets;

	private ShipModuleClass[] passiveModules;

	public ModuleSystem() {

	}

	private void AddPassiveModule(ShipModuleClass module) {
		//TODO: Implemnet this.
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
		if (socket.maxEnergyOutput < moduleAsset.idleEnergyDrain) {
			return new OperationResult(OperationResult.Status.FAIL, "Cannot install asset in socket, not enough power output.");
		} 
		else if (socket.maxCpuBandwidth < moduleAsset.idleCpuUsage) { 
			return new OperationResult(OperationResult.Status.FAIL, "Cannot install asset in socket, not enough CPU bandwidth.");
		}
		else {
			return OperationResult.OK;
		}
	}

	/// <summary>
	/// Get the details about a ship socket by name.
	/// </summary>
	/// <param name="socketName">The name of the socket to get.</param>
	/// <returns>The ship socket, if found, otherwise returns the empty socket.</returns>
	public ShipSocket GetSocket(string socketName) {
		for (int i = 0; i < sockets.Length; ++i) {
			if (sockets[i].socketName == socketName) {
				return sockets[i];
			}
		}
		return ShipSocket.empty;
	}

	public void InitFromAssetInSocket(ShipModuleClass module, InstallableModuleAsset moduleAsset, ShipSocket socket) {
		module.InitFromAssetInSocket(moduleAsset, socket);

		if (module.GetModuleType() == ShipModuleClass.ModuleType.Passive) {
			AddPassiveModule(module);
		}

		//TODO: Add to the appropriate group if it is an active module.
	}

	public OperationResult TryEnable(ShipModuleClass module, ShipActorComponent ship) {
		if (CanEnable(module, ship)) {
			module.EnableOnShip(ship);
			return OperationResult.OK;
		}
		else {
			return new OperationResult(OperationResult.Status.PARTIAL, "Not enough power or CPU bandwidth to enable the Module.");
		}
	}
}
