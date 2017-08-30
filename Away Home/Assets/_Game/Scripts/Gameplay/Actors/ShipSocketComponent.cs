using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSocketComponent : MonoBehaviour {

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

	/**
	 * Create and install the component specified by the asset into the socket.
	 */
	public IShipModule InstallModule(InstallableModuleAsset asset) {
		IShipModule mod = gameObject.AddComponent(asset.GetScriptType()) as IShipModule;
		mod.InitFromAssetInSocket(asset, this);
		return mod;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
