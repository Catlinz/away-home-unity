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

	/// <summary>The amount / state of the armor for the ship.</summary>
	public ArmorSystem armor;

    /// <summary>The state of the computer system for the ship.</summary>
	public ComputerSystem computer;

    /// <summary>The state of the power system for the ship.</summary>
    public PowerSystem power;

	/// <summary>The system that holds and manages all the Modules attached to the ship.</summary>
	public ModuleSystem modules;


    public InstallableModuleAsset test;

	/// <summary>The ships movement is controlled by this.</summary>
	private ShipMovementComponent movement;

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
		ShipSocket socket = modules.GetSocket(socketName);

		// See if we can install the module into the socket.
		OperationResult canInstall = modules.CanInstallInSocket(asset, socket);
		if (canInstall.status != OperationResult.Status.OK) {
			return canInstall; /* Return the reason we failed to install. */
		}

		// Create The prefab and attach it to the root object in the right place.
		if (asset.prefab == null) {
			return new OperationResult(OperationResult.Status.FAIL, "Cannot install asset, invalid Prefab.");
		}

        GameObject go = GameObject.Instantiate(asset.prefab, socket.position, socket.rotation, gameObject.transform);
		ShipModuleClass mod = go.GetComponent<ShipModuleClass>();
		modules.InitFromAssetInSocket(mod, asset, socket);

		// Try and enable the component, if we can.
		return modules.TryEnable(mod, this);
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
