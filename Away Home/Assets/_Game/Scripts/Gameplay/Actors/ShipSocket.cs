using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SocketArc {
    public float up;
    public float down;
    public float left;
    public float right;
}

[System.Serializable]
public struct ShipSocket {

	/** The name of the socket */
	public string socketName;

	/** The max CPU that the slot can supply. */
	public ModifiableFloat maxCpuBandwidth;

	/** The max power that the slot can supply. */
	public ModifiableFloat maxPowerOutput;

    public SocketArc arcLimits;

    [Header("Transform")]
	public Vector3 position;
	public Quaternion rotation;

    public ShipSocket(float cpu, float power) {
        socketName = null;
        maxCpuBandwidth = cpu;
        maxPowerOutput = power;
        arcLimits = new SocketArc();
        position = Vector3.zero;
        rotation = Quaternion.identity;
    }

    public static readonly ShipSocket empty = new ShipSocket(0, 0);
}