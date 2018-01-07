using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The ShipActorComponent is the core foundation of the logic and data 
/// that makes an object into a ship.  
/// <para>
/// It contains the ship stats and info, the 
/// list of sockets that modules can be attached to and the logic for installing 
/// and interacting with the modules.
/// </para>
/// </summary>
[RequireComponent(typeof(ShipNavComponent))]
public class ShipActorComponent : MonoBehaviour {

    public CoreSystemComponent system;

    public ShipNavComponent movement;

    public GameObject test;

    /// <summary>
    /// Handle cleanup of things that should be cleaned up from Start().
    /// </summary>
    private void OnDisable() {
        // Remove handlers from the computer system.
        system.computer.onResourcesChanged -= HandleComputerResourcesChanged;
        system.computer.onDamaged -= HandleComputerDamaged;

        // Remove handlers from the power system.
        system.power.onEnergyChanged -= HandleEnergyChanged;
        system.power.onDamaged -= HandlePowerDamaged;
    }

    // Use this for initialization
    void Start () {
		movement = GetComponent<ShipNavComponent>();
        system = GetComponent<CoreSystemComponent>();
        StructuralComponent structure = GetComponent<StructuralComponent>();

        if (structure != null && test != null) {
            structure.InstallModuleIn(structure.GetHardpoint("TEST"), test);
        }
       
        // Add the handlers for the computer system.
        system.computer.onResourcesChanged += HandleComputerResourcesChanged;
        system.computer.onDamaged += HandleComputerDamaged;

        // Add the handlers for the power system.
        system.power.onEnergyChanged += HandleEnergyChanged;
        system.power.onDamaged += HandlePowerDamaged;
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    private void HandleComputerDamaged(float damage) {

    }

    private void HandleComputerResourcesChanged(float resources) {

    }


    private void HandleEnergyChanged(float energy) {

    }


    private void HandlePowerDamaged(float damage) {

    }
}
