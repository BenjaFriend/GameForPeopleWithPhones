using System;
using UnityEngine;

/// <summary>
/// Parent class for singleton MonoBehaviours. 
/// </summary>
/// <typeparam name="T">The type of the child class</typeparam>
public abstract class SingletonBehaviour<T> : Photon.PunBehaviour where T : SingletonBehaviour<T>
{
    protected static T instance;
    public static T Instance
    {
        get
        {
            if (instance != null)
                return instance;
            else
            {
                Debug.LogWarningFormat(
                    "[SingletonBehaviour<{0}>] Attempted to get Instance of singleton, but the instance is null!",
                    typeof(T).ToString());
                return null;
            }
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            setInstance();
        }
        else if(instance != this)
        {
            replaceInstance();
        }
    }

    protected virtual void OnDestroy()
    {
        // release instance
        if (instance == this)
            instance = null;
    }

    /// <summary>
    /// Sets the current instance of the singleton. Be sure to set `instance` before returning!
    /// </summary>
    protected abstract void setInstance();

    /// <summary>
    /// Handles behaviour of singleton when there's already a singleton instance
    /// </summary>
    protected virtual void replaceInstance()
    {
        Debug.LogWarningFormat(
            "[SingletonBehaviour<{0}>] An instance of the singleton already exists in the scene, destroying self. Override replaceInstance() in child class to change behaviour.",
            typeof(T).ToString());

        DestroyImmediate(this);
    }
}
