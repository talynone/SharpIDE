using System;
using System.Threading.Tasks;
using Godot;

namespace SharpIDE.Godot;

public static class NodeExtensions
{
    public static Task InvokeAsync(this Node node, Action workItem)
    {
        var taskCompletionSource = new TaskCompletionSource();
        //WorkerThreadPool.AddTask();
        Callable.From(() =>
        {
            workItem();
            taskCompletionSource.SetResult();
        }).CallDeferred();
        return taskCompletionSource.Task;
    }
    
    public static Task InvokeAsync(this Node node, Func<Task> workItem)
    {
        var taskCompletionSource = new TaskCompletionSource();
        //WorkerThreadPool.AddTask();
        Callable.From(async void () =>
        {
            try
            {
                await workItem();
                taskCompletionSource.SetResult();
            }
            catch (Exception ex)
            {
                taskCompletionSource.SetException(ex);
            }
        }).CallDeferred();
        return taskCompletionSource.Task;
    }
}