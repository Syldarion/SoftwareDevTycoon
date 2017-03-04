using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get { return instance; }
        set
        {
            if (instance == null)
                instance = value;
            else
            {
                Debug.Log(string.Format("Singleton of type {0} already exists.", typeof(T).FullName));
                Destroy(value.gameObject);
            }
        }
    }
}
