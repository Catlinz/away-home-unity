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

[System.Serializable]
public class SocketGroup
{
    private ShipSocket[] sockets;

    public SocketGroup() {
        sockets = null;
    }

    /// <summary>
    /// Add a socket into the socket group, if it doesn't already exist.
    /// </summary>
    /// <param name="socket">The ShipSocket to add to the group.</param>
    /// <returns>True if the socket was added, false if it already is in the group.</returns>
    public bool Add(ShipSocket socket) {
        if (Contains(socket.socketName)) { return false; }

        sockets = AHArray.Added(sockets, socket);
        return true;
    }

    /// <summary>
    /// Check to see if a socket is in the group.
    /// </summary>
    /// <param name="socketName">The name of the socket to look for.</param>
    /// <returns>True if the socket is in the group.</returns>
    public bool Contains(string socketName) {
        if (sockets == null) { return false; }

        int numSockets = sockets.Length;
        for (int i = 0; i < numSockets; ++i) {
            if (sockets[i].socketName == socketName) {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Remove the specified socket from the socket group.
    /// </summary>
    /// <param name="socketName">The name of the socket to remove.</param>
    /// <returns>The ShipSocket that was removed, or null if the socket wasn't found.</returns>
    public ShipSocket Remove(string socketName) {
        for (int i = 0; i < sockets.Length; ++i) {
            if (sockets[i].socketName == socketName) {
                ShipSocket socket = sockets[i];
                sockets = AHArray.Removed(sockets, i);
                return socket;
            }
        }
        return null;
    }

    /// <summary>
    /// Remove the specified socket from the socket group.
    /// </summary>
    /// <param name="socket">The ShipSocket to remove.</param>
    /// <returns>The ShipSocket that was removed or null if it wasn't found.</returns>
    public ShipSocket Remove(ShipSocket socket) {
        return Remove(socket.socketName);
    }
}