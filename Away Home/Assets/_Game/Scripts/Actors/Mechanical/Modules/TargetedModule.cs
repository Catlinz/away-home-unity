using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetedModule : ActorModule {

    #region FIELDS
    [Header("Targeted Module")]
    /** The amount of energy used per activation or per second. */
    public int activationEnergy;

    /** The number of seconds between firings. */
    public float fireRate;

    /** The cooldown time for the module in seconds */
    public float cooldownSec;

    /** If true, the module will keep activating until it is deactivated by the player */
    public bool isToggle = false;

    /** Whether or not the module is currently activated. */
    public bool IsActive {
        get { return _isActive; }
    }

    /** Whether or not the module is currently activated. */
    protected bool _isActive;

    protected Cooldown _cooldown = null;
    #endregion

    #region MODULE METHODS
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

        ModuleResult result;
        bool shouldDeactivate = false;
        // If we get here, then the module is now active.
        if (_core.power.Consume(activeEnergyDrain)) {
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
        return StartCoroutine(CooldownRoutine(Time.time, cooldownSec));
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

        if (isToggle && _isActive) {
            Trigger();
        }
    }
    #endregion
}
