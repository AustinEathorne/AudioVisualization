using UnityEngine;
using System.Collections;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    private static object objLock = new object();

    public static T Instance
    {
        get
        {
            lock (objLock)
            {
                if (instance == null)
                {
                    instance = (T)FindObjectOfType(typeof(T));

                    // Make sure for whatever reason there isn't multiple singletons already
                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        //Debug.LogError("[MonoSingleton] More than one instance of '" + typeof(T).ToString() +
                            //"' was found");
                        return instance;
                    }

                    if (instance == null)
                    {
                        GameObject singleton = new GameObject();
                        instance = singleton.AddComponent<T>();
                        singleton.name = "[Singleton] " + typeof(T).ToString();

                        DontDestroyOnLoad(singleton);

                        //Debug.Log("[MonoSingleton] An instance of '" + typeof(T).ToString() + "' is needed, " +
                            //singleton + " was created");
                    }
                    else
                    {
                        //Debug.Log("[MonoSingleton] An instance of '" + typeof(T).ToString() + "' is needed, " +
                            //instance.gameObject.name + " has already been created");
                    }
                }

                return instance;
            }
        }
    }


    protected bool isRunning;
    public abstract IEnumerator Initialize();
    public abstract IEnumerator Run();
    public abstract IEnumerator Stop();
}
