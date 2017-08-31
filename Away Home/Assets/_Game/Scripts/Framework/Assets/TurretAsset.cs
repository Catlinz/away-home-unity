using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ship/Modules/Turret")]
public class TurretAsset : InstallableModuleAsset {

    /** The fire rate (seconds beftween firing) of the turret */
    public float fireRate;

    /** The base fire range of the turret */
    public float baseRange;

    /** The base particle velocity of the turret */
    public float baseVelocity;

    /** The types of ammo that the weapon can load/fire */
    //public AmmoAsset ammoType;

    /** The amount of ammo that can be loaded at once */
    public int magazineCapacity;

    /** The time (in seconds) to reload the turret */
    public float reloadTimeSec;

    /** Sounds to play each time we fire */
    public AudioClip activationSound;
}
