using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// A custom PropertyDrawer for drawing the movement arcs for a socket in the Inspector GUI.
/// </summary>
[CustomPropertyDrawer(typeof(SocketArc))]
public class SocketArcDrawer : PropertyDrawer {

    /// <summary>
    /// Draws the SocketArc into the Inspector GUI as four inputs for 
    /// left, right, up, down.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="property"></param>
    /// <param name="label"></param>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        // Being the property control
        label = EditorGUI.BeginProperty(position, label, property);

        // Render the label for the ShipSocket.
        Rect rect = EditorGUI.PrefixLabel(position, label);
        int oldIndentLevel = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        GUIContent[] labels = new GUIContent[4];
        labels[0] = new GUIContent("L");
        labels[1] = new GUIContent("R");
        labels[2] = new GUIContent("U");
        labels[3] = new GUIContent("D");

        SerializedProperty left = property.FindPropertyRelative("left");
        SerializedProperty right = property.FindPropertyRelative("right");
        SerializedProperty up = property.FindPropertyRelative("up");
        SerializedProperty down = property.FindPropertyRelative("down");
        float[] values = new float[4];
        values[0] = left.floatValue;
        values[1] = right.floatValue;
        values[2] = up.floatValue;
        values[3] = down.floatValue;

        EditorGUI.BeginChangeCheck();
        EditorGUI.MultiFloatField(rect, labels, values);
        if (EditorGUI.EndChangeCheck()) {
            left.floatValue = values[0];
            right.floatValue = values[1];
            up.floatValue = values[2];
            down.floatValue = values[3];
        }

        EditorGUI.indentLevel = oldIndentLevel;

        // Finish rendering the property.
        EditorGUI.EndProperty();
    }
}
