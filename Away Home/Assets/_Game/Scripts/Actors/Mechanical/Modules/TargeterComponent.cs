using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargeterComponent : MonoBehaviour {

    #region FIELDS
    // Default yaw and pitch for the targeter.
    public float defaultYaw;
    public float defaultPitch;

    /** The object to rotate horizontally */
    public Transform yawObject;
    /** The object to rotate vertically */
    public Transform pitchObject;

    protected ITarget _curTarget = null;
    #endregion

    #region TARGET METHODS
    public void ClearTarget() {
        // TODO: Implement this.
    }

    public bool CanTarget(ITarget target) {
        // TODO: Implement this.
        return true;
    }

    public ITarget GetTarget() {
        // TODO: Implement this
        return _curTarget;
    }

    public bool HasTarget(ITarget target) {
        // TODO: Implement this;
        return _curTarget == target;
    }

    public void SetTarget(ITarget target) {
        // TODO: Implement this.
    }
    #endregion

    #region BEHAVIOUR METHODS
    public void ResetToDefault() {
        ClearTarget();
    }
    #endregion
}
