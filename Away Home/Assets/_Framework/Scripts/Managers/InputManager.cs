using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager> {

    ShipNavComponent nav;

    protected override void SingleAwake() {
        persistent = true;

    }

    // Use this for initialization
    void Start () {
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        nav = obj.GetComponent<ShipNavComponent>();
	}
	
	// Update is called once per frame
	void Update () {
        float up = Input.GetAxis("Vertical");
        nav.SetThrottle(up);
    }
}
