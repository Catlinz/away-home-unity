using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The component that controls the Movement of the ship.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class ShipMovementComponent : MonoBehaviour
{

    [Header("Turn")]
    /** The max role that occurs when the ship turns */
    public float maxRoll = 20f;

    /** The max rate at which the ship can turn */
    public float maxTurnRate = 90f;

    /** The acceleration for the ships turn rate */
    public float maxTurnAcceleration = 360f;
    [Space(10)]

    [Header("Speed")]
    /** The speed with which the ship accelerates */
    public float maxAcceleration = 10f;

    /** The max velocity of the ship */
    public float maxSpeed = 10f;


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

    /** The RigidBody component of the ship */
    private Rigidbody rb;

    public float ComputeHeading(float deltaTime) {
        float curHeading = heading;
        float headingDelta = Mathf.DeltaAngle(curHeading, desiredHeading);

        if (headingDelta != 0f && turnScale != 0f) {
            float curTurnRate = turnRate;
            float maxTurnDelta = maxTurnAcceleration * deltaTime;
            float scaledMaxTurnRate = maxTurnRate * turnScale;

            float newTurnRate = (headingDelta > 0f) ? curTurnRate + maxTurnDelta : curTurnRate - maxTurnDelta;
            turnRate = Mathf.Clamp(newTurnRate, -scaledMaxTurnRate, scaledMaxTurnRate);

            // Calculate the new heading
            float turnDelta = turnRate * deltaTime;
            float newHeading = AHMath.NormalizeAngle(curHeading + ((Mathf.Abs(headingDelta) > Mathf.Abs(turnDelta)) ? turnDelta : headingDelta));
            heading = newHeading;
            return newHeading;
        }
        else {
            turnRate = 0f;
            heading = curHeading;
            return curHeading;
        }

    }

    /** Update the Banking of the ship based on the turn rate. */
    public float ComputeRoll(float deltaTime, Vector3 rotation) {
        float desiredRoll = -maxRoll * (turnRate / maxTurnRate);
        float curRoll = rotation.z;
        float curRate = maxRoll * (maxTurnAcceleration / maxTurnRate);

        float newRoll = Mathf.MoveTowardsAngle(curRoll, desiredRoll, curRate * deltaTime);
        rollDeg = newRoll;
        return newRoll;
    }

    public Vector3 ComputeVelocity(float deltaTime, Vector3 oldVelocity) {
        float desiredSpeed = throttle * maxSpeed;
        float maxAccel = maxAcceleration;

        Vector3 desiredVelocity = AHMath.HeadingAngleToVectorXZ(heading) * desiredSpeed;
        Vector3 newVelocity = Vector3.MoveTowards(oldVelocity, desiredVelocity, deltaTime * maxAccel);

        return newVelocity;
    }

    /** The desired heading angle */
    public float GetDesiredHeading() { return desiredHeading; }

    /** The current throttle setting */
    public float GetThrottle() { return throttle; }

    /** Set the desired heading vector */
    public void SetDesiredHeading(Vector3 headingVector) {
        desiredHeading = AHMath.VectorXZToHeadingAngle(headingVector);
        turnScale = headingVector.magnitude;
    }

    /** Set the throttle percentage for the ship */
    public void SetThrottle(float newThrottle, float maxThrottle = 1f) {
        throttle = Mathf.Clamp(newThrottle, 0f, maxThrottle);
    }

    // Initial basic initialization
    private void Awake() {
        turnRate = 0f;
        rollDeg = 0f;
        throttle = 0f;
    }


    // Use this for initialization after Awake().
    void Start() {
        heading = gameObject.transform.rotation.eulerAngles.y * Mathf.Rad2Deg;
        desiredHeading = heading;

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezePositionY;
    }

    // Update during the physics steps.
    private void FixedUpdate() {

        Vector3 rotation = transform.rotation.eulerAngles;

        float newHeading = ComputeHeading(Time.deltaTime);
        float newRoll = ComputeRoll(Time.deltaTime, rotation);

        rotation.y = newHeading;
        rotation.z = newRoll;

        transform.rotation = Quaternion.Euler(rotation);

        Vector3 newVelocity = ComputeVelocity(Time.deltaTime, rb.velocity);
        rb.velocity = newVelocity;
    }
}
