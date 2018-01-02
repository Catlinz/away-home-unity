using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class to easily implement safe singleton classes.
/// Code from http://wiki.unity3d.com/index.php/Singleton.
/// </summary>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour {

    #region FIELDS
    private static T _instance;

    private static readonly object _lock;

    /// <summary>Whether or not should use DontDestroyOnLoad().</summary>
    protected bool _persistent = true;
    #endregion

    #region PROPERTIES
    public static bool Quitting { get; private set; }

    public static T Instance {
        get {
            if (Quitting) {
                Debug.LogWarning("[Singleton]<" + typeof(T) + "> won't be returned since application is quitting.");
                return null;
            }

            lock(_lock) {
                if (_instance == null) {
                    T[] instances = FindObjectsOfType<T>();
                    if (instances.Length > 1) {
                        Debug.LogError("Singleton<" + typeof(T) + "> has " + instances.Length + " instances, this should never happen!");

                    }
                    else if (instances.Length == 1) {
                        _instance = instances[0];
                        Debug.Log("Singleton<" + typeof(T) + "> attached to existing instance in scene.");
                    }
                    
                    // If still null, create a new GameObject to attach an instance to.
                    if (_instance == null) {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = "Singleton<" + typeof(T).ToString() + ">";

                        Debug.Log("Singleton<" + typeof(T) + "> was created in the scene.");
                    }
                }

                return _instance;
            }
        }
    }
    #endregion

    #region METHODS
    private void Awake() {
        if (_persistent) {
            DontDestroyOnLoad(gameObject);
        }
        SingleAwake();
    }

    private void OnApplicationQuit() {
        Quitting = true;
    }

    protected virtual void SingleAwake() { }
    #endregion
}

