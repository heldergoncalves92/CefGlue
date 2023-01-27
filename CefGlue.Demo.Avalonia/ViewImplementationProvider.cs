using System;
using System.Diagnostics;

namespace ServiceStudio.View {
    public abstract class ViewImplementationProvider : IApplication {
        private static ViewImplementationProvider instance;

        [DebuggerNonUserCode]
        public static ViewImplementationProvider Instance {
            [DebuggerNonUserCode]
            get {
                lock (typeof(ViewImplementationProvider)) {
                    if (constructor != null) {
                        instance = constructor();
                        constructor = null;

                        // discover services in the provider's assembly. Note that the Runtime class
                        // doesn't do this automatically because it doesn't know which view provider
                        // will be initialized.
                    }
                    return instance;
                }
            }
        }

        private static Func<ViewImplementationProvider> constructor;

        public static void SetInstance(Func<ViewImplementationProvider> constructor) {
            lock (typeof(ViewImplementationProvider)) {
                ViewImplementationProvider.constructor = constructor;
            }
        }

        public static event Action<Exception> UnhandledException;

        protected static void TriggerUnhandledExceptionEventHandler(Exception e) {
            UnhandledException?.Invoke(e);
        }

        public static event Action Exit;

        protected void RaiseExit() {
            if (Exit != null) {
                Exit();
                Exit = null;
            }
        }

        public static void InitializeThemeSettings() { }

        public static bool HasStarted { get; protected set; }

        public static event Action Starting;

        protected void RaiseStarting() {
            Starting?.Invoke();
        }

        public abstract void Start();

        public virtual void DeleteSubmitFeedbackViewFiles() { }

        public abstract IAggregatorWindowView CreateAggregatorWindow();
    }
}
