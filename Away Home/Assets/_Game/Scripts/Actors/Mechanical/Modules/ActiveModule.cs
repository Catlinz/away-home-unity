using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveModule : ActorModule {

    #region FIELDS
    [Header("Active Module")]
    /** The amount of energy used per activation or per second. */
    public int activationEnergy;

    /** The amount of CPU required to keep the module active. */
    public int activeCpuResources;

    /** The time, in seconds, between activations / firing of the module.  */
    public float activationTimeSec;

    /** The reload time for the module in seconds */
    public float reloadTimeSec;

    /** If true, the module will keep activating until it is deactivated by the player */
    public bool isToggle = false;

    /** Whether or not the module is currently activated. */
    public bool IsActive {
        get { return _isActive; }
    }

    /** Whether or not the module is currently activated. */
    protected bool _isActive;

    /** The object for tracking cooldown periods */
    protected Cooldown _cooldown = null;

    /** An optional Targeter for modules that can target things */
    protected TargeterComponent _targeter = null;

    /** An optional AmmoStore for modules that need ammo. */
    protected AmmoStoreComponent _ammoStore = null;
    #endregion

    #region MODULE METHODS
    /// <summary>
    /// When enabled, tries to get a TargeterComponent to see if the 
    /// module can have and track a target.
    /// </summary>
    public override ModuleResult EnableModule() {
        ModuleResult result = base.EnableModule();
        if (result == ModuleResult.Success) {
            // Try and get the Targeter if there is one.
            _targeter = GetComponent<TargeterComponent>();
            // Try and get the ammoStore if there is one.
            _ammoStore = GetComponent<AmmoStoreComponent>();
        }
        return result;
    }

    /// <summary>
    /// Make sure to remove the reference to the TargeterComponent, if there was one.
    /// </summary>
    public override ModuleResult DisableModule(DisabledReason reason = DisabledReason.User) {
        ModuleResult result = base.DisableModule(reason);
        // Reset the targeter to default state if there is one.
        if (_targeter != null) {
            _targeter.ResetToDefault();
            _targeter = null;
        }
        // Unload any remaining ammo if there is any.
        if (_ammoStore != null) {
            _ammoStore.UnloadToInventory();
            _ammoStore = null;
        }
        return result;
    }

    /// <summary>
    /// Method to try and trigger the module.  If successful, the module will then 
    /// go on cooldown, and at the end of cooldown, if the module is a toggleable module, 
    /// it will be triggered again.
    /// </summary>
    public ModuleResult Trigger() {
        if (!_isActive) {
            // Make sure we're in the right state to trigger.
            if (!_isEnabled) { return ModuleResult.ModuleDisabled; }
            if (_isActive) { return ModuleResult.AlreadyActive; }
            if (_cooldown != null) { return ModuleResult.InCooldownState; }

            // Make sure we have the Core component.
            if (!_core) {
                _core = gameObject.GetComponentInParent<MechaCoreComponent>();
                if (!_core) { return ModuleResult.InvalidSystem; }
            }

            if (_core.computer.AllocateCpu(activeCpuResources)) {
                _isActive = true;
            }
            else { // Not enough CPU to enable module.
                return ModuleResult.InsufficientCpu;
            }
        }

        // Make sure we're not in a cooldown state.
        if (_cooldown != null) {
            return ModuleResult.InCooldownState;
        }

        ModuleResult result;
        bool shouldDeactivate = false;
        // If we get here, then the module is now active.
        if (_core.power.Consume(activationEnergy)) {
            result = ModuleActivate();
            if (result == ModuleResult.Success) {
                if (!isToggle) { shouldDeactivate = true; }
                StartCooldown();
            }
            else { shouldDeactivate = true; }
        }
        else { // Not enough power to enable module.
            shouldDeactivate = true;
            result = ModuleResult.InsufficientPower;
        }

        if (shouldDeactivate) {
            Deactivate();
        }
        return result;
    }

    /// <summary>
    /// Try and deactivate the module.
    /// </summary>
    public ModuleResult Deactivate() {
        // Make sure we're in the right state to Deactivate.
        if (_isActive) {
            ModuleResult result = ModuleDeactivate();
            _core.computer.DeallocateCpu(activeCpuResources);
            _isActive = false;
            return result;
        }
        else {
            return ModuleResult.AlreadyInactive;
        }
    }

    /// <summary>
    /// Method for modules to call to apply the effect of activating the module.
    /// </summary>
    public virtual ModuleResult ModuleActivate() {
        return ModuleResult.NotImplemented;
    }

    /// <summary>
    /// Method for modules to call to apply the effect of DEactivating the module.
    /// </summary>
    public virtual ModuleResult ModuleDeactivate() {
        return ModuleResult.Success;
    }

    #endregion

    #region COOLDOWN
    public Coroutine StartCooldown() {
        return StartCoroutine(CooldownRoutine(Time.time, activationTimeSec));
    }

    /// <summary>
    /// The coroutine to wait for the cooldown to finish.
    /// </summary>
    /// <param name="start">The time, in seconds, that the cooldown began.</param>
    /// <param name="length">The length, in seconds, of the cooldown period.</param>
    /// <returns></returns>
    protected IEnumerator CooldownRoutine(float start, float length) {
        _cooldown = new Cooldown(start, length);
        do {
            yield return _cooldown.WaitFor(0.5f);
            Debug.Log("Cooldown Tick");
        } while (_cooldown.Tick(Time.time));

        _cooldown = null;
        
        if (_isActive) {
            Trigger();
        }
    }
    #endregion
}
