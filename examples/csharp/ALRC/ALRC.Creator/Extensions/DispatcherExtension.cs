using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ALRC.Creator.Extensions;

public static class DispatcherExtension
{
    public static Dispatcher? Dispatcher { get; set; }
    public static int UiThreadId { get; set; }

    public static void Invoke(Action action)
    {
        if (Environment.CurrentManagedThreadId == UiThreadId)
            action.Invoke();
        else
            Dispatcher?.Invoke(action);
    }
}