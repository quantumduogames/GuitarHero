using UnityEngine;

public class DontDestroyOnLoadObject : MonoBehaviour
{
    private static DontDestroyOnLoadObject Singleton;

    private void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
            return;
        }

        Singleton = this;
        DontDestroyOnLoad(gameObject);
    }
}

