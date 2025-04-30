using UnityEngine;

public class PersistentSingleton<T> : MonoBehaviour where T : Component
{
    #region Fields
    public bool unparentOnAwake = true;

    public static bool HasInstance => instance != null;
    public static T Current => instance;

    protected static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<T>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name + "AutoCreated";
                    instance = obj.AddComponent<T>();
                }
            }

            return instance;
        }
    }

    protected virtual void Awake() => InitializeSingleton();

    #endregion

    #region Main Functions
    protected virtual void InitializeSingleton()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (unparentOnAwake)
        {
            transform.SetParent(null);
        }

        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(transform.gameObject);
            enabled = true;
        }
        else
        {
            if (this != instance)
            {
                Destroy(this.gameObject);
            }
        }
    }
    #endregion
}

public class Singleton<T> : MonoBehaviour where T : Component
{
    #region Fields
    public bool unparentOnAwake = true;

    protected static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<T>();
                if (instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name + "AutoCreated");
                    instance = obj.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    public static bool HasInstance => instance != null;
    public static T Current => instance;
    #endregion

    #region Main Functions
    protected virtual void Awake() => InitializeSingleton();

    protected virtual void InitializeSingleton()
    {
        if (!Application.isPlaying)
            return;

        if (unparentOnAwake)
            transform.SetParent(null);

        if (instance == null)
        {
            instance = this as T;
            enabled = true;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    #endregion

}