using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using ServiceStudio.View;

namespace ServiceStudio.Presenter {
    partial class RuntimeImplementation {
        protected readonly Dictionary<IAggregatorWindowView, IAggregatorWindow> aggregatorWindows = new();
        protected readonly Dictionary<IAggregatorView, IAggregatorPresenter> aggregators = new();

        private static RuntimeImplementation instance;

        [DebuggerNonUserCode]
        public new static RuntimeImplementation Instance {
            [DebuggerNonUserCode]
            get {
                if (instance == null) {
                    instance = (RuntimeImplementation)Runtime.Instance;
                }
                return instance;
            }
        }

        public RuntimeImplementation(string[] commandLineArgs, object notificationsService, object protectedResourcesAccessService, Thread mainThread)
            :
            this(default,
                notificationsService,
                protectedResourcesAccessService,
                false,
                false,
                Thread.CurrentThread) { }

        private RuntimeImplementation(
            string[] commandLineArgs,
            object notificationsService,
            object protectedResourcesAccessService,
            bool debugModeRequested,
            bool hotfixModeRequested,
            Thread mainThread)
            : base(commandLineArgs, protectedResourcesAccessService, debugModeRequested, hotfixModeRequested, _ => true) {
            mainThread.Name ??= "Main";
        }

        protected override void Initialize() {
            base.Initialize();

            // Initialize installation data (installerId, ...)
            InitializeInstallationDataProvider();

            ViewImplementationProvider.InitializeThemeSettings();
        }

        private void InitializeInstallationDataProvider() {
        }

        protected virtual void OnUnhandledException(object sender, Exception exception) { }

        public void Start(Action onFirstWindowShown = null) {
            InitializeProviders();
            ViewImplementationProvider.Exit += Cleanup;

            ProcessCommandLineArgs(onFirstWindowShown);
        }

        protected virtual void ProcessCommandLineArgs(Action onFirstWindowShown = null) {
            StartView(onFirstWindowShown);
        }

        private static void InitializeProviders() { }

        [DebuggerNonUserCode]
        public IEnumerable<IAggregatorWindow> AggregatorWindows {
            [DebuggerNonUserCode]
            get { return aggregatorWindows.Values; }
        }

        public IEnumerable<ITopLevelPresenter> GetAllTopLevelPresenters() {
            return AggregatorWindows.SelectMany(a => new ITopLevelPresenter[] { a.ServerPagePresenter }.Concat(a.Aggregators)).Where(t => t != null);
        }

        public IAggregatorWindow GetAggregatorWindow(IAggregatorPresenter aggregator) {
            return AggregatorWindows.ToArray().SingleOrDefault(window => window.Aggregators.Contains(aggregator));
        }

        internal void AddAggregatorWindow(IAggregatorWindow aggregatorWindow) {
            aggregatorWindows.Add(aggregatorWindow.View, aggregatorWindow);
        }

        internal void AddAggregator(IAggregatorPresenter aggregator) {
            lock (aggregators) {
                aggregators.Add(aggregator.View, aggregator);
            }
        }

        protected ITopLevelPresenter InnerStartView(Action onFirstWindowShown = null) {
            if (ViewImplementationProvider.HasStarted) {
                return AggregatorWindows.Single().SelectedTopLevelPresenter;
            }

            ViewImplementationProvider.Instance.Start(); // must be called before first aggregator window is created

            var aggregatorWindow = ShowNewAggregatorWindow();

            var selectedTopLevelPresenter = aggregatorWindow.SelectedTopLevelPresenter;

            if (selectedTopLevelPresenter != null) {
                // Needs to stay above Login for authorization redirects from browser to work as well as other re-use cases
                StartProcessReuseWatcher();
            } else {
                StartProcessReuseWatcher();
            }

            return selectedTopLevelPresenter;
        }

        private void StartProcessReuseWatcher() { }

        private void TryRunAutoUpdater() { }

        private bool ShouldUpdate() {
            return !DebugMode
                && !System.Diagnostics.Debugger.IsAttached
                && !RunningUnitTests;
        }

        protected virtual ITopLevelPresenter StartView(Action onFirstWindowShown = null) {
            // trying to run autoupdater first to not depend on possible errors during starting the view
            TryRunAutoUpdater();

            return InnerStartView(onFirstWindowShown);
        }

        public IAggregatorWindow ShowNewAggregatorWindow() {
            var window = CreateAggregatorWindow();
            window.Show();

            return window;
        }

        public IAggregatorWindow CreateAggregatorWindow() {
            var aggregatorWindowView = ViewImplementationProvider.Instance.CreateAggregatorWindow();
            return CreateAggregatorWindow(aggregatorWindowView);
        }

        private IAggregatorWindow CreateAggregatorWindow(IAggregatorWindowView view) => new AggregatorWindowPresenter(view);

        protected virtual void Cleanup() { }

        
    }
}
