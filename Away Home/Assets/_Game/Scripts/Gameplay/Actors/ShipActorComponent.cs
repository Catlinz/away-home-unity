using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ShipMovementComponent))]
public class ShipActorComponent : MonoBehaviour {

	// The maximum Power the ship can supply.
	public ModifiableFloat maxPower = 0;	
	// The maximum CPU bandwidth the ship can supply.
	public ModifiableFloat maxCpu = 0;	

	private float usedPower = 0;
	private float usedCpu = 0;

	public float AvailablePower {
		get { return maxPower - usedPower; }
	}
	public float AvailableCpu {
		get { return maxCpu - usedCpu; }
	}

	public ShipSocket[] sockets;

	private ShipMovementComponent movement;

	/**
	 * Test to see if a component can be enabled based on power / CPU usage.
	 */
	public bool CanEnableModule(IShipModule module) {
		return (AvailableCpu >= module.IdleCpuUsage) && (AvailablePower >= module.IdlePowerUsage);
	}

	/**
	 * Test to see if we can install the specified component into the ship.
	 */
	public OperationResult CanInstallModule(InstallableModuleAsset asset, ShipSocketComponent socket) {
		if (socket.maxPowerOutput < asset.idlePowerUsage) {
			return new OperationResult(OperationResult.Status.FAIL, "Cannot install asset in socket, not enough power output.");
		} 
		else if (socket.maxCpuBandwidth < asset.idleCpuUsage) { 
			return new OperationResult(OperationResult.Status.FAIL, "Cannot install asset in socket, not enough CPU bandwidth.");
		}
		else {
			return OperationResult.OK;
		}
	}

	public void ConsumeCpu(float cpu) {
		usedCpu += cpu;
	}

	public void ConsumePower(float power) {
		usedPower += power;
	}

	/**
	 * Try and find the ShipSocketComponent with the specified name.
	 */
	public ShipSocketComponent GetSocket(string socketName) {
		ShipSocketComponent[] sockets = GetComponentsInChildren<ShipSocketComponent>(true);

		foreach (ShipSocketComponent socket in sockets) {
			if (socket.name == socketName) {
				return socket;
			}
		}

		return null;
	}

	/**
	 * Try and install the given component into the specified slot on the ship.
	 */
	public OperationResult InstallModule(InstallableModuleAsset asset, string socketName) {
		ShipSocketComponent socket = GetSocket(socketName);

		OperationResult canInstall = CanInstallModule(asset, socket);
		if (canInstall.status != OperationResult.Status.OK) {
			return canInstall; /* Return the reason we failed to install. */
		}

		// Create the appropriate component type and add it to the socket.
		System.Type scriptType = asset.GetScriptType();
		if (scriptType == null) {
			return new OperationResult(OperationResult.Status.FAIL, "Cannot install asset, invalid script type.");
		}
		IShipModule module = socket.InstallModule(asset);

		// Try and enable the component, if we can.
		if (CanEnableModule(module)) {
			module.EnableOnShip(this);
			return OperationResult.OK; 
		}
		else {
			return new OperationResult(OperationResult.Status.PARTIAL, "Not enough power or CPU bandwidth to enable the Module.");
		}




	}

	// Use this for initialization
	void Start () {
		movement = GetComponent<ShipMovementComponent>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
		


}
