using System;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace ServiceStudio.WebViewImplementation.Framework {
    internal static class DispatcherExtensions {

        private static Action<Exception> unhandledExceptionHandler;

        public static R ExecuteInUIThread<R>(this Dispatcher dispatcher, Func<R> func, DispatcherPriority priority = DispatcherPriority.Normal) {
            try {
                if (dispatcher.CheckAccess()) {
                    return func();
                }

                return dispatcher.InvokeAsync(func, priority).Result;
            } catch (Exception e) {
                unhandledExceptionHandler?.Invoke(e);
                throw;
            }
        }

        public static void ExecuteInUIThread(this Dispatcher dispatcher, Action action, DispatcherPriority priority = DispatcherPriority.Normal) {
            dispatcher.ExecuteInUIThread(() => { action(); return true; }, priority);
        }

        public static void AsyncExecuteInUIThread(this Dispatcher dispatcher, Action action, DispatcherPriority priority = DispatcherPriority.Normal) {
            dispatcher.InvokeAsync(action, priority)
                .ContinueWith(t => unhandledExceptionHandler?.Invoke(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
        }
        
        /// <summary>
        /// Use this extension method to simulate a DispatcherTimer.RunOnce, which doesn't work in sync with Modal windows in macOS.
        /// (It doesn't work on macOS due to the NSApplication.runModalForWindow)
        /// </summary>
        /// <param name="dispatcher"></param>
        /// <param name="action"></param>
        /// <param name="delay"></param>
        public static IDisposable DelayedExecute(this Dispatcher dispatcher, Action action, TimeSpan delay) {
            var cts = new CancellationTokenSource();
            var isCtsDisposed = false;
            
            // We can't use Avalonia's DispatcherTimer to run this task, as it will fail when there's a modal window being shown
            Task.Delay(delay, cts.Token).ContinueWith(t => {
                lock (cts) {
                    isCtsDisposed = true;
                    cts.Dispose();
                }
                if (!t.IsCanceled) {
                    dispatcher.AsyncExecuteInUIThread(action);
                }
            }, TaskContinuationOptions.RunContinuationsAsynchronously);

            return Disposable.Create(() => {
                lock (cts) {
                    if (!isCtsDisposed) {
                        cts.Cancel();
                    }
                }
            });
        }

        public static void AddUnhandledExceptionHandler(this Dispatcher dispatcher, Action<Exception> handler) {
            unhandledExceptionHandler += handler;
        }
    }
}
