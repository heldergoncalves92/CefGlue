using ServiceStudio.View;

namespace ServiceStudio.Presenter {
    public interface ITopLevelPresenter  {

        ITopLevelView View { get; }
        ITopLevelView GetView();


    }
}
