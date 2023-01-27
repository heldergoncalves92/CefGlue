using System;
using System.ComponentModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using ServiceStudio.View;

namespace ServiceStudio.WebViewImplementation {

    public class App : Application, IApplication {

        public override void Initialize() {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted() {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime) {
                lifetime.Startup += delegate { HasStarted = true; };
                lifetime.ShutdownRequested += OnShutdownRequested;
            }
            base.OnFrameworkInitializationCompleted();
        }

        // #RMAC-9008 We can close Service Studio mac in two ways
        //
        //     - 1 Clicking on the red semaphore
        //     - 2 Selecting top menu bar quit, or cmd+q
        //
        //     NOTE: When all windows are closed, Avalonia fires the ShutdownRequested event that can be canceled.
        //
        //     1 - When we click on the red semaphore, the TryClose() gets called and SS will ask if we have pending changes. Then ShutdownRequested is called.
        //
        //     2 - When we quit the application, ShutdownRequested is called and we cancel it. Then we need to close all open SS windows.
        //         So we will call TryClose() on each aggregator. When all windows are closed the ShutdownRequested is called again, and at this, time since there are no more open windows we don't cancel the event.
        private void OnShutdownRequested(object sender, ShutdownRequestedEventArgs e) {
            var hasOpenWindows = ((ClassicDesktopStyleApplicationLifetime)sender).Windows.Count > 0;
            if (!hasOpenWindows) {
                IsShuttingDown = true;
                return;
            }

            e.Cancel = true;
            QuitApplicationRequested?.Invoke();
        }

        public event Action AboutRequested;
        public event Action PreferencesRequested;
        public event Action QuitApplicationRequested;

        private void OnAboutClick(object sender, EventArgs args) => AboutRequested();

        private void OnPreferencesClick(object sender, EventArgs args) => PreferencesRequested();

        public bool HasStarted { get; private set; }

        public bool IsShuttingDown { get; private set; }
    }
}
