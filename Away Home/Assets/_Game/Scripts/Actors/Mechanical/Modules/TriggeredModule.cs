using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggeredModule : ActorModule {

    #region FIELDS
    [Header("Utility Module")]
    /** The amount of energy used per activation or per second. */
    public int energyDrain;

    /** The amount of CPU required to keep the module active. */
    public int cpuResources;

    /** Whether or not the module is currently activated */
    protected bool _isActive;
    #endregion

    #region MODULE METHODS
    public ModuleResult Trigger() {
        // Make sure we're in the right state to trigger.
        if (!_isEnabled) { return ModuleResult.ModuleDisabled; }
        if (_isActive) { return ModuleResult.AlreadyActive; }

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
}
