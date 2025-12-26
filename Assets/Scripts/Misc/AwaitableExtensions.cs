using System;
using UnityEngine;

public static class AwaitableExtensions
{
    public static async void Forget(this Awaitable awaitable)
    {
        try
        {
            await awaitable;
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}
