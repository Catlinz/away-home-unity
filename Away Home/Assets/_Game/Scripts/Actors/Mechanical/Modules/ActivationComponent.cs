using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Placing an ActivationComponent on a GameObject with a ActorModule 
/// component will allow the ActorModule to be able to be activated.
/// 
/// This enables behaviour such as firing a gun or missile, activating 
/// shields, ect.
/// </summary>
public class ActivationComponent : MonoBehaviour {

    #region FIELDS
    [Header("Active Module")]
    /** The amount of energy used per activation or per second. */
    public int energyUse;

    /** The amount of CPU required to keep the module active, or activate it. */
    public int cpuResources;

    /** The time, in seconds, between activations / firing of the module.  */
    public float activationTimeSec;

    /** If true, the module will keep activating until it is deactivated by the player */
    public bool isContinuous = false;

    /** Whether or not the module is currently activated. */
    public bool IsActive {
        get { return _isActive; }
    }

    /** Whether or not the module is currently activated. */
    protected bool _isActive;

    /** The object for tracking cooldown periods */
    protected Cooldown _cooldown = null;

    #endregion


    #region ACTIVATION
    /// <summary>
    /// Method to try and trigger the module.  If successful, the module will then 
    /// go on cooldown, and at the end of cooldown, if the module is a toggleable module, 
    /// it will be triggered again.
    /// </summary>
    public ModuleResult Activate(ActorModule module) {
        // Do the basic checks to see if we can activate it.
        if (_isActive) {
            return ModuleResult.AlreadyActive;
        }
        else if (_cooldown != null) {
            return ModuleResult.InCooldownState;
        }

        // Check if we are using ammo, and if so, that we have some.
        AmmoStoreComponent ammoStore = module.ammoStore;

        if (ammoStore != null && !ammoStore.HasAmmo) {
            if (ammoStore.IsReloading) {
                return ModuleResult.Reloading;
            }
            else {
                return ModuleResult.NoAmmo;
            }
        }

        // Try and allocate the resources for the activation.
        MechaCoreComponent core = module.MechaCore;

        if (!core.computer.AllocateCpu(cpuResources)) {
            return ModuleResult.InsufficientCpu;
        }

        if ((isContinuous && !core.power.Reserve(energyUse)) ||
            (!core.power.Consume(energyUse))) {
            core.computer.DeallocateCpu(cpuResources);
            return ModuleResult.InsufficientPower;
        }

        // All good, so do the activation.
        _isActive = true;
        ModuleResult result = OnActivation(module);

        
        if (result != ModuleResult.Success) {
            core.computer.DeallocateCpu(cpuResources);
            if (isContinuous) {
                core.power.Free(energyUse);
            }
            _isActive = false;
        }
        else if (!isContinuous) {
            core.computer.DeallocateCpu(cpuResources);
            _isActive = false;
            StartCooldown();
        }

        return result;
    }

    public ModuleResult Deactivate(ActorModule module) {
        // Make sure we're in the right state to Deactivate.
        if (!_isActive) {
            return ModuleResult.AlreadyInactive;
        }

        ModuleResult result = OnDeactivation(module);

        MechaCoreComponent core = module.MechaCore;
        core.computer.DeallocateCpu(cpuResources);
        core.power.Free(energyUse);
        _isActive = false;

        return result;
    }
    #endregion

    #region ACTIVATION_HOOKS
    public virtual ModuleResult OnActivation(ActorModule module) {
        return ModuleResult.NotImplemented;
    }

    public virtual ModuleResult OnDeactivation(ActorModule module) {
        return ModuleResult.NotImplemented;
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
    }
    #endregion
}
