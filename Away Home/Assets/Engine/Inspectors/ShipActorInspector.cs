using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Customize the Inspector and scene view drawing for the 
/// ShipActorComponent.
/// </summary>
[CustomEditor(typeof(ShipActorComponent))]
public class ShipActorInspector : Editor {

	private ShipActorComponent ship;
	private Transform shipTx;
	private Quaternion shipRot;

    private static GUILayoutOption socketButtonWidth = GUILayout.Width(20f);
    private static GUIContent socketMoveLabel = new GUIContent("\u21b4", "Move down");
    private static GUIContent socketDupLabel = new GUIContent("+", "Duplicate");
    private static GUIContent socketDelLabel = new GUIContent("-", "Delete");

    /// <summary>
    /// Draws the custom inspector GUI for the ShipActorComponent.
    /// </summary>
    ///
    public override void OnInspectorGUI() {
        serializedObject.Update();

		// Draw the Script property like the default InspectorGUI does.
		EditorGUI.BeginDisabledGroup(true);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"), true, new GUILayoutOption[0]);
		EditorGUI.EndDisabledGroup();
		EditorGUILayout.Space();

        // Draw the basic fields.
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("computer"), true);
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("power"), true);
		//EditorGUILayout.PropertyField(serializedObject.FindProperty("armor"), true);

		EditorGUILayout.Separator();
        // Draw the sockets.
        //InspectorHandleSockets();

		EditorGUILayout.Space();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("test"));

        serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// Draw a single socket element in the sockets array for the ShipActorComponent.
    /// It will draw the socket foldable, with buttons next to the socket label to manipulate
    /// the entry in the array.
    /// </summary>
    /// <param name="sockets">The serialized sockets array for the ShipActorComponent.</param>
    /// <param name="index">The index of the specific ShipSocket that we are rendering the UI for.</param>
    private void InspectorDrawSocket(SerializedProperty sockets, int index) {
        SerializedProperty socket = sockets.GetArrayElementAtIndex(index);

        EditorGUILayout.BeginHorizontal();
        // Draw the label and foldout for the socket, with a row of buttons on the same line
        // to manipulate the array entry.
        bool moveDown = false, duplicate = false, delete = false;
        socket.isExpanded = EditorGUILayout.Foldout(socket.isExpanded, socket.displayName, true);
        if (GUILayout.Button(socketMoveLabel, EditorStyles.miniButtonLeft, socketButtonWidth)) { moveDown = true; }
        if (GUILayout.Button(socketDupLabel, EditorStyles.miniButtonMid, socketButtonWidth)) { duplicate = true;  }
        if (GUILayout.Button(socketDelLabel, EditorStyles.miniButtonRight, socketButtonWidth)) { delete = true;  }
        
        EditorGUILayout.EndHorizontal();
        if (socket.isExpanded) {
            EditorGUILayout.PropertyField(socket);
        }

        // Handle the button actions here, since otherwise, we could delete an item then try and draw it.
        if (moveDown) { sockets.MoveArrayElement(index, index + 1); }
        if (duplicate) { InspectorAddSocket(sockets, index); }
        if (delete) {
            int oldSize = sockets.arraySize;
            sockets.DeleteArrayElementAtIndex(index);
        }
    }

    /// <summary>
    /// The code that handles rendering the sockets array in the Inspector GUI.
    /// </summary>
    private void InspectorHandleSockets() {
        // Draw the sockets.
		SerializedProperty sockets = serializedObject.FindProperty("modules").FindPropertyRelative("sockets");
        sockets.isExpanded = EditorGUILayout.Foldout(sockets.isExpanded, sockets.displayName, true);
        if (sockets.isExpanded) {
            EditorGUI.indentLevel += 1;
            for (int i = 0; i < sockets.arraySize; ++i) {
                InspectorDrawSocket(sockets, i);
            }

            if (sockets.arraySize == 0) {
                if (GUILayout.Button("Add Socket")) {
                    InspectorAddSocket(sockets, -1);
                }
            }
        }
    }

    /// <summary>
    /// Called when adding or inserting a socket into the sockets array from the Inspector GUI.
    /// The added socket is initialized to the default values.
    /// </summary>
    /// <param name="sockets">The Serialized sockets array to add a socket into.</param>
    /// <param name="index">The index to insert the socket, if we are inserting.</param>
    private void InspectorAddSocket(SerializedProperty sockets, int index) {
        if (index == -1) {
            index = sockets.arraySize;
            sockets.arraySize += 1;
        }
        else {
            sockets.InsertArrayElementAtIndex(index);
        }
        Hardpoint def = new Hardpoint();
        SerializedProperty socket = sockets.GetArrayElementAtIndex(index);
        socket.FindPropertyRelative("socketName").stringValue = def.name;

        SerializedProperty arc = socket.FindPropertyRelative("arcLimits");
        arc.FindPropertyRelative("up").floatValue = def.arcLimits.up;
        arc.FindPropertyRelative("down").floatValue = def.arcLimits.down;
        arc.FindPropertyRelative("left").floatValue = def.arcLimits.left;
        arc.FindPropertyRelative("right").floatValue = def.arcLimits.right;

        socket.FindPropertyRelative("position").vector3Value = def.position;
        socket.FindPropertyRelative("rotation").quaternionValue = def.rotation;
    }

    /// <summary>
    /// Handles rendering custom GUI elements in the scene view for the ShipActorComponent.
    /// It renders the sockets and allows positioning of the sockets with the transform gizmos.
    /// </summary>
    private void OnSceneGUI() {
        ship = target as ShipActorComponent;
        shipTx = ship.transform;
        shipRot = (Tools.pivotRotation == PivotRotation.Local) ? shipTx.rotation : Quaternion.identity;

        StructuralComponent structure = ship.GetComponent<StructuralComponent>();
        Hardpoint[] hardpoints = structure.GetAllHardpoints();;
		for (int i = 0; i < hardpoints.Length; ++i) {
			SceneGUIHandleSocket(ref hardpoints[i]);
        }
    }

	/// <summary>
    /// Handles the drawing of a socket into the scene view.
    /// </summary>
    /// <param name="socket">The socket to draw into the scene view.</param>
	private void SceneGUIHandleSocket(ref Hardpoint socket) {
		Vector3 point = shipTx.TransformPoint(socket.position);
		Quaternion rot = shipRot * socket.rotation;


		if (Tools.current == Tool.Move || Tools.current == Tool.Rotate) {
            // Draw the transform gizmo for the socket if we are rotating or moving.
			EditorGUI.BeginChangeCheck();
			if (Tools.current == Tool.Move) { // Draw the move gizmo
				point = Handles.DoPositionHandle(point, shipRot);
				if (EditorGUI.EndChangeCheck()) {
					Undo.RecordObject(ship, "Move Socket");
					EditorUtility.SetDirty(ship);
					socket.position = shipTx.InverseTransformPoint(point);
				}
			}
			else if (Tools.current == Tool.Rotate) { // Draw the rotation gizmo
				rot = Handles.DoRotationHandle(rot, point);
				if (EditorGUI.EndChangeCheck()) {
					Undo.RecordObject(ship, "Rotate Socket");
					EditorUtility.SetDirty(ship);
					socket.rotation = Quaternion.Inverse(shipRot) * rot;
				}
			}
		}

        // Draw the socket itself.
        SceneGUIDrawSocket(ref socket, point, rot);
	}

    /// <summary>
    /// Draw the representation of the socket into the scene view.  Will draw the 
    /// arc limits.
    /// </summary>
    /// <param name="socket">The socket to draw.</param>
    /// <param name="position">The position of the socket in world space.</param>
    /// <param name="rotation">The rotation of the socket in world space.</param>
    private void SceneGUIDrawSocket(ref Hardpoint socket, Vector3 position, Quaternion rotation) {
        Matrix4x4 mat4 = Handles.matrix;
        Color color = Handles.color;

        // Set the matrix to make it local to the socket.
        Handles.matrix = Matrix4x4.TRS(position, rotation, Vector3.one);

        // Draw the horizontal arc limits.
        Handles.color = new Color(1, 0, 0, 0.3f);
        Handles.DrawSolidArc(
            Vector3.zero, Vector3.up, 
            AHMath.HeadingAngleToVectorXZ(socket.arcLimits.left), 
            socket.arcLimits.left + socket.arcLimits.right, 
            0.05f
            );
        // Draw the vertical arg limits.
        Handles.color = new Color(0, 1, 0, 0.3f);
        Handles.DrawSolidArc(
            Vector3.zero, Vector3.left,
            AHMath.HeadingAngleToVectorYZ(-socket.arcLimits.down),
            socket.arcLimits.down + socket.arcLimits.up,
            0.05f
            );

        // Restore the Handles stuff.
        Handles.matrix = mat4;
        Handles.color = color;
    }
}
