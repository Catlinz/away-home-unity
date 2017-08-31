using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveModule : MonoBehaviour, IShipModule {

	private PassiveModuleAsset moduleAsset;

	public float IdlePowerUsage {
		get { return moduleAsset.idlePowerUsage; }
	}

	public float IdleCpuUsage {
		get { return moduleAsset.idleCpuUsage; }
	}

	public virtual void EnableOnShip(ShipActorComponent ship) {
		ship.ConsumeCpu(IdleCpuUsage);
		ship.ConsumePower(IdlePowerUsage);
	}

	/**
	 * Initialize the passive module from the asset.
	 */
	public void InitFromAsset(InstallableModuleAsset asset) {
		moduleAsset = asset as PassiveModuleAsset;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
