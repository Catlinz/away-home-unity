using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentSlot : MonoBehaviour {

    /** The name of the socket */
    public string socketName;

    /** The max CPU that the slot can supply. */
    public float maxCPUOutput;

    /** The max power that the slot can supply. */
    public float maxPowerOutput;

    /** The minimum pitch angle */
    public float minPitch;

    /** The maximum pitch angle */
    public float maxPitch;

    /** The start of the allowed Yaw range (sweeps clockwise) */
    public float yawStart;

    /** The end of the allowed Yaw range (sweeps clockwise) */
    public float yawEnd;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
