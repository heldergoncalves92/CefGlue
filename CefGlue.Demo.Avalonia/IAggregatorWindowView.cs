using System;

namespace ServiceStudio.View {
    public interface IAggregatorWindowView : IView {
        ITopLevelView SelectedView { get; set; }
        IAggregatorView CreateAggregatorView();

        void RemoveAggregator(IAggregatorView aggregatorView);
        void InsertAggregator(IAggregatorView aggregatorView, int? index);

        event Action<ITopLevelView> SelectedViewChanged;

        void Show();
    }
}
