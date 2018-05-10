using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoStoreComponent : MonoBehaviour {

    /** The max amount of ammo the component can store */
    public int capacity;

    /** The reload time for the module in seconds */
    public float reloadTimeSec;

    /// <summary>Do we have any usable ammo yet?</summary>
    public bool HasAmmo {
        get { return _reloading == null && _count > 0; }
    }

    /// <summary>Is the AmmoStore currently reloading?</summary>
    public bool IsReloading {
        get { return _reloading != null;  }
    }

    /** The current amount of ammo loaded */
    protected int _count = 0;

    /** The current amount of ammo being loaded */
    protected int _beingLoaded = 0;

    /** The object for tracking reloading periods */
    protected Cooldown _reloading = null;

    /// <summary>
    /// Tries to load enough ammo from the InventoryComponent 
    /// to bring the loaded ammo back up to full.
    /// </summary>
    public void LoadFromInventory() {
        StartCoroutine(ReloadRoutine(Time.time, reloadTimeSec));
    }

    /// <summary>
    /// Method to unload all the currently loaded ammo to the 
    /// parent GameObject's Inventory component.
    /// </summary>
    public void UnloadToInventory() {
        StopAllCoroutines();

        int ammoToUnload = _count + _beingLoaded;

        // TODO: Unload the ammo into the inventory.
    }

    #region RELOADING
    /// <summary>
    /// The coroutine to wait for the cooldown to finish.
    /// </summary>
    /// <param name="start">The time, in seconds, that the cooldown began.</param>
    /// <param name="length">The length, in seconds, of the cooldown period.</param>
    /// <returns></returns>
    protected IEnumerator ReloadRoutine(float start, float length) {
        _reloading = new Cooldown(start, length);
        do {
            yield return _reloading.WaitFor(0.5f);
            Debug.Log("Reloading Tick");
        } while (_reloading.Tick(Time.time));

        _reloading = null;
    }
    #endregion
}
