using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerModule : ShipModuleClass {

    #region PROTECTED_VARS

    /// <summary>The module's own copy of the cooldown time, so it can be modified.</summary>
    protected ModifiableFloat cooldownSec;

    /// <summary>The module's own copy of the trigger energy to use.</summary>
    protected ModifiableFloat triggerEnergyUse;

    /// <summary>Whether or not the module is in the cooldown state currently.</summary>
    protected bool isInCooldown = false;

    #endregion

    /// <summary>Makes sure the module is deactivated before disabling.</summary>
    /// <seealso cref="ShipModuleClass.DisableModule"/>
    public override ModuleResult DisableModule() {
        return base.DisableModule();
    }

    /// <summary>Allocates the idle CPU and reserves the idle energy required.</summary>
    /// <seealso cref="ShipModuleClass.EnableModule"/>
    public override ModuleResult EnableModule() {
        return base.EnableModule();
    }

    /// <summary>
    /// Returns that this is an Active module.
    /// </summary>
    public override ShipModuleClass.TypeFlags GetTypeFlags() {
        return ShipModuleClass.TypeFlags.Trigger;
    }

    /// <summary>Copies some data from the module asset as well.</summary>
    /// <seealso cref="ShipModuleClass.InitFromAssetInSocket"/>
    /// <returns>
    /// A <c>ModuleResult</c> indicating if the activation was successful.
	/// <list type="bullet">
	/// <item>
	/// <description><c>InvalidAsset</c> if the module asset is null or cannot be converted to an <c>TriggerModuleAsset</c></description>
	/// </item>
	/// </list>
    /// </returns>
    public override ModuleResult InitFromAssetInSocket(InstallableModuleAsset asset, ShipSocket socket) {
        ModuleResult baseResult = base.InitFromAssetInSocket(asset, socket);

        if (baseResult == ModuleResult.Success) {
            // Convert the module asset to the appropriate type.
            TriggerModuleAsset tAsset = asset as TriggerModuleAsset;
            if (!tAsset) { return ModuleResult.InvalidAsset; }

            // Copy some variables from the asset.
            cooldownSec = tAsset.cooldownSec;
            triggerEnergyUse = tAsset.triggerEnergyUse;

            return ModuleResult.Success;
        }
        else { // Failed to initialize the base module class.
            return baseResult;
        }
    }

    /// <summary>
	/// Try and activate the module.  If there is not enough energy or CPU to do so, it will fail.
	/// If the CPU and/or energy levels fall too low, the module will be deactivated.
	/// </summary>
	/// <returns>
	/// A <c>ModuleResult</c> indicating if the activation was successful.
	/// <list type="bullet">
	/// <item>
	/// <description><c>InCooldownState</c> if the module is still in the cooldown state.</description>
	/// </item>
	/// <item>
	/// <description><c>InvalidShip</c> if the ship reference is null.</description>
	/// </item>
	/// <item>
	/// <description><c>InsufficientPower</c> if there is not enough power to trigger the module.</description>
	/// </item>
	/// </list>
	/// </returns>
	public ModuleResult Trigger() {
        if (isInCooldown) {
            return ModuleResult.InCooldownState;
        }

        // Do sanity checks to make sure we have a valid ship.
        if (!ship) {
            return ModuleResult.InvalidShip;
        }

        // Try and allocate the required energy to trigger the module.
        if (ship.power.FreeEnergy >= triggerEnergyUse) {
            ModuleResult implResult = TriggerImpl();
            if (implResult != ModuleResult.Success) {
                ship.power.Consume(triggerEnergyUse);
            }
        }
        else { // Not enough energy
            return ModuleResult.InsufficientPower;
        }
    }

    /// <summary>
    /// The method for subclasses to implement that determines what happens when the module is 
    /// triggered.  At this point, we know we have consumed the energy and have a valid ship reference.
    /// </summary>
    /// <returns>A <c>ModuleResult</c> that indicates how things went.</returns>
    public virtual ModuleResult TriggerImpl() {
        return ModuleResult.NotImplemented;
    }
}
