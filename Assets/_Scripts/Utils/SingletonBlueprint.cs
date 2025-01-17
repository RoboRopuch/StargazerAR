using UnityEngine;

/*
| **Feature**               | **StaticInstance<T>**           | **Singleton<T>**                   | **PersistentSingleton<T>**          |
|---------------------------|---------------------------------|------------------------------------|-------------------------------------|
| **Static Access**         | Provides static `Instance`      | Provides static `Instance`         | Provides static `Instance`          |
| **Duplicate Protection**  | No protection                   | Destroys duplicates                | Destroys duplicates                 |
| **Scene Persistence**     | Destroyed on scene load         | Destroyed on scene load            | Persists across scenes              |
| **Use Case**              | Simple global access            | Strict singleton within a scene    | Singleton across the entire game    |
| **Cleanup on Quit**       | Clears `Instance` on exit       | Clears `Instance` on exit          | Clears `Instance` on exit           |
*/

public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }
    protected virtual void Awake() => Instance = this as T;

    protected virtual void OnApplicationQuit()
    {
        Instance = null;
        Destroy(gameObject);
    }

}

public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        base.Awake();
    }
}

public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}