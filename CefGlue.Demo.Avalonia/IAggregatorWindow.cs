using System.Collections.Generic;
using ServiceStudio.View;

namespace ServiceStudio.Presenter {

    public interface IAggregatorWindow  {
        IEnumerable<IAggregatorPresenter> Aggregators { get; }
        ITopLevelPresenter SelectedTopLevelPresenter { get; set; }

        IServerPagePresenter ServerPagePresenter { get; }

        void RemoveAggregator(IAggregatorPresenter aggregatorPresenter, bool removeFromView = true);

        void AttachAggregator(IAggregatorPresenter aggregator);


        void Show();

        IAggregatorWindowView View { get; }
    }
}
