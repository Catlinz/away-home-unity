using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void TargetableDestroyedDelegate(ITargetable target);
public interface ITargetable {

    /// <summary>Event to tell when the target has been destroyed.</summary>
    event TargetableDestroyedDelegate OnTargetDestroyed;

    /// <summary>
    /// Get the name of the targetable object to display.
    /// </summary>
    /// <returns>The display name for the targetable object.</returns>
    string GetTargetName();

}
