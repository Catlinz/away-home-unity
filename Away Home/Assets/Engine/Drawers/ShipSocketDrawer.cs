using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom property drawer for rendering a ShipSocket in the inspector GUI.
/// </summary>
[CustomPropertyDrawer(typeof(ShipSocket))]
public class ShipSocketDrawer : PropertyDrawer {

    /// <summary>
    /// Returns the height required by the ShipSocket for rendering into the Inspector GUI.
    /// </summary>
    /// <param name="property">The property being drawn.</param>
    /// <param name="label">The label for the property.</param>
    /// <returns>The height the property needs to draw.</returns>
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return (EditorGUIUtility.singleLineHeight * 7) + EditorGUIUtility.standardVerticalSpacing * 10;
    }

    /// <summary>
    /// Handles the drawing of the ShipSocket in the Inspector GUI.
    /// </summary>
    /// <param name="position">The space in which we can draw.</param>
    /// <param name="property">The ShipSocket to draw.</param>
    /// <param name="label">The label of the ShipSocket.</param>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        // Begin the property control
        label = EditorGUI.BeginProperty(position, label, property);
        // Set the height back to a single line height, not the entire height of the property.
        position.height = EditorGUIUtility.singleLineHeight;

        // Set the indent level to 1.
        int oldIndentLevel = EditorGUI.indentLevel;
        EditorGUI.indentLevel += 1;

        Rect rect = new Rect(position);
        EditorGUI.PropertyField(rect, property.FindPropertyRelative("socketName"), GUIContent.none);
        rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
        EditorGUI.PropertyField(rect, property.FindPropertyRelative("maxCpuBandwidth"));
        rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
        EditorGUI.PropertyField(rect, property.FindPropertyRelative("maxPowerOutput"));
        rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
        EditorGUI.PropertyField(rect, property.FindPropertyRelative("arcLimits"));
        rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
        EditorGUI.PropertyField(rect, property.FindPropertyRelative("position"));

        // Draw the rotation as Euler angles instead of a quaternion.
        rect.y += (rect.height * 2) + EditorGUIUtility.standardVerticalSpacing * 5;
        EditorGUI.BeginChangeCheck();
        SerializedProperty rotProp = property.FindPropertyRelative("rotation");
        Vector3 rot = rotProp.quaternionValue.eulerAngles;
        rot = EditorGUI.Vector3Field(rect, rotProp.displayName, rot);
        if (EditorGUI.EndChangeCheck()) {
            rotProp.quaternionValue = Quaternion.Euler(rot);
        }
  
        // Restore the intendation level.
        EditorGUI.indentLevel = oldIndentLevel;

        // Finish rendering the property.
        EditorGUI.EndProperty();
    }
}
