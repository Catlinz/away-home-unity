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
    #region PUBLIC_VARS
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
    #endregion

    #region PRIVATE_VARS
    /// <summary>The module that is currently installed in the socket (if any).</summary>
    private ShipModuleClass module;

    #endregion

    #region PUBLIC_PROPS
    /// <summary>The module that is current installed in the socket (if any).</summary>
    public ShipModuleClass Module { get { return module;  } }
    #endregion

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
    /// Check if the socket has a module installed that can target the provided target.
    /// </summary>
    /// <param name="target">The targetable object to test.</param>
    /// <returns>True if there is a module installed that can target the target.</returns>
    public bool CanTarget(ITargetable target) {
        return (module != null && module.CanTarget(target));
    }

    /// <summary>
    /// Clear any references from the Socket, for when the module is removed from it.
    /// </summary>
    public void Clear() {
        SetTarget(null);
        module = null;
    }

    /// <summary>
    /// Check if the socket has a module installed that is currently targeting target.
    /// </summary>
    /// <param name="target">The targetable object to test.</param>
    /// <returns>True if there is a module installed that is currently targeting target.</returns>
    public bool HasTarget(ITargetable target) {
        return (module != null && module.HasTarget(target));
    }

    /// <summary>
    /// Set the module that is currently installed in the socket.
    /// </summary>
    /// <param name="module">The module to install into the socket.</param>
    public void SetModule(ShipModuleClass module) {
        this.module = module;
    }

    /// <summary>
    /// Set the current target for the installed module (if any).
    /// </summary>
    /// <param name="target">The targetable object for the module to target.</param>
    public void SetTarget(ITargetable target) {
        if (module) {
            module.SetTarget(target);
        }
    }
}

/// <summary>
/// The SocketGroup holds a group of Sockets that can be activated as a group.
/// </summary>
[System.Serializable]
public class SocketGroup
{
    /// <summary>Enum of constants to use for default socket groups indexes.</summary>
    public enum Index { Primary, Secondary, Utility, Passive };

    /// <summary>The name of the socket group.</summary>
    public string groupName;
    

    /// <summary>The list of sockets in the group.</summary>
    private ShipSocket[] sockets;

    /// <summary>The current target for the socket group.</summary>
    private ITargetable target;

    /// <summary>Default constructor.</summary>
    public SocketGroup() {
        groupName = null;
        sockets = null;
        target = null;
    }

    /// <summary>Create a new named SocketGroup.</summary>
    /// <param name="name"></param>
    public SocketGroup(string name) : this() {
        groupName = name;
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

    /// <summary>
    /// Set the target for the socket group.  Makes sure to unset the 
    /// previous target and listen for when the target is destroyed.
    /// </summary>
    /// <param name="newTarget">The new target for the modules in the socket group.</param>
    public void SetTarget(ITargetable newTarget) {
        if (target != null) {
            target.OnTargetDestroyed -= HandleTargetDestroyed;
        }

        target = newTarget;
        if (target != null) {
            target.OnTargetDestroyed += HandleTargetDestroyed;
        }

        int numSockets = sockets.Length;
        for (int i = 0; i < numSockets; ++i) {
            if (sockets[i].CanTarget(newTarget)) {
                sockets[i].SetTarget(target);
            }
        }
    }

    /// <summary>
    /// Handle when the target of the socket group has been destroyed.
    /// </summary>
    /// <param name="destroyedTarget">The target that was destroyed.</param>
    private void HandleTargetDestroyed(ITargetable destroyedTarget) {
        int numSockets = sockets.Length;
        for (int i = 0; i < numSockets; ++i) {
            if (sockets[i].HasTarget(destroyedTarget)) {
                sockets[i].SetTarget(null);
            }
        }
        if (destroyedTarget == target) {
            target = null;
        }
    }
}