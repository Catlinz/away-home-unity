using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargeterComponent : MonoBehaviour {

    #region FIELDS
    // Default yaw and pitch for the turret.
    public float defaultYaw;
    public float defaultPitch;

    public Transform turretBase;
    public Transform turretGuns;
    #endregion

    #region TARGET METHODS
    public void ClearTarget() {
        // TODO: Implement this.
    }
    #endregion

    #region BEHAVIOUR METHODS
    public void ResetToDefault() {
        ClearTarget();

    }
    #endregion
}
