using System;

namespace ServiceStudio.Presenter {

    public enum RefreshMode {
        Silent,
        Force,
        ForceSilent,
        ForceSilentOffline,
        Cancelled,
    }

    public interface IServerPagePresenter : ITopLevelPresenter, IDisposable {
     
    }
}
