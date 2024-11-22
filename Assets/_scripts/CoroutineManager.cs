using UnityEngine;
using System.Collections;


// Coroutine assistant for classes that do not inherit from MonoBehaviour

public class CoroutineManager : MonoBehaviour
{
    private static CoroutineManager _instance;

    // Singleton instance
    public static CoroutineManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("CoroutineManager");
                _instance = obj.AddComponent<CoroutineManager>();
                DontDestroyOnLoad(obj); // Ensure it persists across scenes
            }
            return _instance;
        }
    }

    // Start a coroutine from anywhere
    public void StartManagedCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
}
