using ServiceStudio.View;

namespace ServiceStudio.Presenter {
    partial class AggregatorPresenter : IAggregatorPresenter {
        ITopLevelView ITopLevelPresenter.View => View;
        public ITopLevelView GetView() => View;

        void IAggregatorPresenter.RefreshTitleBarAndStatusBar() {
            titleAndStatusBarManager.RefreshTitleBarAndStatusBar();
        }

    }
}
