using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Service {

    /// <summary>The singleton instance of the class.</summary>
    private static Service instance;

    /// <summary>The list of registered services.</summary>
    private Dictionary<System.Type, object> services = new Dictionary<System.Type, object>();
	
    /// <summary>Set the singleton instance.</summary>
    public Service() {
        if (instance != null) {
            Debug.LogError("Cannot have two instances of Service singleton.");
            return;
        }

        instance = this;
    }

    /// <summary>Getter to get or create the singleton instance.</summary>
    public static Service Instance {
        get {
            if (instance == null) {
                new Service();
            }
            return instance;
        }
    }

    /// <summary>
    /// Store an object as a service.
    /// </summary>
    /// <typeparam name="T">The type of object to store as a service.</typeparam>
    /// <param name="service">The object to store as a service.</param>
    public static void Set<T>(T service) where T : class {
        Instance.services.Add(typeof(T), service);
    }

    /// <summary>
    /// Get a service from the Service manager.
    /// </summary>
    /// <typeparam name="T">The type of registered service to get.</typeparam>
    /// <returns>The registered service object or null if not found.</returns>
    public static T Get<T>() where T : class {
        T ret = null;
        try {
            ret = Instance.services[typeof(T)] as T;
        }
        catch (KeyNotFoundException) { }
        return ret;
    }

    public static void Clear<T>() where T : class {
        if (Instance.services.ContainsKey(typeof(T))) {
            Instance.services.Remove(typeof(T));
        }
    }

    /// <summary>
    /// Clears internal dictionary of service instances.
    /// This will not clear out any global state that they contain,
    /// unless there are no other references to the object.
    /// </summary>
    public static void ClearAll() {
        Instance.services.Clear();
    }
}
