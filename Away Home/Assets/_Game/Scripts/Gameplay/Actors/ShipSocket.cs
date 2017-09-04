using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the yaw and pitch limits for a socket, so as 
/// to limit the rotation of turrets placed into the socket.
/// </summary>
[System.Serializable]
public struct SocketArc {
    public float up;
    public float down;
    public float left;
    public float right;

    public SocketArc(float left, float right, float up, float down) {
        this.up = up;
        this.down = down;
        this.left = left;
        this.right = right;
    }
}

/// <summary>
/// Represents a socket into which a module can be placed on a ShipActorComponent.
/// </summary>
[System.Serializable]
public struct ShipSocket {

	/// <summary>The name of the socket to identify it by.</summary>
	public string socketName;

	/// <summary>The maximum CPU bandwidth the socket can supply.</summary>
	public ModifiableFloat maxCpuBandwidth;

    /// <summary>The maximum energy the socket can supply.</summary>
	public ModifiableFloat maxEnergyOutput;

    /// <summary>The yaw and pitch limits for a turret in this socket.</summary>
    public SocketArc arcLimits;

    /// <summary>The position of the socket relative to the ship.</summary>
    [Header("Transform")]
	public Vector3 position;
    /// <summary>The rotation of the socket relative to the ship.</summary>
	public Quaternion rotation;

    public ShipSocket(float cpu, float energy) {
        socketName = null;
        maxCpuBandwidth = cpu;
		maxEnergyOutput = energy;
        arcLimits = new SocketArc(45, 45, 45, 0);
        position = Vector3.zero;
        rotation = Quaternion.identity;
    }

    /// <summary>
    /// Checks whether a socket is valid or not.
    /// </summary>
    /// <returns>True if the socket is valid (has a name).</returns>
    public bool IsValid() {
        return socketName == null;
    }

    /// <summary>The values for a default empty socket.</summary>
    public static readonly ShipSocket empty = new ShipSocket(0, 0);
}