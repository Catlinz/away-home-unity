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
public class ShipSocket {

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

    /// <summary>The module that is currently installed in the socket (if any).</summary>
    private ShipModuleClass module;

    /// <summary>The module that is current installed in the socket (if any).</summary>
    public ShipModuleClass Module { get { return module;  } }

    /// <summary>Default constructor</summary>
    public ShipSocket() {
        socketName = null;
        maxCpuBandwidth = 0.0f;
        maxEnergyOutput = 0.0f;
        arcLimits = new SocketArc(45, 45, 45, 0);
        position = Vector3.zero;
        rotation = Quaternion.identity;
        module = null;
    }

    /// <summary>
    /// Create a new ShipSocket with the specific cpu and energy specifications.
    /// </summary>
    /// <param name="cpu">The amount of CPU bandwidth the socket can supply.</param>
    /// <param name="energy">The amount of energy the socket can supply.</param>
    public ShipSocket(float cpu, float energy) 
        : this() {
        maxCpuBandwidth = cpu;
		maxEnergyOutput = energy;
    }

    /// <summary>
    /// Set the module that is currently installed in the socket.
    /// </summary>
    /// <param name="module">The module to install into the socket.</param>
    public void SetModule(ShipModuleClass module) {
        this.module = module;
    }
}