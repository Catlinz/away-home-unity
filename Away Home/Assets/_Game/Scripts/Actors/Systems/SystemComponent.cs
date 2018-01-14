using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemComponent : MonoBehaviour {

    #region DELEGATES
    /// <summary>
    /// Delegate to listen for when the a system receives damage.
    /// </summary>
    /// <param name="percentDamage">The percentage of damage the system has currently [0-1].</param>
    public delegate void Damaged(float percentDamage);
    #endregion

    #region FIELDS
    [Header("System Properties")]
    /* Automatically enable the system if there is enough resources. */
    public bool autoEnable = true;

    /* The amount of power required by the system when active. */
    public ModifiableInt powerUsage;

    /* The amount of computer resources required by the system when active */
    public ModifiableInt cpuUsage;

    protected SystemModifierList _modifiers;
    protected CoreSystemComponent _core;

    protected bool _isActive = false;
    #endregion

    #region PROPERTIES
    /// <summary>Can the System component be manually triggered?</summary>
    public virtual bool IsTriggerable {
        get { return false; }
    }
    /// <summary>Is the system currently online?</summary>
    public bool IsOnline {
        get { return _isActive; }
    }
    #endregion

    #region SYSTEM CONTROL
    /// <summary>
    /// Try and activate the system by allocating resources and 
    /// calling the virtual SystemActivate() method.  
    /// </summary>
    public OperationResult Activate() {
        if (CanActivate()) {
            // Activate the system by reserving resources.
            _isActive = true;
            _core.power.Reserve(powerUsage);
            _core.computer.AllocateCpu(cpuUsage);
            // Do any other initialization for the System.
            OperationResult activated = SystemActivate();

            // If the System failed to activate properly, deallocate 
            // the allocated resources.
            if (activated != OperationResult.Status.OK) {
                _core.power.Free(powerUsage);
                _core.computer.DeallocateCpu(cpuUsage);
                _isActive = false;
            }
            return activated;
        }
        else {
            return OperationResult.Fail("Not enough resources to activate system");
        }
        
    }

    /// <summary>
    /// Returns true if the system has enough power and CPU to be activated.
    /// </summary>
    public bool CanActivate() {
        return _core.power.FreeEnergy >= powerUsage &&
               _core.computer.IdleResources >= cpuUsage;
    }

    /// <summary>
    /// Deactivates the system by calling SystemDeactivate() then 
    /// freeing the allocated resources.
    /// </summary>
    public OperationResult Deactivate() {
        if (_isActive) {
            OperationResult result = SystemDeactivate();
            _core.power.Free(powerUsage);
            _core.computer.DeallocateCpu(cpuUsage);
            _isActive = false;

            return result;
        }
        else {
            return OperationResult.Partial("System already deactivated");
        }
    }

    /// <summary>
    /// Virtual method for system to use for custom setup on activation.  
    /// Is called after resources are allocated.  
    /// 
    /// Returning a result other than OK is considered a failure and will result in the 
    /// resources being deallocated again.
    /// </summary>
    protected virtual OperationResult SystemActivate() {
        return OperationResult.OK();
    }

    /// <summary>
    /// Virtual method for system to use for custom deactivation.
    /// Is called before resources are deallocated.
    /// </summary>
    protected virtual OperationResult SystemDeactivate() {
        return OperationResult.OK();
    }

    /// <summary>
    /// Virtual method for system to use for custom Start() 
    /// behaviour.  Called before any attempt to activate the 
    /// system.
    /// </summary>
    protected virtual void SystemStart() {}
    #endregion

    #region MODIFIERS
    /// <summary>
    /// Adds a modifier to the System Component.
    /// </summary>
    /// <param name="modifier">The SystemModifier to add.</param>
    public virtual void AddModifier(SystemModifier modifier) {
        _modifiers.Add(modifier);
        if (_isActive) {
            RecalculateStat(modifier.stat);
        }
    }

    /// <summary>
    /// Removes a modifier from the System Component.
    /// </summary>
    /// <param name="modifier">The ISystemModifier to remove.  Uses Equals().</param>
    public virtual void RemoveModifier(SystemModifier modifier) {
        if (_modifiers.Remove(modifier) && _isActive) {
            RecalculateStat(modifier.stat);
        }
    }

    /// <summary>
    /// Replaces an existing modifier with a new one, or adds a new one if it doesn't exist yet.
    /// </summary>
    /// <param name="modifier">The SystemModifier to add or replace.</param>
    public virtual void ReplaceModifier(SystemModifier modifier) {
        _modifiers.Replace(modifier);
        if (_isActive) {
            RecalculateStat(modifier.stat);
        }
    }

    ///<summary>
    /// Recalculates the specified stat based on the current modifiers.
    /// Used by the default implementation of the Modifier methods.
    ///</summary>
    protected virtual void RecalculateStat(ModifiableStat stat) {
        Debug.Assert(false, "Must implement RecalculateStat()");
    }
    #endregion

    #region UNITY HOOKS
    /// <summary>
    /// Automatically grab the core system component.
    /// </summary>
    private void Start() {
        _core = GetComponent<CoreSystemComponent>();

        // Called before component is registered.
        SystemStart();

        // Register the system with the CoreSystemComponent
        _core.systems.Register(this);
    }
    #endregion
}
