using ServiceStudio.View;

namespace ServiceStudio.Presenter {

    public interface IAggregatorPresenter : ITopLevelPresenter {
        internal void RefreshTitleBarAndStatusBar();
        new IAggregatorView View { get; }

    }
}
