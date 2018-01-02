using UnityEngine;

/// <summary>
/// Status utility methods for mathematical calculations.
/// </summary>
public class AHMath
{
    /// <summary>
    /// Normalizes the angle two be between [-180, 180].
    /// </summary>
    /// <param name="angleInDegrees">The angle to normalize.</param>
    /// <returns>The angle as an angle between [-180, 180].</returns>
    public static float NormalizeAngle(float angleInDegrees) {
        float offsetValue = angleInDegrees + 180f;
        return (offsetValue - (Mathf.Floor(offsetValue / 360f) * 360f)) - 180f;
    }

    /// <summary>
    /// Convert a single 2D heading angle into a vector in the XZ-plane. 
    /// <para>The Z-axis is forward, i.e., (x=0,z=1) is 0 degrees.</para>
    /// </summary>
    /// <param name="angleInDegrees">The heading angle in degrees.</param>
    /// <returns>A vector in the XZ plane representing the direction of the heading angle.</returns>
    public static Vector3 HeadingAngleToVectorXZ(float angleInDegrees) {
        float angleInRad = angleInDegrees * Mathf.Deg2Rad;
        return new Vector3(-Mathf.Sin(angleInRad), 0, Mathf.Cos(angleInRad));
    }

    /// <summary>
    /// Convert a single 2D heading angle into a vector in the YZ-plane. 
    /// <para>The Z-axis is forward, i.e., (y=0,z=1) is 0 degrees.</para>
    /// </summary>
    /// <param name="angleInDegrees">The heading angle in degrees.</param>
    /// <returns>A vector in the YZ plane representing the direction of the heading angle.</returns>
    public static Vector3 HeadingAngleToVectorYZ(float angleInDegrees) {
        float angleInRad = angleInDegrees * Mathf.Deg2Rad;
        return new Vector3(0, Mathf.Sin(angleInRad), Mathf.Cos(angleInRad));
    }

    /// <summary>
    /// Converts a 2D direction vector in the XZ plane into a heading angle.
    /// <para>The Z-axis is forward, i.e., (x=0,z=1) is 0 degrees.</para>
    /// </summary>
    /// <param name="headingVector">The direction vector in the XZ plane.</param>
    /// <returns>A heading angle representing an angle in the XZ plane.</returns>
    public static float VectorXZToHeadingAngle(Vector3 headingVector) {
        return Mathf.Atan2(-headingVector.x, headingVector.z) * Mathf.Rad2Deg;
    }
}