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
    public ModifiableInt computerUsage;

    protected SystemModifierList _modifiers;
    protected CoreSystemComponent _sys;
    #endregion

    #region PROPERTIES
    /// <summary>Can the System component be manually triggered?</summary>
    public virtual bool IsTriggerable {
        get { return false; }
    }
    #endregion

    #region SYSTEM CONTROL
    public virtual OperationResult Activate() {

    }

    /// <summary>
    /// Returns true if the system has enough power and CPU to be activated.
    /// </summary>
    public bool CanActivate() {
        return _sys.power.FreeEnergy >= powerUsage &&
               _sys.computer.IdleResources >= computerUsage;
    }

    public virtual OperationResult Deactivate() {

    }
    #endregion

    #region MODIFIERS
    /// <summary>
    /// Adds a modifier to the System Component.
    /// </summary>
    /// <param name="modifier">The SystemModifier to add.</param>
    public virtual void AddModifier(SystemModifier modifier) {
        _modifiers.Add(modifier);
        RecalculateStat(modifier.stat);
    }

    /// <summary>
    /// Removes a modifier from the System Component.
    /// </summary>
    /// <param name="modifier">The ISystemModifier to remove.  Uses Equals().</param>
    public virtual void RemoveModifier(SystemModifier modifier) {
        if (_modifiers.Remove(modifier)) {
            RecalculateStat(modifier.stat);
        }
    }

    /// <summary>
    /// Replaces an existing modifier with a new one, or adds a new one if it doesn't exist yet.
    /// </summary>
    /// <param name="modifier">The SystemModifier to add or replace.</param>
    public virtual void ReplaceModifier(SystemModifier modifier) {
        _modifiers.Replace(modifier);
        RecalculateStat(modifier.stat);
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
        _sys = GetComponent<CoreSystemComponent>();
    }
    #endregion
}
