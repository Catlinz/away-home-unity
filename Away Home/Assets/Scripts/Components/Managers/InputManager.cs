using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    private static InputManager instance;
    private const string NAME = "InputManager";

    public static InputManager Get() {
        if (!instance) {
            GameObject obj;
            // If no instance yet, try and find one in the scene.
            obj = GameObject.Find(NAME);

            if (obj) {
                instance = obj.GetComponent<InputManager>();
                if (instance) { return instance; }
            }

            // If not found one in the scene, then create new one.
            instance = CreateManagerInstance(NAME);
        }

        return instance;
    }

    private static InputManager CreateManagerInstance(string objName) {
        GameObject obj = new GameObject(objName);
        InputManager inputManager = obj.AddComponent<InputManager>();

        // Try and put the new game object under the Managers game object.
        GameObject managers = GameObject.Find("Managers");
        if (managers) {
            obj.transform.SetParent(managers.transform);
        }

        return inputManager;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
