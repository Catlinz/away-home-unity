using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void TargetDestroyed(ITarget target);

/// <summary>
/// An interface for components to implement that makes a GameObject targetable.
/// </summary>
public interface ITarget {

    /// <summary>The display name for the targetable object.</summary>
    string TargetName { get; }

    /// <summary>Event to tell when the target has been destroyed.</summary>
    event TargetDestroyed OnDestroyed;
}
