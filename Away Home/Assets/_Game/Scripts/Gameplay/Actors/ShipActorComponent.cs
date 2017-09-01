using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The ShipActorComponent is the core foundation of the logic and data 
/// that makes an object into a ship.  
/// <para>
/// It contains the ship stats and info, the 
/// list of sockets that modules can be attached to and the logic for installing 
/// and interacting with the modules.
/// </para>
/// </summary>
[RequireComponent(typeof(ShipMovementComponent))]
public class ShipActorComponent : MonoBehaviour {

    /// <summary>The state of the computer system for the ship.</summary>
    public ComputerSystem computer;

    /// <summary>The state of the power system for the ship.</summary>
    public PowerSystem power;

    /// <summary>The list of ShipSockets that modules can be installed into.</summary>
	public ShipSocket[] sockets;

    /// <summary>The ships movement is controlled by this.</summary>
	private ShipMovementComponent movement;

    public InstallableModuleAsset test;

	/// <summary>
    /// Check to see if a module can be enabled, that is, if there is enough 
    /// resources for it to be enabled.
    /// </summary>
    /// <param name="module">The ship module to enable.</param>
    /// <returns>True if there are enough resources to enable the module.</returns>
	public bool CanEnableModule(IShipModule module) {
		return (computer.AvailableCpu >= module.IdleCpuUsage) && (power.AvailablePower >= module.IdlePowerUsage);
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
	public OperationResult CanInstallModule(InstallableModuleAsset asset, ShipSocket socket) {
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

	/// <summary>
    /// Install a module into the specified ShipSocket.  It can fail, and if it does will 
    /// return a message as to why it fails.
    /// </summary>
    /// <param name="asset">The installable module we want to install on the ship.</param>
    /// <param name="socketName">The name of the socket we want to install the module into.</param>
    /// <returns>
    /// An OperationResult that indicates if the module was installed or not.  If the module couldn't be installed, 
    /// a FAIL result is returned with a message indicating why.  If the module was installed but couldn't be enabled, 
    /// a PARTIAL result is returned with a message indicating why.  Otherwise, an OK result is returned.
    /// </returns>
	public OperationResult InstallModule(InstallableModuleAsset asset, string socketName) {
		ShipSocket socket = GetSocket(socketName);

		OperationResult canInstall = CanInstallModule(asset, socket);
		if (canInstall.status != OperationResult.Status.OK) {
			return canInstall; /* Return the reason we failed to install. */
		}

		// Create The prefab and attach it to the root object in the right place.
		if (asset.prefab == null) {
			return new OperationResult(OperationResult.Status.FAIL, "Cannot install asset, invalid Prefab.");
		}

        GameObject go = GameObject.Instantiate(asset.prefab, socket.position, socket.rotation, gameObject.transform);
        IShipModule mod = go.GetComponent<IShipModule>();
        mod.InitFromAssetInSocket(asset, socket);

		// Try and enable the component, if we can.
		if (CanEnableModule(mod)) {
			mod.EnableOnShip(this);
			return OperationResult.OK; 
		}
		else {
			return new OperationResult(OperationResult.Status.PARTIAL, "Not enough power or CPU bandwidth to enable the Module.");
		}
	}

	// Use this for initialization
	void Start () {
		movement = GetComponent<ShipMovementComponent>();
        InstallModule(test, "TEST");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
		


}
