using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The component that controls the Movement of the ship.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class ShipNavComponent : MonoBehaviour {
	#region PUBLIC FIELDS
	[Header("Visual")]
    /** The max role that occurs when the ship turns */
    public float maxRoll = 20f;
	#endregion

	#region PUBLIC PROPERTIES
	public float DesiredHeading {
		get { return desiredHeading; }
	}

	public float Throttle {
		get { return throttle; }
	}
	#endregion

	#region PRIVATE FIELDS
	/// <summary>A reference to the ShipThrustComponent.</summary>
	private ShipThrustComponent engine;
	
	/** The RigidBody component of the ship */
    private Rigidbody rb;

	/** The rate at which the ship is currently turning */
    private float turnRate;

    /** The amount to scale the turn rate by */
    private float turnScale;

	/** The amount the ship is currently rolled */
    private float rollDeg;

    /** The current heading of the ship */
    private float heading;

	/** The desired heading for the ship */
    private float desiredHeading;

    /** The current throttle percentage for the ship */
    private float throttle;
	#endregion

	#region PUBLIC METHODS
	public float ComputeHeading(float deltaTime) {
        return desiredHeading;
    }

	/** Update the Banking of the ship based on the turn rate. */
    public float ComputeRoll(float deltaTime, Vector3 rotation) {
        return 0f;
    }

	public Vector3 ComputeVelocity(float deltaTime, Vector3 oldVelocity) {
        float accel = engine.Acceleration(throttle);
        return new Vector3(0, 0, accel * deltaTime) + oldVelocity;
    }

	/** Set the desired heading vector */
    public void SetDesiredHeading(Vector3 headingVector) {
        desiredHeading = AHMath.VectorXZToHeadingAngle(headingVector);
        turnScale = headingVector.magnitude;
    }

	/** Set the throttle percentage for the ship */
    public void SetThrottle(float newThrottle, float maxThrottle = 1f) {
        throttle = Mathf.Clamp(newThrottle, 0f, maxThrottle);
    }
	#endregion

	#region UNITY HOOKS
	// Initial basic initialization
    private void Awake() 
	{
		engine = null;
		rb = null;

        turnRate = 0f;
        rollDeg = 0f;
        throttle = 0f;
    }

    // Gets the component references and initiales the heading.
    void Start() {
        heading = gameObject.transform.rotation.eulerAngles.y * Mathf.Rad2Deg;
        desiredHeading = heading;

		engine = GetComponent<ShipThrustComponent>();

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezePositionY;
    }

	// Update during the physics steps.
    void FixedUpdate() {

        Vector3 rotation = transform.rotation.eulerAngles;

        float newHeading = ComputeHeading(Time.deltaTime);
        float newRoll = ComputeRoll(Time.deltaTime, rotation);

        rotation.y = newHeading;
        rotation.z = newRoll;

        transform.rotation = Quaternion.Euler(rotation);

        Vector3 newVelocity = ComputeVelocity(Time.deltaTime, rb.velocity);
        rb.velocity = newVelocity;
    }
	#endregion
}
