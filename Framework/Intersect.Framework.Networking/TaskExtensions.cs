namespace Intersect.Framework.Networking;

public static class TaskExtensions
{
    public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
    {
        var taskCompletionSource = new TaskCompletionSource<bool>();
        using (cancellationToken.Register(source => (source as TaskCompletionSource<bool> ?? throw new ArgumentNullException(nameof(source))).TrySetResult(true), taskCompletionSource))
        {
            if (task != await Task.WhenAny(task, taskCompletionSource.Task))
            {
                throw new OperationCanceledException(cancellationToken);
            }
        }

        return task.Result;
    }
}
