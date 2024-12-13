using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class UnityMainThread : MonoBehaviour
{
    private static UnityMainThread instance;
    private static Queue<Action> executionQueue = new Queue<Action>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Update()
    {
        while (executionQueue.Count > 0)
        {
            executionQueue.Dequeue().Invoke();
        }
    }

    public static async Task ExecuteInUpdate(Action action)
    {
        if (instance == null)
        {
            var go = new GameObject("UnityMainThread");
            instance = go.AddComponent<UnityMainThread>();
        }

        var tcs = new TaskCompletionSource<bool>();
        executionQueue.Enqueue(() =>
        {
            try
            {
                action();
                tcs.SetResult(true);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        });
        await tcs.Task;
    }
}