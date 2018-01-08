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

    private static readonly object _lock = new object();

    /// <summary>Whether or not should use DontDestroyOnLoad().</summary>
    public bool persistent = true;

    /// <summary>The name of the GameObject to group under.</summary>
    public virtual string Group {
        get { return null; }
    }
    #endregion

    #region PROPERTIES
    public static bool Quitting { get; private set; }

    /// <summary>
    /// Gets the instance of the Singleton MonoBehaviour object.  
    /// 
    /// If the instance doesn't exist yet, then it is created.  If there are 
    /// multiple instances, an error is reported and we delete all the instances 
    /// and create a new one.
    /// </summary>
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
                        for (int i = 0; i < instances.Length; ++i) {
                            Destroy(instances[i]);
                        }
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
    // Set DontDestroyOnLoad if this singleton is persistant, and add it to 
    // the desired grouping GameObject, if there is one and it doesn't have a parent,
    // and do any custom initialization via virtual SingleAwake() call.
    private void Awake() {
        SingleAwake();

        if (persistent) {
            DontDestroyOnLoad(gameObject);
        }
        string group = Group;
        if (group != null && gameObject.transform.parent != null) {
            // Try and put the new game object under the Managers game object.
            GameObject groupObject = GameObject.Find(group);
            if (groupObject) {
                gameObject.transform.SetParent(groupObject.transform);
            }
        }
    }

    // Make sure we know when the application is quitting, so we can not return 
    // a reference to the singleton.
    private void OnApplicationQuit() {
        Quitting = true;
    }

    /// <summary>
    /// Virtual method for subclasses to implement for initialization that would
    /// normally be done in the Awake() method.
    /// </summary>
    protected virtual void SingleAwake() { }

    #endregion
}

