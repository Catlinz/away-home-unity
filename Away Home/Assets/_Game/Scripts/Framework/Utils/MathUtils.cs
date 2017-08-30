using UnityEngine;

public class AHMath
{
    /** Normalizes the angle two be between [-180, 180]. **/
    public static float NormalizeAngle(float angleInDegrees) {
        float offsetValue = angleInDegrees + 180f;
        return (offsetValue - (Mathf.Floor(offsetValue / 360f) * 360f)) - 180f;
    }

    /** Convert a heading angle in the xz-plane into a vector */
    public static Vector3 HeadingAngleToVector(float angleInDegrees) {
        float angleInRad = angleInDegrees * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(angleInRad), 0, Mathf.Cos(angleInRad));
    }

    /** Converts a heading vector in the XZ-plane into a heading angle */
    public static float VectorToHeadingAngle(Vector3 headingVector) {
        return Mathf.Atan2(headingVector.x, headingVector.z) * Mathf.Rad2Deg;
    }
}