using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveModule : ActorModule {


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

    #endregion

    #region TARGET METHODS
    public bool CanTarget(ITarget target) {
        //TODO: Implement this.
        return false;
    }

    public ITarget GetTarget() {
        //TODO: Implement this.
        return null;
    }

    public bool HasTarget(ITarget target) {
        //TODO: Implement this.
        return false;
    }

    public void SetTarget(ITarget target) {
        //TODO: Implement this.
    }

    #endregion
}
