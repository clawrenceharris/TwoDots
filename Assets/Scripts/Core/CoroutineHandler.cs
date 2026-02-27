
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class CoroutineHandler : MonoBehaviour
{
    private static CoroutineHandler instance;

   
    

    private void Awake()
    {
        // Ensure only one instance of CoroutineHandler exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

 
    public static Coroutine StartStaticCoroutine(IEnumerator coroutine, Action callback = null)
    {
        return instance.StartCoroutine(RunCoroutine(coroutine, callback));
    }

    // Wrapper coroutine that runs the original coroutine and then invokes the callback
    private static IEnumerator RunCoroutine(IEnumerator coroutine, Action callback)
    {
        yield return coroutine;
        callback?.Invoke();
    }

    public static void StopStaticCoroutine(Coroutine coroutine)
    {
        if(coroutine != null)
            instance.StopCoroutine(coroutine);
    }


}
