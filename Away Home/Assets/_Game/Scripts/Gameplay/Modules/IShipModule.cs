using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for any Ship Module component to implement.
/// </summary>
public interface IShipModule {

    /// <summary>The amount of power consumed by the Module when enabled.</summary>
	int IdlePowerUsage { get; }
    /// <summary>The amount of CPU resources consumed by the Module when enabled.</summary>
	int IdleCpuUsage { get; }

    /// <summary>Called to try and enable the module a ShipActorComponent.</summary>
    /// <param name="ship">The ship to enable the Module for.</param>
    void EnableOnShip(ShipActorComponent ship);

    /// <summary>Initializes the Module component from an asset for the module.</summary>
    /// <param name="asset">The module asset to initialize from.</param>
    /// <param name="socket">The ShipSocket that the module is being installed on.</param>
    void InitFromAssetInSocket(InstallableModuleAsset asset, ShipSocket socket);

}
