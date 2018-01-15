using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An enum of flags for the different types of hardpoint modules that 
/// can be installed into a hardpoint.
/// </summary>
[System.Flags]
public enum HardpointType
{
    /// <summary>A structural module, such as a cargo bay or hangar.</summary>
    Structural = 0x1,
    /// <summary>A module that passively effects a stat or actor in some way, just by being installed.</summary>
    Passive = 0x2,
    /// <summary>A Utility module such as a Repairer, or shield, etc.  Needs to be activated and consumes power.</summary>
    Utility = 0x4,
    /// <summary>A targeted module like a turret.  Needs to be activated, consumes power and computer resources.</summary>
    Targeted = 0x8
}

/// <summary>
/// The enums of all the different HardPoint socket types.
/// </summary>
public enum HPSocket
{
    None = 0,

    // The module types for a socket.
    Structural = HardpointType.Structural,
    Passive = HardpointType.Passive,
    Utility = HardpointType.Utility,
    Targeted = HardpointType.Targeted,

    // The socket types.
    Passive_5M = 0xA | Passive,
    Utility_5M = 0xA | Utility,
    Targeted_5M = 0xA | Targeted
}


/// <summary>
/// Represents the yaw and pitch limits for a turret, so as 
/// to limit the rotation of turrets placed into the hardpoint.
/// </summary>
[System.Serializable]
public struct TurretArc {
    public float up;
    public float down;
    public float left;
    public float right;

    public TurretArc(float left, float right, float up, float down) {
        this.up = up;
        this.down = down;
        this.left = left;
        this.right = right;
    }
}

/// <summary>
/// Represents a HardPoint into which a module can be placed on an Actor.
/// </summary>
[System.Serializable]
public class Hardpoint
{
    #region FIELDS
    /// <summary>The name of the hardpoint to identify it by.</summary>
    public string name;

    /// <summary>The socket type of the hardpoint.  Restricts what can go into it.</summary>
    public HPSocket socket = HPSocket.None;

    /// <summary>The yaw and pitch limits for a turret in this hardpoint (if it supports turrets).</summary>
    public TurretArc arcLimits;

    /// <summary>The position of the hardpoint relative to the actor.</summary>
    [Header("Transform")]
	public Vector3 position = Vector3.zero;
    /// <summary>The rotation of the hardpoint relative to the actor.</summary>
	public Quaternion rotation = Quaternion.identity;

    /// <summary>The module that is current installed in the socket (if any).</summary>
    public ActorModule Module { get { return module;  } }

    /// <summary>Whether or not the hardpoint has a module.</summary>
    public bool IsEmpty {
        get {
            return module == null;
        }
    }

    /// <summary>Is there an offline module installed into the hardpoint?</summary>
    public bool IsOffline {
        get {
            return (module != null && !module.IsEnabled);
        }
    }

    /// <summary>Is there a online module installed into the hardpoint?</summary>
    public bool IsOnline {
        get {
            return (module != null && module.IsEnabled);
        }
    }

    /// <summary>The module that is currently installed in the socket (if any).</summary>
    private ActorModule module;

    #endregion


    /// <summary>Default constructor</summary>
    public Hardpoint() {
        name = null;
        arcLimits = new TurretArc(45, 45, 45, 0);
        module = null;
    }

    /// <summary>
    /// Create a new HardPoint with the specific socket type.
    /// </summary>
    public Hardpoint(HPSocket socket) 
        : this() {
        this.socket = socket;
    }

    /// <summary>
    /// Check if the hardpoint has a module installed that can target the provided target.
    /// </summary>
    public bool CanTarget(ITarget target) {
        TargetedModule tmod = module as TargetedModule;
        return (tmod != null && tmod.CanTarget(target));
    }

    /// <summary>
    /// Clear any references from the HardPoint, for when the module is removed from it.
    /// </summary>
    public void Clear() {
        SetTarget(null);
        module = null;
    }

    /// <summary>
    /// Get the current target of the installed module, if any.
    /// </summary>
    public ITarget GetTarget() {
        TargetedModule tMod = module as TargetedModule;
        return (tMod != null) ? tMod.GetTarget() : null;
    }

    /// <summary>
    /// Check if the HardPoint has a module installed that is currently targeting
    /// the provided target.
    /// </summary>
    public bool HasTarget(ITarget target) {
        TargetedModule tMod = module as TargetedModule;
        return (tMod != null && tMod.HasTarget(target));
    }

    /// <summary>
    /// Returns whether the hardpoint can install a module with the 
    /// provided type of socket.
    /// </summary>
    public bool IsCompatible(HPSocket toInstall) {
        return socket == toInstall;
    }

    /// <summary>
    /// Set the module that is currently installed in the HardPoint.
    /// </summary>
    public void SetModule(ActorModule module) {
        this.module = module;
    }

    /// <summary>
    /// Set the current target for the installed module (if any).
    /// </summary>
    public void SetTarget(ITarget target) {
        TargetedModule tMod = module as TargetedModule;
        if (tMod != null) {
            tMod.SetTarget(target);
        }
    }
}

/// <summary>
/// The <c>HardPointGroup</c> holds a group of <c>HardPoint</c>s that can be activated as a group.
/// </summary>
[System.Serializable]
public class HardpointGroup
{
    /// <summary>Enum of constants to use for default hardpoint groups indexes.</summary>
    public enum Index { Primary, Secondary, Utility, Passive };

	/// <summary>
	/// An enum of the different flags a socket group can have.
	/// </summary>
	[System.Flags] public enum Flags { 
		None = 0, 
		CanDelete = 0x1, 
		CanRename = 0x2, 
		Passive = 0x4, 
		Utility = 0x8,
		Targeted = 0x10
	};

    /// <summary>The name of the hardpoint group.</summary>
    public string name;

	/// <summary>The flags that are set on the HardPointGroup.</summary>
	public Flags flags;

	/// <summary>
	/// Gets a value indicating whether this instance can be deleted.
	/// </summary>
	/// <value><c>true</c> if this instance can be deleted; otherwise, <c>false</c>.</value>
	public bool CanDelete { 
		get { 
			return (flags & Flags.CanDelete) != Flags.CanDelete; 
		} 
	}

	/// <summary>
	/// Gets a value indicating whether this instance can be renamed
	/// </summary>
	/// <value><c>true</c> if this instance can be renamed; otherwise, <c>false</c>.</value>
	public bool CanRename { 
		get { 
			return (flags & Flags.CanRename) != Flags.CanRename; 
		} 
	}

    /// <summary>The list of sockets in the group.</summary>
    private Hardpoint[] hardpoints;

    /// <summary>Default constructor.</summary>
    public HardpointGroup() {
        name = null;
        hardpoints = null;
    }

    /// <summary>Create a new named HardPointGroup.</summary>
    /// <param name="name"></param>
	public HardpointGroup(
		string name, 
		Flags flags = Flags.CanDelete | Flags.CanRename | Flags.Passive | Flags.Utility | Flags.Targeted
	) : this() {
        this.name = name;
		this.flags = flags;
    }

    /// <summary>
    /// Add a HardPoint into the group, if it doesn't already exist.
    /// </summary>
    /// <param name="hardpoint">The HardPoint to add to the group.</param>
    /// <returns>True if the hardpoint was added, false if it already is in the group.</returns>
    public bool Add(Hardpoint hardpoint) {
        if (Contains(hardpoint.name)) { return false; }

        hardpoints = AHArray.Added(hardpoints, hardpoint);
        return true;
    }

    /// <summary>
    /// Determines whether this group can add the specified hardpoint.
    /// </summary>
    /// <param name="hardpoint">The hardpoint to try and add to the group.</param>
    /// <returns><c>true</c> if this group can add the specified hardpoint; otherwise, <c>false</c>.</returns>
    public bool CanAddHardPoint(Hardpoint hardpoint) {
		if (hardpoint.Module == null) {
			return false; // Cannot add an empty socket.
		}

        // See if the socket group can take the module.
        ActorModule module = hardpoint.Module;
        HPSocket typeFlags = module.socket;
		if ((typeFlags & HPSocket.Passive) == HPSocket.Passive) {
			return (flags & Flags.Passive) == Flags.Passive;
		}

		if ((typeFlags & HPSocket.Utility) == HPSocket.Utility) {
			return (flags & Flags.Utility) == Flags.Utility;
		}

		if ((typeFlags & HPSocket.Targeted) == HPSocket.Targeted) {
			return (flags & Flags.Targeted) == Flags.Targeted;
		}

		// Dunno what happened, so no.
		return false;
	}

    /// <summary>
    /// Check to see if a socket is in the group.
    /// </summary>
    /// <param name="socketName">The name of the socket to look for.</param>
    /// <returns>True if the socket is in the group.</returns>
    public bool Contains(string socketName) {
        if (hardpoints == null) { return false; }

        int numSockets = hardpoints.Length;
        for (int i = 0; i < numSockets; ++i) {
            if (hardpoints[i].name == socketName) {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Get the target that is assigned to this group, if any.
    /// </summary>
    public ITarget GetTarget() {
        if (hardpoints == null || hardpoints.Length == 0) {
            return null;
        }
        return hardpoints[0].GetTarget();
    }

    /// <summary>
    /// Remove the specified socket from the socket group.
    /// </summary>
    /// <param name="socketName">The name of the socket to remove.</param>
    /// <returns>The ShipSocket that was removed, or null if the socket wasn't found.</returns>
    public Hardpoint Remove(string socketName) {
        for (int i = 0; i < hardpoints.Length; ++i) {
            if (hardpoints[i].name == socketName) {
                Hardpoint hardpoint = hardpoints[i];
                hardpoints = AHArray.Removed(hardpoints, i);
                return hardpoint;
            }
        }
        return null;
    }

    /// <summary>
    /// Remove the specified hardpoint from the hardpoint group.
    /// </summary>
    /// <param name="hardpoint">The HardPoint to remove.</param>
    /// <returns>The HardPoint that was removed or null if it wasn't found.</returns>
    public Hardpoint Remove(Hardpoint hardpoint) {
        return Remove(hardpoint.name);
    }

    /// <summary>
    /// Set the target for the hardpoint group.  Makes sure to unset the 
    /// previous target and listen for when the target is destroyed.
    /// </summary>
    /// <param name="newTarget">The new target for the modules in the hardpoint group.</param>
    public void SetTarget(ITarget newTarget) {
        int numHardpoints = hardpoints.Length;
        for (int i = 0; i < numHardpoints; ++i) {
            if (hardpoints[i].CanTarget(newTarget)) {
                hardpoints[i].SetTarget(newTarget);
            }
        }
    }
}