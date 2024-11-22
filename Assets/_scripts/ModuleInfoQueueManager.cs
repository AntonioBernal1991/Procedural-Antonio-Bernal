using System.Collections.Generic;
using UnityEngine;


//Queses the info that the module and path generator needs
public static class ModuleInfoQueueManager
{
    private static Queue<ModuleInfo> moduleQueue = new Queue<ModuleInfo>();

    /// <summary>
    /// Enqueues a ModuleInfo onto the queue.
    /// </summary>
    public static void Enqueue(ModuleInfo module)
    {
        moduleQueue.Enqueue(module);
        Debug.Log($"Enqueued {module} to the queue. Queue size is now {moduleQueue.Count}.");
    }

    /// <summary>
    /// Dequeues a ModuleInfo from the queue.
    /// </summary>
    public static ModuleInfo Dequeue()
    {
        if (moduleQueue.Count > 0)
        {
            ModuleInfo dequeuedModule = moduleQueue.Dequeue();
            Debug.Log($"Dequeued {dequeuedModule} from the queue. Queue size is now {moduleQueue.Count}.");
            return dequeuedModule;
        }
        else
        {
            Debug.LogWarning("Attempted to dequeue from an empty queue.");
            return null; // Return null if the queue is empty
        }
    }

    /// <summary>
    /// Peeks at the front ModuleInfo in the queue without removing it.
    /// </summary>
    public static ModuleInfo Peek()
    {
        if (moduleQueue.Count > 0)
        {
            return moduleQueue.Peek();
        }
        else
        {
            Debug.LogWarning("Attempted to peek at an empty queue.");
            return null;
        }
    }

    /// <summary>
    /// Gets the current size of the queue.
    /// </summary>
    public static int Count => moduleQueue.Count;

    /// <summary>
    /// Clears the queue.
    /// </summary>
    public static void Clear()
    {
        moduleQueue.Clear();
        Debug.Log("Cleared the queue.");
    }
}
