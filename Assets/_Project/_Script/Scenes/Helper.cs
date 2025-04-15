using UnityEngine;
using System.Threading.Tasks;

public static class UnityAsyncExtensions
{
    public static Task AsTask(this AsyncOperation asyncOperation)
    {
        var tcs = new TaskCompletionSource<bool>();
        asyncOperation.completed += _ => tcs.SetResult(true);
        return tcs.Task;
    }
}