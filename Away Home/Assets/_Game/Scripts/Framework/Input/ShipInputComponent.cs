using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A component that translates user input into ship movement commands.
/// </summary>
public class ShipInputComponent : MonoBehaviour {

    private ShipMovementComponent movement;

	// Use this for initialization
	void Start () {
        movement = GetComponent<ShipMovementComponent>();
        InputManager.Get();
	}
	
	// Update is called once per frame
	void Update () {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (movement) {
            movement.SetDesiredHeading(new Vector3(h, 0, v));
            movement.SetThrottle(movement.GetThrottle() + GetThrottleDelta());
        }
        else {
            Debug.LogError("Failed to get ShipMovementComponent");
        }
	}

    private float GetThrottleDelta() {
        if (Input.GetKey(KeyCode.Equals)) {
            return 1f * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.Minus)) {
            return -1f * Time.deltaTime;
        }
        else {
            return 0f;
        }
    }
}
