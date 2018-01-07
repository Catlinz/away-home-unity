using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The base class for all Ship Modules to inherit from.
/// </summary>
public class ActorModule : MonoBehaviour {

	#region TYPES
	public enum RemovalReason { Uninstalled, Destroyed };
	public enum Change { Installed, Uninstalled, Enabled, Disabled, Destroyed};

	public delegate void StatusChanged(Change change, ActorModule module);
	#endregion

    #region PUBLIC FIELDS
	/// <summary>The amount of computer resources consumed by the Module when enabled.</summary>
	public int idleComputerResources;

	/// <summary>The amount of energy consumed by the Module when enabled.</summary>
	public int idleEnergyDrain;

    /// <summary>The type of hardpoint socket that is required to install the module.</summary>
    public HPSocket socket;

    /// <summary>Whether or not the Module is currently enabled.</summary>
    public bool IsEnabled {
        get { return isEnabled; }
    }
    #endregion

    #region PROTECTED FIELDS
	/// <summary>The CoreSystemComponent of the actor this module is installed on.</summary>
    protected CoreSystemComponent system;

	/// <summary>A reference to the prefab that instantiated the module.</summary>
	protected GameObject prefab;

    /// <summary>Whether or not the module is currently enabled.</summary>
    protected bool isEnabled;
    #endregion

	/// <summary>
	/// Instantiates an instance of a module prefab and installs it into a specified hardpoint on a 
	/// specified parent GameObject.  The instance of the module component is returned.
	/// </summary>
	public static ActorModule Instantiate(GameObject prefab, Hardpoint hardpoint, GameObject parent) {
		GameObject go = GameObject.Instantiate(prefab, parent.transform);
		ActorModule mod = go.GetComponent<ActorModule>();
		mod.prefab = prefab;
		mod.InstantiateModule(hardpoint.position, hardpoint.rotation);
		hardpoint.SetModule(mod);
		return mod;
	}

	/// <summary>
	/// Virtual method that is used to destroy the module.  This method 
	/// destroys the GameObject the module is attached to and returns 
	/// a reference to the prefab used to create the module, if any.
	/// </summary>
	public virtual GameObject DestroyModule() {
		GameObject prefab = this.prefab;
		this.prefab = null;

		GameObject.Destroy(gameObject);
		
		return prefab;
	}

    /// <summary>Frees up the Idle CPU and Energy from the ships systems.</summary>
	/// <returns>
	/// A <c>ModuleResult</c> to indicate the result of disabling the module.
	/// <list type="bullet">
	/// <item>
	/// <description><c>AlreadyDisabled</c> if the module is already disabled.</description>
	/// </item>
	/// <item>
	/// <description><c>InvalidSystem</c> if the system reference is null already.</description>
	/// </item>
	/// </list>
	/// </returns>
	public virtual ModuleResult DisableModule() {
		if (!isEnabled) { return ModuleResult.AlreadyDisabled; }

		isEnabled = false;
		if (system != null) {
            system.computer.DeallocateCpu(idleComputerResources);
            system.power.Free(idleEnergyDrain);
            system = null;
			return ModuleResult.Success;
		}
		else {
			return ModuleResult.InvalidSystem;
		}
	}

	/// <summary>Allocates the idle CPU and reserves the idle energy required.</summary>
	/// <returns>
	/// A <c>ModuleResult</c> to indicate the result of disabling the module.
	/// <list type="bullet">
	/// <item>
	/// <description><c>AlreadyEnabled</c> if the module is already enabled.</description>
	/// </item>
	/// <item>
	/// <description><c>InvalidSystem</c> if the system reference is null.</description>
	/// </item>
	/// <item>
	/// <description><c>InsufficientPower</c> if there is not enough power to enable the module.</description>
	/// </item>
	/// <item>
	/// <description><c>InsufficientCpu</c> if there is not enough CPU to enable the module.</description>
	/// </item>
	/// </list>
	/// </returns>
	public virtual ModuleResult EnableModule() {
        if (isEnabled) { return ModuleResult.AlreadyEnabled; }

		system = gameObject.GetComponentInParent<CoreSystemComponent>();
		if (!system) { return ModuleResult.InvalidSystem; }

		if (system.computer.AllocateCpu(idleComputerResources)) {
			if (system.power.Reserve(idleEnergyDrain)) {
				isEnabled = true;
				return ModuleResult.Success;
			}
			else { // Not enough power to enable module.
                system.computer.DeallocateCpu(idleComputerResources);
                system = null;
				return ModuleResult.InsufficientPower;
			}
		}
		else { // Not enough CPU to enable module.
            system = null;
			return ModuleResult.InsufficientCpu;
		}
	}

	/// <summary>
	/// Virtual Instantiate method to setup placing the module on the parent 
	/// actor, be it a ship or other vehicle.
	/// </summary>
	public virtual void InstantiateModule(Vector3 relPosition, Quaternion relRotation) {
		gameObject.transform.localPosition = relPosition;
		gameObject.transform.localRotation = relRotation;
	}
}
