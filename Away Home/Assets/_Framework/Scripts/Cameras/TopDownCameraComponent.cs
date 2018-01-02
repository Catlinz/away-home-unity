using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A basic Top-down camera that follows a target (usually a ship).
/// </summary>
public class TopDownCameraComponent : MonoBehaviour {

    /// <summary>The target that the camera will follow.</summary>
    public Transform target;

    /// <summary>The vertical distance from the target.</summary>
    public float distance = 10f;
    /// <summary>The speed of the camera follow movement.</summary>
    public float speed = 0.2f;

    private Transform tx;

	// Use this for initialization
	void Start () {
        InitializeCamera();
    }

    private void OnEnable() {
        InitializeCamera();
    }

    /// <summary>
    /// Update the camera position based on the target's position.
    /// </summary>
    private void FixedUpdate() {
        Vector3 targetPos = target.position;
        Vector3 thisPos = tx.position;

        if (!Mathf.Approximately(targetPos.x, thisPos.x) || !Mathf.Approximately(targetPos.z, thisPos.z)) {
            targetPos.y += distance;

            tx.position = Vector3.Lerp(thisPos, targetPos, speed);

        }
    }

    /// <summary>
    /// Initialize the camera to track the target.
    /// </summary>
    private void InitializeCamera() {
        tx = GetComponent<Transform>();

        Vector3 targetPos = target.position;
        targetPos.y += distance;
        tx.position = targetPos;

        tx.LookAt(target);
    }
}
