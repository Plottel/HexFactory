using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            return _instance;
        }
    }

    public static bool Exists
    {
        get { return _instance != null; }
    }

    protected virtual void Awake()
    {
        EnsureInstance();
        //DontDestroyOnLoad(gameObject);
    }

    protected void EnsureInstance()
    {
        if (_instance != null && _instance != this)
        {
            if (Application.isEditor)
                DestroyImmediate(this);
            else
                Destroy(this);
        }

        if (_instance == null)
            _instance = FindObjectOfType<T>();
    }
}