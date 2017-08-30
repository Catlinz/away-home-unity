using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCameraComponent : MonoBehaviour {

    public Transform target;

    public float distance = 10f;
    public float speed = 0.2f;

    private Transform tx;

	// Use this for initialization
	void Start () {
        InitializeCamera();
    }

    private void OnEnable() {
        InitializeCamera();
    }

    // Update is called once per frame
    void Update () {

    }

    private void FixedUpdate() {
        Vector3 targetPos = target.position;
        Vector3 thisPos = tx.position;

        if (!Mathf.Approximately(targetPos.x, thisPos.x) || !Mathf.Approximately(targetPos.z, thisPos.z)) {
            targetPos.y += distance;

            tx.position = Vector3.Lerp(thisPos, targetPos, speed);

        }
        
    }

    private void InitializeCamera() {
        tx = GetComponent<Transform>();

        Vector3 targetPos = target.position;
        targetPos.y += distance;
        tx.position = targetPos;

        tx.LookAt(target);
    }
}
