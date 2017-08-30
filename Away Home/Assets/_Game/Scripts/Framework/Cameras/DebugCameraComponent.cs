using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Debug Camera Component script to enable moving the camera in the game
 * with the WASD keys + Q,E and mouse.
 */
public class DebugCameraComponent : MonoBehaviour
{
    public Vector3 moveSpeed = new Vector3(0.12f, 0.12f, 0.12f);
    public Vector3 rotationSpeed = new Vector3(3f, 3f, 0f);

    private Transform tx;
    private Vector3 rotation;


	// Use this for initialization
	void Start () {
        tx = GetComponent<Transform>();
        rotation = tx.rotation.eulerAngles;
    }
	
	// Update is called once per frame
	void Update () {
        // If right mouse button is pressed...
		if (Input.GetMouseButton(1)) {
            UpdateFlythroughCameraMovement();
            UpdateFlythroughCameraRotation();
        }
	}

    // Update the camera movement based on the current keys pressed.
    private void UpdateFlythroughCameraMovement() {
        float x = 0, y = 0, z = 0;
        if (Input.GetKey(KeyCode.A)) { x -= 1f; }
        if (Input.GetKey(KeyCode.D)) { x += 1f; }
        if (Input.GetKey(KeyCode.Q)) { y -= 1f; }
        if (Input.GetKey(KeyCode.E)) { y += 1f; }
        if (Input.GetKey(KeyCode.W)) { z += 1f; }
        if (Input.GetKey(KeyCode.S)) { z -= 1f; }

        tx.Translate(x * moveSpeed.x, y * moveSpeed.y, z * moveSpeed.z);
    }

    // Update the camera movement based on the mouse movement.
    private void UpdateFlythroughCameraRotation() {
        float rotX = Input.GetAxis("Mouse Y") * rotationSpeed.x;
        float rotY = Input.GetAxis("Mouse X") * rotationSpeed.y;

        rotation.x -= rotX;
        rotation.y += rotY;

        tx.rotation = Quaternion.Euler(rotation);
    }
}
