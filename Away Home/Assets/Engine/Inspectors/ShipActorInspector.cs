using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShipActorComponent))]
public class ShipActorInspector : Editor {

	private ShipActorComponent ship;
	private Transform shipTx;
	private Quaternion shipRot;

	private void OnSceneGUI() {
		ship = target as ShipActorComponent;
		shipTx = ship.transform;
		shipRot = (Tools.pivotRotation == PivotRotation.Local) ? shipTx.rotation : Quaternion.identity;


		for (int i = 0; i < ship.sockets.Length; ++i) {
			SceneGUIHandleSocket(ref ship.sockets[i]);
		}
	}

	/**
	 * Handle the drawing of the socket for the OnSceneGUI method
	 */
	private void SceneGUIHandleSocket(ref ShipSocket socket) {
		Vector3 point = shipTx.TransformPoint(socket.position);
		Quaternion rot = shipRot * socket.rotation;


		if (Tools.current == Tool.Move || Tools.current == Tool.Rotate) {
			EditorGUI.BeginChangeCheck();
			if (Tools.current == Tool.Move) {
				point = Handles.DoPositionHandle(point, shipRot);
				if (EditorGUI.EndChangeCheck()) {
					Undo.RecordObject(ship, "Move Socket");
					EditorUtility.SetDirty(ship);
					socket.position = shipTx.InverseTransformPoint(point);
				}
			}
			else if (Tools.current == Tool.Rotate) {
				rot = Handles.DoRotationHandle(rot, point);
				if (EditorGUI.EndChangeCheck()) {
					Undo.RecordObject(ship, "Rotate Socket");
					EditorUtility.SetDirty(ship);
					socket.rotation = Quaternion.Inverse(shipRot) * rot;
				}
			}
		}

        SceneGUIDrawSocket(ref socket, point, rot);
	}

    private void SceneGUIDrawSocket(ref ShipSocket socket, Vector3 position, Quaternion rotation) {
        Matrix4x4 mat4 = Handles.matrix;
        Color color = Handles.color;

        Handles.matrix = Matrix4x4.TRS(position, rotation, Vector3.one);

        Handles.color = new Color(1, 0, 0, 0.3f);
        Handles.DrawSolidArc(
            Vector3.zero, Vector3.up, 
            AHMath.HeadingAngleToVectorXZ(socket.arcLimits.left), 
            socket.arcLimits.left + socket.arcLimits.right, 
            0.05f
            );
        Handles.color = new Color(0, 1, 0, 0.3f);
        Handles.DrawSolidArc(
            Vector3.zero, Vector3.left,
            AHMath.HeadingAngleToVectorYZ(-socket.arcLimits.down),
            socket.arcLimits.down + socket.arcLimits.up,
            0.05f
            );
        //Handles.DrawWireCube(Vector3.zero, new Vector3(0.5f, 0.5f, 0.5f));
        Handles.matrix = mat4;
        Handles.color = color;
    }
}
