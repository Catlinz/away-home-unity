using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ShipMovement))]
public class Ship : MonoBehaviour {

    private ShipMovement movement;

	// Use this for initialization
	void Start () {
        movement = GetComponent<ShipMovement>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
