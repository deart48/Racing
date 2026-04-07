using System;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RacingApp.ViewModels;

public abstract class ViewModelBase : ObservableObject
{

    protected static void RunOnUiThread(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);
        Dispatcher.UIThread.Post(action);
    }


    protected static void RunSafely(Action action, Action<string>? onError = null)
    {
        ArgumentNullException.ThrowIfNull(action);
        try
        {
            action();
        }
        catch (Exception ex)
        {
            onError?.Invoke(ex.Message);
        }
    }
}
