using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IShipModule {

	float IdlePowerUsage { get; }
	float IdleCpuUsage { get; }

	/**
	 * Enable the comonent installed on the specified ship.
	 */
	void EnableOnShip(ShipActorComponent ship);

	/**
	 * Initialize the component from the specified asset.
	 */
	void InitFromAssetInSocket(InstallableModuleAsset asset, ShipSocketComponent socket);

}
