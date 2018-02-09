using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The base class for all Ship Modules to inherit from.
/// </summary>
public class ActorModule : GameItemComponent {

	#region TYPES
	public enum RemovalReason { Uninstalled, Destroyed };
	public enum Change { Installed, Uninstalled, Enabled, Disabled, Destroyed};
    public enum DisabledReason { NotDisabled, User, ResourceLoss, Damage };

	public delegate void StatusChanged(Change change, ActorModule module);
    #endregion

    #region FIELDS
    [Header("Basic Module")]
	/// <summary>The amount of computer resources consumed by the Module when enabled.</summary>
	public int idleComputerResources;

	/// <summary>The amount of energy consumed by the Module when enabled.</summary>
	public int idleEnergyDrain;

    /// <summary>The type of hardpoint socket that is required to install the module.</summary>
    public HPSocket socket;

    /// <summary>Whether or not the Module is currently enabled.</summary>
    public bool IsEnabled {
        get { return _isEnabled; }
    }

    /// <summary>Why the module was disabled, if it is disabled.</summary>
    public DisabledReason DisabledBy {
        get { return _disabledBy; }
    }

	/// <summary>The CoreSystemComponent of the actor this module is installed on.</summary>
    protected MechaCoreComponent _core;

	/// <summary>A reference to the prefab that instantiated the module.</summary>
	protected GameObject _prefab;

    protected DisabledReason _disabledBy = DisabledReason.ResourceLoss;

    /// <summary>Whether or not the module is currently enabled.</summary>
    protected bool _isEnabled;
    #endregion

    #region LIFECYCLE METHODS
    /// <summary>
    /// Instantiates an instance of a module prefab and installs it into a specified hardpoint on a 
    /// specified parent GameObject.  The instance of the module component is returned.
    /// </summary>
    public static ActorModule Instantiate(GameObject prefab, Hardpoint hardpoint, GameObject parent) {
		GameObject go = GameObject.Instantiate(prefab, parent.transform);
		ActorModule mod = go.GetComponent<ActorModule>();
		mod._prefab = prefab;
		mod.InstantiateModule(hardpoint.position, hardpoint.rotation);
		hardpoint.SetModule(mod);
		return mod;
	}

    /// <summary>
	/// Virtual Instantiate method to setup placing the module on the parent 
	/// actor, be it a ship or other vehicle.
	/// </summary>
	public virtual void InstantiateModule(Vector3 relPosition, Quaternion relRotation) {
        gameObject.transform.localPosition = relPosition;
        gameObject.transform.localRotation = relRotation;
    }

    /// <summary>
    /// Virtual method that is used to destroy the module.  This method 
    /// destroys the GameObject the module is attached to and returns 
    /// a reference to the prefab used to create the module, if any.
    /// </summary>
    public virtual GameObject DestroyModule() {
        GameObject prefab = this._prefab;
        this._prefab = null;

        GameObject.Destroy(gameObject);

        return prefab;
    }
    #endregion

    #region MODULE METHODS
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
    public virtual ModuleResult DisableModule(DisabledReason reason = DisabledReason.User) {
		if (!_isEnabled) {
            // Override any other disabled by reason if the user manually disabled the module, 
            // so it won't auto re-enable.
            if (reason == DisabledReason.User) {
                _disabledBy = reason;
            }
            return ModuleResult.AlreadyDisabled;
        }

        _isEnabled = false;
        _disabledBy = reason;
		if (_core != null) {
            _core.computer.DeallocateCpu(idleComputerResources);
            _core.power.Free(idleEnergyDrain);
            _core = null;
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
        if (_isEnabled) { return ModuleResult.AlreadyEnabled; }

        if (!_core) {
            _core = gameObject.GetComponentInParent<MechaCoreComponent>();
        }

		if (!_core) { return ModuleResult.InvalidSystem; }

		if (_core.computer.AllocateCpu(idleComputerResources)) {
			if (_core.power.Reserve(idleEnergyDrain)) {
                _isEnabled = true;
                _disabledBy = DisabledReason.NotDisabled;
				return ModuleResult.Success;
			}
			else { // Not enough power to enable module.
                _core.computer.DeallocateCpu(idleComputerResources);
                _core = null;
				return ModuleResult.InsufficientPower;
			}
		}
		else { // Not enough CPU to enable module.
            _core = null;
			return ModuleResult.InsufficientCpu;
		}
	}
    #endregion

    #region GAME ITEM METHODS
    public override InventoryItem CreateInventoryItem(InventoryItem item = null) {
        InventoryItem newItem = base.CreateInventoryItem(item);
        return newItem;
    }
    #endregion

    #region UNITY METHODS
    private void Start() {
        _core = GetComponentInParent<MechaCoreComponent>();
        if (_core) {
            Hardpoint hp = _core.hardpoints.Get(this);
            _core.EnableModule(hp);
        }
    }
    #endregion
}
