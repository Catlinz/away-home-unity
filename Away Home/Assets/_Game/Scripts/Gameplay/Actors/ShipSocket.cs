using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ShipSocket {

	/** The name of the socket */
	public string socketName;

	/** The max CPU that the slot can supply. */
	public ModifiableFloat maxCpuBandwidth;

	/** The max power that the slot can supply. */
	public ModifiableFloat maxPowerOutput;

	/** The minimum pitch angle */
	public float minPitch;

	/** The maximum pitch angle */
	public float maxPitch;

	/** The start of the allowed Yaw range (sweeps clockwise) */
	public float yawStart;

	/** The end of the allowed Yaw range (sweeps clockwise) */
	public float yawEnd;

	public Vector3 position;
	public Quaternion rotation;

	//	public IShipModule InstallModule(InstallableModuleAsset asset) {
	//		IShipModule mod = gameObject.AddComponent(asset.GetScriptType()) as IShipModule;
	//		mod.InitFromAssetInSocket(asset, this);
	//		return mod;
	//	}
}